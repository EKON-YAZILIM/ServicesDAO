using System;
using System.Collections.Generic;
using System.Text;

namespace Helpers.Models.DtoModels.RfpDbDto
{
    [Serializable]
    public class RfpDto
    {
        public int RfpID { get; set; }
        public int UserId { get; set; }
        public string FormInput { get; set; }
        public DateTime CreateDate { get; set; }
        public string Status { get; set; }
    }
}
