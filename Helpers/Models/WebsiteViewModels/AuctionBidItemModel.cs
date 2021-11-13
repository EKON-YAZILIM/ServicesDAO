﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Helpers.Models.WebsiteViewModels
{
    public class AuctionBidItemModel
    {
        public int AuctionID { get; set; }
        public int AuctionBidID { get; set; }
        public int UserId { get; set; }
        public double Price { get; set; }
        public string Time { get; set; }
        public double ReputationStake { get; set; }
        public string UserName { get; set; }
    }
}
