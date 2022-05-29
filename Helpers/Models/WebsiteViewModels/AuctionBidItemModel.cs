using System;
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
        public string NameSurname { get; set; }
        public string UserNote { get; set; }
        public string UserType { get; set; }
        public double UsersTotalReputation { get; set; }
        public string GithubLink { get; set; }
        public string ResumeLink { get; set; }
        public string Referrer { get; set; }
        public bool VaOnboarding { get; set; }
    }
}
