using System;
using System.Collections.Generic;
using System.Text;

namespace Helpers.Models.DtoModels
{
    public class UsersDto
    {
        public int UserId { get; set; }
        public string NameSurname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string GSM { get; set; }
        public bool? Newsletter { get; set; }
        public bool? MobileNotification { get; set; }
        public bool? EmailNotification { get; set; }
        public bool? IsAdmin { get; set; }
        public bool? IsBlocked { get; set; }
        public string LicenseType { get; set; }
        public string UserType { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool? ActiveStatus { get; set; }
        public int? FailedLoginCount { get; set; }
        public string ProfileImage { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public string ApiKey { get; set; }
        public DateTime? ApiKeyCreateDate { get; set; }
    }
}
