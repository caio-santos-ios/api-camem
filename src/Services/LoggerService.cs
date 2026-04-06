using api_camem.src.Interfaces;
using api_camem.src.Models;
using api_camem.src.Models.Base;
using api_camem.src.Shared.DTOs;
using api_camem.src.Shared.Utils;
using AutoMapper;

namespace api_camem.src.Services
{
    public class LoggerService(ILoggerRepository loggerRepository, IMapper _mapper) : ILoggerService
    {
        #region READ
        public async Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request)
        {
            try
            {
                PaginationUtil<Logger> pagination = new(request.QueryParams);
                ResponseApi<List<dynamic>> loggers = await loggerRepository.GetAllAsync(pagination);
                int count = await loggerRepository.GetCountDocumentsAsync(pagination);
                return new(loggers.Data, count, pagination.PageNumber, pagination.PageSize);
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
                ResponseApi<dynamic?> logger = await loggerRepository.GetByIdAggregateAsync(id);
                if(logger.Data is null) return new(null, 404, "Log não encontrado");
                return new(logger.Data);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion

        #region CREATE
        public async Task<ResponseApi<Logger?>> CreateAsync(CreateLoggerDTO request)
        {
            try
            {
                Logger logger = _mapper.Map<Logger>(request);
                ResponseApi<Logger?> response = await loggerRepository.CreateAsync(logger);
                if(response.Data is null) return new(null, 400, "Falha ao criar conta.");
                
                return new(null, 201, "Log criado com sucesso");
            }
            catch
            {                
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde");
            }
        }
        
        #endregion
        
        #region UPDATE
        public async Task<ResponseApi<Logger?>> UpdateAsync(UpdateLoggerDTO request)
        {
            try
            {
                ResponseApi<Logger?> loggerResponse = await loggerRepository.GetByIdAsync(request.Id);

                Logger logger = _mapper.Map<Logger>(request);
                logger.UpdatedAt = DateTime.UtcNow;

                
                ResponseApi<Logger?> response = await loggerRepository.UpdateAsync(logger);
                if(!response.IsSuccess) return new(null, 400, "Falha ao atualizar");

                return new(response.Data, 201, "Atualizado com sucesso");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion
        
        #region DELETE
        public async Task<ResponseApi<Logger>> DeleteAsync(DeleteDTO request)
        {
            try
            {
                ResponseApi<Logger> logger = await loggerRepository.DeleteAsync(request);
                if(!logger.IsSuccess) return new(null, 400, logger.Message);
                return logger;
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion        
    }
}