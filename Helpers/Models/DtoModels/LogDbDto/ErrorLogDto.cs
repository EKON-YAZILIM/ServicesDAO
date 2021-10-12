using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Helpers.Models.DtoModels.LogDbDto
{
    [Serializable]
    public partial class ErrorLogDto
    {
        public int ErrorLogId { get; set; }
        public string Server { get; set; }
        public string Application { get; set; }
        public string Message { get; set; }
        public string Target { get; set; }
        public string Trace { get; set; }
        public DateTime Date { get; set; }
        public string IdFieldName { get; set; }
        public int IdField { get; set; }
        public string Type { get; set; }
    }
}
