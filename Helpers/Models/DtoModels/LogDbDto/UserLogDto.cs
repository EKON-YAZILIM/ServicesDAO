using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Helpers.Models.DtoModels.LogDbDto
{
    [Serializable]
    public partial class UserLogDto
    {
        public int UserLogId { get; set; }
        public int UserId { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; }
        public string Explanation { get; set; }
        public string Application { get; set; }
        public string Ip { get; set; }
        public string Port { get; set; }
        public string IdField { get; set; }
        public int IdValue { get; set; }
    }
}
