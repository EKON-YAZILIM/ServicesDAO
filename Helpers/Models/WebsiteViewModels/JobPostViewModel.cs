using Helpers.Models.DtoModels.MainDbDto;
using Helpers.Models.DtoModels.VoteDbDto;
using System;
using System.Collections.Generic;
using System.Text;
using static Helpers.Constants.Enums;

namespace Helpers.Models.WebsiteViewModels
{
    public class JobPostViewModel
    {
        public string Title { get; set; }
        public DateTime CreateDate { get; set; }
        public string JobDescription { get; set; }
        public DateTime LastUpdate { get; set; }
        public int CommentCount { get; set; }
        public int JobID { get; set; }
        public JobStatusTypes Status { get; set; }
        public double Amount { get; set; }
        public bool? DosFeePaid { get; set; }
        public int JobDoerUserID { get; set; }
        public string JobDoerUsername { get; set; }
        public int JobPosterUserID { get; set; }
        public string JobPosterUserName { get; set; }
        public string Tags { get; set; }
        public string CodeUrl { get; set; }
        public bool IsUserFlagged { get; set; }
        public int FlagCount { get; set; }
        public string TimeFrame { get; set; }
        public AuctionDto Auction { get; set; } = new AuctionDto();
        public List<AuctionBidItemModel> AuctionBids { get; set; } = new List<AuctionBidItemModel>();
        public List<VotingDto> Voting {get;set;} = new List<VotingDto>();
    }
}
