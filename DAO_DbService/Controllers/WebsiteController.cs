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
                              let explanation = job.JobDescription.Substring(0, 100)
                              let count = db.JobPostComments.Count(x => x.JobID == job.JobID)
                              where status == null || job.Status == status
                              select new JobPostViewModel
                              {
                                  Title = job.Title,
                                  UserName = user.UserName,
                                  CreateDate = job.CreateDate,
                                  JobDescription = explanation,
                                  LastUpdate = job.LastUpdate,
                                  JobID = job.JobID,
                                  Status = job.Status,
                                  Amount = job.Amount,
                                  CommentCount = count,
                                  JobDoerUserID = job.JobDoerUserID,
                                  DosFeePaid = job.DosFeePaid,
                                  JobPosterUserID = job.UserID
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
                    result = (from comment in db.JobPostComments
                              join user in db.Users on comment.UserID equals user.UserId
                              let upvote = db.UserCommentVotes.Count(x => x.IsUpVote == true && x.JobPostCommentID == comment.JobPostCommentID)
                              let downvote = db.UserCommentVotes.Count(x => x.IsUpVote == false && x.JobPostCommentID == comment.JobPostCommentID)
                              let isComment = comment.UserID == userid ? true : false
                              where comment.JobID == jobid
                              select new JobPostCommentModel
                              {
                                  JobPostCommentID = comment.JobPostCommentID,
                                  ProfileImage = user.ProfileImage,
                                  UserName = user.UserName,
                                  Date = comment.Date,
                                  Comment = comment.Comment,
                                  SubCommentID = comment.SubCommentID,
                                  UpVote = upvote,
                                  DownVote = downvote,
                                  IsUsersComment = isComment,
                                  Points = 0,
                                  IsPinned = comment.IsPinned
                              }).ToList();

                    var commentIds = result.Select(x => x.JobPostCommentID).ToList();

                    var commentVotesOfUser = db.UserCommentVotes.Where(x => x.UserId == userid && commentIds.Contains(x.JobPostCommentID));

                    foreach (var commentVote in commentVotesOfUser)
                    {
                        var comment = result.First(x => x.JobPostCommentID == commentVote.JobPostCommentID);
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
                        CommentCount = count,
                        JobDoerUserID = jobPost.JobDoerUserID,
                        DosFeePaid = jobPost.DosFeePaid,
                        JobPosterUserID = jobPost.UserID
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
        public MyJobsViewModel GetUserJobs(int userid)
        {
            MyJobsViewModel result = new MyJobsViewModel();
            result.ownedJobs = new List<JobPostViewModel>();
            result.doerJobs = new List<JobPostViewModel>();

            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    result.ownedJobs = (from job in db.JobPosts
                                        join user in db.Users on job.UserID equals user.UserId
                                        let count = db.JobPostComments.Count(x => x.JobID == job.JobID)
                                        let explanation = job.JobDescription.Substring(0, 150)
                                        where job.UserID == userid
                                        select new JobPostViewModel
                                        {
                                            Title = job.Title,
                                            UserName = user.UserName,
                                            CreateDate = job.CreateDate,
                                            JobDescription = explanation,
                                            LastUpdate = job.LastUpdate,
                                            JobID = job.JobID,
                                            Status = job.Status,
                                            Amount = job.Amount,
                                            CommentCount = count,
                                            JobDoerUserID = job.JobDoerUserID,
                                            DosFeePaid = job.DosFeePaid,
                                            JobPosterUserID = job.UserID
                                        }).ToList();

                    result.doerJobs = (from job in db.JobPosts
                                       join user in db.Users on job.UserID equals user.UserId
                                       join auction in db.Auctions on job.JobID equals auction.JobID
                                       join auctionbid in db.AuctionBids on auction.AuctionID equals auctionbid.AuctionID
                                       let count = db.JobPostComments.Count(x => x.JobID == job.JobID)
                                       let explanation = job.JobDescription.Substring(0, 150)
                                       where auctionbid.UserID == userid && auctionbid.AuctionBidID == auction.WinnerAuctionBidID
                                       select new JobPostViewModel
                                       {
                                           Title = job.Title,
                                           UserName = user.UserName,
                                           CreateDate = job.CreateDate,
                                           JobDescription = explanation,
                                           LastUpdate = job.LastUpdate,
                                           JobID = job.JobID,
                                           Status = job.Status,
                                           Amount = job.Amount,
                                           CommentCount = count,
                                           JobDoerUserID = job.JobDoerUserID,
                                           DosFeePaid = job.DosFeePaid,
                                           JobPosterUserID = job.UserID
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
        [Route("GetJobsByProgressTypes")]
        [HttpGet]
        public List<JobPostDto> GetJobsByProgressTypes(Helpers.Constants.Enums.JobStatusTypes? status)
        {
            List<JobPostDto> res = new List<JobPostDto>();

            using (dao_maindb_context db = new dao_maindb_context())
            {
                res = (from job in db.JobPosts
                       join user in db.Users on job.UserID equals user.UserId
                       let explanation = job.JobDescription.Substring(0, 100)
                       where job.Status == status
                       select new JobPostDto
                       {
                           JobID = job.JobID,
                           CreateDate = job.CreateDate,
                           UserID = user.UserId,
                           Title = job.Title,
                           JobDescription = explanation,
                           Amount = job.Amount,
                           DosFeePaid = job.DosFeePaid,
                           TimeFrame = job.TimeFrame,
                           LastUpdate = job.LastUpdate,
                           Status = job.Status,
                           JobDoerUserID = job.JobDoerUserID
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
        [Route("GetAuctions")]
        [HttpGet]
        public List<AuctionViewModel> GetAuctions(Helpers.Constants.Enums.AuctionStatusTypes? status)
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
                                  JobID = Convert.ToInt32(act.JobID),
                                  InternalAuctionEndDate = Convert.ToDateTime(act.InternalAuctionEndDate),
                                  PublicAuctionEndDate = Convert.ToDateTime(act.PublicAuctionEndDate),
                                  CreateDate = act.CreateDate,
                                  JobPosterUserId = Convert.ToInt32(act.JobPosterUserID),
                                  WinnerAuctionBidID = act.WinnerAuctionBidID,
                                  UserName = user.UserName,
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
                              join user in db.Users on act.UserID equals user.UserId
                              where act.AuctionID == auctionid
                              select new AuctionBidViewModel
                              {
                                  AuctionID = act.AuctionID,
                                  UserId = Convert.ToInt32(act.UserID),
                                  Price = act.Price,
                                  Time = act.Time,
                                  ReputationStake = Convert.ToDouble(act.ReputationStake),
                                  UserName = user.UserName,
                                  AuctionBidID = act.AuctionBidID
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
        [Route("GetDashBoardAdmin")]
        [HttpGet]
        public GetDashBoardViewModel GetDashBoardAdmin(int userid)
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
                        res.JobPostDtos = GetJobsByProgressTypes(Helpers.Constants.Enums.JobStatusTypes.AdminApprovalPending);

                        //Get auction model from GetAuctions function
                        res.AuctionViewModels = GetAuctions(Helpers.Constants.Enums.AuctionStatusTypes.InternalBidding);

                        //Get auction model from GetVoteJobsByStatus function
                        res.VotingViewModels = GetVotingsByStatus(Helpers.Constants.Enums.VoteStatusTypes.Active);

                        //Get model from Service_Log_Url
                        res.ApplicationLogDtos = Helpers.Serializers.DeserializeJson<List<ApplicationLogDto>>(Helpers.Request.Get(Program._settings.Service_Log_Url + "/ApplicationLog/GetLastWithCount?count=" + 20));

                        //Get model from Service_Log_Url
                        res.UserLogDtos = Helpers.Serializers.DeserializeJson<List<UserLogDto>>(Helpers.Request.Get(Program._settings.Service_Log_Url + "/UserLog/GetLastWithCount?Count=" + 20));

                        //Get users registered in the last mounth
                        var date = DateTime.Now.AddMonths(-1);
                        var date2 = DateTime.Now.AddMonths(-2);

                        var usersModel = db.Users.Where(x => x.CreateDate > date).ToList();
                        res.UserDtos = _mapper.Map<List<User>, List<UserDto>>(usersModel).ToList();

                        //Get users count                    
                        res.UserCount = db.Users.Count();

                        //Get job post count

                        res.JobCount = db.JobPosts.Count();

                        //Get auction count
                        //Get model from Voting_Engine_Url
                        res.AuctionCount = db.Auctions.Count();

                        //Get voting count
                        //Get model from Voting_Engine_Url
                        var VotingModel = Helpers.Serializers.DeserializeJson<List<VotingDto>>(Helpers.Request.Get(Program._settings.Voting_Engine_Url + "/Voting/Get?"));
                        res.VotingCount = VotingModel.Count();

                        res.UserRatio = 0;
                        res.JobRatio = 0;
                        res.AuctionRatio = 0;
                        res.VotingRatio = 0;

                        //Get user ratio from the comparison of the last two months
                        var UserPreviousCount = db.Users.Where(x => x.CreateDate > date2 && x.CreateDate < date).Count();
                        if (usersModel.Count() != 0) { res.UserRatio = ((usersModel.Count() * UserPreviousCount) / usersModel.Count()) * 100; }

                        //Get job ratio from the comparison of the last two months
                        var JobPreviousCount = db.JobPosts.Where(x => x.CreateDate > date2 && x.CreateDate < date).Count();
                        var JobCount = db.JobPosts.Where(x => x.CreateDate > date).Count();
                        if (JobCount != 0) { res.JobRatio = ((JobCount * JobPreviousCount) / JobCount) * 100; }

                        //Get auction ratio from the comparison of the last two months
                        var AuctionPreviousCount = db.Auctions.Where(x => x.CreateDate > date2 && x.CreateDate < date).Count();
                        var AuctionCount = db.Auctions.Where(x => x.CreateDate > date).Count();
                        if (AuctionCount != 0) { res.AuctionRatio = ((AuctionCount * AuctionPreviousCount) / AuctionCount) * 100; }

                        //Get voting ratio from the comparison of the last two months
                        var VotingPreviousCount = VotingModel.Where(x => x.CreateDate > date2 && x.CreateDate < date).Count();
                        var VotingCount = VotingModel.Where(x => x.CreateDate > date).Count();
                        if (VotingCount != 0) { res.VotingRatio = ((VotingCount * VotingPreviousCount) / VotingCount) * 100; }

                        DateTime CardGraphDate = DateTime.Now.AddYears(-1);
                        DateTime GraphDate = DateTime.Now.AddMonths(-6);

                        //Function that gets user records for the last 1 year month by month.
                        //User registration dates are grouped on a month by month using the group by method.
                        res.UserCardGraph = db.Users.Where(x => x.CreateDate > CardGraphDate).ToList().GroupBy(
                            User => User.CreateDate.Month,
                            User => User.CreateDate,
                                (Users, Counts) => new DashboardGraphModel
                                {
                                    Month = Users,
                                    Count = Counts.Count(),
                                }).OrderBy(x => x.Month).ToList();

                        //Function that gets job records for the last 1 year month by month.
                        //Job registration dates are grouped on a month by month using the group by method.
                        res.JobCardGraph = db.JobPosts.Where(x => x.CreateDate > CardGraphDate).ToList().GroupBy(
                            JobPost => JobPost.CreateDate.Month,
                            JobPost => JobPost.CreateDate,
                                (JobPosts, Counts) => new DashboardGraphModel
                                {
                                    Month = JobPosts,
                                    Count = Counts.Count(),
                                }).OrderBy(x => x.Month).ToList();

                        //Function that gets auction records for the last 1 year month by month.
                        //Auction registration dates are grouped on a month by month using the group by method.
                        res.AuctionCardGraph = db.Auctions.Where(x => x.CreateDate > CardGraphDate).ToList().GroupBy(
                            Auction => Auction.CreateDate.Month,
                            Auction => Auction.CreateDate,
                                (Auctions, Counts) => new DashboardGraphModel
                                {
                                    Month = Auctions,
                                    Count = Counts.Count(),
                                }).OrderBy(x => x.Month).ToList();

                        //Function that gets voting records for the last 1 year month by month.
                        //Voting registration dates are grouped on a month by month using the group by method.
                        res.VotingCardGraph = VotingModel.Where(x => x.CreateDate > CardGraphDate).ToList().GroupBy(
                            Voting => Voting.CreateDate.Month,
                            Voting => Voting.CreateDate,
                                (Voting, Counts) => new DashboardGraphModel
                                {
                                    Month = Voting,
                                    Count = Counts.Count(),
                                }).OrderBy(x => x.Month).ToList();

                        //Function that gets auction records according to internal and public for the last 6 month.
                        //Auction registration dates are grouped on a month by month using the group by method.
                        res.AuctionGraph = db.Auctions.Where(x => x.CreateDate > GraphDate).ToList().GroupBy(
                            Auction => Auction.CreateDate.Month,
                            Auction => Auction,
                            (Auctions, AuctionModel) => new AdminDashboardCardGraphModel
                            {
                                Month = Auctions,
                                Count1 = AuctionModel.Where(x => x.Status == Helpers.Constants.Enums.AuctionStatusTypes.InternalBidding).Count(),
                                Count2 = AuctionModel.Where(x => x.Status == Helpers.Constants.Enums.AuctionStatusTypes.PublicBidding).Count(),
                            }).OrderBy(x => x.Month).ToList();

                        //Function that gets voting records according to formal and informal for the last 6 month.
                        //Voting registration dates are grouped on a month by month using the group by method.
                        res.VotingGraph = VotingModel.Where(x => x.CreateDate > GraphDate).ToList().GroupBy(
                           Voting => Voting.CreateDate.Month,
                           Voting => Voting,
                           (Votings, VotingModel) => new AdminDashboardCardGraphModel
                           {
                               Month = Votings,
                               Count1 = VotingModel.Where(x => x.IsFormal == true).Count(),
                               Count2 = VotingModel.Where(x => x.IsFormal == false).Count(),
                           }).OrderBy(x => x.Month).ToList();

                    }
                    else if (userType == "VotingAssociate")
                    {
                        //Get job post model from GetVoteJobsByProgressTypes function
                        res.JobPostDtos = GetJobsByProgressTypes(Helpers.Constants.Enums.JobStatusTypes.InternalAuction);

                        //Get auction model from GetVoteJobsByStatus function
                        res.VotingViewModels = GetVotingsByStatus(Helpers.Constants.Enums.VoteStatusTypes.Active);
                    }
                    else
                    {
                        //Get job post model from GetVoteJobsByProgressTypes function
                        res.JobPostDtos = GetJobsByProgressTypes(Helpers.Constants.Enums.JobStatusTypes.PublicAuction);

                        //Get auction model from GetVoteJobsByStatus function
                        res.VotingViewModels = GetVotingsByStatus(Helpers.Constants.Enums.VoteStatusTypes.Active);
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
        [Route("GetVotingDetail")]
        [HttpGet]
        public List<VoteDto> GetVotingDetail(int votingid)
        {
            List<VoteDto> res = new List<VoteDto>();
            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    string voteJson = Helpers.Request.Get(Program._settings.Voting_Engine_Url + "/Vote/GetAllVotesByVotingId?votingid=" + votingid);
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
        [Route("GetVotingsByStatus")]
        [HttpGet]
        public List<VotingViewModel> GetVotingsByStatus(Helpers.Constants.Enums.VoteStatusTypes? status)
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
                           where status == null || votejob.Status == status
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

    }
}

