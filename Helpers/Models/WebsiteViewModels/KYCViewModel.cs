using Helpers.Models.DtoModels.MainDbDto;
using Helpers.Models.KYCModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Helpers.Models.WebsiteViewModels
{
    public class KYCViewModel
    {
        public List<KYCCountries> Countries { get; set; }
        public UserKYCDto Status { get; set; }

    }
}
