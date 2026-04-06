using api_camem.src.Configuration;
using api_camem.src.Interfaces;
using api_camem.src.Models;
using api_camem.src.Models.Base;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace api_camem.src.Repository
{
    public class DashboardRepository(AppDbContext context) : IDashboardRepository
    {
        #region FINANCIAL CARDS
        public async Task<ResponseApi<dynamic>> GetAccountReceivableCard(DateTime startDate, DateTime endDate)
        {
            try
            {
                List<AccountReceivable> openAmountList = await context.AccountsReceivable.Find(x => !x.Deleted && x.DueDate.Date > DateTime.UtcNow.Date && x.IssueDate.Date >= startDate.Date && x.IssueDate.Date <= endDate.Date && (x.Status == "Em Aberto" || x.Status == "Recebido Parcial")).ToListAsync();
                decimal openAmount = openAmountList.Sum(x => x.Amount) - openAmountList.Sum(x => x.AmountPaid);
                long openCount = await context.AccountsReceivable.Find(x => !x.Deleted && x.DueDate.Date > DateTime.UtcNow.Date && x.IssueDate.Date >= startDate.Date && x.IssueDate.Date <= endDate.Date && (x.Status == "Em Aberto" || x.Status == "Recebido Parcial")).CountDocumentsAsync();
                


                List<AccountReceivable> cancelAmountList = await context.AccountsReceivable.Find(x => !x.Deleted && x.IssueDate.Date >= startDate.Date && x.IssueDate.Date <= endDate.Date && x.Status == "Cancelado").ToListAsync();
                decimal cancelAmount = cancelAmountList.Sum(x => x.Amount);
                long cancelCount = await context.AccountsReceivable.Find(x => !x.Deleted && x.IssueDate.Date >= startDate.Date && x.IssueDate.Date <= endDate.Date && x.Status == "Cancelado").CountDocumentsAsync();
                


                List<AccountReceivable> overdueAmountList = await context.AccountsReceivable.Find(x => !x.Deleted && x.DueDate.Date < DateTime.UtcNow.Date && x.IssueDate.Date >= startDate.Date && x.IssueDate.Date <= endDate.Date && x.Status != "Cancelado" && x.Status != "Recebido").ToListAsync();
                decimal overdueAmount = overdueAmountList.Sum(x => x.Amount) - overdueAmountList.Sum(x => x.AmountPaid);
                long overdueCount = await context.AccountsReceivable.Find(x => !x.Deleted && x.DueDate.Date < DateTime.UtcNow.Date && x.IssueDate.Date >= startDate.Date && x.IssueDate.Date <= endDate.Date && x.Status != "Cancelado" && x.Status != "Recebido").CountDocumentsAsync();
                
                
                dynamic data = new
                {
                    openAmount,
                    openCount,
                    overdueAmount,
                    overdueCount,
                    cancelAmount,
                    cancelCount,
                    totalAmount = openAmount + overdueAmount + cancelAmount,
                    totalCount = openCount + overdueCount + cancelCount
                };

                return new(data);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        public async Task<ResponseApi<dynamic>> GetAccountPayableCard(DateTime startDate, DateTime endDate)
        {
            try
            {
                List<AccountPayable> openAmountList = await context.AccountsPayable.Find(x => !x.Deleted && x.DueDate.Date > DateTime.UtcNow.Date && x.IssueDate.Date >= startDate.Date && x.IssueDate.Date <= endDate.Date && (x.Status == "Em Aberto" || x.Status == "Pago Parcial")).ToListAsync();
                decimal openAmount = openAmountList.Sum(x => x.Amount) - openAmountList.Sum(x => x.AmountPaid);
                long openCount = await context.AccountsPayable.Find(x => !x.Deleted && x.DueDate.Date > DateTime.UtcNow.Date && x.IssueDate.Date >= startDate.Date && x.IssueDate.Date <= endDate.Date && (x.Status == "Em Aberto" || x.Status == "Pago Parcial")).CountDocumentsAsync();

                
                List<AccountPayable> cancelAmountList = await context.AccountsPayable.Find(x => !x.Deleted && x.IssueDate.Date >= startDate.Date && x.IssueDate.Date <= endDate.Date && x.Status == "Cancelado").ToListAsync();
                decimal cancelAmount = cancelAmountList.Sum(x => x.Amount);
                long cancelCount = await context.AccountsPayable.Find(x => !x.Deleted && x.IssueDate.Date >= startDate.Date && x.IssueDate.Date <= endDate.Date && x.Status == "Cancelado").CountDocumentsAsync();

                
                List<AccountPayable> overdueAmountList = await context.AccountsPayable.Find(x => !x.Deleted && x.DueDate.Date < DateTime.UtcNow.Date && x.IssueDate.Date >= startDate.Date && x.IssueDate.Date <= endDate.Date && x.Status != "Cancelado" && x.Status != "Pago").ToListAsync();
                decimal overdueAmount = overdueAmountList.Sum(x => x.Amount) - overdueAmountList.Sum(x => x.AmountPaid);
                long overdueCount = await context.AccountsPayable.Find(x => !x.Deleted && x.DueDate.Date < DateTime.UtcNow.Date && x.IssueDate.Date >= startDate.Date && x.IssueDate.Date <= endDate.Date && x.Status != "Cancelado" && x.Status != "Pago").CountDocumentsAsync();


                List<AccountPayable> totalAmountList = await context.AccountsPayable.Find(x => !x.Deleted && x.IssueDate.Date >= startDate.Date && x.IssueDate.Date <= endDate.Date).ToListAsync();
                decimal totalAmount = totalAmountList.Sum(x => x.Amount);
                long totalCount = await context.AccountsPayable.Find(x => !x.Deleted && x.IssueDate.Date >= startDate.Date && x.IssueDate.Date <= endDate.Date).CountDocumentsAsync();

                
                dynamic data = new
                {
                    openAmount,
                    openCount,
                    overdueAmount,
                    overdueCount,
                    cancelAmount,
                    cancelCount,
                    totalAmount = openAmount + overdueAmount + cancelAmount,
                    totalCount = openCount + overdueCount + cancelCount
                };

                return new(data);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        public async Task<ResponseApi<dynamic>> GetCashFlowCard(DateTime startDate, DateTime endDate)
        {
            try
            {
                List<AccountReceivable> entriesList = await context.AccountsReceivable.Find(x => !x.Deleted && x.IssueDate.Date >= startDate.Date && x.IssueDate.Date <= endDate.Date && (x.Status == "Recebido" || x.Status == "Recebido Parcial")).ToListAsync();
                decimal entrieTotal = entriesList.Where(x => x.Status == "Recebido").Sum(x => x.Amount);
                decimal entriePartial = entriesList.Where(x => x.Status == "Recebido Parcial").Sum(x => x.AmountPaid);
                
                List<AccountPayable> exitsList = await context.AccountsPayable.Find(x => !x.Deleted && x.IssueDate.Date >= startDate.Date && x.IssueDate.Date <= endDate.Date && (x.Status == "Pago" || x.Status == "Pago Parcial")).ToListAsync();
                decimal exitTotal = exitsList.Where(x => x.Status == "Pago").Sum(x => x.Amount);
                decimal exitPartial = exitsList.Where(x => x.Status == "Pago Parcial").Sum(x => x.AmountPaid);
                
                decimal entries = entrieTotal + entriePartial;
                decimal exits = exitTotal + exitPartial;

                dynamic data = new
                {
                    entries,
                    exits,
                    balance = entries - exits
                };

                return new(data);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion
        #region FINANCIAL BARS
        public async Task<ResponseApi<dynamic>> GetEntrieExitBar(DateTime startDate, DateTime endDate)
        {
            try
            {
                List<string> categories = GenerateMonths(startDate, endDate);
                List<AccountReceivable> entrieList = await context.AccountsReceivable.Find(x => !x.Deleted && x.IssueDate.Date >= startDate.Date && x.IssueDate.Date <= endDate.Date && (x.Status == "Recebido" || x.Status == "Recebido Parcial") && x.Status != "Cancelado").ToListAsync();

                decimal[] entries = new decimal[categories.Count];

                for (int i = 0; i < entries.Length; i++)
                {
                    entries[i] = 0;
                    string category = categories[i];

                    var arrayCategory = category.Split("/");

                    if(arrayCategory.Length == 2)
                    {
                        int month = Convert.ToInt32(arrayCategory[0]);
                        int year  = Convert.ToInt32(arrayCategory[1]);
                        List<AccountReceivable> listReceivableTotal = entrieList.Where(x => x.IssueDate.Date.Month == month && x.IssueDate.Year == year && x.Status == "Recebido").ToList();
                        List<AccountReceivable> listReceivableParcial = entrieList.Where(x => x.IssueDate.Date.Month == month && x.IssueDate.Year == year && x.Status == "Recebido Parcial").ToList();

                        decimal totalReceivable = listReceivableTotal.Sum(x => x.Amount);
                        decimal parcialReceivable = listReceivableParcial.Sum(x => x.AmountPaid);
                        entries[i] = totalReceivable + parcialReceivable;
                    }
                }
                
                List<AccountPayable> exitList = await context.AccountsPayable.Find(x => !x.Deleted && x.IssueDate.Date >= startDate.Date && x.IssueDate.Date <= endDate.Date && (x.Status == "Pago" || x.Status == "Pago Parcial") && x.Status != "Cancelado").ToListAsync();

                decimal[] exits = new decimal[categories.Count];

                for (int i = 0; i < exits.Length; i++)
                {
                    exits[i] = 0;
                    string category = categories[i];

                    var arrayCategory = category.Split("/");

                    if(arrayCategory.Length == 2)
                    {
                        int month = Convert.ToInt32(arrayCategory[0]);
                        int year  = Convert.ToInt32(arrayCategory[1]);

                        List<AccountPayable> listPayableTotal = exitList.Where(x => x.IssueDate.Date.Month == month && x.IssueDate.Year == year && x.Status == "Pago").ToList();
                        List<AccountPayable> listPayableParcial = exitList.Where(x => x.IssueDate.Date.Month == month && x.IssueDate.Year == year && x.Status == "Pago Parcial").ToList();
                        
                        decimal totalPayable = listPayableTotal.Sum(x => x.Amount);
                        decimal parcialPayable = listPayableParcial.Sum(x => x.AmountPaid);
                        exits[i] = totalPayable + parcialPayable;
                    }
                }

                dynamic data = new
                {
                    entries,
                    exits,
                    categories
                };

                return new(data);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        public async Task<ResponseApi<dynamic>> GetTopRevenueBar(DateTime startDate, DateTime endDate)
        {
            try
            {
                List<ChartOfAccounts> categories = await context.ChartOfAccounts.Find(x => !x.Deleted && x.Type == "receita").ToListAsync();

                List<string> labels = new();
                List<decimal> values = new();
                decimal total = 0; 
                foreach (ChartOfAccounts category in categories)
                {
                    List<AccountReceivable> listReceivableTotal = await context.AccountsReceivable.Find(x => 
                        !x.Deleted && 
                        x.ChartOfAccountId == category.Id &&
                        x.IssueDate.Date >= startDate.Date &&
                        x.IssueDate.Date <= endDate.Date &&
                        x.Status == "Recebido")
                    .ToListAsync();
                    
                    List<AccountReceivable> listReceivableParcial = await context.AccountsReceivable.Find(x => 
                        !x.Deleted && 
                        x.ChartOfAccountId == category.Id &&
                        x.IssueDate.Date >= startDate.Date &&
                        x.IssueDate.Date <= endDate.Date &&
                        x.Status == "Recebido Parcial")
                    .ToListAsync();

                    decimal totalReceivable = listReceivableTotal.Sum(x => x.Amount);
                    decimal parcialReceivable = listReceivableParcial.Sum(x => x.AmountPaid);
                    
                    if(listReceivableTotal.Count > 0 || listReceivableParcial.Count > 0)
                    {
                        labels.Add(category.Name);
                        values.Add(totalReceivable + parcialReceivable);
                        total += totalReceivable + parcialReceivable;
                    }                    
                }

                dynamic data = new
                {
                    categories = labels,
                    balances = values,
                    total
                };

                return new(data);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion
        #region FINANCIAL PIE
        public async Task<ResponseApi<dynamic>> GetExpenseCategoryPie(DateTime startDate, DateTime endDate)
        {
            try
            {
                List<ChartOfAccounts> categories = await context.ChartOfAccounts.Find(x => !x.Deleted && x.Type == "despesa").ToListAsync();

                List<string> labels = new();
                List<decimal> values = new();
                decimal total = 0; 
                foreach (ChartOfAccounts category in categories)
                {
                    List<AccountPayable> listPayableTotal = await context.AccountsPayable.Find(x => 
                        !x.Deleted && 
                        x.ChartOfAccountId == category.Id &&
                        x.IssueDate.Date >= startDate.Date &&
                        x.IssueDate.Date <= endDate.Date &&
                        x.Status == "Pago")
                    .ToListAsync();
                    
                    List<AccountPayable> listPayableParcial = await context.AccountsPayable.Find(x => 
                        !x.Deleted && 
                        x.ChartOfAccountId == category.Id &&
                        x.IssueDate.Date >= startDate.Date &&
                        x.IssueDate.Date <= endDate.Date &&
                        x.Status == "Pago Parcial")
                    .ToListAsync();

                    decimal totalPayable = listPayableTotal.Sum(x => x.Amount);
                    decimal parcialPayable = listPayableParcial.Sum(x => x.AmountPaid);
                    
                    if(listPayableTotal.Count > 0 || listPayableParcial.Count > 0)
                    {
                        labels.Add(category.Name);
                        values.Add(totalPayable + parcialPayable);
                    }
                    
                    total += totalPayable + parcialPayable;
                }

                dynamic data = new
                {
                    labels,
                    values,
                    total
                };

                return new(data);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion
        #region FINANCIAL AREA
        public async Task<ResponseApi<dynamic>> GetEvolutionBalanceArea(DateTime startDate, DateTime endDate)
        {
            try
            {
                List<string> categories = GenerateMonths(startDate, endDate);
                List<AccountReceivable> entrieList = await context.AccountsReceivable.Find(x => !x.Deleted && x.IssueDate.Date >= startDate.Date && x.IssueDate.Date <= endDate.Date && (x.Status == "Recebido" || x.Status == "Recebido Parcial") && x.Status != "Cancelado").ToListAsync();
                List<AccountPayable> exitList = await context.AccountsPayable.Find(x => !x.Deleted && x.IssueDate.Date >= startDate.Date && x.IssueDate.Date <= endDate.Date && (x.Status == "Pago" || x.Status == "Pago Parcial") && x.Status != "Cancelado").ToListAsync();

                decimal[] balances = new decimal[categories.Count];

                for (int i = 0; i < balances.Length; i++)
                {
                    balances[i] = 0;
                    string category = categories[i];
                    var arrayCategory = category.Split("/");

                    if(arrayCategory.Length == 2)
                    {
                        int month = Convert.ToInt32(arrayCategory[0]);
                        int year  = Convert.ToInt32(arrayCategory[1]);

                        List<AccountReceivable> listReceivableTotal = entrieList.Where(x => x.IssueDate.Date.Month == month && x.IssueDate.Year == year && x.Status == "Recebido").ToList();
                        List<AccountReceivable> listReceivableParcial = entrieList.Where(x => x.IssueDate.Date.Month == month && x.IssueDate.Year == year && x.Status == "Recebido Parcial").ToList();

                        decimal totalReceivable = listReceivableTotal.Sum(x => x.Amount);
                        decimal parcialReceivable = listReceivableParcial.Sum(x => x.AmountPaid);

                        List<AccountPayable> listPayableTotal = exitList.Where(x => x.IssueDate.Date.Month == month && x.IssueDate.Year == year && x.Status == "Pago").ToList();
                        List<AccountPayable> listPayableParcial = exitList.Where(x => x.IssueDate.Date.Month == month && x.IssueDate.Year == year && x.Status == "Pago Parcial").ToList();
                        
                        decimal totalPayable = listPayableTotal.Sum(x => x.Amount);
                        decimal parcialPayable = listPayableParcial.Sum(x => x.AmountPaid);

                        decimal receivable = totalReceivable + parcialReceivable;
                        decimal payable = totalPayable + parcialPayable; 
                        balances[i] = receivable - payable;
                    }
                }

                dynamic data = new
                {
                    balances,
                    categories
                };

                return new(data);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion
        #region FUNCTIONS
        private readonly List<string> monthList = ["Jan", "Fev", "Mar", "Abr", "Mai", "Jun", "Jul", "Ago", "Set", "Out", "Nov", "Dez"];
        private List<string> GenerateMonths(DateTime startDate, DateTime endDate)
        {
            List<string> months = new();

            DateTime controlDate = startDate;

            while(controlDate < endDate)
            {
                controlDate = controlDate.AddMonths(1);
                months.Add(controlDate.ToString("MM/yyyy"));
            }

            return months;
        }
        #endregion
    }
}
