using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DAO_VotingEngine.Models
{
    public class Vote
    {
        [Key]
        public int VoteId { get; set; }
        public int VoteJobID { get; set; }
        public double Reputation { get; set; }
        public string Side { get; set; }
        public DateTime Date { get; set; }
    }
}
