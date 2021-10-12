using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Helpers.Models.DtoModels.LogDbDto
{
    [DataContract]
    [Serializable]
    public partial class ApplicationLogDto
    {
        [DataMember]
        public int ApplicationLogID { get; set; }

        [DataMember]
        public string Application { get; set; }

        [DataMember]
        public string Server { get; set; }

        [DataMember]
        public DateTime Date { get; set; }

        [DataMember]
        public string IdFieldName { get; set; }

        [DataMember]
        public int IdField { get; set; }

        [DataMember]
        public string Type { get; set; }

        [DataMember]
        public string Explanation { get; set; }
    }
}
