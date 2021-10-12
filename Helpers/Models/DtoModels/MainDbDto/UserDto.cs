using System;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace Helpers.Models.DtoModels.MainDbDto
{
    [DataContract]
    [Serializable]
    public partial class UserDto
    {
        [DataMember]
        public int UserId { get; set; }

        [DataMember]
        public string NameSurname { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public string UserAlias { get; set; }

        [DataMember]
        public string Password { get; set; }

        [DataMember]
        public bool Newsletter { get; set; }

        [DataMember]
        public string UserType { get; set; }

        [DataMember]
        public bool IsBlocked { get; set; }

        [DataMember]
        public bool IsActive { get; set; }

        [DataMember]
        public DateTime CreateDate { get; set; }

        [DataMember]
        public bool KYCStatus { get; set; }

        [DataMember]
        public int? FailedLoginCount { get; set; }

        [DataMember]
        public string ProfileImage { get; set; }
    }
}
