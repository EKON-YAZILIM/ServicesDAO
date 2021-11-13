using Helpers.Models.DtoModels.MainDbDto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Helpers.Models.WebsiteViewModels
{
   public class AuctionDetailViewModel
    {
        public List<AuctionBidItemModel> BidItems { get; set; }
        public AuctionDto Auction { get; set; } 
    }
}
