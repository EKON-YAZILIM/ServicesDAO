using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using static Helpers.Constants.Enums;

namespace Helpers.Models.DtoModels.MainDbDto
{
    [Serializable]
    public partial class AuctionDto
    {
        public int AuctionID { get; set; }
        public int JobID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreateDate { get; set; }
        public int JobPosterUserId { get; set; }
        public int WinnerAuctionBidID { get; set; }
        public JobStatusTypes Status { get; set; }
    }
}
