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
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static Helpers.Constants.Enums;
using Newtonsoft.Json;
using PagedList.Core;
using Helpers.Models.NotificationModels;
using Helpers.Models.KYCModels;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

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
                    DashBoardViewModelAdmin dashModel = new DashBoardViewModelAdmin();
                    //Get dashboard data from ApiGateway
                    string dashboardJson = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/Website/GetDashBoardAdmin?userid=" + HttpContext.Session.GetInt32("UserID"), HttpContext.Session.GetString("Token"));
                    //Parse response
                    dashModel = Helpers.Serializers.DeserializeJson<DashBoardViewModelAdmin>(dashboardJson);
                    return View("Index_Admin", dashModel);
                }
                //User type control for associate
                if (userType == Helpers.Constants.Enums.UserIdentityType.Associate.ToString())
                {
                    DashBoardViewModelVA dashModel = new DashBoardViewModelVA();
                    //Get dashboard data from ApiGateway
                    string dashboardJson = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/Website/GetDashBoardAssociate?userid=" + HttpContext.Session.GetInt32("UserID"), HttpContext.Session.GetString("Token"));
                    //Parse response
                    dashModel = Helpers.Serializers.DeserializeJson<DashBoardViewModelVA>(dashboardJson);

                    //Get model from ApiGateway
                    var ReputationUrl = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Reputation/UserReputationHistory/GetLastReputation?userid=" + HttpContext.Session.GetInt32("UserID"), HttpContext.Session.GetString("Token"));

                    //Parse response
                    dashModel.UserReputation = Helpers.Serializers.DeserializeJson<UserReputationHistoryDto>(ReputationUrl);

                    return View("Index_Associate", dashModel);
                }
                //User type control for voting associate
                if (userType == Helpers.Constants.Enums.UserIdentityType.VotingAssociate.ToString())
                {
                    DashBoardViewModelVA dashModel = new DashBoardViewModelVA();
                    //Get dashboard data from ApiGateway
                    string dashboardJson = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/Website/GetDashBoardVA?userid=" + HttpContext.Session.GetInt32("UserID"), HttpContext.Session.GetString("Token"));
                    //Parse response
                    dashModel = Helpers.Serializers.DeserializeJson<DashBoardViewModelVA>(dashboardJson);

                    //Get model from ApiGateway
                    var reputationJson = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Reputation/UserReputationHistory/GetLastReputation?userid=" + HttpContext.Session.GetInt32("UserID"), HttpContext.Session.GetString("Token"));

                    //Parse response
                    dashModel.UserReputation = Helpers.Serializers.DeserializeJson<UserReputationHistoryDto>(reputationJson);

                    return View("Index_VotingAssociate", dashModel);
                }

            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return View("../Public/Error.cshtml");
        }

        #region Job Post

        /// <summary>
        /// User's Job Page
        /// </summary>
        /// <returns></returns>
        [Route("My-Jobs")]
        public IActionResult My_Jobs(JobStatusTypes? status, string query)
        {
            ViewBag.Title = "My Jobs";

            MyJobsViewModel myJobsModel = new MyJobsViewModel();

            try
            {
                //Get jobs data from ApiGateway
                string jobsJson = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/Website/GetUserJobs?status=" + status + "&userid=" + HttpContext.Session.GetInt32("UserID")+ "&query=" + query, HttpContext.Session.GetString("Token"));
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
        [Route("Home/All-Jobs")]
        public IActionResult All_Jobs(JobStatusTypes? status, string query, int page = 1, int pageCount = 10)
        {
            ViewBag.Title = "All Jobs";

            IPagedList<JobPostViewModel> pagedModel = new PagedList<JobPostViewModel>(null, 1, 1);

            try
            {
                //Get jobs data from ApiGateway
                string jobsJson = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/Website/GetAllJobs?status=" + status + "&userid=" + HttpContext.Session.GetInt32("UserID")+ "&query=" + query + "&page=" + page + "&pageCount=" + pageCount, HttpContext.Session.GetString("Token"));
                //Parse response
                var jobsListPaged = Helpers.Serializers.DeserializeJson<PaginationEntity<JobPostViewModel>>(jobsJson);

                pagedModel = new StaticPagedList<JobPostViewModel>(
                    jobsListPaged.Items,
                    jobsListPaged.MetaData.PageNumber,
                    jobsListPaged.MetaData.PageSize,
                    jobsListPaged.MetaData.TotalItemCount
                    );

                //var result = JsonConvert.DeserializeObject<PagedList<JobPostViewModel>>(jobsJson);

            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return View(pagedModel);

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
        [PreventDuplicateRequest]
        [ValidateAntiForgeryToken]
        public JsonResult New_Job_Post(string title, double amount, string time, string description, string tags, string codeurl)
        {
            if (!ModelState.IsValid)
            {
                return Json(new SimpleResponse { Success = false, Message = "Double post action prevented." });
            }

            SimpleResponse result = new SimpleResponse();

            try
            {
                //Empty fields control
                if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(time) || string.IsNullOrEmpty(description) || string.IsNullOrEmpty(codeurl) || amount <= 0)
                {
                    result.Success = false;
                    result.Message = "You must fill all the fields to post a job.";
                    return Json(result);
                }
                
                //Create JobPost model
                JobPostDto model = new JobPostDto() { UserID = Convert.ToInt32(HttpContext.Session.GetInt32("UserID")), Amount = amount, JobDescription = description, CreateDate = DateTime.Now, TimeFrame = time, LastUpdate = DateTime.Now, Title = title, Tags = tags, CodeUrl = codeurl, Status = Enums.JobStatusTypes.AdminApprovalPending };

                //Post model to ApiGateway
                string jobPostResponseJson = Helpers.Request.Post(Program._settings.Service_ApiGateway_Url + "/Db/JobPost/Post", Helpers.Serializers.SerializeJson(model), HttpContext.Session.GetString("Token"));
                //Parse reponse
                model = Helpers.Serializers.DeserializeJson<JobPostDto>(jobPostResponseJson);

                if (model != null && model.JobID > 0)
                {
                    result.Success = true;
                    result.Message = "Job posted successfully and will be available after admin review.";

                    result.Content = model;

                    Program.monitizer.AddUserLog(Convert.ToInt32(HttpContext.Session.GetInt32("UserID")), Helpers.Constants.Enums.UserLogType.Request, "User added a new job. #" + model.JobID, Utility.IpHelper.GetClientIpAddress(HttpContext), Utility.IpHelper.GetClientPort(HttpContext));


                    //Set server side toastr because page will be redirected
                    try
                    {
                        TempData["toastr-message"] = result.Message;
                        TempData["toastr-type"] = "success";
                    } catch (Exception) { }
                    

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
                    Program.monitizer.AddUserLog(Convert.ToInt32(HttpContext.Session.GetInt32("UserID")), Helpers.Constants.Enums.UserLogType.Request, "User tried to edit job that is not yours.", Utility.IpHelper.GetClientIpAddress(HttpContext), Utility.IpHelper.GetClientPort(HttpContext));

                    return Json(new SimpleResponse { Success = false, Message = Lang.UnauthorizedAccess });
                }

                //Put model to ApiGateway
                string updateResponseJson = Helpers.Request.Put(Program._settings.Service_ApiGateway_Url + "/Db/JobPost/Update", Helpers.Serializers.SerializeJson(Model), HttpContext.Session.GetString("Token"));
                //Parse response
                JobPostDto model = Helpers.Serializers.DeserializeJson<JobPostDto>(updateResponseJson);

                if (model != null && model.JobID > 0)
                {
                    Program.monitizer.AddUserLog(Convert.ToInt32(HttpContext.Session.GetInt32("UserID")), Helpers.Constants.Enums.UserLogType.Request, "User updated job.", Utility.IpHelper.GetClientIpAddress(HttpContext), Utility.IpHelper.GetClientPort(HttpContext));

                    result.Success = true;
                    result.Message = "Job updated succesfully.";

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
                var url = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/Website/GetJobDetail?jobid=" + JobID + "&userid=" + HttpContext.Session.GetInt32("UserID"), HttpContext.Session.GetString("Token"));
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
                model.JobPostWebsiteModel.Voting = Helpers.Serializers.DeserializeJson<List<VotingDto>>(votingJson);

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
        [PreventDuplicateRequest]
        [ValidateAntiForgeryToken]
        public JsonResult AddNewComment(int JobId, int CommentId, string Comment)
        {
            if (!ModelState.IsValid)
            {
                return Json(new SimpleResponse { Success = false, Message = "Double post action prevented." });
            }

            SimpleResponse result = new SimpleResponse();

            try
            {
                //KYC Control
                if (Program._settings.ForumKYCRequired && HttpContext.Session.GetString("KYCStatus") != "true")
                {
                    result.Success = false;
                    result.Message = "Please complete the KYC from User Profile to add a new comment";
                    return Json(result);
                }

                //Check if this job entered voting phase. If yes user can't post comment anymore
                var informalVotingJson = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Voting/Voting/GetInformalVotingByJobId?jobid=" + JobId, HttpContext.Session.GetString("Token"));
                var informalVoting = Helpers.Serializers.DeserializeJson<VotingDto>(informalVotingJson);
                if (informalVoting != null && informalVoting.VotingID > 0)
                {
                    result.Success = false;
                    result.Message = "This job is in voting process or completed. Posting comments are disabled.";
                    return Json(result);
                }

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
                    Program.monitizer.AddUserLog(Convert.ToInt32(HttpContext.Session.GetInt32("UserID")), Helpers.Constants.Enums.UserLogType.Request, "User commented.", Utility.IpHelper.GetClientIpAddress(HttpContext), Utility.IpHelper.GetClientPort(HttpContext));

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
                        Program.monitizer.AddUserLog(Convert.ToInt32(HttpContext.Session.GetInt32("UserID")), Helpers.Constants.Enums.UserLogType.Request, "User deleted their own comment.", Utility.IpHelper.GetClientIpAddress(HttpContext), Utility.IpHelper.GetClientPort(HttpContext));

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

                    Program.monitizer.AddUserLog(Convert.ToInt32(HttpContext.Session.GetInt32("UserID")), Helpers.Constants.Enums.UserLogType.Request, "User upvoted to Comment #" + JobPostCommentId, Utility.IpHelper.GetClientIpAddress(HttpContext), Utility.IpHelper.GetClientPort(HttpContext));
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

                    Program.monitizer.AddUserLog(Convert.ToInt32(HttpContext.Session.GetInt32("UserID")), Helpers.Constants.Enums.UserLogType.Request, "user downvoted to Comment #" + JobPostCommentId, Utility.IpHelper.GetClientIpAddress(HttpContext), Utility.IpHelper.GetClientPort(HttpContext));
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
        /// Inserts flag comment to the job
        /// </summary>
        /// <param name="jobid">Job id</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult FlagJob(int jobid, string flagreason)
        {
            SimpleResponse result = new SimpleResponse();

            try
            {
                //KYC Control
                if (Program._settings.ForumKYCRequired && HttpContext.Session.GetString("KYCStatus") != "true")
                {
                    result.Success = false;
                    result.Message = "Please complete the KYC from User Profile";
                    return Json(result);
                }

                //Check if this job entered voting phase. If yes user can't flag anymore
                var informalVotingJson = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Voting/Voting/GetInformalVotingByJobId?jobid=" + jobid, HttpContext.Session.GetString("Token"));
                var informalVoting = Helpers.Serializers.DeserializeJson<VotingDto>(informalVotingJson);
                if (informalVoting != null && informalVoting.VotingID > 0)
                {
                    result.Success = false;
                    result.Message = "This job is in voting process. Flagging is disabled.";
                    return Json(result);
                }

                //Create new comment model
                JobPostCommentDto model = new JobPostCommentDto()
                {
                    Comment = flagreason,
                    Date = DateTime.Now,
                    JobID = jobid,
                    SubCommentID = 0,
                    UserID = Convert.ToInt32(HttpContext.Session.GetInt32("UserID")),
                    DownVote = 0,
                    UpVote = 0,
                    IsFlagged = true
                };

                //Post model to ApiGateway
                model = Helpers.Serializers.DeserializeJson<JobPostCommentDto>(Helpers.Request.Post(Program._settings.Service_ApiGateway_Url + "/Db/JobPostComment/Post", Helpers.Serializers.SerializeJson(model), HttpContext.Session.GetString("Token")));

                if (model != null && model.JobPostCommentID > 0)
                {
                    Program.monitizer.AddUserLog(Convert.ToInt32(HttpContext.Session.GetInt32("UserID")), Helpers.Constants.Enums.UserLogType.Request, "User commented.", Utility.IpHelper.GetClientIpAddress(HttpContext), Utility.IpHelper.GetClientPort(HttpContext));

                    result.Success = true;
                    result.Message = "Job flagged succesfully.";
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
        /// Removes flag comment from the job
        /// </summary>
        /// <param name="jobid">Job id</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult RemoveFlag(int jobid)
        {
            SimpleResponse result = new SimpleResponse();

            try
            {
                //Post model to ApiGateway
                result = Helpers.Serializers.DeserializeJson<SimpleResponse>(Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/Website/RemoveFlag?jobid=" + jobid + "&userid=" + HttpContext.Session.GetInt32("UserID"), HttpContext.Session.GetString("Token")));

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
        ///  Creates a new job and restarts job flow
        /// </summary>
        /// <param name="jobid">Job id</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult RestartJob(int jobid)
        {
            SimpleResponse result = new SimpleResponse();

            try
            {
                //Get Model from ApiGateway          
                var jobJson = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/JobPost/GetId?id=" + jobid, HttpContext.Session.GetString("Token"));

                //Parse result
                var JobModel = Helpers.Serializers.DeserializeJson<JobPostDto>(jobJson);

                //Set status to FailRestart
                JobModel.Status = JobStatusTypes.FailRestart;
                JobModel = Helpers.Serializers.DeserializeJson<JobPostDto>(Helpers.Request.Put(Program._settings.Service_ApiGateway_Url + "/Db/JobPost/Update", Helpers.Serializers.SerializeJson(JobModel), HttpContext.Session.GetString("Token")));

                JobModel.Status = JobStatusTypes.InternalAuction;
                JobModel.CreateDate = DateTime.Now;
                JobModel.JobID = 0;

                //Post model to ApiGateway
                string jobPostResponseJson = Helpers.Request.Post(Program._settings.Service_ApiGateway_Url + "/Db/JobPost/Post", Helpers.Serializers.SerializeJson(JobModel), HttpContext.Session.GetString("Token"));

                //Parse reponse
                var model = Helpers.Serializers.DeserializeJson<JobPostDto>(jobPostResponseJson);

                if (model != null && model.JobID > 0)
                {

                    Program.monitizer.AddUserLog(Convert.ToInt32(HttpContext.Session.GetInt32("UserID")), Helpers.Constants.Enums.UserLogType.Request, "User restarted job #" + jobid, Utility.IpHelper.GetClientIpAddress(HttpContext), Utility.IpHelper.GetClientPort(HttpContext));
                    Program.monitizer.AddUserLog(Convert.ToInt32(HttpContext.Session.GetInt32("UserID")), Helpers.Constants.Enums.UserLogType.Request, "User added a new job (Restart) #" + model.JobID, Utility.IpHelper.GetClientIpAddress(HttpContext), Utility.IpHelper.GetClientPort(HttpContext));

                    //Set auction end dates
                    DateTime internalAuctionEndDate = DateTime.Now.AddDays(Program._settings.InternalAuctionTime);
                    DateTime publicAuctionEndDate = DateTime.Now.AddDays(Program._settings.InternalAuctionTime + Program._settings.PublicAuctionTime);

                    if (Program._settings.AuctionTimeType == "week")
                    {
                        internalAuctionEndDate = DateTime.Now.AddDays(Program._settings.InternalAuctionTime * 7);
                        publicAuctionEndDate = DateTime.Now.AddDays((Program._settings.InternalAuctionTime + Program._settings.PublicAuctionTime) * 7);
                    }
                    else if (Program._settings.AuctionTimeType == "minute")
                    {
                        internalAuctionEndDate = DateTime.Now.AddMinutes(Program._settings.InternalAuctionTime);
                        publicAuctionEndDate = DateTime.Now.AddMinutes(Program._settings.InternalAuctionTime + Program._settings.PublicAuctionTime);
                    }

                    AuctionDto AuctionModel = new AuctionDto()
                    {
                        JobID = model.JobID,
                        JobPosterUserId = model.UserID,
                        CreateDate = DateTime.Now,
                        Status = AuctionStatusTypes.InternalBidding,
                        InternalAuctionEndDate = internalAuctionEndDate,
                        PublicAuctionEndDate = publicAuctionEndDate
                    };

                    //Post model to ApiGateway
                    //Add new auction
                    AuctionModel = Helpers.Serializers.DeserializeJson<AuctionDto>(Helpers.Request.Post(Program._settings.Service_ApiGateway_Url + "/Db/Auction/Post", Helpers.Serializers.SerializeJson(AuctionModel), HttpContext.Session.GetString("Token")));

                    if (AuctionModel != null && AuctionModel.AuctionID > 0)
                    {
                        result.Success = true;
                        result.Message = "Restart successful. New job created and internal auction process started for the job.";
                        result.Content = AuctionModel;
                    }

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

        #endregion

        #region Auctions

        /// <summary>
        /// Auctions Page
        /// </summary>
        /// <returns></returns>
        [Route("Auctions")]
        public IActionResult Auctions(string query)
        {
            ViewBag.Title = "Auctions";

            List<AuctionViewModel> auctionsModel = new List<AuctionViewModel>();

            try
            {
                //Get auctions from ApiGateway
                var auctionsJson = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/Website/GetAuctions?query="+query, HttpContext.Session.GetString("Token"));
                //Parse response
                auctionsModel = Helpers.Serializers.DeserializeJson<List<AuctionViewModel>>(auctionsJson);

                //Get user's bid for all active auctions from ApiGateway
                var bidsJson = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/Website/GetUserBids?userid=" + HttpContext.Session.GetInt32("UserID"), HttpContext.Session.GetString("Token"));
                //Parse response
                List<AuctionBidDto> bidsModel = Helpers.Serializers.DeserializeJson<List<AuctionBidDto>>(bidsJson);

                //Match users existing bids with auctions
                foreach (var bid in bidsModel)
                {
                    if(auctionsModel.Count(x=>x.AuctionID == bid.AuctionID) > 0)
                    {
                        var auction = auctionsModel.First(x => x.AuctionID == bid.AuctionID);
                        auction.UsersBidId = bid.AuctionBidID;
                    }
                }
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }
            return View(auctionsModel);
        }

        /// <summary>
        /// My Bids Page
        /// </summary>
        /// <returns></returns>
        [Route("My-Bids")]
        public IActionResult My_Bids()
        {
            ViewBag.Title = "My Bids";

            List<MyBidsViewModel> bidsModel = new List<MyBidsViewModel>();

            try
            {
                //Get auctions from ApiGateway
                var mybidsJson = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/Website/GetMyBids?userid=" + HttpContext.Session.GetInt32("UserID"), HttpContext.Session.GetString("Token"));
                //Parse response
                bidsModel = Helpers.Serializers.DeserializeJson<List<MyBidsViewModel>>(mybidsJson);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return View(bidsModel);
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
                //Get auction model from ApiGateway
                var auctionJson = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/Auction/GetId?id=" + AuctionID, HttpContext.Session.GetString("Token"));
                //Parse response
                AuctionDetailModel.Auction = Helpers.Serializers.DeserializeJson<AuctionDto>(auctionJson);

                //If auction isn't completed only job poster and admin can see the bids
                if (HttpContext.Session.GetString("UserType") != Enums.UserIdentityType.Admin.ToString() &&
                    HttpContext.Session.GetInt32("UserID") != AuctionDetailModel.Auction.JobPosterUserId)
                {
                    if (AuctionDetailModel.Auction.Status == AuctionStatusTypes.PublicBidding || AuctionDetailModel.Auction.Status == AuctionStatusTypes.InternalBidding)
                    {
                        return RedirectToAction("Auctions");
                    }
                }

                //Get bids model from ApiGateway
                var auctionBidsJson = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/Website/GetAuctionBids?auctionid=" + AuctionID, HttpContext.Session.GetString("Token"));
                //Parse response
                AuctionDetailModel.BidItems = Helpers.Serializers.DeserializeJson<List<AuctionBidItemModel>>(auctionBidsJson);
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

                //Check inputs
                if (Model.Price <= 0 || string.IsNullOrEmpty(Model.Time))
                {
                    return Json(new SimpleResponse { Success = false, Message = "Please fill the necessary fields" });
                }

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
                if (bids.Count(x => x.UserId == Convert.ToInt32(HttpContext.Session.GetInt32("UserID"))) > 0)
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

                Program.monitizer.AddUserLog(Convert.ToInt32(HttpContext.Session.GetInt32("UserID")), Helpers.Constants.Enums.UserLogType.Request, "The user has bid on the auction. Auction #" + Model.AuctionID, Utility.IpHelper.GetClientIpAddress(HttpContext), Utility.IpHelper.GetClientPort(HttpContext));

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
                SimpleResponse releaseStakeResponse = Helpers.Serializers.DeserializeJson<SimpleResponse>(Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Reputation/UserReputationStake/ReleaseSingleStake?referenceID=" + id + "&reftype=" + Enums.StakeType.Bid, HttpContext.Session.GetString("Token")));

                //Post model to ApiGateway
                var deleteBidResponse = Helpers.Serializers.DeserializeJson<bool>(Helpers.Request.Delete(Program._settings.Service_ApiGateway_Url + "/Db/AuctionBid/Delete?id=" + id, HttpContext.Session.GetString("Token")));

                if (deleteBidResponse)
                {
                    result.Success = true;
                    result.Message = "Bid succesffully deleted.";

                    //Set server side toastr because page will be redirected
                    TempData["toastr-message"] = result.Message;
                    TempData["toastr-type"] = "success";

                    Program.monitizer.AddUserLog(Convert.ToInt32(HttpContext.Session.GetInt32("UserID")), Helpers.Constants.Enums.UserLogType.Request, "The user deleted bid on the auction. Auction #" + auctionBid.AuctionID, Utility.IpHelper.GetClientIpAddress(HttpContext), Utility.IpHelper.GetClientPort(HttpContext));
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
                        UserReputationStakeDto stake = new UserReputationStakeDto() { UserID = auctionBid.UserId, Amount = auctionBid.Price * Program._settings.ReputationConversionRate, CreateDate = DateTime.Now, Type = StakeType.Mint, ReferenceID = auction.JobID, ReferenceProcessID = auction.JobID, Status = ReputationStakeStatus.Staked };
                        //Post model to ApiGateway
                        string mintJson = Helpers.Request.Post(Program._settings.Service_ApiGateway_Url + "/Reputation/UserReputationStake/SubmitStake", Helpers.Serializers.SerializeJson(stake), HttpContext.Session.GetString("Token"));
                        //Parse response
                        SimpleResponse mintReponse = Helpers.Serializers.DeserializeJson<SimpleResponse>(mintJson);

                        if (mintReponse.Success)
                        {
                            result.Success = true;
                            result.Message = "Winner bid selected.";

                            //Set server side toastr because page will be redirected
                            TempData["toastr-message"] = result.Message;
                            TempData["toastr-type"] = "success";

                            Program.monitizer.AddUserLog(Convert.ToInt32(HttpContext.Session.GetInt32("UserID")), Helpers.Constants.Enums.UserLogType.Request, "Job poster selected the winner bid. Job #" + auction.JobID, Utility.IpHelper.GetClientIpAddress(HttpContext), Utility.IpHelper.GetClientPort(HttpContext));

                            //Send notification email to winner

                            //Get winner user object 
                            var userJson = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/Users/GetId?id=" + auctionBid.UserId, HttpContext.Session.GetString("Token"));
                            //Parse result
                            var userModel = Helpers.Serializers.DeserializeJson<UserDto>(userJson);

                            //Set email title and content
                            string emailTitle = "You won the auction for job #" + auction.JobID;
                            string emailContent = "Greetings, " + userModel.NameSurname.Split(' ')[0] + ", <br><br> You won the auction of '" + jobStatusResult.Title + "'.<br><br> Please post your job completion evidence as a comment to the related job and start informal voting process within expected timeframe";

                            SendEmailModel emailModel = new SendEmailModel() { Subject = emailTitle, Content = emailContent, To = new List<string> { userModel.Email } };
                            Program.rabbitMq.Publish(Helpers.Constants.FeedNames.NotificationFeed, "email", Helpers.Serializers.Serialize(emailModel));

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

        /// <summary>
        /// Returns data for reputation pie chart in the payment policy confirmation.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ReputationPieChartModel GetVaReputationChart()
        {
            ReputationPieChartModel result = new ReputationPieChartModel();
            result.Labels = new List<string>();
            result.Values = new List<int>();

            try
            {
                //Get VA user ids
                string vaJson = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/Users/GetUserIdsByType?type=" + Enums.UserIdentityType.VotingAssociate, HttpContext.Session.GetString("Token"));
                //Parse response
                List<int> vaIds = Helpers.Serializers.DeserializeJson<List<int>>(vaJson);

                //Get VA reputations
                var reputationsTotalJson = Helpers.Request.Post(Program._settings.Service_ApiGateway_Url + "/Reputation/UserReputationHistory/GetLastReputationByUserIds", Helpers.Serializers.SerializeJson(vaIds), HttpContext.Session.GetString("Token"));
                var reputationsTotal = Helpers.Serializers.DeserializeJson<List<UserReputationHistoryDto>>(reputationsTotalJson);

                List<string> anonymizedReputations = new List<string>();
                foreach (var item in reputationsTotal)
                {
                    anonymizedReputations.Add(Utility.StringHelper.AnonymizeReputation(item.LastTotal));
                }

                foreach (var group in anonymizedReputations.GroupBy(x => x))
                {
                    result.Labels.Add(group.Key);
                    result.Values.Add(group.Count());
                }
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, Enums.LogTypes.ApplicationError, true);
            }

            return result;
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
            ViewBag.Title = "Voting";

            List<VotingViewModel> votingsModel = new List<VotingViewModel>();
            try
            {
                //Get model from ApiGateway
                var votingJson = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/Website/GetVotingsByStatus", HttpContext.Session.GetString("Token"));

                //Parse response
                votingsModel = Helpers.Serializers.DeserializeJson<List<VotingViewModel>>(votingJson);

                //Get user's votes
                string votesJson = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Voting/Vote/GetAllVotesByUserId?userid=" + HttpContext.Session.GetInt32("UserID"), HttpContext.Session.GetString("Token"));
                List<VoteDto> votesModel = Helpers.Serializers.DeserializeJson<List<VoteDto>>(votesJson);

                foreach (var voting in votingsModel)
                {
                    if (votesModel.Count(x => x.VotingID == voting.VotingID) > 0)
                    {
                        var vote = votesModel.First(x => x.VotingID == voting.VotingID);
                        if (vote.Direction == Helpers.Constants.Enums.StakeType.For || vote.Direction == Helpers.Constants.Enums.StakeType.Against)
                        {
                            voting.UserVote = vote.Direction;
                        }
                    }

                    //For simple votes job poster and job doer is equal
                    if(voting.JobDoerUserID == 0) voting.JobDoerUserID = voting.JobOwnerUserID;
                }

                //Get user's available reputation and save it to session to show in vote modal
                var reputationJson = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Reputation/UserReputationHistory/GetLastReputation?userid=" + HttpContext.Session.GetInt32("UserID"), HttpContext.Session.GetString("Token"));
                if (!string.IsNullOrEmpty(reputationJson))
                {
                    HttpContext.Session.SetString("LastUsableReputation", Helpers.Serializers.DeserializeJson<UserReputationHistoryDto>(reputationJson).LastUsableTotal.ToString());
                }
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }
            return View(votingsModel);
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
                var reputations = Helpers.Serializers.DeserializeJson<List<UserReputationStakeDto>>(reputationsJson).OrderByDescending(x => x.UserReputationStakeID);

                //Get usernames of voters
                var usernamesJson = Helpers.Request.Post(Program._settings.Service_ApiGateway_Url + "/Db/Users/GetUsernamesByUserIds", Helpers.Serializers.SerializeJson(votes.Select(x => x.UserID)), HttpContext.Session.GetString("Token"));
                var usernames = Helpers.Serializers.DeserializeJson<List<string>>(usernamesJson);

                //Get informal voting results
                if (voting.IsFormal)
                {
                    var informalVotingJson = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Voting/Voting/GetInformalVotingByJobId?jobid=" + voting.JobID, HttpContext.Session.GetString("Token"));
                    var informalVoting = Helpers.Serializers.DeserializeJson<VotingDto>(informalVotingJson);

                    //Get reputation stakes of informal voting from reputation service
                    var informalReputationsJson = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Reputation/UserReputationStake/GetByProcessId?referenceProcessID=" + informalVoting.VotingID + "&reftype=" + StakeType.For, HttpContext.Session.GetString("Token"));
                    var informalReputations = Helpers.Serializers.DeserializeJson<List<UserReputationStakeDto>>(informalReputationsJson);

                    //Get total reputations staked for both sides in informal voting
                    voteDetailModel.InformalFor = informalReputations.Where(x => x.Type == StakeType.For).Sum(x => x.Amount);
                    voteDetailModel.InformalAgainst = informalReputations.Where(x => x.Type == StakeType.Against).Sum(x => x.Amount);
                }

                //Combine results into VoteItemModel
                List<VoteItemModel> voteItems = new List<VoteItemModel>();

                for (int i = 0; i < votes.Count; i++)
                {
                    VoteItemModel vt = new VoteItemModel();
                    vt.UserID = votes[i].UserID;
                    vt.Date = votes[i].Date;
                    vt.Direction = votes[i].Direction;
                    vt.VoteID = votes[i].VoteID;
                    vt.VotingID = votes[i].VotingID;
                    vt.UserName = usernames[i];
                    if (reputations.Count(x => x.UserID == vt.UserID) > 0)
                    {
                        vt.ReputationStake = reputations.First(x => x.UserID == vt.UserID).Amount;
                    }
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
                    Program.monitizer.AddUserLog(Convert.ToInt32(HttpContext.Session.GetInt32("UserID")), Helpers.Constants.Enums.UserLogType.Request, "User tried to start informal voting for job that is not yours. Job #" + auction.JobID, Utility.IpHelper.GetClientIpAddress(HttpContext), Utility.IpHelper.GetClientPort(HttpContext));

                    return Json(new SimpleResponse { Success = false, Message = "User is not authorized to start informal voting for this job." });
                }

                //Start informal voting
                VotingDto informalVoting = new VotingDto();
                informalVoting.JobID = jobid;
                informalVoting.StartDate = DateTime.Now;
                informalVoting.PolicingRate = Program._settings.DefaultPolicingRate;
                informalVoting.QuorumRatio = Program._settings.QuorumRatio;
                informalVoting.Type = Enums.VoteTypes.JobCompletion;
                informalVoting.EndDate = DateTime.Now.AddDays(Program._settings.VotingTime);

                if (Program._settings.VotingTimeType == "week")
                {
                    informalVoting.EndDate = DateTime.Now.AddDays(Program._settings.VotingTime * 7);
                }
                else if (Program._settings.VotingTimeType == "minute")
                {
                    informalVoting.EndDate = DateTime.Now.AddMinutes(Program._settings.VotingTime);
                }

                //Get related job post
                var jobJson = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/JobPost/GetId?id=" + jobid, HttpContext.Session.GetString("Token"));
                JobPostDto job = Helpers.Serializers.DeserializeJson<JobPostDto>(jobJson);

                //Get total dao member count
                int daoMemberCount = Convert.ToInt32(Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/Users/GetCount?type=" + UserIdentityType.VotingAssociate, HttpContext.Session.GetString("Token")));
                //Eligible user count = VA Count
                informalVoting.EligibleUserCount = daoMemberCount;
                //Quorum count is calculated with total user count - 2(job poster, job doer)
                informalVoting.QuorumCount = Convert.ToInt32(Math.Ceiling(Program._settings.QuorumRatio * Convert.ToDouble(informalVoting.EligibleUserCount)));

                string jsonResult = Helpers.Request.Post(Program._settings.Service_ApiGateway_Url + "/Voting/Voting/StartInformalVoting", Helpers.Serializers.SerializeJson(informalVoting), HttpContext.Session.GetString("Token"));
                res = Helpers.Serializers.DeserializeJson<SimpleResponse>(jsonResult);
                res.Content = null;

                //Change job status 
                Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/JobPost/ChangeJobStatus?jobid=" + jobid + "&status=" + JobStatusTypes.InformalVoting, HttpContext.Session.GetString("Token"));

                //Set server side toastr because page will be redirected
                TempData["toastr-message"] = res.Message;
                TempData["toastr-type"] = "success";

                Program.monitizer.AddUserLog(Convert.ToInt32(HttpContext.Session.GetInt32("UserID")), Helpers.Constants.Enums.UserLogType.Request, "User started informal voting . Job #" + auction.JobID, Utility.IpHelper.GetClientIpAddress(HttpContext), Utility.IpHelper.GetClientPort(HttpContext));

                //Send email notification to VAs
                SendEmailModel emailModel = new SendEmailModel() { Subject = "Informal Voting Started For Job #" + jobid, Content = "Informal voting process started for job #" + jobid + "<br><br>Please submit your vote until " + informalVoting.EndDate.ToString(), TargetGroup = Enums.UserIdentityType.VotingAssociate };
                Program.rabbitMq.Publish(Helpers.Constants.FeedNames.NotificationFeed, "email", Helpers.Serializers.Serialize(emailModel));

                return Json(res);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return Json(new SimpleResponse { Success = false, Message = Lang.ErrorNote });
        }

        /// <summary>
        ///  User submit vote action.
        /// </summary>
        /// <param name="VotingID"></param>
        /// <param name="Direction"></param>
        /// <param name="ReputationStake"></param>
        /// <returns></returns>
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
                    Program.monitizer.AddUserLog(Convert.ToInt32(HttpContext.Session.GetInt32("UserID")), Helpers.Constants.Enums.UserLogType.Request, "User tried to submit vote for closed job. Voting #" + VotingID, Utility.IpHelper.GetClientIpAddress(HttpContext), Utility.IpHelper.GetClientPort(HttpContext));

                    return Json(new SimpleResponse { Success = false, Message = "You can't submit vote to a closed voting." });
                }

                //Check if user trying to submit bid for his/her own job
                if (job.JobDoerUserID == Convert.ToInt32(HttpContext.Session.GetInt32("UserID")))
                {
                    Program.monitizer.AddUserLog(Convert.ToInt32(HttpContext.Session.GetInt32("UserID")), Helpers.Constants.Enums.UserLogType.Request, "User tried to submit vote for her/his own job. Voting #" + VotingID, Utility.IpHelper.GetClientIpAddress(HttpContext), Utility.IpHelper.GetClientPort(HttpContext));

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

                    Program.monitizer.AddUserLog(Convert.ToInt32(HttpContext.Session.GetInt32("UserID")), Helpers.Constants.Enums.UserLogType.Request, "User voted job. Voting #" + VotingID, Utility.IpHelper.GetClientIpAddress(HttpContext), Utility.IpHelper.GetClientPort(HttpContext));
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
        /// New Simple Vote Page
        /// </summary>
        /// <returns></returns>
        [Route("New-Simple-Vote")]
        public IActionResult New_Simple_Vote()
        {
            ViewBag.Title = "Start A New Simple Vote";

            return View();
        }

        /// <summary>
        ///  New simple vote post function
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="description">Description</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult New_SimpleVote_Post(NewSimpleVoteModel model)
        {
            SimpleResponse result = new SimpleResponse();

            try
            {
                if (model.type == "va")
                {
                    string userJson = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/Users/GetByUsername?username=" + model.vausername, HttpContext.Session.GetString("Token"));
                    var userObj = Helpers.Serializers.DeserializeJson<UserDto>(userJson);

                    if(userObj == null || userObj.UserId <= 0)
                    {
                        result.Success = false;
                        result.Message = "Could not find a user with this username.";
                        return Json(result);
                    }

                    model.title = "New VA Onboarding ("+model.vausername+")";
                    model.description = model.vausername+" has indicated his interest and willingness to serve as a VA. In accordance with DAO policy, "+model.vausername+" is herewith proposed as a voting associate.";
                }

                if (model.type == "governance")
                {
                    model.title = "Governance Vote (DAO Variables)";
                    model.description = "Variables listed below will be applied to DAO"+Environment.NewLine+Environment.NewLine+ JsonConvert.SerializeObject(model.settings, Formatting.Indented);
                }

                //Empty fields control
                if (string.IsNullOrEmpty(model.title) || string.IsNullOrEmpty(model.description))
                {
                    result.Success = false;
                    result.Message = "You must fill all the fields.";
                    return Json(result);
                }

                //Create JobPost model
                JobPostDto jobPostModel = new JobPostDto() { UserID = Convert.ToInt32(HttpContext.Session.GetInt32("UserID")), Amount = 0, JobDescription = model.description, CreateDate = DateTime.Now, LastUpdate = DateTime.Now, Title = model.title, Status = Enums.JobStatusTypes.InformalVoting };
                //Post model to ApiGateway
                string jobPostResponseJson = Helpers.Request.Post(Program._settings.Service_ApiGateway_Url + "/Db/JobPost/Post", Helpers.Serializers.SerializeJson(jobPostModel), HttpContext.Session.GetString("Token"));
                //Parse reponse
                jobPostModel = Helpers.Serializers.DeserializeJson<JobPostDto>(jobPostResponseJson);

                if (jobPostModel != null && jobPostModel.JobID > 0)
                {
                    //Start informal voting
                    VotingDto informalVoting = new VotingDto();
                    informalVoting.JobID = jobPostModel.JobID;
                    informalVoting.StartDate = DateTime.Now;
                    informalVoting.PolicingRate = Program._settings.DefaultPolicingRate;
                    informalVoting.QuorumRatio = Program._settings.QuorumRatio;
                    if(model.type == "governance")
                    {
                        informalVoting.Type = Enums.VoteTypes.Governance;
                    }
                    else
                    {
                        informalVoting.Type = Enums.VoteTypes.Simple;
                    }
                    informalVoting.EndDate = DateTime.Now.AddDays(Program._settings.SimpleVotingTime);

                    if (Program._settings.SimpleVotingTimeType == "week")
                    {
                        informalVoting.EndDate = DateTime.Now.AddDays(Program._settings.SimpleVotingTime * 7);
                    }
                    else if (Program._settings.SimpleVotingTimeType == "minute")
                    {
                        informalVoting.EndDate = DateTime.Now.AddMinutes(Program._settings.SimpleVotingTime);
                    }

                    //Get total dao member count
                    int daoMemberCount = Convert.ToInt32(Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/Users/GetCount?type=" + UserIdentityType.VotingAssociate, HttpContext.Session.GetString("Token")));
                    //Eligible user count = VA Count - 1 (Job Doer)
                    informalVoting.EligibleUserCount = daoMemberCount - 1;
                    //Quorum count is calculated with total user count - 2(job poster, job doer)
                    informalVoting.QuorumCount = Convert.ToInt32(Program._settings.QuorumRatio * Convert.ToDouble(informalVoting.EligibleUserCount));

                    string jsonResult = Helpers.Request.Post(Program._settings.Service_ApiGateway_Url + "/Voting/Voting/StartInformalVoting", Helpers.Serializers.SerializeJson(informalVoting), HttpContext.Session.GetString("Token"));
                    result = Helpers.Serializers.DeserializeJson<SimpleResponse>(jsonResult);
                    result.Content = null;

                    Program.monitizer.AddUserLog(Convert.ToInt32(HttpContext.Session.GetInt32("UserID")), Helpers.Constants.Enums.UserLogType.Request, "User started new simple vote.", Utility.IpHelper.GetClientIpAddress(HttpContext), Utility.IpHelper.GetClientPort(HttpContext));

                    //Set server side toastr because page will be redirected
                    TempData["toastr-message"] = result.Message;
                    TempData["toastr-type"] = "success";

                    //Send email notification to VAs
                    SendEmailModel emailModel = new SendEmailModel() { Subject = "New Simple Vote Submitted #" + jobPostModel.JobID, Content = "New simple vote started. Title:" + jobPostModel.Title + "<br><br>Please submit your vote until " + informalVoting.EndDate.ToString(), TargetGroup = Enums.UserIdentityType.VotingAssociate };
                    Program.rabbitMq.Publish(Helpers.Constants.FeedNames.NotificationFeed, "email", Helpers.Serializers.Serialize(emailModel));

                    return Json(result);
                }
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

        /// <summary>
        ///  Exports reputation history to csv
        /// </summary>
        /// <returns></returns>
        public async Task<FileResult> ExportReputationHistoryCsv(DateTime? start, DateTime? end)
        {
            try
            {
                //Get reputation history data from ApiGateway
                string repsJson = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Reputation/UserReputationHistory/GetByUserIdDate?userid=" + HttpContext.Session.GetInt32("UserID")+"&start="+start.ToString()+"&end="+end.ToString(), HttpContext.Session.GetString("Token"));
                //Parse response
                List<UserReputationHistoryDto> model = Helpers.Serializers.DeserializeJson<List<UserReputationHistoryDto>>(repsJson);

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Date;Title;Explanation;Earned Amount;Lost Amount; Staked Total; Usable Total; Cumulative Total");
                foreach (var item in model.OrderByDescending(x => x.Date))
                {
                    sb.AppendLine(item.Date + ";" + item.Title + ";" + item.Explanation + ";" + item.EarnedAmount + ";"  + item.LostAmount + ";"  + item.LastStakedTotal + ";" + item.LastUsableTotal + ";" + item.LastTotal);
                }

                byte[] fileBytes = System.Text.Encoding.UTF8.GetBytes(sb.ToString()); ;

                return File(fileBytes, "text/csv", "CRDAO Reputation History.csv");
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return File(new List<byte>().ToArray(), "text/csv", "CRDAO Reputation History.csv");
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

        /// <summary>
        ///  Profile save changes action
        /// </summary>
        /// <param name="image"></param>
        /// <param name="File"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ProfileUpdate")]
        public JsonResult ProfileUpdate(string image, IFormFile File)
        {
            SimpleResponse result = new SimpleResponse();

            try
            {
                //Get user
                UserDto modeluser = Helpers.Serializers.DeserializeJson<UserDto>(Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/Users/GetId?id=" + HttpContext.Session.GetInt32("UserID"), HttpContext.Session.GetString("Token")));

                if (modeluser != null && modeluser.UserId > 0)
                {
                    //If custom image uploaded
                    if (File != null)
                    {
                        var file = File;
                        var ext = (Path.GetExtension(file.FileName).ToLower());

                        //File extension control
                        if (ext != ".png" && ext != ".jpg" && ext != ".jpeg" && ext != ".gif")
                        {
                            return Json(new SimpleResponse { Success = false, Message = "Please upload a supported format. (.png, .jpg, .gif)" });
                        }


                        using (var ms = new MemoryStream())
                        {
                            File.CopyTo(ms);
                            var fileBytes = ms.ToArray();
                            string s = ResizeImage(fileBytes, 150, 150);
                            modeluser.ProfileImage = s;
                        }
                    }
                    else
                    {
                        modeluser.ProfileImage = Path.GetFileName(image);
                    }

                    //Update user  
                    var updatemodel = Helpers.Serializers.DeserializeJson<UserDto>(Helpers.Request.Put(Program._settings.Service_ApiGateway_Url + "/Db/Users/Update", Helpers.Serializers.SerializeJson(modeluser), HttpContext.Session.GetString("Token")));

                    if (updatemodel != null && updatemodel.UserId > 0)
                    {
                        Program.monitizer.AddUserLog(Convert.ToInt32(HttpContext.Session.GetInt32("UserID")), Helpers.Constants.Enums.UserLogType.Request, "User updated their profile photo.", Utility.IpHelper.GetClientIpAddress(HttpContext), Utility.IpHelper.GetClientPort(HttpContext));

                        HttpContext.Session.SetString("ProfileImage", modeluser.ProfileImage);
                        return Json(new SimpleResponse { Success = true, Message = "Save changes successful." });
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
        ///  Resize uploaded profile image
        /// </summary>
        /// <param name="data">Image as byte array</param>
        /// <param name="w">Expected width</param>
        /// <param name="h">Expected height</param>
        /// <returns></returns>
        public static string ResizeImage(byte[] data, double w, double h)
        {
            using (var ms = new MemoryStream(data))
            {
                var image = Image.FromStream(ms);

                var ratioX = (double)w / image.Width;
                var ratioY = (double)h / image.Height;
                var ratio = Math.Min(ratioX, ratioY);

                var width = (int)(image.Width * ratio);
                var height = (int)(image.Height * ratio);

                var newImage = new Bitmap(width, height);
                Graphics.FromImage(newImage).DrawImage(image, 0, 0, width, height);
                Bitmap bmp = new Bitmap(newImage);

                System.IO.MemoryStream ms2 = new MemoryStream();
                bmp.Save(ms2, ImageFormat.Jpeg);
                byte[] byteImage = ms2.ToArray();

                return Convert.ToBase64String(byteImage);
            }
        }

        /// <summary>
        /// User KYC Verification Page
        /// </summary>
        /// <returns></returns>

        [Route("KYC-Verification")]
        public IActionResult KYC_Verification()
        {
            KYCViewModel model = new KYCViewModel();

            try
            {
                model.Countries = Helpers.Serializers.DeserializeJson<List<KYCCountries>>(Helpers.Request.Post(Program._settings.Service_ApiGateway_Url + "/Identity/Kyc/GetKycCountries", "", HttpContext.Session.GetString("Token")));

                if (model.Countries == null)
                    model.Countries = new List<KYCCountries>();

                model.Status = Helpers.Serializers.DeserializeJson<UserKYCDto>(Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Identity/Kyc/GetKycStatus?id=" + HttpContext.Session.GetInt32("UserID"), HttpContext.Session.GetString("Token")));

                if (model.Status == null)
                    model.Status = new UserKYCDto();

            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError);
                return View(new KYCViewModel());
            }

            return View(model);
        }

        /// <summary>
        ///  Submits form data for the KYC verification
        /// </summary>
        /// <param>User information</param>
        /// <returns>Generic Simple Response class</returns>
        [Route("UploadKYCDoc")]
        public JsonResult UploadKYCDoc(KYCFileUpload File)
        {
            SimpleResponse model = new SimpleResponse();
            try
            {
                //Send files to Identity server          

                model = Helpers.Request.Upload(Program._settings.Service_ApiGateway_Url + "/Identity/Kyc/SubmitKYCFile?Type=" + File.Type + "&Name=" + File.Name + "&Surname=" + File.Surname + "&Dob=" + File.DoB + "&Email=" + File.Email + "&Country=" + File.Country + "&DocumentNumber=" + File.DocumentNumber + "&IssueDate=" + File.IssueDate + "&ExpiryDate=" + File.ExpiryDate + "&UserID=" + HttpContext.Session.GetInt32("UserID"), HttpContext.Session.GetString("Token"), File.UploadedFile1, File.UploadedFile2);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError);
                return Json(new SimpleResponse());
            }
            return Json(model);
        }

        /// <summary>
        ///  Public user pay dos fee action
        /// </summary>
        /// <returns></returns>
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

                //Set auction end dates
                DateTime internalAuctionEndDate = DateTime.Now.AddDays(Program._settings.InternalAuctionTime);
                DateTime publicAuctionEndDate = DateTime.Now.AddDays(Program._settings.InternalAuctionTime + Program._settings.PublicAuctionTime);

                if (Program._settings.AuctionTimeType == "week")
                {
                    internalAuctionEndDate = DateTime.Now.AddDays(Program._settings.InternalAuctionTime * 7);
                    publicAuctionEndDate = DateTime.Now.AddDays((Program._settings.InternalAuctionTime + Program._settings.PublicAuctionTime) * 7);
                }
                else if (Program._settings.AuctionTimeType == "minute")
                {
                    internalAuctionEndDate = DateTime.Now.AddMinutes(Program._settings.InternalAuctionTime);
                    publicAuctionEndDate = DateTime.Now.AddMinutes(Program._settings.InternalAuctionTime + Program._settings.PublicAuctionTime);
                }

                AuctionDto AuctionModel = new AuctionDto()
                {
                    JobID = JobId,
                    JobPosterUserId = JobModel.UserID,
                    CreateDate = DateTime.Now,
                    Status = AuctionStatusTypes.InternalBidding,
                    InternalAuctionEndDate = internalAuctionEndDate,
                    PublicAuctionEndDate = publicAuctionEndDate
                };

                //Check existing auction related with this job
                var AuctionModelByJobid = Helpers.Serializers.DeserializeJson<AuctionDto>(Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/Auction/GetByJobId?jobid=" + JobId, HttpContext.Session.GetString("Token")));
                if (AuctionModelByJobid != null && AuctionModelByJobid.AuctionID > 0)
                {
                    result.Success = false;
                    result.Message = "There is an existing auction related with this job.";
                    result.Content = AuctionModel;

                    return Json(result);
                }

                //Post model to ApiGateway
                //Add new auction
                AuctionModel = Helpers.Serializers.DeserializeJson<AuctionDto>(Helpers.Request.Post(Program._settings.Service_ApiGateway_Url + "/Db/Auction/Post", Helpers.Serializers.SerializeJson(AuctionModel), HttpContext.Session.GetString("Token")));

                if (AuctionModel != null && AuctionModel.AuctionID > 0)
                {
                    result.Success = true;
                    result.Message = "DoS fee successfully paid. Internal auction process started for the job.";
                    result.Content = AuctionModel;

                    Program.monitizer.AddUserLog(Convert.ToInt32(HttpContext.Session.GetInt32("UserID")), Helpers.Constants.Enums.UserLogType.Request, "User paid DoS fee. Job # " + JobModel.JobID, Utility.IpHelper.GetClientIpAddress(HttpContext), Utility.IpHelper.GetClientPort(HttpContext));

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

        #region Payment History

        /// <summary>
        ///  Payment History view
        /// </summary>
        /// <returns></returns>
        [Route("Payment-History")]
        public IActionResult Payment_History()
        {
            ViewBag.Title = "Payment History";

            PaymentHistoryViewModel model = new PaymentHistoryViewModel();

            try
            {
                //Get payment history data from ApiGateway
                string paymentsJson = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/Website/PaymentHistoryByUserId?userid=" + HttpContext.Session.GetInt32("UserID"), HttpContext.Session.GetString("Token"));
                //Parse response
                model = Helpers.Serializers.DeserializeJson<PaymentHistoryViewModel>(paymentsJson);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return View(model);
        }

        /// <summary>
        ///  Exports payment history to csv
        /// </summary>
        /// <returns></returns>
        public async Task<FileResult> ExportPaymentHistoryCsv(DateTime? start, DateTime? end)
        {
            try
            {
                //Get payment history data from ApiGateway
                string paymentsJson = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/PaymentHistory/ExportPaymentHistoryByDate?userid=" + HttpContext.Session.GetInt32("UserID")+"&start="+start.ToString()+"&end="+end.ToString(), HttpContext.Session.GetString("Token"));
                //Parse response
                List<PaymentExport> model = Helpers.Serializers.DeserializeJson<List<PaymentExport>>(paymentsJson);

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("JobID;Job Name;Job Post Date;Payment Date;Job Poster;Job Doer;Bid Price;Payment Amount");
                foreach (var item in model.OrderByDescending(x => x.paymentHistory.CreateDate))
                {
                    sb.AppendLine(item.job.JobID + ";" + item.job.Title + ";" + item.job.CreateDate + ";" + item.paymentHistory.CreateDate + ";"  + item.JobPosterUsername + ";"  + item.JobDoerUsername + ";" + item.winnerBid.Price + ";" + item.paymentHistory.Amount);
                }

                byte[] fileBytes = System.Text.Encoding.UTF8.GetBytes(sb.ToString()); ;

                return File(fileBytes, "text/csv", "CRDAO Payment History.csv");
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return File(new List<byte>().ToArray(), "text/csv", "CRDAO Payment History.csv");
        }


        #endregion

        #region Admin Views & Methods

        /// <summary>
        ///  Approves job with "AdminApprovalPending" status
        /// </summary>
        /// <param name="JobId"></param>
        /// <returns></returns>
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

                    //Set auction end dates
                    DateTime internalAuctionEndDate = DateTime.Now.AddDays(Program._settings.InternalAuctionTime);
                    DateTime publicAuctionEndDate = DateTime.Now.AddDays(Program._settings.InternalAuctionTime + Program._settings.PublicAuctionTime);

                    if (Program._settings.AuctionTimeType == "week")
                    {
                        internalAuctionEndDate = DateTime.Now.AddDays(Program._settings.InternalAuctionTime * 7);
                        publicAuctionEndDate = DateTime.Now.AddDays((Program._settings.InternalAuctionTime + Program._settings.PublicAuctionTime) * 7);
                    }
                    else if (Program._settings.AuctionTimeType == "minute")
                    {
                        internalAuctionEndDate = DateTime.Now.AddMinutes(Program._settings.InternalAuctionTime);
                        publicAuctionEndDate = DateTime.Now.AddMinutes(Program._settings.InternalAuctionTime + Program._settings.PublicAuctionTime);
                    }

                    AuctionDto AuctionModel = new AuctionDto()
                    {
                        JobID = JobId,
                        JobPosterUserId = JobModel.UserID,
                        CreateDate = DateTime.Now,
                        Status = AuctionStatusTypes.InternalBidding,
                        InternalAuctionEndDate = internalAuctionEndDate,
                        PublicAuctionEndDate = publicAuctionEndDate
                    };

                    //Check existing auction related with this job
                    var AuctionModelByJobid = Helpers.Serializers.DeserializeJson<AuctionDto>(Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/Auction/GetByJobId?jobid=" + JobId, HttpContext.Session.GetString("Token")));
                    if (AuctionModelByJobid != null && AuctionModelByJobid.AuctionID > 0)
                    {
                        result.Success = false;
                        result.Message = "There is an existing auction related with this job.";
                        result.Content = AuctionModel;

                        return Json(result);
                    }

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

                Program.monitizer.AddUserLog(Convert.ToInt32(HttpContext.Session.GetInt32("UserID")), Helpers.Constants.Enums.UserLogType.Request, "Admin approved the job.Job #" + JobId, Utility.IpHelper.GetClientIpAddress(HttpContext), Utility.IpHelper.GetClientPort(HttpContext));

                //Send notification email to job poster

                //Set email title and content
                string emailTitle = "Your job is approved by system administrator (Job #" + JobId + ")";
                string emailContent = "";

                if (JobModel.Status == Enums.JobStatusTypes.InternalAuction)
                {
                    emailContent = "Greetings, " + userModel.NameSurname.Split(' ')[0] + ", <br><br> Your job is approved by system administrator<br><br> Internal auction process started for the job.";
                }
                else if (JobModel.Status == Enums.JobStatusTypes.KYCPending)
                {
                    emailContent = "Greetings, " + userModel.NameSurname.Split(' ')[0] + ", <br><br> Your job is approved by system administrator<br><br> You have to complete KYC before auction phase. Please complete your KYC from job detail.";
                }
                else if (JobModel.Status == Enums.JobStatusTypes.DoSFeePending)
                {
                    emailContent = "Greetings, " + userModel.NameSurname.Split(' ')[0] + ", <br><br> Your job is approved by system administrator<br><br> Please pay the Dos fee to start internal auction process.";
                }

                SendEmailModel emailModel = new SendEmailModel() { Subject = emailTitle, Content = emailContent, To = new List<string> { userModel.Email } };
                Program.rabbitMq.Publish(Helpers.Constants.FeedNames.NotificationFeed, "email", Helpers.Serializers.Serialize(emailModel));

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
        ///  Disapproves job with "AdminApprovalPending" status
        /// </summary>
        /// <param name="JobId"></param>
        /// <returns></returns>
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
                if (JobModel.JobID > 0 && JobModel.Status == JobStatusTypes.Rejected)
                {
                    result.Success = true;
                    result.Message = "Job disapproved.";
                }

                //Send notification email to job poster

                //Set email title and content
                string emailTitle = "Your job is disapproved by system administrator (Job #" + JobId + ")";
                string emailContent = "Greetings, " + userModel.NameSurname.Split(' ')[0] + ", <br><br> Your job is disapproved by system administrator.<br><br> Please read the job posting rules and contact with system administrator.";

                SendEmailModel emailModel = new SendEmailModel() { Subject = emailTitle, Content = emailContent, To = new List<string> { userModel.Email } };
                Program.rabbitMq.Publish(Helpers.Constants.FeedNames.NotificationFeed, "email", Helpers.Serializers.Serialize(emailModel));


                Program.monitizer.AddUserLog(Convert.ToInt32(HttpContext.Session.GetInt32("UserID")), Helpers.Constants.Enums.UserLogType.Request, "Admin disapproved the job.Job #" + JobId, Utility.IpHelper.GetClientIpAddress(HttpContext), Utility.IpHelper.GetClientPort(HttpContext));

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
        ///  This view shows global parameters of the DAO
        /// </summary>
        /// <returns></returns>
        [Route("Dao-Variables")]
        [AuthorizeAdmin]
        public IActionResult Dao_Variables()
        {
            ViewBag.Title = "DAO Variables";

            return View();
        }

        /// <summary>
        ///  DAO Variables save changes
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("DaoVariablesPost")]
        public JsonResult DaoVariablesPost(PlatformSettingDto model)
        {
            SimpleResponse result = new SimpleResponse();

            try
            {
                model.CreateDate = DateTime.Now;
                model.UserID = HttpContext.Session.GetInt32("UserID");

                //Post model to ApiGateway
                string jsonResult = Helpers.Request.Post(Program._settings.Service_ApiGateway_Url + "/Db/PlatformSetting/Post", Helpers.Serializers.SerializeJson(model), HttpContext.Session.GetString("Token"));
                //Parse result
                PlatformSettingDto resultParsed = Helpers.Serializers.DeserializeJson<PlatformSettingDto>(jsonResult);

                if (resultParsed.PlatformSettingID > 0)
                {
                    result.Success = true;
                    result.Message = "DAO Variables changed successfully.";

                    //Set server side toastr because page will be redirected
                    TempData["toastr-message"] = result.Message;
                    TempData["toastr-type"] = "success";

                    Startup.LoadDaoSettings(null, null);
                }
                else
                {
                    result.Success = false;
                    result.Message = "Error occured while changing DAO variables.";
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
        ///  This view shows users of the DAO
        /// </summary>
        /// <returns></returns>
        [Route("Users-List")]
        [Route("Home/Users-List")]
        [AuthorizeAdmin]
        public IActionResult Users_List(int page = 1, int pageCount = 20)
        {
            ViewBag.Title = "Users List";

            IPagedList<UserDto> pagedModel = new PagedList<UserDto>(null, 1, 1);

            try
            {
                //Get users data from ApiGateway
                string usersJson = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/Users/GetPaged?page=" + page + "&pageCount=" + pageCount, HttpContext.Session.GetString("Token"));
                //Parse response
                var jobsListPaged = Helpers.Serializers.DeserializeJson<PaginationEntity<UserDto>>(usersJson);

                pagedModel = new StaticPagedList<UserDto>(
                    jobsListPaged.Items,
                    jobsListPaged.MetaData.PageNumber,
                    jobsListPaged.MetaData.PageSize,
                    jobsListPaged.MetaData.TotalItemCount
                    );
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }
            return View(pagedModel);
        }

        /// <summary>
        ///  Finds user info from query
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthorizeAdmin]
        public JsonResult UserSearch(string searchText)
        {
            List<UserDto> userList = new List<UserDto>();
            try
            {
                string usersJson = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/Users/UserSearch?query=" + searchText, HttpContext.Session.GetString("Token"));
                //Parse response
                userList = Helpers.Serializers.DeserializeJson<List<UserDto>>(usersJson);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return Json(userList);
        }

        /// <summary>
        ///  This view shows reputation logs of the DAO
        /// </summary>
        /// <returns></returns>
        [Route("Reputation-Logs")]
        [Route("Home/Reputation-Logs")]
        [AuthorizeAdmin]
        public IActionResult Reputation_Logs(int page = 1, int pageCount = 20)
        {
            ViewBag.Title = "Reputation Logs";

            IPagedList<ReputationLogsDto> pagedModel = new PagedList<ReputationLogsDto>(null, 1, 1);

            try
            {
                //Get jobs data from ApiGateway
                string usersJson = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Reputation/UserReputationHistory/GetPaged?page=" + page + "&pageCount=" + pageCount, HttpContext.Session.GetString("Token"));
                //Parse response
                var jobsListPaged = Helpers.Serializers.DeserializeJson<PaginationEntity<ReputationLogsDto>>(usersJson);

                pagedModel = new StaticPagedList<ReputationLogsDto>(
                    jobsListPaged.Items,
                    jobsListPaged.MetaData.PageNumber,
                    jobsListPaged.MetaData.PageSize,
                    jobsListPaged.MetaData.TotalItemCount
                    );
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }
            return View(pagedModel);
        }

        /// <summary>
        ///  Finds user reputations from user id
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthorizeAdmin]
        public JsonResult ReputationSearch(string searchText)
        {
            List<ReputationLogsDto> repList = new List<ReputationLogsDto>();
            try
            {
                string repJson = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Reputation/UserReputationHistory/GetByUserId?userid=" + searchText, HttpContext.Session.GetString("Token"));
                //Parse response
                repList = Helpers.Serializers.DeserializeJson<List<ReputationLogsDto>>(repJson);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return Json(repList);
        }

        /// <summary>
        ///  This view shows application logs of the DAO
        /// </summary>
        /// <returns></returns>
        [Route("Application-Logs")]
        [Route("Home/Application-Logs")]
        [AuthorizeAdmin]
        public IActionResult Application_Logs(int page = 1, int pageCount = 20)
        {
            ViewBag.Title = "Application Logs";

            IPagedList<Helpers.Models.DtoModels.LogDbDto.ApplicationLogDto> pagedModel = new PagedList<Helpers.Models.DtoModels.LogDbDto.ApplicationLogDto>(null, 1, 1);

            try
            {
                //Get jobs data from ApiGateway
                string usersJson = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Log/ApplicationLog/GetPaged?page=" + page + "&pageCount=" + pageCount, HttpContext.Session.GetString("Token"));
                //Parse response
                var jobsListPaged = Helpers.Serializers.DeserializeJson<PaginationEntity<Helpers.Models.DtoModels.LogDbDto.ApplicationLogDto>>(usersJson);

                pagedModel = new StaticPagedList<Helpers.Models.DtoModels.LogDbDto.ApplicationLogDto>(
                    jobsListPaged.Items,
                    jobsListPaged.MetaData.PageNumber,
                    jobsListPaged.MetaData.PageSize,
                    jobsListPaged.MetaData.TotalItemCount
                    );
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }
            return View(pagedModel);
        }

        /// <summary>
        ///  Approves job with "AdminApprovalPending" status
        /// </summary>
        /// <param name="JobId"></param>
        /// <returns></returns>
        [AuthorizeAdmin]
        [HttpGet]
        public JsonResult RestartVoting(int votingid)
        {
            SimpleResponse result = new SimpleResponse();

            try
            {
                //Get response from ApiGateway
                string jsonResponse = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Voting/Voting/RestartVoting?votingid=" + votingid, HttpContext.Session.GetString("Token"));
                result = Helpers.Serializers.DeserializeJson<SimpleResponse>(jsonResponse);
                result.Content = null;

                //Change job status
                VotingDto voteModel = Helpers.Serializers.DeserializeJson<VotingDto>(Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Voting/Voting/GetId?id=" + votingid, HttpContext.Session.GetString("Token")));
                JobPostDto jobModel = Helpers.Serializers.DeserializeJson<JobPostDto>(Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/JobPost/GetId?id=" + voteModel.JobID, HttpContext.Session.GetString("Token")));
                jobModel.Status = JobStatusTypes.InformalVoting;
                jobModel = Helpers.Serializers.DeserializeJson<JobPostDto>(Helpers.Request.Put(Program._settings.Service_ApiGateway_Url + "/Db/JobPost/Update", Helpers.Serializers.SerializeJson(jobModel), HttpContext.Session.GetString("Token")));

                //Set server side toastr because page will be redirected                                
                TempData["toastr-message"] = result.Message;
                TempData["toastr-type"] = "success";

                Program.monitizer.AddUserLog(Convert.ToInt32(HttpContext.Session.GetInt32("UserID")), Helpers.Constants.Enums.UserLogType.Request, "Voting restarted by admin user. Voting #" + votingid, Utility.IpHelper.GetClientIpAddress(HttpContext), Utility.IpHelper.GetClientPort(HttpContext));

                return Json(result);

            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return Json(new SimpleResponse { Success = false, Message = Lang.ErrorNote });
        }

        /// <summary>
        ///  Approves job with "AdminApprovalPending" status
        /// </summary>
        /// <param name="JobId"></param>
        /// <returns></returns>
        [AuthorizeAdmin]
        [HttpGet]
        public JsonResult RestartAuction(int auctionid)
        {
            SimpleResponse result = new SimpleResponse();

            try
            {
                //Get auction model from ApiGateway
                var auctionJson = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/Auction/GetId?id=" + auctionid, HttpContext.Session.GetString("Token"));
                //Parse response
                var auction = Helpers.Serializers.DeserializeJson<AuctionDto>(auctionJson);

                if (auction.Status != AuctionStatusTypes.Expired)
                {
                    result.Message = "Only expired auctions can be restarted";
                    result.Success = false;
                }

                auction.Status = AuctionStatusTypes.InternalBidding;

                //Set auction end dates
                DateTime internalAuctionEndDate = DateTime.Now.AddDays(Program._settings.InternalAuctionTime);
                DateTime publicAuctionEndDate = DateTime.Now.AddDays(Program._settings.InternalAuctionTime + Program._settings.PublicAuctionTime);

                if (Program._settings.AuctionTimeType == "week")
                {
                    internalAuctionEndDate = DateTime.Now.AddDays(Program._settings.InternalAuctionTime * 7);
                    publicAuctionEndDate = DateTime.Now.AddDays((Program._settings.InternalAuctionTime + Program._settings.PublicAuctionTime) * 7);
                }
                else if (Program._settings.AuctionTimeType == "minute")
                {
                    internalAuctionEndDate = DateTime.Now.AddMinutes(Program._settings.InternalAuctionTime);
                    publicAuctionEndDate = DateTime.Now.AddMinutes(Program._settings.InternalAuctionTime + Program._settings.PublicAuctionTime);
                }

                auction.InternalAuctionEndDate = internalAuctionEndDate;
                auction.PublicAuctionEndDate = publicAuctionEndDate;

                var auctionUpdateJson = Helpers.Request.Put(Program._settings.Service_ApiGateway_Url + "/Db/Auction/Update", Helpers.Serializers.SerializeJson(auction), HttpContext.Session.GetString("Token"));
                auction = Helpers.Serializers.DeserializeJson<AuctionDto>(auctionUpdateJson);

                if (auction.AuctionID > 0)
                {
                    result.Message = "Auction restarted succesfully.";
                    result.Success = true;

                    //Change job status
                    JobPostDto jobModel = Helpers.Serializers.DeserializeJson<JobPostDto>(Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/JobPost/GetId?id=" + auction.JobID, HttpContext.Session.GetString("Token")));
                    jobModel.Status = JobStatusTypes.InternalAuction;
                    jobModel = Helpers.Serializers.DeserializeJson<JobPostDto>(Helpers.Request.Put(Program._settings.Service_ApiGateway_Url + "/Db/JobPost/Update", Helpers.Serializers.SerializeJson(jobModel), HttpContext.Session.GetString("Token")));
                }

                //Set server side toastr because page will be redirected
                TempData["toastr-message"] = result.Message;
                TempData["toastr-type"] = "success";

                Program.monitizer.AddUserLog(Convert.ToInt32(HttpContext.Session.GetInt32("UserID")), Helpers.Constants.Enums.UserLogType.Request, "Auction restarted by admin user. Auction #" + auctionid, Utility.IpHelper.GetClientIpAddress(HttpContext), Utility.IpHelper.GetClientPort(HttpContext));

                return Json(result);

            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return Json(new SimpleResponse { Success = false, Message = Lang.ErrorNote });
        }

        #endregion

        #region  VA Directory
        /// <summary>
        ///  VA Directory view
        /// </summary>
        /// <returns></returns>
        [Route("VA-Directory")]
        public IActionResult VA_Directory()
        {
            ViewBag.Title = "VA Directory";

            List<VADirectoryViewModel> model = new List<VADirectoryViewModel>();

            try
            {
                //Get VA Directory data from ApiGateway
                string paymentsJson = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/Website/GetVADirectory", HttpContext.Session.GetString("Token"));
                //Parse response
                model = Helpers.Serializers.DeserializeJson<List<VADirectoryViewModel>>(paymentsJson);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return View(model);
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

        [HttpGet]
        public ActionResult GetProfileImage()
        {
            try
            {
                if (!string.IsNullOrEmpty(HttpContext.Session.GetString("ProfileImage")))
                {
                    string image = HttpContext.Session.GetString("ProfileImage");

                    //User's profile image is one of the stock images
                    if (image.Length < 50)
                    {
                        byte[] img = System.IO.File.ReadAllBytes("./wwwroot/Home/images/avatars/" + image);
                        return this.File(img, "image/png", "image.png");
                    }
                    //User's profile image is custom uploaded image
                    else
                    {
                        var arr = Convert.FromBase64String(image);
                        return this.File(arr, "image/png", "image.png");
                    }
                }
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            //Return default profile image
            byte[] defaultImage = System.IO.File.ReadAllBytes("./wwwroot/Home/images/avatars/default.png");
            return this.File(defaultImage, "image/png", "image.png");
        }
        #endregion
    }
}
