using System;
using System.Collections.Generic;
using System.Text;

namespace Helpers.Models.WebsiteViewModels
{
    public class RfpModel
    {
        public int rfpID { get; set; }
        public int UserId { get; set; }
        public string  FormInput { get; set; }
        public DateTime createDate { get; set; }
        public string status { get; set; }
    }
}
