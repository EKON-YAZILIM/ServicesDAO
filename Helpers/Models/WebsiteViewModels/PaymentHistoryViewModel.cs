using System;
using System.Collections.Generic;
using System.Text;

namespace Helpers.Models.WebsiteViewModels
{
    public class PaymentHistoryViewModel
    {
        public List<UserPaymentHistoryItem> UserPaymentHistoryList { get; set; } 
        public double TotalAmount { get; set; }
        public double LastMonthAmount { get; set; }

    }
}
