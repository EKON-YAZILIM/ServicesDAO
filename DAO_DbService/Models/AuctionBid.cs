using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DAO_DbService.Models
{
    public class AuctionBid
    {
        [Key]
        public int AuctionBidID { get; set; }
        public int AuctionID { get; set; }
        public int UserID { get; set; }
        public double Price { get; set; }
        public string Time { get; set; }
        public double ReputationStake { get; set; }

    }
}
