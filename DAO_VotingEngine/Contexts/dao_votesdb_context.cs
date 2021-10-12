using DAO_VotingEngine.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAO_VotingEngine.Contexts
{
    public class dao_votesdb_context : DbContext
    {
        public dao_votesdb_context(DbContextOptions options) : base(options)
        {
            Database.Migrate();
        }

       // public DbSet<InformalVote> InformalVotes { get; set; }
        //public DbSet<FormalVotes> FormalVotes { get; set; }
        public DbSet<VoteJob> VoteJobs { get; set; }
        public DbSet<Vote> Votes { get; set; }

    }
}
