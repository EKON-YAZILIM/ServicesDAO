using DAO_WebPortal.Models;
using DAO_WebPortal.Providers;
using DAO_WebPortal.Resources;
using Helpers.Constants;
using Helpers.Models.DtoModels.MainDbDto;
using Helpers.Models.DtoModels.ReputationDbDto;
using Helpers.Models.DtoModels.VoteDbDto;
using Helpers.Models.SharedModels;
using Helpers.Models.WebsiteViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using static Helpers.Constants.Enums;

namespace DAO_WebPortal.Controllers
{
    [AuthorizeUser]
    public class HomeController : Controller
    {
        #region Views

        /// <summary>
        /// Dashboard Page
        /// </summary>
        /// <returns></returns>
        [Route("Home")]
        public IActionResult Index()
        {
            GetDashBoardViewModel dashModel = new GetDashBoardViewModel();
            try
            {
                var userType = HttpContext.Session.GetString("UserType");
                //User type control for admin
                if (userType == Helpers.Constants.Enums.UserIdentityType.Admin.ToString())
                {
                    //Get model from ApiGateway
                    var url = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/Website/GetDashBoardAdmin?userid=" + HttpContext.Session.GetInt32("UserID"), HttpContext.Session.GetString("Token"));
                    //Parse response
                    dashModel = Helpers.Serializers.DeserializeJson<GetDashBoardViewModel>(url);
                    return View(dashModel);
                }
                //User type control for associate
                if (userType == Helpers.Constants.Enums.UserIdentityType.Associate.ToString())
                {

                    return View("Index_Associate", dashModel);
                }
                //User type control for voting associate
                if (userType == Helpers.Constants.Enums.UserIdentityType.VotingAssociate.ToString())
                {
                    return View("Index_VotingAssociate", dashModel);
                }

            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return View(new GetDashBoardViewModel());
            }

            return View(dashModel);
        }

        /// <summary>
        /// User's Job Page
        /// </summary>
        /// <returns></returns>
        [Route("My-Jobs")]
        public IActionResult My_Jobs()
        {
            List<JobPostViewModel> myJobsModel = new List<JobPostViewModel>();
            try
            {
                //Get model from ApiGateway
                var url = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/Website/GetUserJobs?userid=" + HttpContext.Session.GetInt32("UserID"), HttpContext.Session.GetString("Token"));

                //Parse response
                myJobsModel = Helpers.Serializers.DeserializeJson<List<JobPostViewModel>>(url);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }
            return View(myJobsModel);
        }

        /// <summary>
        /// All Jobs Page
        /// </summary>
        /// <returns></returns>
        [Route("All-Jobs")]
        public IActionResult All_Jobs()
        {
            List<JobPostViewModel> allJobsModel = new List<JobPostViewModel>();
            try
            {
                //Get model from ApiGateway
                var url = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/Website/GetAllJobs", HttpContext.Session.GetString("Token"));

                //Parse response
                allJobsModel = Helpers.Serializers.DeserializeJson<List<JobPostViewModel>>(url);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }
            return View(allJobsModel);
        }

        /// <summary>
        /// Auctions Page
        /// </summary>
        /// <returns></returns>
        [Route("Auctions")]
        public IActionResult Auctions()
        {
            List<AuctionViewModel> auctionModel = new List<AuctionViewModel>();
            try
            {
                //Get model from ApiGateway
                var url = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/Website/GetAuctions", HttpContext.Session.GetString("Token"));

                //Parse response
                auctionModel = Helpers.Serializers.DeserializeJson<List<AuctionViewModel>>(url);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }
            return View(auctionModel);
        }

        /// <summary>
        /// Auction Detail Page
        /// </summary>
        /// <param name="AuctionID">Auction Id</param>
        /// <returns></returns>
        [Route("Auction-Detail/{AuctionID}")]
        public IActionResult Auction_Detail(int AuctionID)
        {
            AuctionBidWebsiteModel AuctionDetailModel = new AuctionBidWebsiteModel();
            try
            {
                //Get bids model from ApiGateway
                var url = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/Website/GetAuctionBids?auctionid=" + AuctionID, HttpContext.Session.GetString("Token"));
                //Get auction model from ApiGateway
                var url2 = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/Auction/GetId?id=" + AuctionID, HttpContext.Session.GetString("Token"));

                //Parse response
                AuctionDetailModel.AuctionBidViewModels = Helpers.Serializers.DeserializeJson<List<AuctionBidViewModel>>(url);
                //Parse response
                AuctionDetailModel.Auction = Helpers.Serializers.DeserializeJson<AuctionDto>(url2);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }
            return View(AuctionDetailModel);
        }

