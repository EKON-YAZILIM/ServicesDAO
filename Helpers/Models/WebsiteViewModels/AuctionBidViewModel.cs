using System;
using System.Collections.Generic;
using System.Text;

namespace Helpers.Models.WebsiteViewModels
{
    public class AuctionBidViewModel
    {
        public int AuctionID { get; set; }
        public int UserId { get; set; }
        public double Price { get; set; }
        public string Time { get; set; }
        public bool IsInternal { get; set; }
        public double ReputationStake { get; set; }
        public string UserName { get; set; }
    }
}
