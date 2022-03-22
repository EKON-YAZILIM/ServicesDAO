using System;
using System.Collections.Generic;
using System.Text;

namespace Helpers.Models.WebsiteViewModels
{
    public class UserPaymentHistoryItem
    {
        public int JobID { get; set; }
        public string UserName { get; set; }
        public string NameSurname { get; set; }
        public double JobAmount { get; set; }
        public double PaymentAmount { get; set; }
        public string WalletAddress { get; set; }
        public string IBAN { get; set; }
        public string Title { get; set; }
        public string Explanation { get; set; }
        public DateTime CreateDate { get; set; }
    }
}