        /// <summary>
        /// Votes Page
        /// </summary>
        /// <returns></returns>
        [Route("Votes")]
        public IActionResult Votes()
        {
            List<VotingViewModel> votesModel = new List<VotingViewModel>();
            try
            {
                //Get model from ApiGateway
                var url = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/Website/GetVotingsByStatus", HttpContext.Session.GetString("Token"));

                //Parse response
                votesModel = Helpers.Serializers.DeserializeJson<List<VotingViewModel>>(url);

            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }
            return View(votesModel);
        }

        /// <summary>
        /// User Reputation History Page
        /// </summary>
        /// <returns></returns>
        [Route("Reputation-History")]
        public IActionResult Reputation_History()
        {
            List<UserReputationHistoryDto> ReputationHistoryModel = new List<UserReputationHistoryDto>();
            try
            {
                //Get model from ApiGateway
                var url = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Reputation/UserReputationHistory/GetByUserId?userid=" + HttpContext.Session.GetInt32("UserID"), HttpContext.Session.GetString("Token"));

                //Parse response
                ReputationHistoryModel = Helpers.Serializers.DeserializeJson<List<UserReputationHistoryDto>>(url);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }
            return View(ReputationHistoryModel);
        }

        /// <summary>
        /// User Payments History Page
        /// </summary>
        /// <returns></returns>
        [Route("Payments-History")]
        public IActionResult Payments_History()
        {
            return View();
        }

