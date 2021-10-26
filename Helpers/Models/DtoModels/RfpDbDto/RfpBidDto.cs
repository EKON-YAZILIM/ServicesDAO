using System;
using System.Collections.Generic;
using System.Text;

namespace Helpers.Models.DtoModels.RfpDbDto
{
    [Serializable]
    public class RfpBidDto
    {
        public int RfpBidID { get; set; }
        public int UserId { get; set; }
        public DateTime CreateDate { get; set; }
        public int RfpID { get; set; }
        public DateTime Time { get; set; }
        public double Amount { get; set; }
        public string Note { get; set; }
    }
}
