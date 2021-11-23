using Helpers.Models.DtoModels.MainDbDto;
using Helpers.Models.DtoModels.ReputationDbDto;
using Helpers.Models.DtoModels.VoteDbDto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Helpers.Models.WebsiteViewModels
{
    public class GetDashBoardViewModelVA
    {
        public MyJobsViewModel MyJobs { get; set; }
        public int MyJobCount { get; set; }
        public double JobRatio { get; set; }

        public int MyAuctionCount { get; set; }
        public double AuctionRatio { get; set; }

        public int MyVotesCount { get; set; }
        public List<VoteDto> MyVotes { get; set; }
        public List<LastCommentsDto> LastComments { get; set; }
        public List<PopularJobsDto> PopularJobs { get; set; }
        public UserReputationHistoryDto UserReputation { get; set; }
    }
}
