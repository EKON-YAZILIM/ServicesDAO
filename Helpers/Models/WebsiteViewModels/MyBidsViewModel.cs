using Helpers.Models.DtoModels.MainDbDto;
using System;
using System.Collections.Generic;
using System.Text;
using static Helpers.Constants.Enums;

namespace Helpers.Models.WebsiteViewModels
{
    public class MyBidsViewModel
    {
        public int AuctionBidID { get; set; }
        public int AuctionID { get; set; }
        public double Price { get; set; }
        public string Time { get; set; }
        public double ReputationStake { get; set; }
        public DateTime CreateDate { get; set; }
        public string AssociateUserNote { get; set; }
        public int JobID { get; set; }
        public string JobName { get; set; }
        public int? WinnerAuctionBidID { get; set; }
        public AuctionStatusTypes? Status { get; set; }

    }
}
