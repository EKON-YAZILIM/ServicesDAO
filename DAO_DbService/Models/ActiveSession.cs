using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DAO_DbService.Models
{
    public class ActiveSession 
    {
        [Key]
        public int ActiveSessionID { get; set; }
        public int UserID { get; set; }
        public DateTime LoginDate { get; set; }
        public string Token { get; set; }
    }
}
