using AutoMapper;
using DAO_ReputationService.Models;
using Helpers.Models.DtoModels.VoteDbDto;

namespace DAO_ReputationService.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserReputation, UserReputationDto>();
            CreateMap<UserReputationDto, UserReputation>();

            CreateMap<UserReputationHistory, UserReputationHistoryDto>();
            CreateMap<UserReputationHistoryDto, UserReputationHistory>();

         

        }
    }
}
