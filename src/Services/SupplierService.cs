using api_camem.src.Interfaces;
using api_camem.src.Models;
using api_camem.src.Models.Base;
using api_camem.src.Shared.DTOs;
using api_camem.src.Shared.Utils;
using AutoMapper;

namespace api_camem.src.Services
{
    public class SupplierService(ISupplierRepository repository, IMapper _mapper) : ISupplierService
    {
        #region READ
        public async Task<ResponseApi<PaginationApi<List<dynamic>>>> GetAllAsync(GetAllDTO request)
        {
            try
            {
                PaginationUtil<Supplier> pagination = new(request.QueryParams);
                ResponseApi<List<dynamic>> suppliers = await repository.GetAllAsync(pagination);
                int count = await repository.GetCountDocumentsAsync(pagination);
                PaginationApi<List<dynamic>> data = new(suppliers.Data, count, pagination.PageNumber, pagination.PageSize);
                return new(data, 200, "Fornecedores listados com sucesso");
            }
            catch(Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }
        
        public async Task<ResponseApi<List<dynamic>>> GetAutocompleteAsync(GetAllDTO request)
        {
            try
            {
                PaginationUtil<Supplier> pagination = new(request.QueryParams);
                ResponseApi<List<dynamic>> suppliers = await repository.GetAutocompleteAsync(pagination);
                return new(suppliers.Data);
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
                ResponseApi<dynamic?> supplier = await repository.GetByIdAggregateAsync(id);
                if(supplier.Data is null) return new(null, 404, "Fornecedor não encontrado");
                return new(supplier.Data);
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
                PaginationUtil<Supplier> pagination = new(request.QueryParams);
                ResponseApi<List<dynamic>> suppliers = await repository.GetSelectAsync(pagination);
                return new(suppliers.Data);
            }
            catch(Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }
        #endregion
        
        #region CREATE
        public async Task<ResponseApi<Supplier?>> CreateAsync(CreateSupplierDTO request)
        {
            try
            {
                string messageName = request.Type == "F" ? "O Nome é obrigatório" : "A Razão Social é obrigatória";
                string messageDocument = request.Type == "F" ? "O CPF é obrigatório" : "O CNPJ é obrigatório";

                if(string.IsNullOrEmpty(request.CorporateName)) return new(null, 400, messageName);
                if(string.IsNullOrEmpty(request.Document)) return new(null, 400, messageDocument);
                if(string.IsNullOrEmpty(request.Email)) return new(null, 400, "O E-mail é obrigatório");

                ResponseApi<Supplier?> existedDocument = await repository.GetByDocumentAsync(request.Document, "");
                string messageExited = request.Type == "F" ? "Este CPF já está sendo utilizado por outro Fornecedor" : "Este CNPJ já está sendo utilizado por outro Fornecedor";
                if(existedDocument.Data is not null) return new(null, 400, messageExited);

                ResponseApi<Supplier?> existedEmail = await repository.GetByEmailAsync(request.Email, "");
                if(existedEmail.Data is not null) return new(null, 400, "Este e-mail já está sendo utilizado por outro Fornecedor");

                Supplier supplier = _mapper.Map<Supplier>(request);
                if(request.Type == "F")
                {
                    supplier.TradeName = request.CorporateName;
                }
                ResponseApi<Supplier?> response = await repository.CreateAsync(supplier);

                if(response.Data is null) return new(null, 400, "Falha ao criar Fornecedor.");

                return new(response.Data, 201, "Fornecedor criado com sucesso.");
            }
            catch
            { 
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde");
            }
        }
        public async Task<ResponseApi<Supplier?>> CreateMinimalAsync(CreateSupplierMinimalDTO request)
        {
            try
            {
                ResponseApi<Supplier?> response = await repository.CreateAsync(new ()
                {
                    CreatedBy = request.CreatedBy,
                    CorporateName = request.Name,
                    TradeName = request.Name
                });

                if(response.Data is null) return new(null, 400, "Falha ao criar Fornecedor.");

                return new(response.Data, 201, "Fornecedor criado com sucesso.");
            }
            catch
            { 
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde");
            }
        }
        #endregion
        
        #region UPDATE
        public async Task<ResponseApi<Supplier?>> UpdateAsync(UpdateSupplierDTO request)
        {
            try
            {
                ResponseApi<Supplier?> SupplierResponse = await repository.GetByIdAsync(request.Id);
                if(SupplierResponse.Data is null) return new(null, 404, "Falha ao atualizar");

                string messageName = request.Type == "F" ? "O Nome é obrigatório" : "A Razão Social é obrigatória";
                string messageDocument = request.Type == "F" ? "O CPF é obrigatório" : "O CNPJ é obrigatório";

                if(string.IsNullOrEmpty(request.CorporateName)) return new(null, 400, messageName);
                if(string.IsNullOrEmpty(request.Document)) return new(null, 400, messageDocument);
                if(string.IsNullOrEmpty(request.Email)) return new(null, 400, "O E-mail é obrigatório");

                ResponseApi<Supplier?> existedDocument = await repository.GetByDocumentAsync(request.Document, request.Id);
                string messageExited = request.Type == "F" ? "Este CPF já está sendo utilizado por outro Fornecedor" : "Este CNPJ já está sendo utilizado por outro Fornecedor";
                if(existedDocument.Data is not null) return new(null, 400, messageExited);

                ResponseApi<Supplier?> existedEmail = await repository.GetByEmailAsync(request.Email, request.Id);
                if(existedEmail.Data is not null) return new(null, 400, "Este e-mail já está sendo utilizado por outro Fornecedor");

                ResponseApi<Supplier?> supplierResponse = await repository.GetByIdAsync(request.Id);
                if(supplierResponse.Data is null) return new(null, 404, "Falha ao atualizar");
                
                Supplier supplier = _mapper.Map<Supplier>(request);
                supplier.UpdatedAt = DateTime.UtcNow;
                supplier.CreatedAt = supplierResponse.Data.CreatedAt;
                if(request.Type == "F")
                {
                    supplier.TradeName = request.CorporateName;
                };
                ResponseApi<Supplier?> response = await repository.UpdateAsync(supplier);
                if(!response.IsSuccess) return new(null, 400, "Falha ao atualizar");            

                return new(response.Data, 200, "Atualizado com sucesso");
            }
            catch(Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }
        #endregion
        
        #region DELETE
        public async Task<ResponseApi<Supplier>> DeleteAsync(string id)
        {
            try
            {
                ResponseApi<Supplier> supplier = await repository.DeleteAsync(id);
                if(!supplier.IsSuccess) return new(null, 400, supplier.Message);
                return new(null, 204, "Excluído com sucesso");
            }
            catch(Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }
        #endregion 
    }
}