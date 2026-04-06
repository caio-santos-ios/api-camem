using api_camem.src.Handlers;
using api_camem.src.Interfaces;
using api_camem.src.Models;
using api_camem.src.Models.Base;
using api_camem.src.Shared.DTOs;
using api_camem.src.Shared.Utils;
using AutoMapper;

namespace api_camem.src.Services
{
    public class PaymentMethodService(IPaymentMethodRepository repository, CountHandler countHandler, IMapper _mapper) : IPaymentMethodService
    {
        #region READ
        public async Task<ResponseApi<PaginationApi<List<dynamic>>>> GetAllAsync(GetAllDTO request)
        {
            try
            {
                PaginationUtil<PaymentMethod> pagination = new(request.QueryParams);
                ResponseApi<List<dynamic>> PaymentMethods = await repository.GetAllAsync(pagination);
                int count = await repository.GetCountDocumentsAsync(pagination);
                PaginationApi<List<dynamic>> data = new(PaymentMethods.Data, count, pagination.PageNumber, pagination.PageSize); 
                return new (data, 200, "Formas de pagamento listados com sucesso");
            }
            catch(Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }
        public async Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id)
        {
            try
            {
                ResponseApi<dynamic?> PaymentMethod = await repository.GetByIdAggregateAsync(id);
                if(PaymentMethod.Data is null) return new(null, 404, "Formas de Pagamentos não encontrada");
                return new(PaymentMethod.Data, 200, "Forma de pagamento obtido com sucesso");
            }
            catch(Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }
        public async Task<ResponseApi<List<dynamic>>> GetSelectAsync(GetAllDTO request)
        {
            try
            {
                PaginationUtil<PaymentMethod> pagination = new(request.QueryParams);
                ResponseApi<List<dynamic>> paymentMethods = await repository.GetSelectAsync(pagination);
                return new(paymentMethods.Data, 200, "Formas de pagamento listados com sucesso");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion
        
        #region CREATE
        public async Task<ResponseApi<PaymentMethod?>> CreateAsync(CreatePaymentMethodDTO request)
        {
            try
            {
                PaymentMethod paymentMethod = _mapper.Map<PaymentMethod>(request);

                paymentMethod.Code = await countHandler.NextCountAsync("payment-method");

                ResponseApi<PaymentMethod?> response = await repository.CreateAsync(paymentMethod);

                if(response.Data is null) return new(null, 400, "Falha ao criar Formas de Pagamentos.");
                return new(response.Data, 201, "Forma de Pagamento criada com sucesso.");
            }
            catch
            { 
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde");
            }
        }
        #endregion
        
        #region UPDATE
        public async Task<ResponseApi<PaymentMethod?>> UpdateAsync(UpdatePaymentMethodDTO request)
        {
            try
            {
                ResponseApi<PaymentMethod?> paymentMethodResponse = await repository.GetByIdAsync(request.Id);
                if(paymentMethodResponse.Data is null) return new(null, 404, "Falha ao atualizar");
                
                PaymentMethod paymentMethod = _mapper.Map<PaymentMethod>(request);
                paymentMethod.UpdatedAt = DateTime.UtcNow;
                paymentMethod.CreatedAt = paymentMethodResponse.Data.CreatedAt;
                paymentMethod.Code = paymentMethodResponse.Data.Code;

                ResponseApi<PaymentMethod?> response = await repository.UpdateAsync(paymentMethod);
                if(!response.IsSuccess) return new(null, 400, "Falha ao atualizar");

                return new(response.Data, 200, "Atualizada com sucesso");
            }
            catch(Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }
    
        #endregion
        
        #region DELETE
        public async Task<ResponseApi<PaymentMethod>> DeleteAsync(string id)
        {
            try
            {
                ResponseApi<PaymentMethod> PaymentMethod = await repository.DeleteAsync(id);
                if(!PaymentMethod.IsSuccess) return new(null, 400, PaymentMethod.Message);
                return new(null, 204, "Excluída com sucesso");
            }
            catch(Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }
        #endregion 
    }
}