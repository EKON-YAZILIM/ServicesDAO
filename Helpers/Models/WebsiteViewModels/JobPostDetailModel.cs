using System;
using System.Collections.Generic;
using System.Text;

namespace Helpers.Models.WebsiteViewModels
{
   public class JobPostDetailModel
    {
        public JobPostWebsiteModel JobPostWebsiteModel { get; set; }
        public List<JobPostCommentModel> JobPostCommentMode { get; set; }
    }
}
