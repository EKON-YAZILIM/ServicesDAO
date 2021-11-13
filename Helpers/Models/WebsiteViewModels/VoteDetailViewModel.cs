using Helpers.Models.DtoModels.VoteDbDto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Helpers.Models.WebsiteViewModels
{
    public class VoteDetailViewModel
    {
        public List<VoteItemModel> VoteItems { get; set; }
        public VotingDto Voting { get; set; }
    }
}
