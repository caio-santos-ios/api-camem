using api_camem.src.Handlers;
using api_camem.src.Interfaces;
using api_camem.src.Models;
using api_camem.src.Models.Base;
using api_camem.src.Shared.DTOs;
using api_camem.src.Shared.Utils;
using AutoMapper;

namespace api_camem.src.Services
{
    public class SituationService(ISituationRepository repository, CountHandler countHandler, IMapper _mapper) : ISituationService
{
    #region READ
    public async Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request)
    {
        try
        {
            PaginationUtil<Situation> pagination = new(request.QueryParams);
            ResponseApi<List<dynamic>> Situationts = await repository.GetAllAsync(pagination);
            int count = await repository.GetCountDocumentsAsync(pagination);
            return new(Situationts.Data, count, pagination.PageNumber, pagination.PageSize);
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    public async Task<ResponseApi<List<dynamic>>> GetSelectAsync(GetAllDTO request)
    {
        try
        {
            PaginationUtil<Situation> pagination = new(request.QueryParams);
            ResponseApi<List<dynamic>> Situationts = await repository.GetAllAsync(pagination);
            return new(Situationts.Data);
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
            ResponseApi<dynamic?> Situationt = await repository.GetByIdAggregateAsync(id);
            if(Situationt.Data is null) return new(null, 404, "Situação não encontrada");
            return new(Situationt.Data);
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }
    #endregion
    
    #region CREATE
    public async Task<ResponseApi<Situation?>> CreateAsync(CreateSituationDTO request)
    {
        try
        {
            ResponseApi<List<Situation>> situations = await repository.GetByAppearsOnPanelAsync();
            
            if(situations.Data is null ||situations.Data.Count >= 4 && request.AppearsOnPanel) return new(null, 400, "É permitido somente 4 situações fazer parte do filtro.");

            Situation category = _mapper.Map<Situation>(request);
            category.Code = await countHandler.NextCountAsync("situations");
            ResponseApi<Situation?> response = await repository.CreateAsync(category);

            if(response.Data is null) return new(null, 400, "Falha ao criar Situação.");
            return new(response.Data, 201, "Situação criada com sucesso.");
        }
        catch
        { 
            return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde");
        }
    }
    #endregion
    
    #region UPDATE
    public async Task<ResponseApi<Situation?>> UpdateAsync(UpdateSituationDTO request)
    {
        try
        {
            ResponseApi<List<Situation>> situations = await repository.GetByAppearsOnPanelAsync();
            if(situations.Data is null) return new(null, 400, "É permitido somente 4 situações fazer parte do filtro.");
            int totalSituation = situations.Data.Where(x => x.Id != request.Id).Count();

            if(totalSituation >= 4 && request.AppearsOnPanel) return new(null, 400, "É permitido somente 4 situações fazer parte do filtro.");
            
            ResponseApi<Situation?> categoryResponse = await repository.GetByIdAsync(request.Id);
            if(categoryResponse.Data is null) return new(null, 404, "Falha ao atualizar");

            Situation category = _mapper.Map<Situation>(request);
            category.UpdatedAt = DateTime.UtcNow;
            category.CreatedAt = categoryResponse.Data.CreatedAt;

            ResponseApi<Situation?> response = await repository.UpdateAsync(category);
            if(!response.IsSuccess) return new(null, 400, "Falha ao atualizar");
            return new(response.Data, 201, "Atualizada com sucesso");
        }
        catch
        {
            return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
        }
    }

    #endregion
    
    #region DELETE
    public async Task<ResponseApi<Situation>> DeleteAsync(string id)
    {
        try
        {
            ResponseApi<Situation> Situation = await repository.DeleteAsync(id);
            if(!Situation.IsSuccess) return new(null, 400, Situation.Message);
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