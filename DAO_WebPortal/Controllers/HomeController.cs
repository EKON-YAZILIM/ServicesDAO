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

        [Route("Home")]
        public IActionResult Index()
        {
            GetDashBoardViewModel dashModel = new GetDashBoardViewModel();
            try
            {
                var url = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/Website/GetDashBoard?userid=" + HttpContext.Session.GetInt32("UserID"), HttpContext.Session.GetString("Token"));
                dashModel = Helpers.Serializers.DeserializeJson<GetDashBoardViewModel>(url);
            }
            catch (Exception)
            {
                return View(new GetDashBoardViewModel());
            }

            return View(dashModel);
        }

        [Route("My-Jobs")]
        public IActionResult My_Jobs()
        {
            List<JobPostViewModel> myJobsModel = new List<JobPostViewModel>();
            try
            {
                var url = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/Website/GetUserJobs?userid=" + HttpContext.Session.GetInt32("UserID"), HttpContext.Session.GetString("Token"));
                myJobsModel = Helpers.Serializers.DeserializeJson<List<JobPostViewModel>>(url);
            }
            catch (Exception ex)
            {

            }
            return View(myJobsModel);
        }

        [Route("All-Jobs")]
        public IActionResult All_Jobs()
        {
            List<JobPostViewModel> allJobsModel = new List<JobPostViewModel>();
            try
            {
                var url = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/Website/GetAllJobs", HttpContext.Session.GetString("Token"));
                allJobsModel = Helpers.Serializers.DeserializeJson<List<JobPostViewModel>>(url);
            }
            catch (Exception ex)
            {

            }

            return View(allJobsModel);
        }

        [Route("Auctions")]
        public IActionResult Auctions()
        {
            List<AuctionViewModel> auctionModel = new List<AuctionViewModel>();
            try
            {
                var url = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/Website/GetAuction", HttpContext.Session.GetString("Token"));
                auctionModel = Helpers.Serializers.DeserializeJson<List<AuctionViewModel>>(url);
            }
            catch (Exception ex)
            {

            }
            return View(auctionModel);
        }

        [Route("Auction-Detail/{AuctionID}")]
        public IActionResult Auction_Detail(int AuctionID)
        {
            AuctionBidWebsiteModel AuctionDetailModel = new AuctionBidWebsiteModel();
            try
            {
                var url = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/Website/GetAuctionBids?auctionid=" + AuctionID, HttpContext.Session.GetString("Token"));
                AuctionDetailModel.AuctionBidViewModels = Helpers.Serializers.DeserializeJson<List<AuctionBidViewModel>>(url);
                AuctionDetailModel.AuctionID = AuctionID;
            }
            catch (Exception ex)
            {

            }
            return View(AuctionDetailModel);
        }

        [Route("Votes")]
        public IActionResult Votes()
        {
            List<VotingViewModel> votesModel = new List<VotingViewModel>();
            try
            {
                var url = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/Website/GetVoteJobsByStatus", HttpContext.Session.GetString("Token"));
                votesModel = Helpers.Serializers.DeserializeJson<List<VotingViewModel>>(url);

            }
            catch (Exception ex)
            {


            }
            return View(votesModel);
        }

        [Route("Reputation-History")]
        public IActionResult Reputation_History()
        {
            List<UserReputationHistoryDto> ReputationHistoryModel = new List<UserReputationHistoryDto>();
            try
            {
                var url = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/Website/ReputationHistory?userid=" + HttpContext.Session.GetInt32("UserID"), HttpContext.Session.GetString("Token"));
                ReputationHistoryModel = Helpers.Serializers.DeserializeJson<List<UserReputationHistoryDto>>(url);
            }
            catch (Exception ex)
            {

            }
            return View(ReputationHistoryModel);
        }

        [Route("Payments-History")]
        public IActionResult Payments_History()
        {
            return View();
        }

        [Route("User-Profile")]
        public IActionResult User_Profile()
        {
            UserDto profileModel = new UserDto();

            try
            {
                var json = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/Users/GetId?id="+ HttpContext.Session.GetInt32("UserID"), HttpContext.Session.GetString("Token"));
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

        [Route("Job-Detail/{Job}")]
        public IActionResult Job_Detail(int Job)
        {
            JobPostDetailModel jobDetailModel = new JobPostDetailModel();
            List<JobPostCommentModel> newList = new List<JobPostCommentModel>();
            try
            {
                var url = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/Website/GetJobDetail?jobid=" + Job, HttpContext.Session.GetString("Token"));
                jobDetailModel = Helpers.Serializers.DeserializeJson<JobPostDetailModel>(url);
                var userid = HttpContext.Session.GetInt32("UserID");
                foreach (var item in jobDetailModel.JobPostCommentModel)
                {
                    var model = Helpers.Serializers.DeserializeJson<List<UserCommentVoteDto>>(Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/UserCommentVote/GetByCommentId?CommentId=" + item.JobPostCommentID, HttpContext.Session.GetString("Token")));
                    if (model.Count(x => x.UserId == userid && x.IsUpVote == true) > 0)
                    {
                        item.IsUpVote = true;
                    }
                    else if (model.Count(x => x.UserId == userid && x.IsUpVote == false) > 0)
                    {
                        item.IsUpVote = false;
                    }
                    else
                    {
                        item.IsUpVote = null;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return View(jobDetailModel);
        }

        [Route("Vote-Detail/{VoteID}")]
        public IActionResult Vote_Detail(int VoteID)
        {
            List<VoteDto> voteDetailModel = new List<VoteDto>();
            try
            {
                var url = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/Website/GetVoteDetail?voteid=" + VoteID, HttpContext.Session.GetString("Token"));
                voteDetailModel = Helpers.Serializers.DeserializeJson<List<VoteDto>>(url);
            }
            catch (Exception ex)
            {

            }
            return View(voteDetailModel);


        }

        [Route("New-Job")]
        public IActionResult New_Job(int VoteID)
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
            AjaxResponse result = new AjaxResponse();
            try
            {
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

        [Route("My-Job-Edit/{Job}")]
        public IActionResult My_Job_Edit(int Job)
        {
            JobPostDto jobDetailModel = new JobPostDto();

            try
            {
                var url = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/JobPost/GetId?id=" + Job, HttpContext.Session.GetString("Token"));
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
            AjaxResponse result = new AjaxResponse();
            try
            {
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
            AjaxResponse result = new AjaxResponse();
            try
            {
                var model = Helpers.Serializers.DeserializeJson<List<UserCommentVoteDto>>(Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/UserCommentVote/GetByCommentId?CommentId=" + JobPostCommentId, HttpContext.Session.GetString("Token")));

                var userid = HttpContext.Session.GetInt32("UserID");
                int UpCount = model.Count(x => x.UserId == userid && x.IsUpVote == true);
                int DownCount = model.Count(x => x.UserId == userid && x.IsUpVote == false);

                if (model.Count(x => x.UserId == userid && x.IsUpVote == true) > 0)
                {               
                    var CommentModel = model.First(x => x.UserId == userid);
                    var deleteModel = Helpers.Request.Delete(Program._settings.Service_ApiGateway_Url + "/Db/UserCommentVote/Delete?ID=" + CommentModel.UserCommentVoteID, HttpContext.Session.GetString("Token"));
                    
                    res[0] = UpCount - 1;
                    res[1] = DownCount;
                    result.Success = true;
                    result.Message = "";
                    result.Content = res;
                }
                else if (model.Count(x => x.UserId == userid && x.IsUpVote == false) > 0)
                {
                    var CommentModel = model.First(x => x.UserId == userid && x.IsUpVote == false);
                    CommentModel.IsUpVote = true;
                    var updateModel = Helpers.Serializers.DeserializeJson<UserCommentVoteDto>(Helpers.Request.Put(Program._settings.Service_ApiGateway_Url + "/Db/UserCommentVote/Update", Helpers.Serializers.SerializeJson(CommentModel), HttpContext.Session.GetString("Token")));

                    res[0] = UpCount + 1;
                    res[1] = DownCount - 1;
                    result.Success = true;
                    result.Message = "";
                    result.Content = res;
                }
                else
                {
                    UserCommentVoteDto CommentModel = new UserCommentVoteDto()
                    {
                        UserId = Convert.ToInt32(userid),
                        IsUpVote = true,
                        JobPostCommentID = JobPostCommentId,
                    };
                    var postModel = Helpers.Serializers.DeserializeJson<UserCommentVoteDto>(Helpers.Request.Post(Program._settings.Service_ApiGateway_Url + "/Db/UserCommentVote/Post", Helpers.Serializers.SerializeJson(CommentModel), HttpContext.Session.GetString("Token")));

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
            AjaxResponse result = new AjaxResponse();
            try
            {
                var model = Helpers.Serializers.DeserializeJson<List<UserCommentVoteDto>>(Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Db/UserCommentVote/GetByCommentId?CommentId=" + JobPostCommentId, HttpContext.Session.GetString("Token")));
                var userid = HttpContext.Session.GetInt32("UserID");
                int UpCount = model.Count(x => x.UserId == userid && x.IsUpVote == true);
                int DownCount = model.Count(x => x.UserId == userid && x.IsUpVote == false);

                if (model.Count(x => x.UserId == userid && x.IsUpVote == false) > 0)
                {
                    var CommentModel = model.First(x => x.UserId == userid);
                    var deleteModel = Helpers.Request.Delete(Program._settings.Service_ApiGateway_Url + "/Db/UserCommentVote/Delete?ID=" + CommentModel.UserCommentVoteID, HttpContext.Session.GetString("Token"));
                    res[0] = UpCount;
                    res[1] = DownCount - 1;
                    result.Success = true;
                    result.Message = "";
                    result.Content = res;
                }
                else if (model.Count(x => x.UserId == userid && x.IsUpVote == true) > 0)
                {
                    var CommentModel = model.First(x => x.UserId == userid && x.IsUpVote == true);
                    CommentModel.IsUpVote = false;
                    var updateModel = Helpers.Serializers.DeserializeJson<UserCommentVoteDto>(Helpers.Request.Put(Program._settings.Service_ApiGateway_Url + "/Db/UserCommentVote/Update", Helpers.Serializers.SerializeJson(CommentModel), HttpContext.Session.GetString("Token")));

                    res[0] = UpCount - 1;
                    res[1] = DownCount + 1;
                    result.Success = true;
                    result.Message = "";
                    result.Content = res;
                }
                else
                {
                    UserCommentVoteDto CommentModel = new UserCommentVoteDto()
                    {
                        UserId = Convert.ToInt32(userid),
                        IsUpVote = false,
                        JobPostCommentID = JobPostCommentId,
                    };
                    var postModel = Helpers.Serializers.DeserializeJson<UserCommentVoteDto>(Helpers.Request.Post(Program._settings.Service_ApiGateway_Url + "/Db/UserCommentVote/Post", Helpers.Serializers.SerializeJson(CommentModel), HttpContext.Session.GetString("Token")));

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
            AjaxResponse result = new AjaxResponse();
            try
            {
                var userid = HttpContext.Session.GetInt32("UserID");
                model = new AuctionBidDto()
                {
                    AuctionID = Model.AuctionID,
                    Price = Model.Price,
                    Time = Model.Time,
                    ReputationStake = Model.ReputationStake,
                    UserId = Convert.ToInt32(userid)
                };
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
