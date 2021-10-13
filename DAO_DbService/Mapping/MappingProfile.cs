using AutoMapper;
using DAO_DbService.Models;
using Helpers.Models.DtoModels.MainDbDto;

namespace DAO_DbService.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();

            CreateMap<Auction, AuctionDto>();
            CreateMap<AuctionDto, Auction>();

            CreateMap<AuctionBid, AuctionBidDto>();
            CreateMap<AuctionBidDto, AuctionBid>();

            CreateMap<JobPost, JobPostDto>();
            CreateMap<JobPostDto, JobPost>();

            CreateMap<JobPostComment, JobPostCommentDto>();
            CreateMap<JobPostCommentDto, JobPostComment>();

            CreateMap<UserKYC, UserKYCDto>();
            CreateMap<UserKYCDto, UserKYC>();

            CreateMap<ActiveSession, ActiveSessionDto>();
            CreateMap<ActiveSessionDto, ActiveSession>();
        }
    }
}
