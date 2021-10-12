using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Helpers.Models.DtoModels.LogDbDto
{
    [DataContract]
    [Serializable]
    public partial class UserLogDto
    {
        [DataMember]
        public int UserLogId { get; set; }

        [DataMember]
        public int UserId { get; set; }

        [DataMember]
        public DateTime Date { get; set; }

        [DataMember]
        public string Type { get; set; }

        [DataMember]
        public string Explanation { get; set; }

        [DataMember]
        public string Application { get; set; }

        [DataMember]
        public string Ip { get; set; }

        [DataMember]
        public string Port { get; set; }

        [DataMember]
        public string IdField { get; set; }

        [DataMember]
        public int IdValue { get; set; }
    }
}
