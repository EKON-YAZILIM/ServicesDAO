using DAO_DbService.Contexts;
using DAO_DbService.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAO_DbService.Test
{
    public class PostUnitTestController
    {
        public UsersController usersControllers;
        public ActiveSessionController activeSessionController;
        public InfoController infoController;
        public JobPostCommentController jobPostCommentController;
        public JobPostController jobPostController;
        public UserCommentVoteController userCommentVoteController;
        public UserKYCController userKYCController;
        public WebsiteController websiteController;

        static PostUnitTestController()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            DAO_DbService.Startup.LoadConfig(config);
        }

        public PostUnitTestController()
        {
            new dao_maindb_context().Database.EnsureDeleted();
            DAO_DbService.Startup.InitializeService();
            var context = new dao_maindb_context();
            
            DummyDataDBInitializer db = new DummyDataDBInitializer();
            db.Seed(context);

            usersControllers = new UsersController();
            usersControllers = new UsersController();
            activeSessionController = new ActiveSessionController();
            infoController = new InfoController();
            jobPostCommentController = new JobPostCommentController();
            jobPostController = new JobPostController();
            userCommentVoteController = new UserCommentVoteController();
            userKYCController = new UserKYCController();
            websiteController = new WebsiteController();
        }

    }
}
