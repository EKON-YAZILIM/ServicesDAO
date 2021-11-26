using System;
using System.Collections.Generic;
using System.Text;
using static Helpers.Constants.Enums;

namespace Helpers.Models.WebsiteViewModels
{
    public class MyJobsViewModel
    {
        public List<JobPostViewModel> ownedJobs { get; set; }
        public List<JobPostViewModel> doerJobs { get; set; }

    }


}
