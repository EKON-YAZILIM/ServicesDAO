using DAO_ReputationService.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAO_ReputationService.Contexts
{
    public class dao_reputationserv_context : DbContext
    {
        //public dao_reputationserv_context(DbContextOptions options) : base(options)
        //{
        //    Database.Migrate();
        //}
        public dao_reputationserv_context()
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySQL(Program._settings.DbConnectionString);
            }
        }

        // public DbSet<Reputation> Reputations { get; set; }
        public DbSet<UserReputation> UserReputations { get; set; }
        public DbSet<UserReputationHistory> UserReputationHistories { get; set; }
    }
}
