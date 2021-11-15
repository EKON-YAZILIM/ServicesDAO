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

        /// <summary>
        /// Dashboard Page
        /// </summary>
        /// <returns></returns>
        [Route("Home")]
        [Route("Dashboard")]
        public IActionResult Index()
        {
            ViewBag.Title = "Dashboard";

            
            try
            {
                //Get user type from session
                string userType = HttpContext.Session.GetString("UserType");

                //User type control for admin
                if (userType == Helpers.Constants.Enums.UserIdentityType.Admin.ToString())
                {
                    GetDashBoardViewModel dashModel = new GetDashBoardViewModel();
                    //Get dashboard data from ApiGateway
                    string dashboardJson = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/Website/GetDashBoardAdmin?userid=" + HttpContext.Session.GetInt32("UserID"), HttpContext.Session.GetString("Token"));
                    //Parse response
                    dashModel = Helpers.Serializers.DeserializeJson<GetDashBoardViewModel>(dashboardJson);
                    return View("Index_Admin", dashModel);
                }
                //User type control for associate
                if (userType == Helpers.Constants.Enums.UserIdentityType.Associate.ToString())
                {
                    GetDashBoardViewModel dashModel = new GetDashBoardViewModel();
                    //Get dashboard data from ApiGateway
                    string dashboardJson = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/Website/GetDashBoardAdmin?userid=" + HttpContext.Session.GetInt32("UserID"), HttpContext.Session.GetString("Token"));
                    //Parse response
                    dashModel = Helpers.Serializers.DeserializeJson<GetDashBoardViewModel>(dashboardJson);
                    return View("Index_Associate", dashModel);
                }
                //User type control for voting associate
                if (userType == Helpers.Constants.Enums.UserIdentityType.VotingAssociate.ToString())
                {
                    GetDashBoardViewModelVA dashModel = new GetDashBoardViewModelVA();
                    //Get dashboard data from ApiGateway
                    string dashboardJson = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/Website/GetDashBoardVA?userid=" + HttpContext.Session.GetInt32("UserID"), HttpContext.Session.GetString("Token"));
                    //Parse response
                    dashModel = Helpers.Serializers.DeserializeJson<GetDashBoardViewModelVA>(dashboardJson);
                    return View("Index_VotingAssociate", dashModel);
                }

            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return View("../Shared/Error.cshtml");
        }

        #region Job Post

        /// <summary>
        /// User's Job Page
        /// </summary>
        /// <returns></returns>
        [Route("My-Jobs")]
        public IActionResult My_Jobs()
        {
            ViewBag.Title = "My Jobs";

            MyJobsViewModel myJobsModel = new MyJobsViewModel();

            try
            {
                //Get jobs data from ApiGateway
                string jobsJson = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/Website/GetUserJobs?userid=" + HttpContext.Session.GetInt32("UserID"), HttpContext.Session.GetString("Token"));
                //Parse response
                myJobsModel = Helpers.Serializers.DeserializeJson<MyJobsViewModel>(jobsJson);
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
            ViewBag.Title = "All Jobs";

            List<JobPostViewModel> allJobsModel = new List<JobPostViewModel>();

            try
            {
                //Get jobs data from ApiGateway
                string jobsJson = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/Website/GetAllJobs", HttpContext.Session.GetString("Token"));
                //Parse response
                allJobsModel = Helpers.Serializers.DeserializeJson<List<JobPostViewModel>>(jobsJson);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return View(allJobsModel);
        }

        /// <summary>
        /// New Job Page
        /// </summary>
        /// <returns></returns>
        [Route("New-Job")]
        public IActionResult New_Job()
        {
            ViewBag.Title = "Post A New Job";

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
            SimpleResponse result = new SimpleResponse();

            try
            {
                //Create JobPost model
                JobPostDto model = new JobPostDto() { UserID = Convert.ToInt32(HttpContext.Session.GetInt32("UserID")), Amount = amount, JobDescription = description, CreateDate = DateTime.Now, TimeFrame = time, LastUpdate = DateTime.Now, Title = title, Status = Enums.JobStatusTypes.AdminApprovalPending };

                //Post model to ApiGateway
                string jobPostResponseJson = Helpers.Request.Post(Program._settings.Service_ApiGateway_Url + "/Db/JobPost/Post", Helpers.Serializers.SerializeJson(model), HttpContext.Session.GetString("Token"));
                //Parse reponse
                model = Helpers.Serializers.DeserializeJson<JobPostDto>(jobPostResponseJson);

                if (model != null && model.JobID > 0)
                {
                    result.Success = true;
                    result.Message = "Job posted successfully and will be available after admin review.";
                    result.Content = model;

                    //Set server side toastr because page will be redirected
                    TempData["toastr-message"] = result.Message;
                    TempData["toastr-type"] = "success";

                    return Json(result);
                }
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
        [Route("My-Job-Edit/{Job}")]
        public IActionResult My_Job_Edit(int Job)
        {
            ViewBag.Title = "Edit Job";

            JobPostDto jobDetailModel = new JobPostDto();

            try
            {
                //Get model from ApiGateway
                string jobJson = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/JobPost/GetId?id=" + Job, HttpContext.Session.GetString("Token"));
                //Parse response
                jobDetailModel = Helpers.Serializers.DeserializeJson<JobPostDto>(jobJson);

                //Check if user trying to edit job for another user
                if (jobDetailModel.UserID != Convert.ToInt32(HttpContext.Session.GetInt32("UserID")))
                {
                    return View("../Shared/Error.cshtml");
                }
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
            SimpleResponse result = new SimpleResponse();

            try
            {
                //Get model from ApiGateway
                string jobJson = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/JobPost/GetId?id=" + Model.JobID, HttpContext.Session.GetString("Token"));
                //Parse response
                JobPostDto jobDetailModel = Helpers.Serializers.DeserializeJson<JobPostDto>(jobJson);

                //Check if user trying to edit job for another user
                if (jobDetailModel.UserID != Convert.ToInt32(HttpContext.Session.GetInt32("UserID")))
                {
                    return Json(new SimpleResponse { Success = false, Message = Lang.UnauthorizedAccess });
                }

                //Put model to ApiGateway
                string updateResponseJson = Helpers.Request.Put(Program._settings.Service_ApiGateway_Url + "/Db/JobPost/Update", Helpers.Serializers.SerializeJson(Model), HttpContext.Session.GetString("Token"));
                //Parse response
                JobPostDto model = Helpers.Serializers.DeserializeJson<JobPostDto>(updateResponseJson);

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

        #endregion

        #region Job Post Detail & Forum

        /// <summary>
        /// Job Detail Page
        /// </summary>
        /// <param name="JobID">Job Id</param>
        /// <returns></returns>
        [Route("Job-Detail/{JobID}")]
        public IActionResult Job_Detail(int JobID)
        {
            ViewBag.Title = "Job Details";

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
                //Create new comment model
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
                        //This indicates job doer posting a comment as "Work of evidence" so it will be pinned comment
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
                model = Helpers.Serializers.DeserializeJson<JobPostCommentDto>(Helpers.Request.Post(Program._settings.Service_ApiGateway_Url + "/Db/JobPostComment/Post", Helpers.Serializers.SerializeJson(model), HttpContext.Session.GetString("Token")));

                if (model != null && model.JobPostCommentID > 0)
                {
                    result.Success = true;
                    result.Message = "Comment succesfully posted.";
                    result.Content = model;

                    //Set server side toastr because page will be redirected
                    TempData["toastr-message"] = result.Message;
                    TempData["toastr-type"] = "success";
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

                //Check if user trying to delete comment for another user
                if (model.UserID != Convert.ToInt32(HttpContext.Session.GetInt32("UserID")))
                {
                    return Json(new SimpleResponse { Success = false, Message = Lang.UnauthorizedAccess });
                }
                else
                {
                    model.Comment = "This comment is deleted by the owner.";

                    //Update comment as deleted        
                    var deleteModelJson = Helpers.Request.Put(Program._settings.Service_ApiGateway_Url + "/Db/JobPostComment/Update", Helpers.Serializers.SerializeJson(model), HttpContext.Session.GetString("Token"));
                    //Parse result
                    model = Helpers.Serializers.DeserializeJson<JobPostCommentDto>(deleteModelJson);

                    if (model != null && model.JobPostCommentID > 0)
                    {
                        result.Success = true;
                        result.Message = "Comment succesfully deleted.";
                        result.Content = "";

                        //Set server side toastr because page will be redirected
                        TempData["toastr-message"] = result.Message;
                        TempData["toastr-type"] = "success";
                    }
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

        #endregion

        #region Auctions

        /// <summary>
        /// Auctions Page
        /// </summary>
        /// <returns></returns>
        [Route("Auctions")]
        public IActionResult Auctions()
        {
            ViewBag.Title = "Auctions";

            List<AuctionViewModel> auctionsModel = new List<AuctionViewModel>();

            try
            {
                //Get model from ApiGateway
                var auctionsJson = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/Website/GetAuctions", HttpContext.Session.GetString("Token"));
                //Parse response
                auctionsModel = Helpers.Serializers.DeserializeJson<List<AuctionViewModel>>(auctionsJson);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }
            return View(auctionsModel);
        }

        /// <summary>
        /// Auction Detail Page
        /// </summary>
        /// <param name="AuctionID">Auction Id</param>
        /// <returns></returns>
        [Route("Auction-Detail/{AuctionID}")]
        public IActionResult Auction_Detail(int AuctionID)
        {
            ViewBag.Title = "Auction Details";

            AuctionDetailViewModel AuctionDetailModel = new AuctionDetailViewModel();

            try
            {
                //Get bids model from ApiGateway
                var auctionBidsJson = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/Website/GetAuctionBids?auctionid=" + AuctionID, HttpContext.Session.GetString("Token"));
                //Get auction model from ApiGateway
                var auctionJson = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/Auction/GetId?id=" + AuctionID, HttpContext.Session.GetString("Token"));

                //Parse response
                AuctionDetailModel.BidItems = Helpers.Serializers.DeserializeJson<List<AuctionBidItemModel>>(auctionBidsJson);
                //Parse response
                AuctionDetailModel.Auction = Helpers.Serializers.DeserializeJson<AuctionDto>(auctionJson);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }
            return View(AuctionDetailModel);
        }

        /// <summary>
        /// Add new bid for auction
        /// </summary>
        /// <param name="Model">AuctionBidDto Model</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Auction_Bid_Add(AuctionBidDto Model)
        {
            SimpleResponse result = new SimpleResponse();

            try
            {
                //Get auction model from ApiGateway
                var auctionJson = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/Auction/GetId?id=" + Model.AuctionID, HttpContext.Session.GetString("Token"));
                //Parse response
                AuctionDto auction = Helpers.Serializers.DeserializeJson<AuctionDto>(auctionJson);

                //Get bids model from ApiGateway
                var auctionBidsJson = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/AuctionBid/GetByAuctionId?auctionid=" + auction.AuctionID, HttpContext.Session.GetString("Token"));
                //Parse response
                List<AuctionBidDto> bids = Helpers.Serializers.DeserializeJson<List<AuctionBidDto>>(auctionBidsJson);

                //Check if public user trying to submit bid for expired or completed auction
                if (auction.Status == Enums.AuctionStatusTypes.Completed || auction.Status == Enums.AuctionStatusTypes.Expired)
                {
                    return Json(new SimpleResponse { Success = false, Message = "You can't submit bid to a closed auction" });
                }

                //Check if user trying to submit bid for his/her own job
                if (auction.JobPosterUserId == Convert.ToInt32(HttpContext.Session.GetInt32("UserID")))
                {
                    return Json(new SimpleResponse { Success = false, Message = "You can't submit bid to your own job." });
                }

                //Check if public user trying to submit bid for internal auction
                if (auction.Status == Enums.AuctionStatusTypes.InternalBidding && HttpContext.Session.GetString("UserType") == Enums.UserIdentityType.Associate.ToString())
                {
                    return Json(new SimpleResponse { Success = false, Message = "This auction is opened for internal members." });
                }

                //Check if user already submitted bid to this auction
                if (bids.Count(x=>x.UserId == Convert.ToInt32(HttpContext.Session.GetInt32("UserID"))) > 0)
                {
                    return Json(new SimpleResponse { Success = false, Message = "You already have an existing bid for this auction." });
                }

                Model.UserId = Convert.ToInt32(HttpContext.Session.GetInt32("UserID"));
                Model.CreateDate = DateTime.Now;

                //Post model to ApiGateway
                Model = Helpers.Serializers.DeserializeJson<AuctionBidDto>(Helpers.Request.Post(Program._settings.Service_ApiGateway_Url + "/Db/AuctionBid/Post", Helpers.Serializers.SerializeJson(Model), HttpContext.Session.GetString("Token")));

                if (Model != null && Model.AuctionBidID > 0)
                {
                    result.Success = true;
                    result.Message = "Bid succesffully submitted.";
                    result.Content = Model;

                    //Stake bid if internal auction
                    if (auction.Status == Enums.AuctionStatusTypes.InternalBidding)
                    {
                        UserReputationStakeDto stake = new UserReputationStakeDto() { UserID = Convert.ToInt32(HttpContext.Session.GetInt32("UserID")), Amount = Model.ReputationStake, CreateDate = DateTime.Now, Type = StakeType.Bid, ReferenceID = Model.AuctionBidID, ReferenceProcessID = Model.AuctionID, Status = ReputationStakeStatus.Staked };

                        //Post model to ApiGateway
                        string reputationJson = Helpers.Request.Post(Program._settings.Service_ApiGateway_Url + "/Reputation/UserReputationStake/SubmitStake", Helpers.Serializers.SerializeJson(stake), HttpContext.Session.GetString("Token"));

                        SimpleResponse reputationResponse = Helpers.Serializers.DeserializeJson<SimpleResponse>(reputationJson);

                        //Delete bid from db if reputation stake is unsuccesful
                        if (reputationResponse.Success == false)
                        {
                            var deleteModel = Helpers.Request.Delete(Program._settings.Service_ApiGateway_Url + "/Db/AuctionBid/Delete?ID=" + Model.AuctionBidID, HttpContext.Session.GetString("Token"));

                            return Json(reputationResponse);
                        }
                    }
                    else
                    {
                        Model.ReputationStake = 0;
                    }
                }

                //Set server side toastr because page will be redirected
                TempData["toastr-message"] = result.Message;
                TempData["toastr-type"] = "success";

                return Json(result);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return Json(new SimpleResponse { Success = false, Message = Lang.ErrorNote });

        }

        /// <summary>
        /// Delete Auction Bid
        /// </summary>
        /// <param name="id">Auction Bid ID</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult Auction_Bid_Delete(int id)
        {
            SimpleResponse result = new SimpleResponse();

            try
            {
                //Get auction bid model from ApiGateway
                var auctionBidJson = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/AuctionBid/GetId?id=" + id, HttpContext.Session.GetString("Token"));
                //Parse response
                AuctionBidDto auctionBid = Helpers.Serializers.DeserializeJson<AuctionBidDto>(auctionBidJson);

                //Check if this bid belongs to user
                if (auctionBid.UserId != Convert.ToInt32(HttpContext.Session.GetInt32("UserID")))
                {
                    return Json(new SimpleResponse { Success = false, Message = Lang.UnauthorizedAccess });
                }

                //Release staked reputation for the bid.
                SimpleResponse releaseStakeResponse = Helpers.Serializers.DeserializeJson<SimpleResponse>(Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Voting/UserReputationStake/DeleteSingleStake?referenceID=" + id + "&reftype=" + Enums.StakeType.Bid, HttpContext.Session.GetString("Token")));

                //Post model to ApiGateway
                var deleteBidResponse = Helpers.Serializers.DeserializeJson<bool>(Helpers.Request.Delete(Program._settings.Service_ApiGateway_Url + "/Db/AuctionBid/Delete?id=" + id, HttpContext.Session.GetString("Token")));

                if (deleteBidResponse)
                {
                    result.Success = true;
                    result.Message = "Bid succesffully deleted.";

                    //Set server side toastr because page will be redirected
                    TempData["toastr-message"] = result.Message;
                    TempData["toastr-type"] = "success";
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
        ///  This method chooses winner bid and changes the status of the auction as completed. 
        ///  Only job poster is authorized to call the method
        /// </summary>
        /// <param name="bidId"></param>
        /// <param name="auctionId"></param>
        /// <param name="jobid"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult ChooseWinnerBid(int bidId)
        {
            SimpleResponse result = new SimpleResponse();
            try
            {                
                //Get auction bid model from ApiGateway
                var auctionBidJson = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/AuctionBid/GetId?id=" + bidId, HttpContext.Session.GetString("Token"));
                //Parse response
                AuctionBidDto auctionBid = Helpers.Serializers.DeserializeJson<AuctionBidDto>(auctionBidJson);

                //Get auction model from ApiGateway
                var auctionJson = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/Auction/GetId?id=" + auctionBid.AuctionID, HttpContext.Session.GetString("Token"));
                //Parse response
                AuctionDto auction = Helpers.Serializers.DeserializeJson<AuctionDto>(auctionJson);

                //Check if user is authorized to choose winner (Must be job poster)
                if (auction.JobPosterUserId != HttpContext.Session.GetInt32("UserID"))
                {
                    return Json(new SimpleResponse { Success = false, Message = Lang.UnauthorizedAccess });
                }

                //Post bid
                bool bidChooseResult = Helpers.Serializers.DeserializeJson<bool>(Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/Auction/SetWinnerBid?bidId=" + bidId, HttpContext.Session.GetString("Token")));
                if (bidChooseResult)
                {
                    //Change job status to Auction Completed
                    JobPostDto jobStatusResult = Helpers.Serializers.DeserializeJson<JobPostDto>(Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/JobPost/ChangeJobStatus?jobid=" + auction.JobID + "&status=" + Helpers.Constants.Enums.JobStatusTypes.AuctionCompleted, HttpContext.Session.GetString("Token")));

                    if (jobStatusResult.JobID > 0)
                    {
                        //Release staked reputations for auction
                        SimpleResponse stakeReleaseResponse = Helpers.Serializers.DeserializeJson<SimpleResponse>(Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Reputation/UserReputationStake/ReleaseStakes?referenceProcessID=" + auction.AuctionID + "&reftype=" + Helpers.Constants.Enums.StakeType.Bid, HttpContext.Session.GetString("Token")));

                        //Mint new reputation with (ReputationConversionRate(DAO Variable) * Bid Price)
                        UserReputationStakeDto stake = new UserReputationStakeDto() { UserID = auctionBid.UserId, Amount = auctionBid.Price * Program._settings.ReputationConversionRate, CreateDate = DateTime.Now, Type = StakeType.Mint, ReferenceID = auctionBid.UserId, ReferenceProcessID = auction.JobID, Status = ReputationStakeStatus.Staked };
                        //Post model to ApiGateway
                        string mintJson = Helpers.Request.Post(Program._settings.Service_ApiGateway_Url + "/Reputation/UserReputationStake/SubmitStake", Helpers.Serializers.SerializeJson(stake), HttpContext.Session.GetString("Token"));
                        //Parse response
                        SimpleResponse mintReponse = Helpers.Serializers.DeserializeJson<SimpleResponse>(mintJson);

                        if (stakeReleaseResponse.Success && mintReponse.Success)
                        {
                            result.Success = true;
                            result.Message = "Winner bid selected.";

                            //Set server side toastr because page will be redirected
                            TempData["toastr-message"] = result.Message;
                            TempData["toastr-type"] = "success";
                        }
                    }

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

        #region Voting

        /// <summary>
        /// Votes Page
        /// </summary>
        /// <returns></returns>
        [Route("Votings")]
        public IActionResult Votings()
        {
            ViewBag.Title = "Votings";

            List<VotingViewModel> votesModel = new List<VotingViewModel>();
            try
            {
                //Get model from ApiGateway
                var votingJson = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/Website/GetVotingsByStatus", HttpContext.Session.GetString("Token"));

                //Parse response
                votesModel = Helpers.Serializers.DeserializeJson<List<VotingViewModel>>(votingJson);

            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }
            return View(votesModel);
        }

        /// <summary>
        /// Vote Detail Page
        /// </summary>
        /// <param name="VoteID">Vote Id</param>
        /// <returns></returns>
        [Route("Vote-Detail/{VotingID}")]
        public IActionResult Vote_Detail(int VotingID)
        {
            ViewBag.Title = "Voting Details";

            VoteDetailViewModel voteDetailModel = new VoteDetailViewModel();

            try
            {
                //Get voting model from ApiGateway
                var votingJson = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Voting/Voting/GetId?id=" + VotingID, HttpContext.Session.GetString("Token"));
                var voting = Helpers.Serializers.DeserializeJson<VotingDto>(votingJson);

                //Get votes model from ApiGateway
                var votesJson = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Voting/Vote/GetAllVotesByVotingId?votingid=" + VotingID, HttpContext.Session.GetString("Token"));
                var votes = Helpers.Serializers.DeserializeJson<List<VoteDto>>(votesJson);

                //Get reputation stakes from reputation service
                var reputationsJson = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Reputation/UserReputationStake/GetByProcessId?referenceProcessID=" + VotingID + "&reftype=" + StakeType.For, HttpContext.Session.GetString("Token"));
                var reputations = Helpers.Serializers.DeserializeJson<List<UserReputationStakeDto>>(reputationsJson);

                //Get usernames of voters
                var usernamesJson = Helpers.Request.Post(Program._settings.Service_ApiGateway_Url + "/Db/Users/GetUsernamesByUserIds", Helpers.Serializers.SerializeJson(votes.Select(x => x.UserID)), HttpContext.Session.GetString("Token"));
                var usernames = Helpers.Serializers.DeserializeJson<List<string>>(usernamesJson);


                //Combine results into VoteItemModel
                List<VoteItemModel> voteItems = new List<VoteItemModel>();
                votes = votes.OrderBy(x => x.UserID).ToList();
                for (int i = 0; i < votes.Count; i++)
                {
                    VoteItemModel vt = new VoteItemModel();
                    vt.UserID = votes[i].UserID;
                    vt.Date = votes[i].Date;
                    vt.Direction = votes[i].Direction;
                    vt.VoteID = votes[i].VoteID;
                    vt.VotingID = votes[i].VotingID;
                    vt.UserName = usernames[i];
                    vt.ReputationStake = reputations.First(x => x.UserID == vt.UserID).Amount;
                    voteItems.Add(vt);
                }

                //Parse response
                voteDetailModel.VoteItems = voteItems;
                //Parse response
                voteDetailModel.Voting = Helpers.Serializers.DeserializeJson<VotingDto>(votingJson);

            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return View(voteDetailModel);
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
                informalVoting.ReputationDistributionRatio = Program._settings.ReputationDistributionRatio;

                //Get related job post
                var jobJson = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/JobPost/GetId?id=" + jobid, HttpContext.Session.GetString("Token"));
                JobPostDto job = Helpers.Serializers.DeserializeJson<JobPostDto>(jobJson);

                //Get total dao member count
                int daoMemberCount = Convert.ToInt32(Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/Users/GetCount?type="+UserIdentityType.VotingAssociate, HttpContext.Session.GetString("Token")));
                //Quorum count is calculated with total user count - 2(job poster, job doer)
                informalVoting.QuorumCount = Convert.ToInt32(Program._settings.QuorumRatio * Convert.ToDouble(daoMemberCount - 2));

                string jsonResult = Helpers.Request.Post(Program._settings.Service_ApiGateway_Url + "/Voting/Voting/StartInformalVoting", Helpers.Serializers.SerializeJson(informalVoting), HttpContext.Session.GetString("Token"));
                res = Helpers.Serializers.DeserializeJson<SimpleResponse>(jsonResult);
                res.Content = null;

                //Change job status 
                Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/JobPost/ChangeJobStatus?jobid=" + jobid + "&status=" + JobStatusTypes.InformalVoting, HttpContext.Session.GetString("Token"));

                return Json(res);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return Json(new SimpleResponse { Success = false, Message = Lang.ErrorNote });
        }

        [HttpPost]
        public JsonResult SubmitVote(int VotingID, StakeType Direction, double? ReputationStake)
        {
            SimpleResponse result = new SimpleResponse();

            try
            {
                //Get voting model from ApiGateway
                var votingJson = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Voting/Voting/GetId?id=" + VotingID, HttpContext.Session.GetString("Token"));
                //Parse response
                VotingDto voting = Helpers.Serializers.DeserializeJson<VotingDto>(votingJson);

                //Get job post model from ApiGateway
                var jobJson = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/JobPost/GetId?id=" + voting.JobID, HttpContext.Session.GetString("Token"));
                //Parse response
                JobPostDto job = Helpers.Serializers.DeserializeJson<JobPostDto>(jobJson);

                //Check if public user trying to submit bid for expired or completed auction
                if (voting.Status != Enums.VoteStatusTypes.Active)
                {
                    return Json(new SimpleResponse { Success = false, Message = "You can't submit vote to a closed voting." });
                }

                //Check if user trying to submit bid for his/her own job
                if (job.UserID == Convert.ToInt32(HttpContext.Session.GetInt32("UserID")) || job.JobDoerUserID == Convert.ToInt32(HttpContext.Session.GetInt32("UserID")))
                {
                    return Json(new SimpleResponse { Success = false, Message = "You can't submit vote to your own job." });
                }

                //Check if user trying to submit bid for his/her own job
                if (voting.Type == VoteTypes.JobCompletion && (ReputationStake == null || ReputationStake <= 0))
                {
                    return Json(new SimpleResponse { Success = false, Message = "You must stake reputation greater than 0 for this voting type." });
                }

                //Post model to ApiGateway
                string jsonResponse = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Voting/Vote/SubmitVote?VotingID=" + VotingID + "&UserID=" + Convert.ToInt32(HttpContext.Session.GetInt32("UserID")) + "&Direction=" + Direction + "&ReputationStake=" + ReputationStake.ToString().Replace(",", "."), HttpContext.Session.GetString("Token"));
                result = Helpers.Serializers.DeserializeJson<SimpleResponse>(jsonResponse);

                if (result.Success)
                {
                    //Set server side toastr because page will be redirected
                    TempData["toastr-message"] = result.Message;
                    TempData["toastr-type"] = "success";
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

        #region Reputation

        /// <summary>
        /// User Reputation History Page
        /// </summary>
        /// <returns></returns>
        [Route("Reputation-History")]
        public IActionResult Reputation_History()
        {
            ViewBag.Title = "Reputation History";

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

        #endregion

        #region User Views & Methods

        /// <summary>
        /// User Profile Page
        /// </summary>
        /// <returns></returns>
        [Route("User-Profile")]
        public IActionResult User_Profile()
        {
            ViewBag.Title = "User Profile";

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

        [HttpGet]
        [Route("ProfileUpdate")]
        public JsonResult ProfileUpdate()
        {
            SimpleResponse result = new SimpleResponse();

            try
            {
            

                return Json(result);

            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return Json(new SimpleResponse { Success = false, Message = Lang.ErrorNote });
        }

        [HttpGet]
        [Route("SubmitKYC")]
        public JsonResult SubmitKYC()
        {
            SimpleResponse result = new SimpleResponse();

            try
            {
                //Get Model from ApiGateway          
                var userJson = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/Users/GetId?id=" + HttpContext.Session.GetInt32("UserID"), HttpContext.Session.GetString("Token"));
                //Parse result
                var userModel = Helpers.Serializers.DeserializeJson<UserDto>(userJson);
                userModel.KYCStatus = true;
                //Update Model 
                var userResponse = Helpers.Serializers.DeserializeJson<UserDto>(Helpers.Request.Put(Program._settings.Service_ApiGateway_Url + "/Db/Users/Update", Helpers.Serializers.SerializeJson(userModel), HttpContext.Session.GetString("Token")));

                if (userResponse != null && userResponse.UserId >= 0 && userResponse.KYCStatus == true)
                {
                    //Get Model from ApiGateway          
                    var jobsJson = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/JobPost/GetByUserId?userid=" + HttpContext.Session.GetInt32("UserID"), HttpContext.Session.GetString("Token"));
                    //Parse result
                    var JobsModel = Helpers.Serializers.DeserializeJson<List<JobPostDto>>(jobsJson);

                    //Update users KYC Pending jobs to DoSFeePending
                    foreach (var pendingJob in JobsModel)
                    {
                        if (pendingJob.Status == JobStatusTypes.KYCPending)
                        {
                            pendingJob.Status = JobStatusTypes.DoSFeePending;
                            //Update Model 
                            var jobUpdatResponse = Helpers.Serializers.DeserializeJson<JobPostDto>(Helpers.Request.Put(Program._settings.Service_ApiGateway_Url + "/Db/JobPost/Update", Helpers.Serializers.SerializeJson(pendingJob), HttpContext.Session.GetString("Token")));
                        }
                    }
                }

                result.Success = true;
                result.Message = "KYC completed successfully.";

                return Json(result);

            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return Json(new SimpleResponse { Success = false, Message = Lang.ErrorNote });
        }

        [HttpGet]
        [Route("PayDosFee/{JobId}")]
        public JsonResult PayDosFee(int JobId)
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
                    result.Message = "DoS fee successfully paid. Internal auction process started for the job.";
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

        #endregion

        #region Admin Views & Methods

        [AuthorizeAdmin]
        [HttpGet]
        public JsonResult AdminJobApproval(int JobId)
        {
            SimpleResponse result = new SimpleResponse();

            try
            {
                //Get Model from ApiGateway          
                var jobJson = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/JobPost/GetId?id=" + JobId, HttpContext.Session.GetString("Token"));
                //Parse result
                var JobModel = Helpers.Serializers.DeserializeJson<JobPostDto>(jobJson);

                //Get job poster user object 
                var userJson = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/Users/GetId?id=" + JobModel.UserID, HttpContext.Session.GetString("Token"));
                //Parse result
                var userModel = Helpers.Serializers.DeserializeJson<UserDto>(userJson);

                //If job poster is admin or VA start auction immediately
                if (userModel.UserType == UserIdentityType.VotingAssociate.ToString() || userModel.UserType == UserIdentityType.Admin.ToString())
                {
                    //Set JobPost Model
                    JobModel.Status = Helpers.Constants.Enums.JobStatusTypes.InternalAuction;

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
                        result.Message = "Job approved. Internal auction process started for the job.";
                        result.Content = AuctionModel;
                    }
                }
                //If job poster is not admin or VA check for KYC
                else
                {
                    if (userModel.KYCStatus == false)
                    {
                        JobModel.Status = Helpers.Constants.Enums.JobStatusTypes.KYCPending;
                        result.Success = true;
                        result.Message = "Job approved. User needs to complete KYC.";
                    }
                    else
                    {
                        JobModel.Status = Helpers.Constants.Enums.JobStatusTypes.DoSFeePending;
                        result.Success = true;
                        result.Message = "Job approved. User needs to pay DoS fee.";
                    }
                }

                //Update job status 
                JobModel = Helpers.Serializers.DeserializeJson<JobPostDto>(Helpers.Request.Put(Program._settings.Service_ApiGateway_Url + "/Db/JobPost/Update", Helpers.Serializers.SerializeJson(JobModel), HttpContext.Session.GetString("Token")));

                return Json(result);

            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return Json(new SimpleResponse { Success = false, Message = Lang.ErrorNote });
        }

        [AuthorizeAdmin]
        [HttpGet]
        public JsonResult AdminJobDisapproval(int JobId)
        {
            SimpleResponse result = new SimpleResponse();

            try
            {
                //Get Model from ApiGateway          
                var jobJson = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/JobPost/GetId?id=" + JobId, HttpContext.Session.GetString("Token"));
                //Parse result
                var JobModel = Helpers.Serializers.DeserializeJson<JobPostDto>(jobJson);

                //Get job poster user object 
                var userJson = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/Users/GetId?id=" + JobModel.UserID, HttpContext.Session.GetString("Token"));
                //Parse result
                var userModel = Helpers.Serializers.DeserializeJson<UserDto>(userJson);

                //Set JobPost Model
                JobModel.Status = Helpers.Constants.Enums.JobStatusTypes.Rejected;

                //Update job status 
                JobModel = Helpers.Serializers.DeserializeJson<JobPostDto>(Helpers.Request.Put(Program._settings.Service_ApiGateway_Url + "/Db/JobPost/Update", Helpers.Serializers.SerializeJson(JobModel), HttpContext.Session.GetString("Token")));
                if(JobModel.JobID>0 && JobModel.Status == JobStatusTypes.Rejected)
                {
                    result.Success = true;
                    result.Message = "Job disapproved.";
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
        ///  This view shows global parameters of the DAO
        /// </summary>
        /// <returns></returns>
        [Route("Dao-Variables")]
        public IActionResult Dao_Variables()
        {
            ViewBag.Title = "DAO Variables";

            return View();
        }
        #endregion

        #region Utility

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
