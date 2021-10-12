using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DAO_ReputationService.Models
{
    public class UserReputation
    {
        [Key]
        public int UserReputationID { get; set; }
        public int UserId { get; set; }
        public double Reputation { get; set; }
    }
}
