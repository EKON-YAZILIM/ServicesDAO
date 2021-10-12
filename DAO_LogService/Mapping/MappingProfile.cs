using AutoMapper;
using DAO_LogService.Models;
using Helpers.Models.DtoModels;
using Helpers.Models.DtoModels.LogDbDto;

namespace DAO_LogService.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ApplicationLog, ApplicationLogDto>();
            CreateMap<ApplicationLogDto, ApplicationLog>();

            CreateMap<ErrorLog, ErrorLogDto>();
            CreateMap<ErrorLogDto, ErrorLog>();

            CreateMap<UserLog, UserLogDto>();
            CreateMap<UserLogDto, UserLog>();

        }
    }
}
