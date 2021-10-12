using AutoMapper;
using DAO_VotingEngine.Models;
using Helpers.Models.DtoModels.VoteDbDto;

namespace DAO_VotingEngine.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Vote, VoteDto>();
            CreateMap<VoteDto, Vote>();

            CreateMap<VoteJob, VoteJobDto>();
            CreateMap<VoteJobDto, VoteJob>();

  

        }
    }
}
