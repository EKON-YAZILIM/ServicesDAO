using Helpers.Models.DtoModels.MainDbDto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Helpers.Models.WebsiteViewModels
{
    public class UserProfileViewModel
    {
        public UserDto User { get; set; }
        public int CommentCount { get; set; }
        public double Reputation { get; set; }
    }
}
