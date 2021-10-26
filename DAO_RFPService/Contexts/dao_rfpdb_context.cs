using DAO_RFPService.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAO_RFPService.Contexts
{
    public class dao_rfpdb_context : DbContext
    {
        public dao_rfpdb_context()
        {
           
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySQL(Program._settings.DbConnectionString);
            }
        }

        public DbSet<Rfp> Rfps { get; set; }
        public DbSet<RfpBid> RfpBids { get; set; }
        //public DbSet<UserLog> UserLogs { get; set; }
    }
}
