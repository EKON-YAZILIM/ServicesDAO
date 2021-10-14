using DAO_DbService.Models;
using Microsoft.EntityFrameworkCore;
using MySql.EntityFrameworkCore.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAO_DbService.Contexts
{
    public class dao_maindb_context : DbContext
    {

        public dao_maindb_context()
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySQL(Program._settings.DbConnectionString);
            }
        }

        public DbSet<User> Users { get; set; }
        public DbSet<AuctionBid> AuctionBids { get; set; }
        public DbSet<Auction> Auctions { get; set; }
        public DbSet<JobPostComment> JobPostComments { get; set; }
        public DbSet<JobPost> JobPosts { get; set; }
        public DbSet<UserKYC> UserKYCs { get; set; }
        public DbSet<ActiveSession> ActiveSessions { get; set; }
        public DbSet<UserCommentVote> UserCommentVotes { get; set; }








    }
}
