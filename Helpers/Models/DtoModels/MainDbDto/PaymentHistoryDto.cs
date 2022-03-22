using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using static Helpers.Constants.Enums;

namespace Helpers.Models.DtoModels.MainDbDto
{
    [Serializable]
    public partial class PaymentHistoryDto
    {
        public int PaymentHistoryID { get; set; }
        public int JobID { get; set; }
        public int UserID { get; set; }
        public double Amount { get; set; }
        public string WalletAddress { get; set; }
        public string IBAN { get; set; }
        public DateTime CreateDate { get; set; }
        public string Explanation { get; set; }
        public PaymentType Status { get; set; }
    }
}
