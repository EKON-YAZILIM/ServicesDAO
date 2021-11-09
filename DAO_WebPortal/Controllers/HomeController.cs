using DAO_WebPortal.Models;
using DAO_WebPortal.Providers;
using Helpers.Models.DtoModels.MainDbDto;
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
                    var url = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/Website/GetDashBoard?userid=" + HttpContext.Session.GetInt32("UserID"), HttpContext.Session.GetString("Token"));
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
            catch (Exception)
            {
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
                var url = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/Website/GetAuction", HttpContext.Session.GetString("Token"));

                //Parse response
                auctionModel = Helpers.Serializers.DeserializeJson<List<AuctionViewModel>>(url);
            }
            catch (Exception ex)
            {

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
                //Get model from ApiGateway
                var url = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/Website/GetAuctionBids?auctionid=" + AuctionID, HttpContext.Session.GetString("Token"));

                //Parse response
                AuctionDetailModel.AuctionBidViewModels = Helpers.Serializers.DeserializeJson<List<AuctionBidViewModel>>(url);
                //Set AuctionID
                AuctionDetailModel.AuctionID = AuctionID;
            }
            catch (Exception ex)
            {

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
                var url = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/Website/GetVoteJobsByStatus", HttpContext.Session.GetString("Token"));

                //Parse response
                votesModel = Helpers.Serializers.DeserializeJson<List<VotingViewModel>>(url);

            }
            catch (Exception ex)
            {

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
                var url = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/Website/ReputationHistory?userid=" + HttpContext.Session.GetInt32("UserID"), HttpContext.Session.GetString("Token"));

                //Parse response
                ReputationHistoryModel = Helpers.Serializers.DeserializeJson<List<UserReputationHistoryDto>>(url);
            }
            catch (Exception ex)
            {

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
        /// <param name="Job">Job Id</param>
        /// <returns></returns>
        [Route("Job-Detail/{Job}")]
        public IActionResult Job_Detail(int Job)
        {
            JobPostDetailModel model = new JobPostDetailModel();

            try
            {
                var userid = HttpContext.Session.GetInt32("UserID");

                //Get model from ApiGateway
                var url = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/Website/GetJobDetail?jobid=" + Job, HttpContext.Session.GetString("Token"));
                //Parse response
                model.JobPostWebsiteModel = Helpers.Serializers.DeserializeJson<JobPostViewModel>(url);

                //Get model from ApiGateway
                var url1 = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/Website/GetJobComment?jobid=" + Job + "&userid=" + userid, HttpContext.Session.GetString("Token"));

                //Parse response
                model.JobPostCommentModel = Helpers.Serializers.DeserializeJson<List<JobPostCommentModel>>(url1);


            }
            catch (Exception ex)
            {

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
                //Post model to ApiGateway
                //Add new job
                model = Helpers.Serializers.DeserializeJson<JobPostDto>(Helpers.Request.Post(Program._settings.Service_ApiGateway_Url + "/Db/JobPost/Post", Helpers.Serializers.SerializeJson(new JobPostDto { UserID = Convert.ToInt32(HttpContext.Session.GetInt32("UserID")), Amount = amount, JobDescription = description, CreateDate = DateTime.Now, TimeFrame = time, LastUpdate = DateTime.Now, Title = title }), HttpContext.Session.GetString("Token")));
                if (model.JobID == 0 || model.JobID == null)
                {
                    result.Success = false;
                    result.Message = "Kayıt esnasında hata oluştu.";
                    result.Content = model;
                }
                else
                {
                    result.Success = true;
                    result.Message = "Kayıt yapıldı.";
                    result.Content = model;
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "İşlem esnasında hata oluştu.";
                result.Content = model;
            }
            return Json(result);
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
                if (model.JobID == 0 || model.JobID == null)
                {
                    result.Success = false;
                    result.Message = "Güncelleme esnasında hata oluştu.";
                    result.Content = model;
                }
                else
                {
                    result.Success = true;
                    result.Message = "Güncelleme yapıldı.";
                    result.Content = model;
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "İşlem esnasında hata oluştu.";
                result.Content = model;
            }
            return Json(result);
        }

        /// <summary>
        /// Comment upvote function
        /// </summary>
        /// <param name="JobPostCommentId">JobPostCommentId</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult UpVote(int JobPostCommentId)
        {
            int[] res = { 0, 0 };
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

                //UpVote control 
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
                //DownVote control 
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
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "İşlem esnasında hata oluştu.";
                result.Content = res;
            }
            return Json(result);
        }

        /// <summary>
        /// Comment downvote function
        /// </summary>
        /// <param name="JobPostCommentId">JobPostCommentId</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult DownVote(int JobPostCommentId)
        {
            int[] res = { 0, 0 };
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

                //DownVote control
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
                //UpVote control
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
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "İşlem esnasında hata oluştu.";
                result.Content = res;
            }
            return Json(result);
        }

        /// <summary>
        /// Add new auction
        /// </summary>
        /// <param name="Model">AuctionBidDto Model</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Auction_Add(AuctionBidDto Model)
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

                if (model.AuctionBidID == 0 || model.AuctionBidID == null)
                {
                    result.Success = false;
                    result.Message = "Ekleme esnasında hata oluştu.";
                    result.Content = model;
                }
                else
                {
                    result.Success = true;
                    result.Message = "Ekleme yapıldı.";
                    result.Content = model;
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "İşlem esnasında hata oluştu.";
                result.Content = model;
            }
            return Json(result);
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
            JobPostCommentDto model = new JobPostCommentDto();

            try
            {
                //Create new comment
                model = new JobPostCommentDto()
                {
                    Comment = Comment,
                    Date = DateTime.Now,
                    JobID = JobId,
                    SubCommentID = CommentId,
                    UserID = Convert.ToInt32(HttpContext.Session.GetInt32("UserID")),
                    DownVote = 0,
                    UpVote = 0,
                };

                //Post model to ApiGateway
                //Add new job
                model = Helpers.Serializers.DeserializeJson<JobPostCommentDto>(Helpers.Request.Post(Program._settings.Service_ApiGateway_Url + "/Db/JobPostComment/Post", Helpers.Serializers.SerializeJson(model), HttpContext.Session.GetString("Token")));
                if (model.JobPostCommentID == 0 || model.JobPostCommentID == null)
                {
                    result.Success = false;
                    result.Message = "Kayıt esnasında hata oluştu.";
                    result.Content = model;
                }
                else
                {
                    result.Success = true;
                    result.Message = "Kayıt yapıldı.";
                    result.Content = model;
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "İşlem esnasında hata oluştu.";
                result.Content = model;
            }
            return Json(result);
        }
        #region UserSerttings
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
