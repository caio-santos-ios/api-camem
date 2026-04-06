using api_camem.src.Handlers;
using api_camem.src.Interfaces;
using api_camem.src.Models;
using api_camem.src.Models.Base;
using api_camem.src.Shared.DTOs;
using api_camem.src.Shared.Utils;
using AutoMapper;

namespace api_camem.src.Services
{
    public class ServiceOrderService(IServiceOrderRepository repository, IAccountReceivableService accountReceivableService, CountHandler countHandler, IMapper _mapper) : IServiceOrderService
    {
        #region READ
        public async Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request)
        {
            try
            {
                PaginationUtil<ServiceOrder> pagination = new(request.QueryParams);
                ResponseApi<List<dynamic>> serviceOrders = await repository.GetAllAsync(pagination);
                int count = await repository.GetCountDocumentsAsync(pagination);
                return new(serviceOrders.Data, count, pagination.PageNumber, pagination.PageSize);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }

        public async Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id)
        {
            try
            {
                ResponseApi<dynamic?> serviceOrder = await repository.GetByIdAggregateAsync(id);
                if (serviceOrder.Data is null) return new(null, 404, "Ordem de Serviço não encontrada");
                return new(serviceOrder.Data);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }

        public async Task<ResponseApi<dynamic?>> CheckWarrantyAsync(string? customerId, string? serialImei)
        {
            try
            {
                ResponseApi<dynamic?> result = await repository.CheckWarrantyAsync(customerId, serialImei);
                return new(result.Data);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion

        #region CREATE
        public async Task<ResponseApi<ServiceOrder?>> CreateAsync(CreateServiceOrderDTO request)
        {
            try
            {
                ServiceOrder serviceOrder = _mapper.Map<ServiceOrder>(request);
                serviceOrder.Status = "Em Aberto";
                serviceOrder.Code = await countHandler.NextCountAsync("service_orders");

                ResponseApi<ServiceOrder?> response = await repository.CreateAsync(serviceOrder);
                if (response.Data is null) return new(null, 400, "Falha ao criar Ordem de Serviço.");

                return new(response.Data, 201, "Ordem de Serviço criada com sucesso.");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion

        #region UPDATE
        public async Task<ResponseApi<ServiceOrder?>> UpdateAsync(UpdateServiceOrderDTO request)
        {
            try
            {
                ResponseApi<ServiceOrder?> existing = await repository.GetByIdAsync(request.Id);
                if (existing.Data is null) return new(null, 404, "Ordem de Serviço não encontrada");
                
                existing.Data.UpdatedAt = DateTime.UtcNow;
                existing.Data.UpdatedBy = request.UpdatedBy;
                existing.Data.CustomerId = request.CustomerId;
                existing.Data.OpeningDate = request.OpeningDate;
                existing.Data.ForecasDate = request.ForecasDate;
                existing.Data.Priority = request.Priority;
                existing.Data.Description = request.Description;
                existing.Data.Notes = request.Notes;

                ResponseApi<ServiceOrder?> response = await repository.UpdateAsync(existing.Data);
                if (!response.IsSuccess) return new(null, 400, "Falha ao atualizar Ordem de Serviço.");
                return new(null, 200, "Ordem de Serviço atualizada com sucesso.");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        public async Task<ResponseApi<ServiceOrder?>> UpdateStatusAsync(UpdateStatusServiceOrderDTO request)
        {
            try
            {
                ResponseApi<ServiceOrder?> existing = await repository.GetByIdAsync(request.Id);
                if (existing.Data is null) return new(null, 404, "Ordem de Serviço não encontrada");
                
                existing.Data.UpdatedAt = DateTime.UtcNow;
                existing.Data.UpdatedBy = request.UpdatedBy;
                existing.Data.Status = request.Status;

                ResponseApi<ServiceOrder?> response = await repository.UpdateAsync(existing.Data);
                if (!response.IsSuccess) return new(null, 400, "Falha ao atualizar Ordem de Serviço.");
                return new(null, 200, "Ordem de Serviço atualizada com sucesso.");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }

        public async Task<ResponseApi<ServiceOrder?>> CloseAsync(CloseServiceOrderDTO request)
        {
            try
            {
                ResponseApi<ServiceOrder?> existing = await repository.GetByIdAsync(request.Id);
                if (existing.Data is null) return new(null, 404, "Ordem de Serviço não encontrada");

                ServiceOrder serviceOrder = existing.Data;
                serviceOrder.UpdatedAt = DateTime.UtcNow;
                serviceOrder.UpdatedBy = request.UpdatedBy;
                serviceOrder.Payment = new ServiceOrderPayment
                {
                    PaymentMethodId = request.PaymentMethodId,
                    NumberOfInstallments = request.NumberOfInstallments,
                    PaidAt = DateTime.UtcNow,
                };

                ResponseApi<ServiceOrder?> response = await repository.UpdateAsync(serviceOrder);
                if (!response.IsSuccess) return new(null, 400, "Falha ao encerrar Ordem de Serviço.");

                DateTime issueDate = DateTime.UtcNow;

                for (int i = 0; i < request.NumberOfInstallments; i++)
                {
                    DateTime currentIssue = issueDate.AddMonths(i);
                    DateTime dueDate = currentIssue.AddDays(3);

                    await accountReceivableService.CreateAsync(new CreateAccountReceivableDTO()
                    {
                        CreatedBy = request.UpdatedBy,
                        CustomerId = serviceOrder.CustomerId,
                        Description = $"O.S. nº {serviceOrder.Code}",
                        DueDate = dueDate,
                        InstallmentNumber = i + 1,
                        OriginId = serviceOrder.Id,
                        OriginType = "service-order",
                        TotalInstallments = request.NumberOfInstallments,
                        PaymentMethodId = request.PaymentMethodId,
                        Amount = Math.Truncate(request.Value * 100) / 100,
                        IssueDate = issueDate,
                        IsPaymented = true
                    });
                }

                return new(response.Data, 200, "Ordem de Serviço encerrada com sucesso.");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion

        #region DELETE
        public async Task<ResponseApi<ServiceOrder>> DeleteAsync(string id)
        {
            try
            {
                ResponseApi<ServiceOrder> serviceOrder = await repository.DeleteAsync(id);
                if (!serviceOrder.IsSuccess) return new(null, 400, serviceOrder.Message);
                return new(null, 204, "Excluída com sucesso");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion
    }
}