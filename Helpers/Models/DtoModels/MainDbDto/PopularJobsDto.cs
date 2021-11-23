using System;
using System.Collections.Generic;
using System.Text;

namespace Helpers.Models.DtoModels.MainDbDto
{
    [Serializable]
    public partial class PopularJobsDto
    {
        public int JobID { get; set; }
        public string Title { get; set; }
        public string JobDescription { get; set; }
        public string ProfileImage { get; set; }
        public string UserName { get; set; }
        public Helpers.Constants.Enums.JobStatusTypes Status { get; set; }
        public DateTime CreateDate { get; set; }


    }
}
