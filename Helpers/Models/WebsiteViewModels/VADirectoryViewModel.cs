using Helpers.Models.DtoModels.MainDbDto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Helpers.Models.WebsiteViewModels
{
    public class VADirectoryViewModel
    {
        public string Name { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public double VTotal { get; set; }
        public double VLastMonth { get; set; }
        public double VThisMonth { get; set; }
        public double TotalRep { get; set; }
        public DateTime DateBecameVA { get; set; }
    }
}
