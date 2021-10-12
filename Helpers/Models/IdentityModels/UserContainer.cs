using Helpers.Models.DtoModels;
using Helpers.Models.DtoModels.MainDbDto;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Helpers.Models.IdentityModels
{
    public class UserContainer
    {
        public UserDto User { get; set; }
        public List<Claim> Claims { get; set; }
        public string Token { get; set; }
        public string LoginChannel { get; set; }
        public string Ip { get; set; }
        public string Port { get; set; }
        public string Type { get; set; }
        public Helpers.Constants.Enums.AppNames? Application { get; set; }
        public DateTime loginDate { get; set; }
        public DateTime lastRequestDate { get; set; }
    }
}
