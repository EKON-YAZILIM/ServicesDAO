using System;
using System.Collections.Generic;
using System.Text;

namespace Helpers.Models.IdentityModels
{
    public class ResetCompleteModel
    {
        public string newpass { get; set; }
        public string newpassagain { get; set; }
        public string passwordchangetoken { get; set; }
    }
}
