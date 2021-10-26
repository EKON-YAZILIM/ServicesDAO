using AutoMapper;
using DAO_RFPService.Models;
using Helpers.Models.DtoModels;
using Helpers.Models.DtoModels.RfpDbDto;

namespace DAO_RFPService.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Rfp, RfpDto>();
            CreateMap<RfpDto, Rfp>();

            CreateMap<RfpBid, RfpBidDto>();
            CreateMap<RfpBidDto, RfpBid>();

        }
    }
}
