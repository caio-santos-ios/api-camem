using api_camem.src.Configuration;
using api_camem.src.Handlers;
using api_camem.src.Interfaces;
using api_camem.src.Models;
using MongoDB.Driver;

namespace api_camem.src.Workers
{
    public class FinancialWork(IServiceProvider serviceProvider) : BackgroundService
    {
        private static readonly TimeSpan CheckInterval = TimeSpan.FromSeconds(60);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessDueTriggers(stoppingToken);
                }
                catch (Exception ex)
                {
                    using var errorScope = serviceProvider.CreateScope();
                    var logger = errorScope.ServiceProvider.GetRequiredService<ILoggerService>();
                    await logger.CreateAsync(new()
                    {
                        Path       = "Work",
                        Message    = $"Work: {ex.Message}",
                        Method     = "WORK",
                        StatusCode = 500
                    });
                }

                await Task.Delay(CheckInterval, stoppingToken);
            }
        }

        private async Task ProcessDueTriggers(CancellationToken ct)
        {
            using var scope = serviceProvider.CreateScope();

            var context       = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var loggerService = scope.ServiceProvider.GetRequiredService<ILoggerService>();
            var countHandler  = scope.ServiceProvider.GetRequiredService<CountHandler>();

            await CreateAccountPayable(context, loggerService, countHandler);
        }

        private async Task CreateAccountPayable(
            AppDbContext context,
            ILoggerService loggerService,
            CountHandler countHandler)
        {
            try
            {
                List<AccountPayable> recurrents = await context.AccountsPayable
                    .Find(x => !x.Deleted && x.IsRecurrent)
                    .ToListAsync();

                if (recurrents.Count == 0) return;

                DateTime now = DateTime.UtcNow;

                foreach (var origin in recurrents)
                {
                    if (origin.TotalInstallments > 0)
                    {
                        long generatedCount = await context.AccountsPayable
                            .CountDocumentsAsync(x =>
                                !x.Deleted &&
                                x.OriginId   == origin.Id &&
                                x.OriginType == "recurrent");

                        if (generatedCount >= origin.TotalInstallments - 1)
                        {
                            var disable = Builders<AccountPayable>.Update
                                .Set(x => x.IsRecurrent, false)
                                .Set(x => x.UpdatedAt,   now);

                            await context.AccountsPayable.UpdateOneAsync(
                                x => x.Id == origin.Id, disable);

                            await loggerService.CreateAsync(new()
                            {
                                Path       = "FinancialWork",
                                Method     = "POST",
                                Message    = $"Recorrência encerrada: {origin.Description} | {generatedCount}/{origin.TotalInstallments - 1} filhos gerados",
                                StatusCode = 200
                            });

                            continue;
                        }
                    }

                    DateTime nextDueDate = CalculateNextDueDate(origin.DueDate, origin.TypeRecurrent);

                    if (nextDueDate > now) continue;

                    bool alreadyGenerated = await context.AccountsPayable
                        .Find(x =>
                            !x.Deleted &&
                            x.OriginId   == origin.Id &&
                            x.OriginType == "recurrent" &&
                            x.DueDate    == nextDueDate)
                        .AnyAsync();

                    if (alreadyGenerated) continue;
                    
                    long currentInstallment = await context.AccountsPayable
                        .CountDocumentsAsync(x =>
                            !x.Deleted &&
                            x.OriginId   == origin.Id &&
                            x.OriginType == "recurrent");

                    int thisInstallmentNumber = (int)(currentInstallment + 2);

                    AccountPayable newEntry = new()
                    {
                        Code             = await countHandler.NextCountAsync("account-payable"),
                        OriginId         = origin.Id,
                        OriginType       = "recurrent",
                        SupplierId       = origin.SupplierId,
                        Description      = origin.Description,
                        PaymentMethodId  = origin.PaymentMethodId,
                        Amount           = origin.Amount,
                        AmountPaid       = 0,
                        DueDate          = nextDueDate,
                        IssueDate        = now,
                        Status           = "Em Aberto",
                        Notes            = origin.Notes,
                        ChartOfAccountId = origin.ChartOfAccountId,
                        IsRecurrent      = false,
                        TypeRecurrent    = string.Empty,
                        InstallmentNumber = thisInstallmentNumber,
                        TotalInstallments = origin.TotalInstallments,
                    };

                    await context.AccountsPayable.InsertOneAsync(newEntry);

                    var update = Builders<AccountPayable>.Update
                        .Set(x => x.DueDate,   nextDueDate)
                        .Set(x => x.UpdatedAt, now);

                    await context.AccountsPayable.UpdateOneAsync(x => x.Id == origin.Id, update);

                    await loggerService.CreateAsync(new()
                    {
                        Path       = "FinancialWork",
                        Method     = "POST",
                        Message    = $"Gerado: {newEntry.Description} | Vencimento: {nextDueDate:dd/MM/yyyy HH:mm}",
                        StatusCode = 201
                    });
                }
            }
            catch (Exception ex)
            {
                await loggerService.CreateAsync(new()
                {
                    Path       = "FinancialWork",
                    Method     = "POST",
                    Message    = $"Erro: {ex.Message}",
                    StatusCode = 500
                });
            }
        }

        private static DateTime CalculateNextDueDate(DateTime currentDueDate, string typeRecurrent) =>
            typeRecurrent switch
            {
                "minutes"    => currentDueDate.AddMinutes(1),
                "daily"      => currentDueDate.AddDays(1),
                "weekly"     => currentDueDate.AddDays(7),
                "biweekly"   => currentDueDate.AddDays(15),
                "monthly"    => currentDueDate.AddMonths(1),
                "bimonthly"  => currentDueDate.AddMonths(2),
                "quarterly"  => currentDueDate.AddMonths(3),
                "semiannual" => currentDueDate.AddMonths(6),
                "annual"     => currentDueDate.AddYears(1),
                _            => currentDueDate.AddMonths(1)
            };
    }
}