using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DAO_RFPService.Models
{
    public class Rfp
    {
        [Key]
        public int RfpID { get; set; }
        public int UserId { get; set; }
        public string FormInput { get; set; }
        public DateTime CreateDate { get; set; }
        public string Status { get; set; }
        public int WinnerRfpBidID { get; set; }
    }
}
