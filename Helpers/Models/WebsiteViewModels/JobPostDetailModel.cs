using Helpers.Models.DtoModels.MainDbDto;
using Helpers.Models.DtoModels.VoteDbDto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Helpers.Models.WebsiteViewModels
{
   public class JobPostDetailModel
    {
        public JobPostViewModel JobPostWebsiteModel { get; set; }
        public List<JobPostCommentModel> JobPostCommentModel { get; set; }
        public AuctionDto Auction { get; set; }
        public AuctionBidDto WinnerBid { get; set; }
    }
}
