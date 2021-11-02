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
                var url = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "//Db/Website/GetAuction", HttpContext.Session.GetString("Token"));
                auctionModel = Helpers.Serializers.DeserializeJson<List<AuctionViewModel>>(url);
            }
            catch (Exception ex)
            {

            }
            return View(auctionModel);
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
            return View();
        }

        [Route("Payments-History")]
        public IActionResult Payments_History()
        {
            return View();
        }

        [Route("User-Profile")]
        public IActionResult User_Profile()
        {
            return View();
        }

        [Route("RFP-Form")]
        public IActionResult RFP_Form()
        {
            return View();
        }

        [Route("RFPs")]
        public IActionResult RFP()
        {
            List<RfpModel> model = new List<RfpModel>();
            try
            {
                var url = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Rfp/Rfp/GetRfpsByStatus", HttpContext.Session.GetString("Token"));
                model = Helpers.Serializers.DeserializeJson<List<RfpModel>>(url);
            }
            catch (Exception)
            {

                return View(new List<RfpModel>());
            }

            return View(model);
        }

        [Route("RFP-Detail/{RFPID}")]
        public IActionResult RFP_Detail(int RFPID)
        {
            List<RfpBidDetailModel> model = new List<RfpBidDetailModel>();
            try
            {
                var url = Helpers.Request.Get(Program._settings.Service_ApiGateway_Url + "/Rfp/Rfp/GetRfpBidsByRfpId?rfpid=" + RFPID, HttpContext.Session.GetString("Token"));
                model = Helpers.Serializers.DeserializeJson<List<RfpBidDetailModel>>(url);
            }
            catch (Exception)
            {
                return View(new List<RfpBidDetailModel>());
            }
            return View(model);
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

        [HttpPost]
        public JsonResult New_Job_Post(string title, double amount, string time, int price, string description)
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

        [HttpPut]
        public JsonResult My_Job_Update(JobPostDto Model)
        {
            JobPostDto model = new JobPostDto();
            AjaxResponse result = new AjaxResponse();
            try
            {
                model = Helpers.Serializers.DeserializeJson<JobPostDto>(Helpers.Request.Put(Program._settings.Service_ApiGateway_Url + "/Db/JobPost/Update", Helpers.Serializers.SerializeJson(Model) , HttpContext.Session.GetString("Token")));
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
