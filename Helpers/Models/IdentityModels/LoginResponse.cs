﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Helpers.Models.IdentityModels
{
    [Serializable]
    public class LoginResponse
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public bool IsSuccessful { get; set; }
        public bool IsBlocked { get; set; }
        public bool IsNotActive { get; set; }
        public bool IsBanned { get; set; }
        public string CustomMessage { get; set; }
        public string NameSurname { get; set; }
        public string LicenseType { get; set; }
        public string ProfileImage { get; set; }
    }
}
