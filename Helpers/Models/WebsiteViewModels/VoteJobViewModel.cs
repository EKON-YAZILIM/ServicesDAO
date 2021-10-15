using Helpers.Models.DtoModels.MainDbDto;
using Helpers.Models.DtoModels.VoteDbDto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Helpers.Models.WebsiteViewModels
{
    public class VoteJobViewModel
    {
        public VoteJobDto voteJob { get; set; }
        public int jobId { get; set; }
    }
}
