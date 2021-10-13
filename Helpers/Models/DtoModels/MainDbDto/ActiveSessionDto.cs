using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Helpers.Models.DtoModels.MainDbDto
{
    [Serializable]
    public partial class ActiveSessionDto
    {
        public int ActiveSessionID { get; set; }
        public int UserID { get; set; }
        public DateTime LoginDate { get; set; }
        public string Token { get; set; }
    }
}
