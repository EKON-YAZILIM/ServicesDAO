using System;
using System.Collections.Generic;
using System.Text;
using static Helpers.Constants.Enums;

namespace Helpers.Models.WebsiteViewModels
{
   public class AuctionViewModel
    {
        public int JobID { get; set; }
        public string Title { get; set; }
        public int AuctionID { get; set; }
        public DateTime InternalAuctionEndDate { get; set; }
        public DateTime PublicAuctionEndDate { get; set; }
        public DateTime CreateDate { get; set; }
        public int JobPosterUserId { get; set; }
        public int? WinnerAuctionBidID { get; set; }
        public string WinnerUserName { get; set; }
        public AuctionStatusTypes? Status { get; set; }
        public bool IsInternal { get; set; }
        public int BidCount { get; set; }

    }
}
