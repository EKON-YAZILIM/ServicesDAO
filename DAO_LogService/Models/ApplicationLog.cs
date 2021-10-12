using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DAO_LogService.Models
{
    public class ApplicationLog
    {
        [Key]
        public int ApplicationLogID { get; set; }
        public string Application { get; set; }
        public string Server { get; set; }
        public DateTime Date { get; set; }
        public string IdFieldName { get; set; }
        public int IdField { get; set; }
        public string Type { get; set; }
        public string Explanation { get; set; }


    }
}
