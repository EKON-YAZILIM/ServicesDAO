using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Helpers.Models.DtoModels.MainDbDto
{
    [Serializable]
    public partial class UserKYCDto
    {
        public int UserKYCID { get; set; }
        public string UserType { get; set; }
    }
}
