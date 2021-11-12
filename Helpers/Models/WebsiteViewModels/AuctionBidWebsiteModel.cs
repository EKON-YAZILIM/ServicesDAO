using Helpers.Models.DtoModels.MainDbDto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Helpers.Models.WebsiteViewModels
{
   public class AuctionBidWebsiteModel
    {
        public List<AuctionBidViewModel> AuctionBidViewModels { get; set; }
        public AuctionDto Auction { get; set; } 
    }
}
