using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DAO_DbService.Models
{
    public class UserKYC 
    {
       [Key]
       public int UserKYCID { get; set; }
       public string UserType { get;set; }
    }
}
