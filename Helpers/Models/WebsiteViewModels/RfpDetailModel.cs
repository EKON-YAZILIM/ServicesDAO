using System;
using System.Collections.Generic;
using System.Text;

namespace Helpers.Models.WebsiteViewModels
{
    public class RfpBidDetailModel
    {
        public int rfpBidID { get; set; }
        public int UserId { get; set; }
        public DateTime createDate { get; set; }
        public int rfpID { get; set; }
        public string time { get; set; }
        public string note { get; set; }
        public Double amount { get; set; }
    }
}
