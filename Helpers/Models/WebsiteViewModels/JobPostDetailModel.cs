using System;
using System.Collections.Generic;
using System.Text;

namespace Helpers.Models.WebsiteViewModels
{
   public class JobPostDetailModel
    {
        public JobPostViewModel JobPostWebsiteModel { get; set; }
        public List<JobPostCommentModel> JobPostCommentModel { get; set; }
    }
}
