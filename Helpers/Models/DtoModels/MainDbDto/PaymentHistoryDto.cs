using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

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
    }
}
