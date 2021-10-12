using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Helpers.Models.DtoModels.MainDbDto
{
    [DataContract]
    [Serializable]
    public partial class UserKYCDto
    {
        [DataMember]
        public int UserKYCID { get; set; }

        [DataMember]
        public string UserType { get; set; }
    }
}
