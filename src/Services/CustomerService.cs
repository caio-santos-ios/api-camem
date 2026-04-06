using api_camem.src.Interfaces;
using api_camem.src.Models;
using api_camem.src.Models.Base;
using api_camem.src.Shared.DTOs;
using api_camem.src.Shared.Utils;
using AutoMapper;

namespace api_camem.src.Services
{
    public class CustomerService(ICustomerRepository repository, IMapper _mapper) : ICustomerService
    {
        #region READ
        public async Task<ResponseApi<PaginationApi<List<dynamic>>>> GetAllAsync(GetAllDTO request)
        {
            try
            {
                PaginationUtil<Customer> pagination = new(request.QueryParams);
                ResponseApi<List<dynamic>> customers = await repository.GetAllAsync(pagination);
                int count = await repository.GetCountDocumentsAsync(pagination);
                PaginationApi<List<dynamic>> data = new(customers.Data, count, pagination.PageNumber, pagination.PageSize);
                return new(data, 200, "Clientes listados com sucesso");
            }
            catch(Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }
        public async Task<ResponseApi<List<dynamic>>> GetMovementAsync(GetAllDTO request)
        {
            try
            {
                PaginationUtil<Customer> pagination = new(request.QueryParams);
                ResponseApi<List<dynamic>> customers = await repository.GetMovementAsync(pagination);
                return new(customers.Data);
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
                ResponseApi<dynamic?> customer = await repository.GetByIdAggregateAsync(id);
                if(customer.Data is null) return new(null, 404, "Cliente não encontrada");
                return new(customer.Data);
            }
            catch(Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }
        #endregion
        
        #region CREATE
        public async Task<ResponseApi<Customer?>> CreateAsync(CreateCustomerDTO request)
        {
            try
            {
                string messageName = request.Type == "F" ? "O Nome é obrigatório" : "A Razão Social é obrigatória";
                string messageDocument = request.Type == "F" ? "O CPF é obrigatório" : "O CNPJ é obrigatório";

                if(string.IsNullOrEmpty(request.CorporateName)) return new(null, 400, messageName);
                if(string.IsNullOrEmpty(request.Document)) return new(null, 400, messageDocument);
                if(string.IsNullOrEmpty(request.Email)) return new(null, 400, "O E-mail é obrigatório");

                ResponseApi<Customer?> existedDocument = await repository.GetByDocumentAsync(request.Document, "");
                string messageExited = request.Type == "F" ? "Este CPF já está sendo utilizado por outro Cliente" : "Este CNPJ já está sendo utilizado por outro Cliente";
                if(existedDocument.Data is not null) return new(null, 400, messageExited);

                ResponseApi<Customer?> existedEmail = await repository.GetByEmailAsync(request.Email, "");
                if(existedEmail.Data is not null) return new(null, 400, "Este e-mail já está sendo utilizado por outro Cliente");
                
                Customer Customer = _mapper.Map<Customer>(request);
                if(request.Type == "F")
                {
                    Customer.TradeName = request.CorporateName;
                };
                
                ResponseApi<Customer?> response = await repository.CreateAsync(Customer);

                if(response.Data is null) return new(null, 400, "Falha ao criar Cliente.");
                return new(response.Data, 201, "Cliente criado com sucesso.");
            }
            catch
            { 
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde");
            }
        }
        public async Task<ResponseApi<Customer?>> CreateMinimalAsync(CreateCustomerMinimalDTO request)
        {
            try
            {
                if(string.IsNullOrEmpty(request.CorporateName)) return new(null, 400, request.Type == "F" ? "O Nome é obrigatório" : "A Razão Social é obrigatória");            

                if(!string.IsNullOrEmpty(request.Document))
                {
                    ResponseApi<Customer?> existedDocument = await repository.GetByDocumentAsync(request.Document, "");
                    string messageExited = request.Type == "F" ? "Este CPF já está sendo utilizado por outro Cliente" : "Este CNPJ já está sendo utilizado por outro Cliente";
                    if(existedDocument.Data is not null) return new(null, 400, messageExited);
                }

                if(!string.IsNullOrEmpty(request.Email))
                {
                    ResponseApi<Customer?> existedEmail = await repository.GetByEmailAsync(request.Email, "");
                    if(existedEmail.Data is not null) return new(null, 400, "Este e-mail já está sendo utilizado por outro Cliente");
                }

                ResponseApi<Customer?> response = await repository.CreateAsync(new()
                {
                    CreatedBy = request.CreatedBy,
                    CorporateName = request.CorporateName,
                    TradeName = request.Type == "F" ? request.CorporateName : request.TradeName,
                    Type = request.Type,
                    Document = request.Document,
                    Email = request.Email,
                    Phone = request.Phone
                });

                if(response.Data is null) return new(null, 400, "Falha ao criar Cliente.");
                return new(response.Data, 201, "Cliente criado com sucesso.");
            }
            catch
            { 
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde");
            }
        }
        #endregion
        
        #region UPDATE
        public async Task<ResponseApi<Customer?>> UpdateAsync(UpdateCustomerDTO request)
        {
            try
            {
                ResponseApi<Customer?> CustomerResponse = await repository.GetByIdAsync(request.Id);
                if(CustomerResponse.Data is null) return new(null, 404, "Falha ao atualizar");

                string messageName = request.Type == "F" ? "O Nome é obrigatório" : "A Razão Social é obrigatória";
                string messageDocument = request.Type == "F" ? "O CPF é obrigatório" : "O CNPJ é obrigatório";

                if(string.IsNullOrEmpty(request.CorporateName)) return new(null, 400, messageName);
                if(string.IsNullOrEmpty(request.Document)) return new(null, 400, messageDocument);
                if(string.IsNullOrEmpty(request.Email)) return new(null, 400, "O E-mail é obrigatório");

                ResponseApi<Customer?> existedDocument = await repository.GetByDocumentAsync(request.Document, request.Id);
                string messageExited = request.Type == "F" ? "Este CPF já está sendo utilizado por outro Cliente" : "Este CNPJ já está sendo utilizado por outro Cliente";
                if(existedDocument.Data is not null) return new(null, 400, messageExited);

                ResponseApi<Customer?> existedEmail = await repository.GetByEmailAsync(request.Email, request.Id);
                if(existedEmail.Data is not null) return new(null, 400, "Este e-mail já está sendo utilizado por outro Cliente");
                
                Customer customer = _mapper.Map<Customer>(request);
                customer.UpdatedAt = DateTime.UtcNow;
                customer.CreatedAt = CustomerResponse.Data.CreatedAt;

                if(request.Type == "F")
                {
                    customer.TradeName = request.CorporateName;
                };

                ResponseApi<Customer?> response = await repository.UpdateAsync(customer);
                if(!response.IsSuccess) return new(null, 400, "Falha ao atualizar");
                return new(response.Data, 200, "Atualizada com sucesso");
            }
            catch(Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }
        public async Task<ResponseApi<Customer?>> UpdateMinimalAsync(UpdateCustomerMinimalDTO request)
        {
            try
            {
                if(string.IsNullOrEmpty(request.CorporateName)) return new(null, 400, request.Type == "F" ? "O Nome é obrigatório" : "A Razão Social é obrigatória");            

                // ResponseApi<Customer?> existedDocument = await repository.GetByDocumentAsync(request.Document, "");
                // string messageExited = request.Type == "F" ? "Este CPF já está sendo utilizado por outro Cliente" : "Este CNPJ já está sendo utilizado por outro Cliente";
                // if(existedDocument.Data is not null) return new(null, 400, messageExited);

                // ResponseApi<Customer?> existedEmail = await repository.GetByEmailAsync(request.Email, "");
                // if(existedEmail.Data is not null) return new(null, 400, "Este e-mail já está sendo utilizado por outro Cliente");
                if(!string.IsNullOrEmpty(request.Document))
                {
                    ResponseApi<Customer?> existedDocument = await repository.GetByDocumentAsync(request.Document, request.Id);
                    string messageExited = request.Type == "F" ? "Este CPF já está sendo utilizado por outro Cliente" : "Este CNPJ já está sendo utilizado por outro Cliente";
                    if(existedDocument.Data is not null) return new(null, 400, messageExited);
                }

                if(!string.IsNullOrEmpty(request.Email))
                {
                    ResponseApi<Customer?> existedEmail = await repository.GetByEmailAsync(request.Email, request.Id);
                    if(existedEmail.Data is not null) return new(null, 400, "Este e-mail já está sendo utilizado por outro Cliente");
                }

                ResponseApi<Customer?> response = await repository.CreateAsync(new()
                {
                    CreatedBy = request.CreatedBy,
                    CorporateName = request.CorporateName,
                    TradeName = request.Type == "F" ? request.CorporateName : request.TradeName,
                    Type = request.Type,
                    Document = request.Document,
                    Email = request.Email,
                    Phone = request.Phone
                });

                if(response.Data is null) return new(null, 400, "Falha ao criar Cliente.");
                return new(response.Data, 200, "Cliente criado com sucesso.");
            }
            catch
            { 
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde");
            }
        }
        #endregion
        
        #region DELETE
        public async Task<ResponseApi<Customer>> DeleteAsync(string id)
        {
            try
            {
                ResponseApi<Customer> customer = await repository.DeleteAsync(id);
                if(!customer.IsSuccess) return new(null, 400, customer.Message);
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