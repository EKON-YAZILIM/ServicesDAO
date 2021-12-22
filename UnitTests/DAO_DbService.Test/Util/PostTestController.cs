using DAO_DbService.Contexts;
using DAO_DbService.Controllers;
using Microsoft.Extensions.Configuration;

namespace DAO_DbService.Tests.Util
{
    /// <summary>
    /// Creates instances of DAO_DbService application controllers classes to be tested.
    /// - ActiveSessionController
    /// - AuctionBidController
    /// - AuctionController
    /// - InfoController
    /// - JobPostCommentController
    /// - JobPostController
    /// - UserCommentVoteController
    /// - UserKYCController
    /// - UsersController
    /// - WebsiteController 
    /// </summary>
    public class PostTestController
    {
        /// DAO_DbService.ActiveSessionController activeSessionController
        public ActiveSessionController activeSessionController;
        /// RFPPortalWebSite.AuthController instance activeSessionController
        public AuctionBidController auctionBidController;
        /// DAO_DbService.AuctionBidController instance authController
        public InfoController infoController;
        /// DAO_DbService.InfoController instance infoController
        public JobPostCommentController jobPostCommentController;
        /// DAO_DbService.JobPosCommentController instance jobPostCommentController
        public JobPostController jobPostController;
        /// DAO_DbService.JobPostController instance jobPostController
        public UserCommentVoteController userCommentVoteController;
        /// DAO_DbService.UserCommentVoteController instance userCommentController
        public UsersController usersController;
        /// DAO_DbService.UsersController instance userController
        public WebsiteController websiteController;
        /// DAO_DbService.WebsiteController instance WebsiteController
        static PostTestController()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.test.json").Build();
            DAO_DbService.Startup.LoadConfig(config);
        }

        /// <summary>
        /// 1.Creates application dbcontext
        /// 2.Deletes the database instance
        /// 3.Creates instances of AuthController, RfpController, BidController
        /// </summary>
        public PostTestController()
        {
            var context = new dao_maindb_context();
            context.Database.EnsureDeleted();
            DAO_DbService.Startup.InitializeService();

            activeSessionController = new ActiveSessionController();
            auctionBidController = new AuctionBidController();
            infoController = new InfoController();
            jobPostCommentController = new JobPostCommentController();
            jobPostController = new JobPostController();
            userCommentVoteController = new UserCommentVoteController();
            usersController = new UsersController();
            websiteController = new WebsiteController();
        }
    }
}