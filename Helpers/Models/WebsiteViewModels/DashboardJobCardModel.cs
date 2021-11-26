using System;
using System.Collections.Generic;
using System.Text;
using static Helpers.Constants.Enums;

namespace Helpers.Models.WebsiteViewModels
{
    public class DashboardJobCardModel
    {
        public string Title { get; set; }
        public string UserName { get; set; }
        public DateTime CreateDate { get; set; }
        public int JobID { get; set; }
        public JobStatusTypes Status { get; set; }
        public double Amount { get; set; }
        public DateTime EndDate { get; set; }
    }
}
