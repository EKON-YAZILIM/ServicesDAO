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

        #region Jobs
        /// <summary>
        /// Get all jobs
        /// </summary>
        /// <param name="status">Job Status Types</param>
        /// <returns></returns>
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

        /// <summary>
        /// Get Job comment and user 
        /// </summary>
        /// <param name="jobid"> Job Id</param>
        /// <param name="userid"> User Id</param>
        /// <returns></returns>
        [Route("GetJobComment")]
        [HttpGet]
        public List<JobPostCommentModel> GetJobComment(int jobid, int userid)
        {
            List<JobPostCommentModel> result = new List<JobPostCommentModel>();
            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    result = (from job in db.JobPostComments
                              join user in db.Users on job.UserID equals user.UserId
                              let upvote = db.UserCommentVotes.Count(x => x.IsUpVote == true && x.JobPostCommentID == job.JobPostCommentID)
                              let downvote = db.UserCommentVotes.Count(x => x.IsUpVote == false && x.JobPostCommentID == job.JobPostCommentID)
                              where job.JobID == jobid
                              select new JobPostCommentModel
                              {
                                  JobPostCommentID = job.JobPostCommentID,
                                  ProfileImage = user.ProfileImage,
                                  UserName = user.UserName,
                                  Date = job.Date,
                                  Comment = job.Comment,
                                  SubCommentID = job.SubCommentID,
                                  UpVote = upvote,
                                  DownVote = downvote,
                                  Points = 0
                              }).ToList();

                    var commentIds = result.Select(x => x.JobPostCommentID).ToList();

                    var commentVotesOfUser = db.UserCommentVotes.Where(x => x.UserId == userid && commentIds.Contains(x.JobPostCommentID));

                    foreach (var commentVote in commentVotesOfUser)
                    {
                        var comment = result.First(x=>x.JobPostCommentID == commentVote.JobPostCommentID);
                        comment.IsUpVote = commentVote.IsUpVote;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return result;
        }

        /// <summary>
        /// Get job detail
        /// </summary>
        /// <param name="jobid"></param>
        /// <returns></returns>
        [Route("GetJobDetail")]
        [HttpGet]
        public JobPostViewModel GetJobDetail(int jobid)
        {
            JobPostViewModel result = new JobPostViewModel();
            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    var jobPost = db.JobPosts.Find(jobid);
                    var user = db.Users.Find(jobPost.UserID);
                    var count = db.JobPostComments.Count(x => x.JobID == jobPost.JobID);
                    result = new JobPostViewModel
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
                }
            }
            catch (Exception ex)
            {

            }
            return result;
        }

        /// <summary>
        /// Get users job
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Get jobpost and user table together 
        /// </summary>
        /// <param name="ProgressType"></param>
        /// <returns></returns>
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

        #endregion

        #region Auction

        /// <summary>
        /// Get auctions
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
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
                                  IsInternal = act.IsInternal,
                                  Status = act.Status,
                                  AuctionID = act.AuctionID,
                                  Title = job.Title
                                  
                              }).ToList();
                }
            }
            catch (Exception ex)
            {

            }
            return result;
        }

        /// <summary>
        /// Get auctions bids
        /// </summary>
        /// <param name="auctionid"></param>
        /// <returns></returns>
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

        #endregion

        #region Dashboard

        /// <summary>
        /// Get dashboard by user type
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
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

                    //User type check
                    if (userType == "Admin")
                    {
                        //Get job post model from GetVoteJobsByProgressTypes function
                        res.JobPostDtos = GetVoteJobsByProgressTypes(Helpers.Constants.Enums.JobProgressTypes.AdminApprovalPending);

                        //Get auction model from GetAuctions function
                        res.AuctionViewModels = GetAuctions(Helpers.Constants.Enums.JobStatusTypes.Active);

                        //Get auction model from GetVoteJobsByStatus function
                        res.VotingViewModels = GetVoteJobsByStatus(Helpers.Constants.Enums.JobStatusTypes.Active);

                        //Get model from Service_Log_Url
                        res.ApplicationLogDtos = Helpers.Serializers.DeserializeJson<List<ApplicationLogDto>>(Helpers.Request.Get(Program._settings.Service_Log_Url + "/ApplicationLog/GetLastWithCount?count=" + 20));

                        //Get model from Service_Log_Url
                        res.UserLogDtos = Helpers.Serializers.DeserializeJson<List<UserLogDto>>(Helpers.Request.Get(Program._settings.Service_Log_Url + "/UserLog/GetLastWithCount?Count=" + 20));

                        //Get users registered in the last mounth
                        var date = DateTime.Now.AddMonths(-1);
                        var usersModel = db.Users.Where(x => x.CreateDate > date).ToList();
                        res.UserDtos = _mapper.Map<List<User>, List<UserDto>>(usersModel).ToList();

                        //Get users count
                        res.UserCount = db.Users.Count();

                        //Get job post count
                        res.JobCount = db.JobPosts.Count();

                        //Get auction count
                        res.AuctionCount = db.Auctions.Count();

                        //Get voting count
                        //Get model from Voting_Engine_Url
                        res.VotingCount = Helpers.Serializers.DeserializeJson<List<VotingDto>>(Helpers.Request.Get(Program._settings.Voting_Engine_Url + "/Voting/Get?")).Count();
                    }
                    else if (userType == "Associate")
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

        #endregion
   
        #region Vote

        /// <summary>
        /// Get vote detail
        /// </summary>
        /// <param name="voteid"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Get voting
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
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
                               Status = votejob.Status

                           }).ToList();
                }
            }
            catch (Exception ex)
            {

            }
            return res;
        }

        #endregion

        #region Reputation

        /// <summary>
        /// Get user reputation history
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        [Route("ReputationHistory")]
        [HttpGet]
        public List<UserReputationHistoryDto> ReputationHistory (int userid)
        {
            List<UserReputationHistoryDto> res = new List<UserReputationHistoryDto>();
            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    var url = Helpers.Request.Get(Program._settings.Voting_Engine_Url + "/UserReputationHistory/GetUserId?userid=" + userid);
                    res = Helpers.Serializers.DeserializeJson<List<UserReputationHistoryDto>>(url);
                }
            }
            catch (Exception ex)
            {

            }
            return res;
        }

        #endregion
    }
}

