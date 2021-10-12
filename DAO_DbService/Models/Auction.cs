using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DAO_DbService.Models
{
    public class Auction 
    {
        [Key]
        public int AuctionID { get; set; }
        public int JobID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreateDate { get; set; }
        public int JobPosterUserId { get; set; }
        public int WinnerAuctionBidID { get; set; }
        public string Status { get; set; }
    }
}
