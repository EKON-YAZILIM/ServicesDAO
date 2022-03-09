using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Helpers.Models.DtoModels.MainDbDto
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
        public DateTime CreateDate { get; set; }
        public string AssociateUserNote { get; set; }
        public bool VaOnboarding { get; set; }
        public string GithubLink { get; set; }
        public string ResumeLink { get; set; }
        public string Referrer { get; set; }
    }
}
