using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DAO_RFPService.Models
{
    public class RfpBid
    {
        [Key]
        public int RfpBidID { get; set; }
        public int UserId { get; set; }
        public DateTime CreateDate { get; set; }
        public int RfpID { get; set; }
        public double Amount { get; set; }
        public string Note { get; set; }
        public string Time { get; set; }
    }
}
