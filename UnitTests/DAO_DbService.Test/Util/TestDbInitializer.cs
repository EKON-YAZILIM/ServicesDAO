﻿using Microsoft.Extensions.Configuration;
using DAO_DbService.Contexts;
using Helpers.Models.DtoModels.MainDbDto;
using System;
using Microsoft.EntityFrameworkCore;
using DAO_DbService.Models;
using System.Collections.Generic;
using PagedList.Core;
using System.ComponentModel;
using System.Linq;
using System.Collections.Immutable;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Org.BouncyCastle.Math.EC.Multiplier;

namespace DAO_DbService.Tests.Util
{

    /// <summary>
    /// TestDbInitializer class includes necessary environment mocking methods.
    /// - Initializes database, creates application dbcontext
    /// - Creates testing environment
    /// </summary>
    public static class TestDbInitializer
    {
        ///Test password generated for user model crud operations tests
        public static string testPassword = Guid.NewGuid().ToString("d").Substring(1, 6);
        static dao_maindb_context context;
        /// <summary>
        /// Creates a DB context which should use a "test DB instance".
        /// Deletes the database.
        /// Builds a database and its entity tables using the DB context        
        /// </summary>
        static TestDbInitializer()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.test.json").Build();
            DAO_DbService.Startup.LoadConfig(config);

            context = new dao_maindb_context();
            context.ChangeTracker.Clear();
            context.Database.EnsureDeleted();
            DAO_DbService.Startup.InitializeService();
        }

        /// <summary>
        /// Truncates Users, ActiveSessions, AuctionBids, Auctions, JobPostComments, JobPosts, commentVotes tables in the database
        /// </summary>
        public static void ClearDatabase()
        {
            foreach (var user in context.Users)
            {
                context.Remove(user);
                context.Entry(user).State = EntityState.Detached;
            }
            foreach (var session in context.ActiveSessions)
            {
                context.Remove(session);
                context.Entry(session).State = EntityState.Detached;
            }
            foreach (var bids in context.AuctionBids)
            {
                context.Remove(bids);
                context.Entry(bids).State = EntityState.Detached;
            }
            foreach (var auction in context.Auctions)
            {
                context.Remove(auction);
                context.Entry(auction).State = EntityState.Detached;
            }
            foreach (var comment in context.JobPostComments)
            {
                context.Remove(comment);
                context.Entry(comment).State = EntityState.Detached;
            }
            foreach (var jobPosts in context.JobPosts)
            {
                context.Remove(jobPosts);
                context.Entry(jobPosts).State = EntityState.Detached;
            }
            foreach (var commentVotes in context.UserCommentVotes)
            {
                context.Remove(commentVotes);
                context.Entry(commentVotes).State = EntityState.Detached;
            }
            foreach (var userKYC in context.UserKYCs)
            {
                context.Remove(userKYC);
                context.Entry(userKYC).State = EntityState.Detached;
            }
            foreach (var user in context.Users)
            {
                context.Remove(user);
                context.Entry(user).State = EntityState.Detached;
            }
            context.SaveChanges();
            context.ChangeTracker.Clear();
        }
        
        ///<summary>
        /// Clears the database.
        /// Creates an active admin user, random active amount of public users between 1 and 100, random amount of active internal users between 1 and 50
        ///</summary>
        ///<returns>The list of users</returns>
        public static List<User> SeedUsers()
        {
            Random random = new Random();
            //generate random public users between 1 and 100
            int total_public_user_count = random.Next(1, 100);
            //generate random internal users between 1 and 100
            int total_internal_user_count = random.Next(5, 50);

            //Adding an admin user
            context.Users.Add(new User
            {
                NameSurname = $"AdminName UserSurname",
                Email = $"username@admin.com",
                Password = testPassword,
                Newsletter = true,
                UserType = "Admin",
                IsBlocked = false,
                IsActive = true,
                CreateDate = DateTime.Now,
                KYCStatus = true,
                FailedLoginCount = 0,
                ProfileImage = "image.jpg",
                UserName = "admin_username",
            }
                );

            //Adding public users
            for (int i = 0; i < total_public_user_count; i++)
            {
                string username = $"public_user{i}name";
                context.Users.Add(new User
                {
                    NameSurname = $"Username{i} UserSurname{i}",
                    Email = $"username{i}@public.com",
                    Password = testPassword,
                    Newsletter = true,
                    UserType = "Public",
                    IsBlocked = false,
                    IsActive = true,
                    CreateDate = DateTime.Now,
                    KYCStatus = true,
                    FailedLoginCount = 0,
                    ProfileImage = "image.jpg",
                    UserName = username,
                }
                );
            }

            //Adding internal users
            for (int i = 0; i < total_internal_user_count; i++)
            {
                string username = $"internal_username{i}";
                context.Users.Add(new User
                {
                    NameSurname = $"Username{i} UserSurname{i}",
                    Email = $"username{i}@internal.com",
                    Password = testPassword,
                    Newsletter = true,
                    UserType = "Internal",
                    IsBlocked = false,
                    IsActive = true,
                    CreateDate = DateTime.Now,
                    KYCStatus = true,
                    FailedLoginCount = 0,
                    ProfileImage = "image.jpg",
                    UserName = username,
                }
                );
            }

            context.SaveChanges();
            context.ChangeTracker.Clear();

            // return user list;
            return context.Users.ToList();
        }
    
        ///<summary>
        /// Clears the database.
        /// Creates an active admin user, random active amount of public users between 1 and 100, random amount of active internal users between 1 and 50
        /// Random amount of posted jobs from randomly selected users
        ///</summary>
        ///<returns>The list of posted jobs</returns>
        public static List<JobPost> SeedJobs(){
            ClearDatabase();
            SeedUsers();
            List<JobPost> postedJobs = new List<JobPost>();

            var randomUsers = context.Users.OrderBy(x => Guid.NewGuid()).Take(15).ToList();

            int i = 1, j = 1;
            Random random = new Random();
            foreach(User user in randomUsers){
                i++;
                j++;
                JobPost post = new JobPost{
                    CreateDate     = DateTime.Now.AddDays((j/(-j))*random.Next(1,5)), 
                    UserID         = user.UserId,
                    JobDoerUserID  = 0,
                    Title          = "Job #" + Math.Abs(i).ToString(),
                    JobDescription = "Description of Job #" + Math.Abs(i).ToString(),
                    Amount         = 1000*random.Next(7,100),
                    TimeFrame      = "65",
                    LastUpdate     = DateTime.Now.AddDays(40),
                    Status         = Helpers.Constants.Enums.JobStatusTypes.AdminApprovalPending,
                    DosFeePaid     = true                    
                };
                postedJobs.Add(post);
            }                        
            context.JobPosts.AddRange(postedJobs);
            context.SaveChanges();
            context.ChangeTracker.Clear();
            return postedJobs;
        }
    }
}
