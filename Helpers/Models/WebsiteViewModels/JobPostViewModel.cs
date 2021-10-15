using System;
using System.Collections.Generic;
using System.Text;
using static Helpers.Constants.Enums;

namespace Helpers.Models.WebsiteViewModels
{
   public class JobPostViewModel
    {
        public string Title { get; set; }
        public string UserName { get; set; }
        public DateTime CreateDate { get; set; }
        public string JobDescription { get; set; }
        public DateTime LastUpdate { get; set; }
        public int CommentCount { get; set; }
        public int JobID { get; set; }
        public JobStatusTypes Status { get; set; }
        public JobProgressTypes ProgressType { get; set; }
        public double Amount { get; set; }
    }
}