        /// <summary>
        /// User Profile Page
        /// </summary>
        /// <returns></returns>
        [Route("User-Profile")]
        public IActionResult User_Profile()
        {
            UserDto profileModel = new UserDto();

            try
            {
                //Get model from ApiGateway
                var json = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/Users/GetId?id=" + HttpContext.Session.GetInt32("UserID"), HttpContext.Session.GetString("Token"));

                //Parse response
                profileModel = Helpers.Serializers.DeserializeJson<UserDto>(json);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return View(profileModel);
        }

        [Route("Contact-Help")]
        public IActionResult Contact_Help()
        {
            return View();
        }

        /// <summary>
        /// Job Detail Page
        /// </summary>
        /// <param name="JobID">Job Id</param>
        /// <returns></returns>
        [Route("Job-Detail/{JobID}")]
        public IActionResult Job_Detail(int JobID)
        {
            JobPostDetailModel model = new JobPostDetailModel();

            try
            {
                var userid = HttpContext.Session.GetInt32("UserID");

                //Get model from ApiGateway
                var url = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/Website/GetJobDetail?jobid=" + JobID, HttpContext.Session.GetString("Token"));
                //Parse response
                model.JobPostWebsiteModel = Helpers.Serializers.DeserializeJson<JobPostViewModel>(url);

                //Get model from ApiGateway
                var url1 = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/Website/GetJobComment?jobid=" + JobID + "&userid=" + userid, HttpContext.Session.GetString("Token"));
                //Parse response
                model.JobPostCommentModel = Helpers.Serializers.DeserializeJson<List<JobPostCommentModel>>(url1);

                //Get related auction if exists
                var auctionJson = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/Auction/GetByJobId?jobid=" + JobID, HttpContext.Session.GetString("Token"));
                model.Auction = Helpers.Serializers.DeserializeJson<AuctionDto>(auctionJson);

                //Get winning bid if exists 
                if (model.Auction != null && model.Auction.WinnerAuctionBidID >= 0)
                {
                    var auctionBidJson = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/AuctionBid/GetId?id=" + model.Auction.WinnerAuctionBidID, HttpContext.Session.GetString("Token"));
                    model.WinnerBid = Helpers.Serializers.DeserializeJson<AuctionBidDto>(auctionBidJson);
                }

                //Get related voting if exists
                var votingJson = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Voting/Voting/GetByJobId?jobid=" + JobID, HttpContext.Session.GetString("Token"));
                model.Voting = Helpers.Serializers.DeserializeJson<VotingDto>(votingJson);

            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return View(model);
        }

        /// <summary>
        /// Vote Detail Page
        /// </summary>
        /// <param name="VoteID">Vote Id</param>
        /// <returns></returns>
        [Route("Vote-Detail/{VoteID}")]
        public IActionResult Vote_Detail(int VoteID)
        {
            List<VoteDto> voteDetailModel = new List<VoteDto>();
            try
            {
                //Get model from ApiGateway
                var url = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/Website/GetVoteDetail?voteid=" + VoteID, HttpContext.Session.GetString("Token"));

                //Parse response
                voteDetailModel = Helpers.Serializers.DeserializeJson<List<VoteDto>>(url);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }
            return View(voteDetailModel);
        }

        /// <summary>
        /// New Job Page
        /// </summary>
        /// <returns></returns>
        [Route("New-Job")]
        public IActionResult New_Job()
        {
            return View();
        }

        /// <summary>
        ///  New job post registration function
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="amount">Amount</param>
        /// <param name="time">Time</param>
        /// <param name="description">Description</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult New_Job_Post(string title, double amount, string time, string description)
        {
            JobPostDto model = new JobPostDto();
            SimpleResponse result = new SimpleResponse();
            try
            {
                //Create JobPost model
                model = new JobPostDto() { UserID = Convert.ToInt32(HttpContext.Session.GetInt32("UserID")), Amount = amount, JobDescription = description, CreateDate = DateTime.Now, TimeFrame = time, LastUpdate = DateTime.Now, Title = title, Status = Enums.JobStatusTypes.AdminApprovalPending };

                //Post model to ApiGateway
                model = Helpers.Serializers.DeserializeJson<JobPostDto>(Helpers.Request.Post(Program._settings.Service_ApiGateway_Url + "/Db/JobPost/Post", Helpers.Serializers.SerializeJson(model), HttpContext.Session.GetString("Token")));

                if (model != null && model.JobID > 0)
                {
                    result.Success = true;
                    result.Message = "Job posted successfully and will be available after admin review.";
                    result.Content = model;
                }
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return Json(new SimpleResponse { Success = false, Message = Lang.ErrorNote });
        }
        #endregion

        /// <summary>
        ///  Job Edit Page
        /// </summary>
        /// <param name="Job">Job Id</param>     
        /// <returns></returns>
        [Route("My-Job-Edit/{Job}")]
        public IActionResult My_Job_Edit(int Job)
        {
            JobPostDto jobDetailModel = new JobPostDto();

            try
            {
                //Get model from ApiGateway
                var url = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/JobPost/GetId?id=" + Job, HttpContext.Session.GetString("Token"));

                //Parse response
                jobDetailModel = Helpers.Serializers.DeserializeJson<JobPostDto>(url);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return View(jobDetailModel);
        }

        /// <summary>
        /// Edit user Job post 
        /// </summary>
        /// <param name="Model">JobPostDto Model</param>
        /// <returns></returns>
        [HttpPut]
        public JsonResult My_Job_Update(JobPostDto Model)
        {
            JobPostDto model = new JobPostDto();
            SimpleResponse result = new SimpleResponse();
            try
            {
                //Put model to ApiGateway
                model = Helpers.Serializers.DeserializeJson<JobPostDto>(Helpers.Request.Put(Program._settings.Service_ApiGateway_Url + "/Db/JobPost/Update", Helpers.Serializers.SerializeJson(Model), HttpContext.Session.GetString("Token")));
                if (model != null && model.JobID > 0)
                {
                    result.Success = false;
                    result.Message = "Job updated succesfully.";
                    result.Content = model;
                }
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return Json(new SimpleResponse { Success = false, Message = Lang.ErrorNote });
        }

        /// <summary>
        /// Comment upvote function
        /// </summary>
        /// <param name="JobPostCommentId">JobPostCommentId</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult UpVote(int JobPostCommentId)
        {

            SimpleResponse result = new SimpleResponse();

            try
            {
                var userid = HttpContext.Session.GetInt32("UserID");

                //Get model from ApiGateway
                var model = Helpers.Serializers.DeserializeJson<List<UserCommentVoteDto>>(Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/UserCommentVote/GetByCommentId?CommentId=" + JobPostCommentId, HttpContext.Session.GetString("Token")));

                //Get upvote comment count
                int UpCount = model.Count(x => x.IsUpVote == true);

                //Get downvote comment count
                int DownCount = model.Count(x => x.IsUpVote == false);

                int[] res = { 0, 0 };

                //If user upvoted same comment in the past
                if (model.Count(x => x.UserId == userid && x.IsUpVote == true) > 0)
                {
                    var CommentModel = model.First(x => x.UserId == userid);

                    //Delete model from ApiGateway
                    var deleteModel = Helpers.Request.Delete(Program._settings.Service_ApiGateway_Url + "/Db/UserCommentVote/Delete?ID=" + CommentModel.UserCommentVoteID, HttpContext.Session.GetString("Token"));

                    //Remove 1 point from upvote
                    res[0] = UpCount - 1;
                    res[1] = DownCount;
                    result.Success = true;
                    result.Message = "";
                    result.Content = res;
                }
                //If user downvoted same comment in the past
                else if (model.Count(x => x.UserId == userid && x.IsUpVote == false) > 0)
                {
                    //Set CommentModel.IsUpVote
                    var CommentModel = model.First(x => x.UserId == userid && x.IsUpVote == false);
                    CommentModel.IsUpVote = true;

                    //Put model to ApiGateway
                    var updateModel = Helpers.Serializers.DeserializeJson<UserCommentVoteDto>(Helpers.Request.Put(Program._settings.Service_ApiGateway_Url + "/Db/UserCommentVote/Update", Helpers.Serializers.SerializeJson(CommentModel), HttpContext.Session.GetString("Token")));

                    //Add 1 point to upvote
                    //Remove 1 point from downvote
                    res[0] = UpCount + 1;
                    res[1] = DownCount - 1;
                    result.Success = true;
                    result.Message = "";
                    result.Content = res;
                }
                else
                {
                    //Create model
                    UserCommentVoteDto CommentModel = new UserCommentVoteDto()
                    {
                        UserId = Convert.ToInt32(userid),
                        IsUpVote = true,
                        JobPostCommentID = JobPostCommentId,
                    };

                    //Post model to ApiGateway
                    var postModel = Helpers.Serializers.DeserializeJson<UserCommentVoteDto>(Helpers.Request.Post(Program._settings.Service_ApiGateway_Url + "/Db/UserCommentVote/Post", Helpers.Serializers.SerializeJson(CommentModel), HttpContext.Session.GetString("Token")));

                    //Add 1 point to upvote
                    res[0] = UpCount + 1;
                    res[1] = DownCount;
                    result.Success = true;
                    result.Message = "";
                    result.Content = res;
                }

                return Json(result);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return Json(new SimpleResponse { Success = false, Message = Lang.ErrorNote });
        }

        /// <summary>
        /// Comment downvote function
        /// </summary>
        /// <param name="JobPostCommentId">JobPostCommentId</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult DownVote(int JobPostCommentId)
        {
            SimpleResponse result = new SimpleResponse();
            try
            {
                var userid = HttpContext.Session.GetInt32("UserID");

                //Get model from ApiGateway
                var model = Helpers.Serializers.DeserializeJson<List<UserCommentVoteDto>>(Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/UserCommentVote/GetByCommentId?CommentId=" + JobPostCommentId, HttpContext.Session.GetString("Token")));

                //Get upvote comment count
                int UpCount = model.Count(x => x.IsUpVote == true);

                //Get downvote comment count
                int DownCount = model.Count(x => x.IsUpVote == false);

                int[] res = { 0, 0 };

                //If user downvoted same comment in the past
                if (model.Count(x => x.UserId == userid && x.IsUpVote == false) > 0)
                {
                    var CommentModel = model.First(x => x.UserId == userid);

                    //Delete model from ApiGateway
                    var deleteModel = Helpers.Request.Delete(Program._settings.Service_ApiGateway_Url + "/Db/UserCommentVote/Delete?ID=" + CommentModel.UserCommentVoteID, HttpContext.Session.GetString("Token"));

                    //Remove 1 point from downvote
                    res[0] = UpCount;
                    res[1] = DownCount - 1;
                    result.Success = true;
                    result.Message = "";
                    result.Content = res;
                }
                //If user upvoted same comment in the past
                else if (model.Count(x => x.UserId == userid && x.IsUpVote == true) > 0)
                {
                    //Set CommentModel.IsUpVote
                    var CommentModel = model.First(x => x.UserId == userid && x.IsUpVote == true);
                    CommentModel.IsUpVote = false;

                    //Put model to ApiGateway
                    var updateModel = Helpers.Serializers.DeserializeJson<UserCommentVoteDto>(Helpers.Request.Put(Program._settings.Service_ApiGateway_Url + "/Db/UserCommentVote/Update", Helpers.Serializers.SerializeJson(CommentModel), HttpContext.Session.GetString("Token")));

                    //Add 1 point to downvote
                    //Remove 1 point from upvote
                    res[0] = UpCount - 1;
                    res[1] = DownCount + 1;
                    result.Success = true;
                    result.Message = "";
                    result.Content = res;
                }
                else
                {
                    //Create model
                    UserCommentVoteDto CommentModel = new UserCommentVoteDto()
                    {
                        UserId = Convert.ToInt32(userid),
                        IsUpVote = false,
                        JobPostCommentID = JobPostCommentId,
                    };

                    //Post model to ApiGateway
                    var postModel = Helpers.Serializers.DeserializeJson<UserCommentVoteDto>(Helpers.Request.Post(Program._settings.Service_ApiGateway_Url + "/Db/UserCommentVote/Post", Helpers.Serializers.SerializeJson(CommentModel), HttpContext.Session.GetString("Token")));

                    //Add 1 point to downvote
                    res[0] = UpCount;
                    res[1] = DownCount + 1;
                    result.Success = true;
                    result.Message = "";
                    result.Content = res;
                }

                return Json(result);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return Json(new SimpleResponse { Success = false, Message = Lang.ErrorNote });
        }

        #region Auction
        /// <summary>
        /// Add new bid for auction
        /// </summary>
        /// <param name="Model">AuctionBidDto Model</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Auction_Bid_Add(AuctionBidDto Model)
        {
            AuctionBidDto model = new AuctionBidDto();
            SimpleResponse result = new SimpleResponse();
            try
            {
                var userid = HttpContext.Session.GetInt32("UserID");

                //Create model
                model = new AuctionBidDto()
                {
                    AuctionID = Model.AuctionID,
                    Price = Model.Price,
                    Time = Model.Time,
                    ReputationStake = Model.ReputationStake,
                    UserId = Convert.ToInt32(userid)
                };

                //Post model to ApiGateway
                model = Helpers.Serializers.DeserializeJson<AuctionBidDto>(Helpers.Request.Post(Program._settings.Service_ApiGateway_Url + "/Db/AuctionBid/Post", Helpers.Serializers.SerializeJson(model), HttpContext.Session.GetString("Token")));

                if (model != null && model.AuctionBidID > 0)
                {
                    result.Success = true;
                    result.Message = "Bid succesffully submitted.";
                    result.Content = model;
                }

                return Json(result);

            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return Json(new SimpleResponse { Success = false, Message = Lang.ErrorNote });

        }

        /// <summary>
        /// Add new bid for auction
        /// </summary>
        /// <param name="Model">AuctionBidDto Model</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult Auction_Bid_Delete(int id)
        {
            SimpleResponse result = new SimpleResponse();
            try
            {
                var userid = HttpContext.Session.GetInt32("UserID");

                //Post model to ApiGateway
                var model = Helpers.Serializers.DeserializeJson<bool>(Helpers.Request.Delete(Program._settings.Service_ApiGateway_Url + "/Db/AuctionBid/Delete?id="+id,  HttpContext.Session.GetString("Token")));

                if (model)
                {
                    result.Success = true;
                    result.Message = "Bid succesffully deleted.";
                }

                return Json(result);

            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return Json(new SimpleResponse { Success = false, Message = Lang.ErrorNote });

        }
        #endregion

        /// <summary>
        /// Add new comment
        /// </summary>
        /// <param name="JobId">Job id</param>
        /// <param name="CommentId">Main comment id</param>
        /// <param name="Comment">Comment</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AddNewComment(int JobId, int CommentId, string Comment)
        {
            SimpleResponse result = new SimpleResponse();

            try
            {
                //Create new comment
                JobPostCommentDto model = new JobPostCommentDto()
                {
                    Comment = Comment,
                    Date = DateTime.Now,
                    JobID = JobId,
                    SubCommentID = CommentId,
                    UserID = Convert.ToInt32(HttpContext.Session.GetInt32("UserID")),
                    DownVote = 0,
                    UpVote = 0,
                };

                //If user posting comment to main topic
                if (CommentId == 0)
                {
                    //Get related auction if exists
                    AuctionDto auction = Helpers.Serializers.DeserializeJson<AuctionDto>(Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/Auction/GetByJobId?jobid=" + JobId, HttpContext.Session.GetString("Token")));

                    //Check if auction have a winner bid
                    if (auction != null && auction.AuctionID > 0 && auction.WinnerAuctionBidID != null)
                    {
                        //Get winner bid
                        AuctionBidDto auctionbid = Helpers.Serializers.DeserializeJson<AuctionBidDto>(Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/AuctionBid/GetId?id=" + auction.WinnerAuctionBidID, HttpContext.Session.GetString("Token")));
                        //Set user comment as "Pinned" if the user is the owner of winner bid
                        if (auctionbid != null && auctionbid.AuctionBidID > 0 && auctionbid.UserId == model.UserID)
                        {
                            model.IsPinned = true;
                        }
                        else
                        {
                            model.IsPinned = false;
                        }
                    }
                }

                //Post model to ApiGateway
                //Add new job
                model = Helpers.Serializers.DeserializeJson<JobPostCommentDto>(Helpers.Request.Post(Program._settings.Service_ApiGateway_Url + "/Db/JobPostComment/Post", Helpers.Serializers.SerializeJson(model), HttpContext.Session.GetString("Token")));

                if (model != null && model.JobPostCommentID > 0)
                {
                    result.Success = true;
                    result.Message = "Comment succesfully posted.";
                    result.Content = model;
                }

                return Json(result);

            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return Json(new SimpleResponse { Success = false, Message = Lang.ErrorNote });
        }

        [HttpGet]
        public JsonResult AdminJobApproval(int JobId)
        {
            SimpleResponse result = new SimpleResponse();

            try
            {
                //Get Model from ApiGateway          
                var url = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/JobPost/GetId?id=" + JobId, HttpContext.Session.GetString("Token"));
                //Parse result
                var JobModel = Helpers.Serializers.DeserializeJson<JobPostDto>(url);
                //Set JobPost Model
                JobModel.Status = Helpers.Constants.Enums.JobStatusTypes.InternalAuction;

                //Update Model 
                JobModel = Helpers.Serializers.DeserializeJson<JobPostDto>(Helpers.Request.Put(Program._settings.Service_ApiGateway_Url + "/Db/JobPost/Update", Helpers.Serializers.SerializeJson(JobModel), HttpContext.Session.GetString("Token")));

                //Set Auction model
                AuctionDto AuctionModel = new AuctionDto()
                {
                    JobID = JobId,
                    JobPosterUserId = JobModel.UserID,
                    CreateDate = DateTime.Now,
                    Status = AuctionStatusTypes.InternalBidding,
                    InternalAuctionEndDate = DateTime.Now.AddDays(Program._settings.InternalAuctionDays),
                    PublicAuctionEndDate = DateTime.Now.AddDays(Program._settings.InternalAuctionDays + Program._settings.PublicAuctionDays)                    
                };

                //Post model to ApiGateway
                //Add new auction
                AuctionModel = Helpers.Serializers.DeserializeJson<AuctionDto>(Helpers.Request.Post(Program._settings.Service_ApiGateway_Url + "/Db/Auction/Post", Helpers.Serializers.SerializeJson(AuctionModel), HttpContext.Session.GetString("Token")));

                if (AuctionModel != null && AuctionModel.AuctionID > 0)
                {
                    result.Success = true;
                    result.Message = "Submit succesfully.";
                    result.Content = AuctionModel;
                }

                return Json(result);

            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return Json(new SimpleResponse { Success = false, Message = Lang.ErrorNote });
        }

        /// <summary>
        ///  Job Edit Page
        /// </summary>
        /// <param name="Job">Job Id</param>     
        /// <returns></returns>
        [Route("StartInformalVoting/{jobid}")]
        public IActionResult StartInformalVoting(int jobid)
        {
            SimpleResponse res = new SimpleResponse();

            try
            {
                //Get related auction if exists
                var auctionJson = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/Auction/GetByJobId?jobid=" + jobid, HttpContext.Session.GetString("Token"));
                AuctionDto auction = Helpers.Serializers.DeserializeJson<AuctionDto>(auctionJson);

                if (auction == null || auction.AuctionID <= 0 || auction.Status != AuctionStatusTypes.Completed)
                {
                    return Json(new SimpleResponse { Success = false, Message = "Could not found completed auction for this job." });
                }

                //Get winner bid
                AuctionBidDto winnerBid = Helpers.Serializers.DeserializeJson<AuctionBidDto>(Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/AuctionBid/GetId?id=" + auction.WinnerAuctionBidID, HttpContext.Session.GetString("Token")));

                if (winnerBid.UserId != HttpContext.Session.GetInt32("UserID"))
                {
                    return Json(new SimpleResponse { Success = false, Message = "User is not authorized to start informal voting for this job." });
                }

                //Start informal voting
                VotingDto informalVoting = new VotingDto();
                informalVoting.JobID = jobid;
                informalVoting.StartDate = DateTime.Now;
                informalVoting.EndDate = DateTime.Now.AddDays(Program._settings.InformalVotingDays);
                //Get total dao member count
                int daoMemberCount = Convert.ToInt32(Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/Users/GetCount", HttpContext.Session.GetString("Token")));
                informalVoting.QuorumCount = Convert.ToInt32(Program._settings.QuorumRatio * Convert.ToDouble(daoMemberCount));

                string jsonResult = Helpers.Request.Post(Program._settings.Service_ApiGateway_Url + "/Voting/Voting/StartInformalVoting", Helpers.Serializers.SerializeJson(informalVoting), HttpContext.Session.GetString("Token"));
                res = Helpers.Serializers.DeserializeJson<SimpleResponse>(jsonResult);
                res.Content = null;

                //Change job status 
                Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/JobPost/ChangeJobStatus?jobid="+jobid+"&status="+ JobStatusTypes.InformalVoting, HttpContext.Session.GetString("Token"));

                return Json(res);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return Json(new SimpleResponse { Success = false, Message = Lang.ErrorNote });
        }

        #region UserSerttings

        /// <summary>
        /// Delete user comment and comments upvote and downvote
        /// </summary>
        /// <param name="CommentId">JobPostCommentID</param>
        /// <returns></returns>
        [HttpDelete]
        public JsonResult DeleteComment(int CommentId)
        {
            SimpleResponse result = new SimpleResponse();
            try
            {
                JobPostCommentDto model = new JobPostCommentDto();

                //Get comment
                var modelJson = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/JobPostComment/GetId?id=" + CommentId, HttpContext.Session.GetString("Token"));

                //Parse result
                model = Helpers.Serializers.DeserializeJson<JobPostCommentDto>(modelJson);
                model.Comment = "This comment is deleted by the owner.";

                //Update comment as deleted        
                var deleteModelJson = Helpers.Request.Put(Program._settings.Service_ApiGateway_Url + "/Db/JobPostComment/Update", Helpers.Serializers.SerializeJson(model), HttpContext.Session.GetString("Token"));
                model = Helpers.Serializers.DeserializeJson<JobPostCommentDto>(deleteModelJson);

                if (model != null && model.JobPostCommentID > 0)
                {
                    result.Success = true;
                    result.Message = "Comment succesfully deleted.";
                    result.Content = "";
                }

                return Json(result);

            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return Json(new SimpleResponse { Success = false, Message = Lang.ErrorNote });

        }

        [HttpGet]
        public JsonResult SetCookie(string src)
        {
            CookieOptions cookies = new CookieOptions();
            cookies.Expires = DateTime.Now.AddDays(90);
            Response.Cookies.Append("theme", src, cookies);
            return Json("");
        }
        #endregion

    }
}
