using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DAO_ReputationService.Models
{
    public class UserReputationHistory
    {
        [Key]
        public int UserReputationHistoryID { get; set; }
        public DateTime Date { get; set; }
        public double Input { get; set; }
        public double Output { get; set; }
        public double LastTotal { get; set; }
        public string Explanation { get; set; }
        public int UserReputationID { get; set; }
    }
}
