using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Helpers.Models.DtoModels.LogDbDto
{
    [DataContract]
    [Serializable]
    public partial class ErrorLogDto
    {
        [DataMember]
        public int ErrorLogId { get; set; }

        [DataMember]
        public string Server { get; set; }

        [DataMember]
        public string Application { get; set; }

        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public string Target { get; set; }

        [DataMember]
        public string Trace { get; set; }

        [DataMember]
        public DateTime Date { get; set; }

        [DataMember]
        public string IdFieldName { get; set; }

        [DataMember]
        public int IdField { get; set; }

        [DataMember]
        public string Type { get; set; }
    }
}
