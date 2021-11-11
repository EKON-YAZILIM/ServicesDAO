using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using static Helpers.Constants.Enums;

namespace Helpers.Models.DtoModels.VoteDbDto
{
    [Serializable]
    public partial class AuctionDto
    {
        public int AuctionID { get; set; }
        public int JobID { get; set; }
        public DateTime CreateDate { get; set; }
        public int JobPosterUserId { get; set; }
        public int? WinnerAuctionBidID { get; set; }
        public AuctionStatusTypes? Status { get; set; }
        public DateTime PublicAuctionEndDate { get; set; }
        public DateTime InternalAuctionEndDate { get; set; }
        public int? DAOMemberCount { get; set; }
    }
}
