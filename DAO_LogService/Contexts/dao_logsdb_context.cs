using DAO_LogService.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAO_LogService.Contexts
{
    public class dao_logsdb_context : DbContext
    {
        public dao_logsdb_context(DbContextOptions options) : base(options)
        {
            Database.Migrate();
        }

        // public DbSet<AppException> appExceptions { get; set; }
        public DbSet<ApplicationLog> ApplicationLogs { get; set; }
        public DbSet<ErrorLog> ErrorLogs { get; set; }
        public DbSet<UserLog> UserLogs { get; set; }
    }
}
