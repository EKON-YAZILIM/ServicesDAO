using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DAO_VotingEngine.Models
{
    public class InformalVote
    {
        [Key]
        public int VoteId { get; set; }
        public int JobId { get; set; }
    }
}
