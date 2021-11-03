using DAO_DbService.Contexts;
using DAO_DbService.Models;
using Helpers.Models.DtoModels.LogDbDto;
using Helpers.Models.DtoModels.MainDbDto;
using Helpers.Models.DtoModels.VoteDbDto;
using Helpers.Models.WebsiteViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static DAO_DbService.Mapping.AutoMapperBase;

namespace DAO_DbService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class WebsiteController : Controller
    {
        [Route("GetAllJobs")]
        [HttpGet]
        public List<JobPostViewModel> GetAllJobs(Helpers.Constants.Enums.JobStatusTypes? status)
        {
            List<JobPostViewModel> result = new List<JobPostViewModel>();
            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    result = (from job in db.JobPosts
                              join user in db.Users on job.UserID equals user.UserId
                              let explenation = job.JobDescription.Substring(0, 100)
                              let count = db.JobPostComments.Count(x => x.JobID == job.JobID)
                              where status == null || job.Status == status
                              select new JobPostViewModel
                              {
                                  Title = job.Title,
                                  UserName = user.UserName,
                                  CreateDate = job.CreateDate,
                                  JobDescription = explenation,
                                  LastUpdate = job.LastUpdate,
                                  JobID = job.JobID,
                                  Status = job.Status,
                                  Amount = job.Amount,
                                  ProgressType = job.ProgressType,
                                  CommentCount = count
                              }).ToList();

                }


            }
            catch (Exception ex)
            {

            }
            return result;
        }

        [Route("GetJobComment")]
        [HttpGet]
        public List<JobPostCommentModel> GetJobComment(int jobid)
        {
            List<JobPostCommentModel> result = new List<JobPostCommentModel>();
            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {

                  
                    result = (from job in db.JobPostComments
                              join user in db.Users on job.UserID equals user.UserId
                              where job.JobID == jobid
                              select new JobPostCommentModel
                              {
                                  JobPostCommentID = job.JobPostCommentID,
                                  ProfileImage = user.ProfileImage,
                                  UserName = user.UserName,
                                  Date = job.Date,
                                  Comment = job.Comment,
                                  SubCommentID = job.SubCommentID,
                                  UpVote = job.UpVote,
                                  DownVote = job.DownVote,
                                  Points = 0
                              }).ToList();
                }

            }
            catch (Exception ex)
            {

            }
            return result;
        }

        [Route("GetJobDetail")]
        [HttpGet]
        public JobPostDetailModel GetJobDetail(int jobid)
        {
            JobPostDetailModel result = new JobPostDetailModel();
            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    var jobPost = db.JobPosts.Find(jobid);
                    var user = db.Users.Find(jobPost.UserID);
                    var count = db.JobPostComments.Count(x => x.JobID == jobPost.JobID);
                    result.JobPostWebsiteModel = new JobPostViewModel
                    {
                        Title = jobPost.Title,
                        UserName = user.UserName,
                        CreateDate = jobPost.CreateDate,
                        JobDescription = jobPost.JobDescription,
                        LastUpdate = jobPost.LastUpdate,
                        JobID = jobPost.JobID,
                        Status = jobPost.Status,
                        Amount = jobPost.Amount,
                        ProgressType = jobPost.ProgressType,
                        CommentCount = count
                    };
                    result.JobPostCommentModel = GetJobComment(jobid);
                }

            }
            catch (Exception ex)
            {

            }
            return result;
        }

        [Route("GetVoteJobsByStatus")]
        [HttpGet]
        public List<VotingViewModel> GetVoteJobsByStatus(Helpers.Constants.Enums.JobStatusTypes? status)
        {
            List<VotingViewModel> res = new List<VotingViewModel>();
            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    string votejobsJson = Helpers.Request.Get(Program._settings.Voting_Engine_Url + "/Voting/GetVotingByStatus?status=" + status);
                    List<VotingDto> model = Helpers.Serializers.DeserializeJson<List<VotingDto>>(votejobsJson);

                    res = (from votejob in model
                           join job in db.JobPosts on votejob.JobID equals job.JobID
                           where status == null || job.Status == status
                           select new VotingViewModel
                           {

                               JobID = job.JobID,
                               VoteID = votejob.VotingID,
                               IsFormal = votejob.IsFormal,
                               CreateDate = votejob.CreateDate,
                               StartDate = votejob.StartDate,
                               EndDate = votejob.EndDate,
                               Title = job.Title,
                               Status = job.Status

                           }).ToList();
                }
            }
            catch (Exception ex)
            {
              
          
            }
           

            return res;
        }

        [Route("GetUserJobs")]
        [HttpGet]
        public List<JobPostViewModel> GetUserJobs(int userid)
        {
            List<JobPostViewModel> result = new List<JobPostViewModel>();
            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    result = (from job in db.JobPosts
                              join user in db.Users on job.UserID equals user.UserId
                              let explenation = job.JobDescription.Substring(0, 100)
                              let count = db.JobPostComments.Count(x => x.JobID == job.JobID)
                              where job.UserID == userid
                              select new JobPostViewModel
                              {
                                  Title = job.Title,
                                  UserName = user.UserName,
                                  CreateDate = job.CreateDate,
                                  JobDescription = job.JobDescription,
                                  LastUpdate = job.LastUpdate,
                                  JobID = job.JobID,
                                  Status = job.Status,
                                  Amount = job.Amount,
                                  ProgressType = job.ProgressType,                       
                                  CommentCount = count

                              }).ToList();
                }
            }
            catch (Exception ex)
            {

            }
            return result;
        }

        [Route("GetAuction")]
        [HttpGet]
        public List<AuctionViewModel> GetAuctions(Helpers.Constants.Enums.JobStatusTypes? status)
        {
            List<AuctionViewModel> result = new List<AuctionViewModel>();
            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    result = (from act in db.Auctions
                              join job in db.JobPosts on act.JobID equals job.JobID
                              join user in db.Users on job.UserID equals user.UserId
                              where status == null || act.Status == status
                              select new AuctionViewModel
                              {
                                  JobID = act.JobID,
                                  StartDate = act.StartDate,
                                  EndDate = act.EndDate,
                                  CreateDate = act.CreateDate,
                                  JobPosterUserId = act.JobPosterUserId,
                                  WinnerAuctionBidID = act.WinnerAuctionBidID,
                                  UserName = user.UserName,
                                  IsInternal = act.IsInternal
                                  
                              }).ToList();
                }
            }
            catch (Exception ex)
            {

            }
            return result;
        }

        [Route("GetAuctionBids")]
        [HttpGet]
        public List<AuctionBidViewModel> GetAuctionBids(int auctionid)
        {
            List<AuctionBidViewModel> result = new List<AuctionBidViewModel>();
            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    result = (from act in db.AuctionBids
                              join user in db.Users on act.UserId equals user.UserId
                              where act.AuctionID == auctionid
                              select new AuctionBidViewModel
                              {
                                  AuctionID = act.AuctionID,
                                  UserId = act.UserId,
                                  Price = act.Price,
                                  Time = act.Time,
                                  ReputationStake = act.ReputationStake,
                                  UserName = user.UserName

                              }).ToList();
                }
            }
            catch (Exception ex)
            {

            }
            return result;
        }

        [Route("GetVoteJobsByProgressTypes")]
        [HttpGet]
        public List<JobPostDto> GetVoteJobsByProgressTypes(Helpers.Constants.Enums.JobProgressTypes? ProgressType)
        {
            List<JobPostDto> res = new List<JobPostDto>();

            using (dao_maindb_context db = new dao_maindb_context())
            {

                res = (from job in db.JobPosts
                       join user in db.Users on job.UserID equals user.UserId
                       let explenation = job.JobDescription.Substring(0, 100)
                       where job.ProgressType == ProgressType
                       select new JobPostDto
                       {
                           JobID = job.JobID,
                           CreateDate = job.CreateDate,
                           UserID = user.UserId,
                           Title = job.Title,
                           JobDescription = explenation,
                           Amount = job.Amount,
                           DosPaid = job.DosPaid,
                           TimeFrame = job.TimeFrame,
                           LastUpdate = job.LastUpdate,
                           Status = job.Status,
                           ProgressType = job.ProgressType
                       }).ToList();
            }

            return res;
        }

        [Route("GetDashBoard")]
        [HttpGet]
        public GetDashBoardViewModel GetDashBoard(int userid)
        {
            GetDashBoardViewModel res = new GetDashBoardViewModel();
            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    var userType = db.Users.First(x => x.UserId == userid).UserType;
                    if (userType == "Admin")
                    {
                        res.JobPostDtos = GetVoteJobsByProgressTypes(Helpers.Constants.Enums.JobProgressTypes.AdminApprovalPending);
                        res.AuctionViewModels = GetAuctions(Helpers.Constants.Enums.JobStatusTypes.Active);
                        res.VotingViewModels = GetVoteJobsByStatus(Helpers.Constants.Enums.JobStatusTypes.Active);
                        res.ApplicationLogDtos = Helpers.Serializers.DeserializeJson<List<ApplicationLogDto>>(Helpers.Request.Get(Program._settings.LogServices_Url + "/ApplicationLog/GetLastWithCount?count=" + 20));
                        res.UserLogDtos = Helpers.Serializers.DeserializeJson<List<UserLogDto>>(Helpers.Request.Get(Program._settings.LogServices_Url + "/UserLog/GetLastWithCount?Count=" + 20));
                        var date = DateTime.Now.AddMonths(-1);
                        var usersModel = db.Users.Where(x => x.CreateDate > date).ToList();
                        res.UserDtos = _mapper.Map<List<User>, List<UserDto>>(usersModel).ToList();
                        res.UserCount = db.Users.Count();
                        res.JobCount = db.JobPosts.Count();
                        res.AuctionCount = db.Auctions.Count();
                        res.VotingCount = Helpers.Serializers.DeserializeJson<List<VotingDto>>(Helpers.Request.Get(Program._settings.Voting_Engine_Url + "/Voting/Get?")).Count();
                    }
                    else if (userType == "Admin")
                    {

                    }
                    else
                    {

                    }
                }
            }
            catch (Exception ex)
            {

            }
            return res;
        }

        [Route("GetVoteDetail")]
        [HttpGet]
        public List<VoteDto> GetVoteDetail(int voteid)
        {
            List<VoteDto> res = new List<VoteDto>();
            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    string voteJson = Helpers.Request.Get(Program._settings.Voting_Engine_Url + "/Vote/GetAllVote?VoteJobID=" + voteid);
                    res = Helpers.Serializers.DeserializeJson<List<VoteDto>>(voteJson);
                }
            }
            catch (Exception ex)
            {

            }
            return res;
        }
    }
}

