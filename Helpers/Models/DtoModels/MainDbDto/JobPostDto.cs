using System;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace Helpers.Models.DtoModels.MainDbDto
{
    [Serializable]
    public partial class JobPostDto
    {
        public int JobID { get; set; }
        public DateTime CreateDate { get; set; }
        public int UserID { get; set; }
        public string Title { get; set; }
        public string JobDescription { get; set; }
        public double Amount { get; set; }
        public double DosPaid { get; set; }
        public string TimeFrame { get; set; }
    }
}
