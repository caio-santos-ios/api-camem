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