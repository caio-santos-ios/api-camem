using api_camem.src.Handlers;
using api_camem.src.Interfaces;
using api_camem.src.Models;
using api_camem.src.Models.Base;
using api_camem.src.Shared.DTOs;
using api_camem.src.Shared.Utils;
using AutoMapper;

namespace api_camem.src.Services
{
    public class CertificateService(ICertificateRepository repository, IMapper _mapper) : ICertificateService
    {
        #region READ
        public async Task<ResponseApi<PaginationApi<List<dynamic>>>> GetAllAsync(GetAllDTO request)
        {
            try
            {
                PaginationUtil<Certificate> pagination = new(request.QueryParams);
                ResponseApi<List<dynamic>> certificates = await repository.GetAllAsync(pagination);
                int count = await repository.GetCountDocumentsAsync(pagination);
                PaginationApi<List<dynamic>> data = new(certificates.Data, count, pagination.PageNumber, pagination.PageSize); 
                return new (data, 200, "Certificados listados com sucesso");
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
                ResponseApi<dynamic?> certificate = await repository.GetByIdAggregateAsync(id);
                if(certificate.Data is null) return new(null, 404, "Certificado não encontrada");
                return new(certificate.Data, 200, "Certificado obtido com sucesso");
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
                PaginationUtil<Certificate> pagination = new(request.QueryParams);
                ResponseApi<List<dynamic>> certificates = await repository.GetSelectAsync(pagination);
                return new(certificates.Data, 200, "Certificados listados com sucesso");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion
        
        #region CREATE
        public async Task<ResponseApi<Certificate?>> CreateAsync(CreateCertificateDTO request)
        {
            try
            {
                Certificate certificate = _mapper.Map<Certificate>(request);

                ResponseApi<Certificate?> response = await repository.CreateAsync(certificate);

                if(response.Data is null) return new(null, 400, "Falha ao criar Certificado.");
                return new(response.Data, 201, "Certificado criado com sucesso.");
            }
            catch
            { 
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde");
            }
        }
        #endregion
        
        #region UPDATE
        public async Task<ResponseApi<Certificate?>> UpdateAsync(UpdateCertificateDTO request)
        {
            try
            {
                ResponseApi<Certificate?> certificateResponse = await repository.GetByIdAsync(request.Id);
                if(certificateResponse.Data is null) return new(null, 404, "Falha ao atualizar");
                
                Certificate certificate = _mapper.Map<Certificate>(request);
                certificate.UpdatedAt = DateTime.UtcNow;
                certificate.CreatedAt = certificateResponse.Data.CreatedAt;

                ResponseApi<Certificate?> response = await repository.UpdateAsync(certificate);
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
        public async Task<ResponseApi<Certificate>> DeleteAsync(string id)
        {
            try
            {
                ResponseApi<Certificate> certificate = await repository.DeleteAsync(id);
                if(!certificate.IsSuccess) return new(null, 400, certificate.Message);
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