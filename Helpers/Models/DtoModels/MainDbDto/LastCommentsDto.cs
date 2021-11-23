using System;
using System.Collections.Generic;
using System.Text;

namespace Helpers.Models.DtoModels.MainDbDto
{
    public partial class LastCommentsDto
    {
        public int JobID { get; set; }
        public string UserName { get; set; }
        public string ProfileImage { get; set; }
        public string Comment { get; set; }
        public DateTime Date { get; set; }

    }
}
