using System;
using System.Collections.Generic;
using System.Text;
using static Helpers.Constants.Enums;

namespace Helpers.Models.WebsiteViewModels
{
   public class AuctionViewModel
    {
        public int JobID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreateDate { get; set; }
        public int JobPosterUserId { get; set; }
        public int WinnerAuctionBidID { get; set; }
        public JobStatusTypes Status { get; set; }
        public string UserName { get; set; }
        public bool IsInternal { get; set; }
    }
}
