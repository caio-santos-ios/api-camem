using api_camem.src.Handlers;
using api_camem.src.Interfaces;
using api_camem.src.Models;
using api_camem.src.Models.Base;
using api_camem.src.Shared.DTOs;
using api_camem.src.Shared.Templates;
using api_camem.src.Shared.Utils;
using AutoMapper;

namespace api_camem.src.Services
{
    public class TemplateService
    (
        ITemplateRepository repository,
        MailHandler mailHandler, 
        MailTemplate mailTemplate,
        IUserRepository userRepository,
        IMapper _mapper
    ) : ITemplateService
    {
        #region READ
        public async Task<PaginationApi<List<dynamic>>> GetAllAsync(GetAllDTO request)
        {
            try
            {
                PaginationUtil<Template> pagination = new(request.QueryParams);
                ResponseApi<List<dynamic>> templates = await repository.GetAllAsync(pagination);
                int count = await repository.GetCountDocumentsAsync(pagination);
                return new(templates.Data, count, pagination.PageNumber, pagination.PageSize);
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
                ResponseApi<dynamic?> Template = await repository.GetByIdAggregateAsync(id);
                if(Template.Data is null) return new(null, 404, "Template não encontrada");
                return new(Template.Data);
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
                PaginationUtil<Template> pagination = new(request.QueryParams);
                ResponseApi<List<dynamic>> template = await repository.GetSelectAsync(pagination);
                return new(template.Data);
            }
            catch(Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        } 
        #endregion
        
        #region CREATE
        public async Task<ResponseApi<Template?>> CreateAsync(CreateTemplateDTO request)
        {
            try
            {
                Template template = _mapper.Map<Template>(request);

                ResponseApi<Template?> response = await repository.CreateAsync(template);

                if(response.Data is null) return new(null, 400, "Falha ao criar Template.");
                return new(response.Data, 201, "Template criada com sucesso.");
            }
            catch
            { 
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde");
            }
        }

        #endregion
        
        #region UPDATE
        public async Task<ResponseApi<Template?>> UpdateAsync(UpdateTemplateDTO request)
        {
            try
            {
                ResponseApi<Template?> templateResponse = await repository.GetByIdAsync(request.Id);
                if(templateResponse.Data is null) return new(null, 404, "Falha ao atualizar");
                
                Template template = _mapper.Map<Template>(request);
                template.UpdatedAt = DateTime.UtcNow;
                template.CreatedAt = templateResponse.Data.CreatedAt;

                ResponseApi<Template?> response = await repository.UpdateAsync(template);
                if(!response.IsSuccess) return new(null, 400, "Falha ao atualizar");
                return new(response.Data, 200, "Atualizado com sucesso");
            }
            catch(Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }
        public async Task<ResponseApi<Template?>> SendAsync(SendTemplateDTO request)
        {
            try
            {
                ResponseApi<Template?> templateResponse = await repository.GetByIdAsync(request.Id);
                if(templateResponse.Data is null) return new(null, 404, "Falha ao atualizar");

                ResponseApi<User?> userResponse = await userRepository.GetByIdAsync(request.CreatedBy);
                if(userResponse.Data is null) return new(null, 404, "Falha ao atualizar");

                switch(request.Code)
                {
                    case "FORGOT_PASSWORD_WEB":
                        await mailHandler.SendMailAsync(userResponse.Data.Email, "Recuperação de Senha", await mailTemplate.ForgotPasswordWeb(userResponse.Data.Name, userResponse.Data.CodeAccess));
                        break;

                    case "FIRST_ACCESS":
                        await mailHandler.SendMailAsync(userResponse.Data.Email, "Primeiro Acesso", await mailTemplate.FirstAccess(userResponse.Data.Name));
                        break;

                    case "CONFIRM_ACCOUNT":
                        await mailHandler.SendMailAsync(userResponse.Data.Email, "Confirmar Conta", await mailTemplate.ConfirmAccount(userResponse.Data.Name, userResponse.Data.CodeAccess));
                        break;

                    case "NEW_CODE_CONFIRM_ACCOUNT":
                        await mailHandler.SendMailAsync(userResponse.Data.Email, "Novo Código de Confirmação de Conta", await mailTemplate.NewCodeConfirmAccount(userResponse.Data.Name, userResponse.Data.CodeAccess));
                        break;

                    case "NEW_LINK_CODE_CONFIRM_ACCOUNT":
                        await mailHandler.SendMailAsync(userResponse.Data.Email, "Novo Link de Confirmação de Conta", await mailTemplate.NewLinkCodeConfirmAccount(userResponse.Data.Name, userResponse.Data.CodeAccess));
                        break;
                }

                return new(templateResponse.Data, 200, "Enviado com sucesso");
            }
            catch(Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }
        #endregion
        
        #region DELETE
        public async Task<ResponseApi<Template>> DeleteAsync(DeleteDTO request)
        {
            try
            {
                ResponseApi<Template> template = await repository.DeleteAsync(request);
                if(!template.IsSuccess) return new(null, 400, template.Message);
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