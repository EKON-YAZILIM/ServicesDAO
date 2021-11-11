using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Helpers.Models.DtoModels.VoteDbDto
{
    [Serializable]
    public partial class AuctionBidDto
    {
        public int AuctionBidID { get; set; }
        public int AuctionID { get; set; }
        public int UserId { get; set; }
        public double Price { get; set; }
        public string Time { get; set; }
        public double ReputationStake { get; set; }
  
    }
}
