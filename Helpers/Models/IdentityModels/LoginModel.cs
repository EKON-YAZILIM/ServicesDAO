 using System;
using System.Collections.Generic;
using System.Text;

namespace Helpers.Models.IdentityModels
{
    public class LoginModel
    {
        public string email { get; set; }
        public string pass { get; set; }
        public Helpers.Constants.Enums.AppNames? application { get; set; }
        public string ip { get; set; } = "";
        public string port { get; set; } = "";
    }
}
