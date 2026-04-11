using AutoMapper;
using api_camem.src.Models;
using api_camem.src.Shared.DTOs;



namespace api_camem.src.Configuration
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {            
            #region MASTER DATA
            CreateMap<UpdateUserDTO, User>().ReverseMap();
            
            CreateMap<CreateProfileUserDTO, ProfileUser>().ReverseMap();
            CreateMap<UpdateProfileUserDTO, ProfileUser>().ReverseMap();
            
            CreateMap<CreateCustomerDTO, Customer>().ReverseMap();
            CreateMap<UpdateCustomerDTO, Customer>().ReverseMap();

            CreateMap<CreateSupplierDTO, Supplier>().ReverseMap();
            CreateMap<UpdateSupplierDTO, Supplier>().ReverseMap(); 
            #endregion

            #region EVENT
            CreateMap<CreateEventDTO, Event>().ReverseMap();
            CreateMap<UpdateEventDTO, Event>().ReverseMap();
            
            CreateMap<CreateEventParticipantDTO, EventParticipant>().ReverseMap();
            CreateMap<UpdateEventParticipantDTO, EventParticipant>().ReverseMap();
            #endregion
            
            #region CERTIFICATE
            CreateMap<CreateCertificateDTO, Certificate>().ReverseMap();
            CreateMap<UpdateCertificateDTO, Certificate>().ReverseMap();
            #endregion

            #region FINANCIAL
            CreateMap<CreateAccountReceivableDTO, AccountReceivable>().ReverseMap();
            CreateMap<UpdateAccountReceivableDTO, AccountReceivable>().ReverseMap();

            CreateMap<CreateAccountPayableDTO, AccountPayable>().ReverseMap();
            CreateMap<UpdateAccountPayableDTO, AccountPayable>().ReverseMap();

            CreateMap<CreatePaymentMethodDTO, PaymentMethod>().ReverseMap();
            CreateMap<UpdatePaymentMethodDTO, PaymentMethod>().ReverseMap();
            #endregion
            
            CreateMap<CreateServiceOrderDTO, ServiceOrder>().ReverseMap();
            CreateMap<UpdateServiceOrderDTO, ServiceOrder>().ReverseMap();
            CreateMap<CreateServiceOrderItemDTO, ServiceOrderItem>().ReverseMap();
            CreateMap<UpdateServiceOrderItemDTO, ServiceOrderItem>().ReverseMap();

            CreateMap<CreateSituationDTO, Situation>().ReverseMap();
            CreateMap<UpdateSituationDTO, Situation>().ReverseMap();

            #region SETTINGS
            CreateMap<CreateLoggerDTO, Logger>().ReverseMap();
            CreateMap<UpdateLoggerDTO, Logger>().ReverseMap();
            
            CreateMap<CreateTemplateDTO, Template>().ReverseMap();
            CreateMap<UpdateTemplateDTO, Template>().ReverseMap();

            CreateMap<CreateTriggerDTO, Trigger>().ReverseMap();
            CreateMap<UpdateTriggerDTO, Trigger>().ReverseMap();
            #endregion
        }
    }
}