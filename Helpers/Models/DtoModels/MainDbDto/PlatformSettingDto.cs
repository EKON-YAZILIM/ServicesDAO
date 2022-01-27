using System;
using System.Collections.Generic;
using System.Text;

namespace Helpers.Models.DtoModels.MainDbDto
{
    public class PlatformSettingDto
    {
        public int PlatformSettingID { get; set; }
        public int? UserID { get; set; }
        public DateTime CreateDate { get; set; }
        public string DosCurrencies { get; set; }
        public string DosFees { get; set; }
        public double? DefaultPolicingRate { get; set; }
        public double? MinPolicingRate { get; set; }
        public double? MaxPolicingRate { get; set; }
        public bool ForumKYCRequired { get; set; }
        public double? QuorumRatio { get; set; }
        public double? InternalAuctionTime { get; set; }
        public double? PublicAuctionTime { get; set; }
        public string AuctionTimeType { get; set; }
        public double? VotingTime { get; set; }
        public string VotingTimeType { get; set; }
        public double? ReputationConversionRate { get; set; }
        public bool VAOnboardingSimpleVote { get; set; }
        public double? SimpleVotingTime { get; set; }
        public string SimpleVotingTimeType { get; set; }
    }
}
