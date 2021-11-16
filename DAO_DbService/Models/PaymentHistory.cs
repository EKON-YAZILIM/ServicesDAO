using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DAO_DbService.Models
{
    public class PaymentHistory
    {
        [Key]
        public int PaymentHistoryID { get; set; }
        public int JobID { get; set; }
        public int UserID { get; set; }
        public double Amount { get; set; }
        public string WalletAddress { get; set; }
        public string IBAN { get; set; }
        public DateTime CreateDate { get; set; }

    }
}
