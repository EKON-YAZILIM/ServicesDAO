using Helpers.Models.DtoModels.LogDbDto;
using Helpers.Models.DtoModels.MainDbDto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Helpers.Models.WebsiteViewModels
{
    public class DashBoardViewModelAdmin
    {
        public List<JobPostDto> JobPostDtos { get; set; }
        public List<AuctionViewModel> AuctionViewModels { get; set; }
        public List<VotingViewModel> VotingViewModels { get; set; }
        public List<ApplicationLogDto> ApplicationLogDtos { get; set; }
        public List<UserLogDto> UserLogDtos { get; set; }
        public List<UserDto> UserDtos { get; set; }
        public int UserCount { get; set; }
        public int JobCount { get; set; }
        public int AuctionCount { get; set; }
        public int VotingCount { get; set; }
        public double UserRatio { get; set; }
        public double JobRatio { get; set; }
        public double AuctionRatio { get; set; }
        public double VotingRatio { get; set; }
        public List<DashboardGraphModel> UserCardGraph { get; set; }
        public List<DashboardGraphModel> JobCardGraph { get; set; }
        public List<DashboardGraphModel> AuctionCardGraph { get; set; }
        public List<DashboardGraphModel> VotingCardGraph { get; set; }
        public List<AdminDashboardCardModel> AuctionGraph { get; set; }
        public List<AdminDashboardCardModel> VotingGraph { get; set; }

    }
}
