using DAO_DbService.Contexts;
using DAO_DbService.Models;
using Helpers.Constants;
using Helpers.Models.DtoModels.LogDbDto;
using Helpers.Models.DtoModels.MainDbDto;
using Helpers.Models.DtoModels.VoteDbDto;
using Helpers.Models.DtoModels.ReputationDbDto;
using Helpers.Models.SharedModels;
using Helpers.Models.WebsiteViewModels;
using Microsoft.AspNetCore.Mvc;
using PagedList.Core;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public PaginationEntity<JobPostViewModel> GetAllJobs(Helpers.Constants.Enums.JobStatusTypes? status, string query, int userid = 0, int page = 1, int pageCount = 30)
        {
            PaginationEntity<JobPostViewModel> res = new PaginationEntity<JobPostViewModel>();

            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    IPagedList<JobPostViewModel> lst = (from job in db.JobPosts
                                                        join user in db.Users on job.UserID equals user.UserId
                                                        let explanation = job.JobDescription.Substring(0, 250)
                                                        let count = db.JobPostComments.Count(x => x.JobID == job.JobID)
                                                        let flagcount = db.JobPostComments.Count(x => x.JobID == job.JobID && x.IsFlagged == true)
                                                        let userflagged = db.JobPostComments.Count(x => x.JobID == job.JobID && x.UserID == userid && x.IsFlagged == true) > 0
                                                        where (status == null || job.Status == status) &&
                                                        (query == null || job.Title.Contains(query))
                                                        orderby job.CreateDate descending
                                                        select new JobPostViewModel
                                                        {
                                                            Title = job.Title,
                                                            JobPosterUserName = user.UserName,
                                                            CreateDate = job.CreateDate,
                                                            JobDescription = explanation,
                                                            LastUpdate = job.LastUpdate,
                                                            JobID = job.JobID,
                                                            Status = job.Status,
                                                            Amount = job.Amount,
                                                            CommentCount = count,
                                                            JobDoerUserID = job.JobDoerUserID,
                                                            DosFeePaid = job.DosFeePaid,
                                                            JobPosterUserID = job.UserID,
                                                            CodeUrl = job.CodeUrl,
                                                            Tags = job.Tags,
                                                            IsUserFlagged = userflagged,
                                                            FlagCount = flagcount,
                                                            TimeFrame = job.TimeFrame
                                                        }).ToPagedList(page, pageCount);

                    //Match auctions and bids with jobs
                    List<int?> jobIds = lst.Select(x => x.JobID).ToList().ConvertAll<int?>(i => i);
                    var auctions = _mapper.Map<List<Auction>, List<AuctionDto>>(db.Auctions.Where(x => jobIds.Contains(x.JobID)).ToList());

                    foreach (var job in lst)
                    {
                        job.Auction = auctions.SingleOrDefault(x => x.JobID == job.JobID);
                        if (job.Auction != null)
                            job.AuctionBids = GetAuctionBids(job.Auction.AuctionID);
                    }

                    res.Items = lst;
                    res.MetaData = new PaginationMetaData() { Count = lst.Count, FirstItemOnPage = lst.FirstItemOnPage, HasNextPage = lst.HasNextPage, HasPreviousPage = lst.HasPreviousPage, IsFirstPage = lst.IsFirstPage, IsLastPage = lst.IsLastPage, LastItemOnPage = lst.LastItemOnPage, PageCount = lst.PageCount, PageNumber = lst.PageNumber, PageSize = lst.PageSize, TotalItemCount = lst.TotalItemCount };
                }
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, Enums.LogTypes.ApplicationError, true);
            }

            return res;
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
                                  UserID = user.UserId,
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
                                  IsPinned = comment.IsPinned,
                                  IsFlagged = comment.IsFlagged
                              }).ToList();

                    var commentIds = result.Select(x => x.JobPostCommentID).ToList();

                    var commentVotesOfUser = db.UserCommentVotes.Where(x => x.UserId == userid && commentIds.Contains(x.JobPostCommentID));

                    foreach (var commentVote in commentVotesOfUser)
                    {
                        var comment = result.First(x => x.JobPostCommentID == commentVote.JobPostCommentID);
                        comment.IsUpVote = commentVote.IsUpVote;
                    }

                    //Get users reputation to attach comment body
                    var userIds = result.Select(x => x.UserID).Distinct().ToList();
                    var userRepsJson = Helpers.Request.Post(Program._settings.Service_Reputation_Url + "/UserReputationHistory/GetLastReputationByUserIds", Helpers.Serializers.SerializeJson(userIds));
                    var userReps = Helpers.Serializers.DeserializeJson<List<Helpers.Models.DtoModels.ReputationDbDto.UserReputationHistoryDto>>(userRepsJson);
                    foreach (var reputation in userReps)
                    {
                        foreach (var comment in result.Where(x => x.UserID == reputation.UserID))
                        {
                            comment.UserReputation = reputation.LastTotal;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, Enums.LogTypes.ApplicationError, true);
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
        public JobPostViewModel GetJobDetail(int jobid, int userid = 0)
        {
            JobPostViewModel result = new JobPostViewModel();
            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    var jobPost = db.JobPosts.Find(jobid);
                    var user = db.Users.Find(jobPost.UserID);
                    var count = db.JobPostComments.Count(x => x.JobID == jobPost.JobID);
                    var flagcount = db.JobPostComments.Count(x => x.JobID == jobid && x.IsFlagged == true);
                    var userflagged = db.JobPostComments.Count(x => x.JobID == jobid && x.UserID == userid && x.IsFlagged == true) > 0;
                    var jobDoerUsername = "";
                    if (jobPost.JobDoerUserID != null && jobPost.JobDoerUserID > 0)
                    {
                        jobDoerUsername = db.Users.Find(jobPost.JobDoerUserID).UserName;
                    }

                    result = new JobPostViewModel
                    {
                        Title = jobPost.Title,
                        JobPosterUserName = user.UserName,
                        CreateDate = jobPost.CreateDate,
                        JobDescription = jobPost.JobDescription,
                        LastUpdate = jobPost.LastUpdate,
                        JobID = jobPost.JobID,
                        Status = jobPost.Status,
                        Amount = jobPost.Amount,
                        CommentCount = count,
                        JobDoerUserID = jobPost.JobDoerUserID,
                        DosFeePaid = jobPost.DosFeePaid,
                        JobPosterUserID = jobPost.UserID,
                        Tags = jobPost.Tags,
                        CodeUrl = jobPost.CodeUrl,
                        IsUserFlagged = userflagged,
                        FlagCount = flagcount,
                        TimeFrame = jobPost.TimeFrame,
                        JobDoerUsername = jobDoerUsername
                    };

                    result.Auction = _mapper.Map<Auction, AuctionDto>(db.Auctions.SingleOrDefault(x => x.JobID == result.JobID));
                    if (result.Auction != null)
                        result.AuctionBids = GetAuctionBids(result.Auction.AuctionID);
                }
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, Enums.LogTypes.ApplicationError, true);
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
        public MyJobsViewModel GetUserJobs(int userid, string query, Helpers.Constants.Enums.JobStatusTypes? status)
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
                                        let explanation = job.JobDescription.Substring(0, 250)
                                        let flagcount = db.JobPostComments.Count(x => x.JobID == job.JobID && x.IsFlagged == true)
                                        let userflagged = db.JobPostComments.Count(x => x.JobID == job.JobID && x.UserID == userid && x.IsFlagged == true) > 0
                                        where job.UserID == userid && 
                                        (status == null || job.Status == status) &&
                                        (query == null || job.Title.Contains(query))
                                        orderby job.CreateDate descending
                                        select new JobPostViewModel
                                        {
                                            Title = job.Title,
                                            JobPosterUserName = user.UserName,
                                            CreateDate = job.CreateDate,
                                            JobDescription = explanation,
                                            LastUpdate = job.LastUpdate,
                                            JobID = job.JobID,
                                            Status = job.Status,
                                            Amount = job.Amount,
                                            CommentCount = count,
                                            JobDoerUserID = job.JobDoerUserID,
                                            DosFeePaid = job.DosFeePaid,
                                            JobPosterUserID = job.UserID,
                                            CodeUrl = job.CodeUrl,
                                            Tags = job.Tags,
                                            IsUserFlagged = userflagged,
                                            FlagCount = flagcount,
                                            TimeFrame = job.TimeFrame
                                        }).ToList();

                    result.doerJobs = (from job in db.JobPosts
                                       join user in db.Users on job.UserID equals user.UserId
                                       join auction in db.Auctions on job.JobID equals auction.JobID
                                       join auctionbid in db.AuctionBids on auction.AuctionID equals auctionbid.AuctionID
                                       let count = db.JobPostComments.Count(x => x.JobID == job.JobID)
                                       let explanation = job.JobDescription.Substring(0, 250)
                                       let flagcount = db.JobPostComments.Count(x => x.JobID == job.JobID && x.IsFlagged == true)
                                       let userflagged = db.JobPostComments.Count(x => x.JobID == job.JobID && x.UserID == userid && x.IsFlagged == true) > 0
                                       where auctionbid.UserID == userid && 
                                       auctionbid.AuctionBidID == auction.WinnerAuctionBidID && 
                                       (status == null || job.Status == status)  &&
                                       (query == null || job.Title.Contains(query))
                                       select new JobPostViewModel
                                       {
                                           Title = job.Title,
                                           JobPosterUserName = user.UserName,
                                           CreateDate = job.CreateDate,
                                           JobDescription = explanation,
                                           LastUpdate = job.LastUpdate,
                                           JobID = job.JobID,
                                           Status = job.Status,
                                           Amount = job.Amount,
                                           CommentCount = count,
                                           JobDoerUserID = job.JobDoerUserID,
                                           DosFeePaid = job.DosFeePaid,
                                           JobPosterUserID = job.UserID,
                                           CodeUrl = job.CodeUrl,
                                           Tags = job.Tags,
                                           IsUserFlagged = userflagged,
                                           FlagCount = flagcount,
                                           TimeFrame = job.TimeFrame
                                       }).ToList();

                    var allJobs = new List<JobPostViewModel>();
                    allJobs.AddRange(result.ownedJobs);
                    allJobs.AddRange(result.doerJobs);

                    //Match auctions and bids with jobs
                    List<int?> jobIds = allJobs.Select(x => x.JobID).ToList().ConvertAll<int?>(i => i);
                    var auctions = _mapper.Map<List<Auction>, List<AuctionDto>>(db.Auctions.Where(x => jobIds.Contains(x.JobID)).ToList());
                    var auctionIds = auctions.Select(x => x.AuctionID).ToList();

                    foreach (var job in result.ownedJobs)
                    {
                        job.Auction = auctions.SingleOrDefault(x => x.JobID == job.JobID);
                        if (job.Auction != null)
                            job.AuctionBids = GetAuctionBids(job.Auction.AuctionID);
                    }

                    foreach (var job in result.doerJobs)
                    {
                        job.Auction = auctions.SingleOrDefault(x => x.JobID == job.JobID);
                        if (job.Auction != null)
                            job.AuctionBids = GetAuctionBids(job.Auction.AuctionID);
                    }
                }
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, Enums.LogTypes.ApplicationError, true);
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
                       let explanation = job.JobDescription.Substring(0, 250)
                       where job.Status == status
                       orderby job.CreateDate descending
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
                           JobDoerUserID = job.JobDoerUserID,
                           Tags = job.Tags,
                           CodeUrl = job.CodeUrl
                       }).ToList();
            }
            return res;
        }

        /// <summary>
        /// Remove flagged comment if exists
        /// </summary>
        /// <param name="jobid"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        [Route("RemoveFlag")]
        [HttpGet]
        public SimpleResponse RemoveFlag(int jobid, int userid)
        {

            SimpleResponse res = new SimpleResponse();

            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    //Get flagged comments of the user for the job
                    var flaggedComments = db.JobPostComments.Where(x => x.JobID == jobid && x.IsFlagged == true && x.UserID == userid).ToList();

                    foreach (var comment in flaggedComments)
                    {
                        var cmt = db.JobPostComments.Find(comment.JobPostCommentID);
                        cmt.IsFlagged = false;
                        cmt.Comment = "This comment is deleted by the owner.";
                        db.SaveChanges();
                    }

                    res.Success = true;
                    res.Message = "Flag removed succesfully.";
                }
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, Enums.LogTypes.ApplicationError, true);
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
        public List<AuctionViewModel> GetAuctions(Helpers.Constants.Enums.AuctionStatusTypes? status, string query)
        {
            List<AuctionViewModel> result = new List<AuctionViewModel>();
            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    result = (from act in db.Auctions
                              join job in db.JobPosts on act.JobID equals job.JobID
                              join userPoster in db.Users on job.UserID equals userPoster.UserId
                              join bid in db.AuctionBids on act.WinnerAuctionBidID equals bid.AuctionBidID into ps
                              from bidRes in ps.DefaultIfEmpty()
                              join user in db.Users on bidRes.UserID equals user.UserId into ps2
                              from userRes in ps2.DefaultIfEmpty()
                              let bidcount = db.AuctionBids.Count(x => x.AuctionID == act.AuctionID)
                              where (status == null || act.Status == status) &&
                              (query == null || job.Title.Contains(query))
                              orderby act.CreateDate descending
                              select new AuctionViewModel
                              {
                                  JobID = Convert.ToInt32(act.JobID),
                                  InternalAuctionEndDate = Convert.ToDateTime(act.InternalAuctionEndDate),
                                  PublicAuctionEndDate = Convert.ToDateTime(act.PublicAuctionEndDate),
                                  CreateDate = act.CreateDate,
                                  JobPosterUserId = Convert.ToInt32(act.JobPosterUserID),
                                  WinnerAuctionBidID = act.WinnerAuctionBidID,
                                  WinnerUserName = userRes.UserName,
                                  Status = act.Status,
                                  AuctionID = act.AuctionID,
                                  Title = job.Title,
                                  BidCount = bidcount,
                                  JobPrice = job.Amount,
                                  JobPosterUsername = userPoster.UserName,
                                  ExpectedTimeframe = job.TimeFrame
                              }).Take(100).ToList();
                }
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, Enums.LogTypes.ApplicationError, true);
            }
            return result;
        }

        /// <summary>
        /// Get user's bids
        /// </summary>
        /// <returns></returns>
        [Route("GetMyBids")]
        [HttpGet]
        public List<MyBidsViewModel> GetMyBids(int userid)
        {
            List<MyBidsViewModel> result = new List<MyBidsViewModel>();
            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    result = (from actbid in db.AuctionBids
                              join act in db.Auctions on actbid.AuctionID equals act.AuctionID
                              join job in db.JobPosts on act.JobID equals job.JobID
                              where actbid.UserID == userid
                              orderby actbid.CreateDate descending
                              select new MyBidsViewModel
                              {
                                  JobID = Convert.ToInt32(act.JobID),
                                  CreateDate = act.CreateDate,
                                  WinnerAuctionBidID = act.WinnerAuctionBidID,
                                  AuctionID = act.AuctionID,
                                  Time = actbid.Time,
                                  AuctionBidID = actbid.AuctionBidID,
                                  AssociateUserNote = actbid.AssociateUserNote,
                                  Price = actbid.Price,
                                  ReputationStake = actbid.ReputationStake,
                                  Status = act.Status,
                                  JobName = job.Title
                              }).Take(100).ToList();
                }
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, Enums.LogTypes.ApplicationError, true);
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
        public List<AuctionBidItemModel> GetAuctionBids(int auctionid)
        {
            List<AuctionBidItemModel> result = new List<AuctionBidItemModel>();
            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    result = (from actbid in db.AuctionBids
                              join user in db.Users on actbid.UserID equals user.UserId
                              where actbid.AuctionID == auctionid
                              select new AuctionBidItemModel
                              {
                                  AuctionID = actbid.AuctionID,
                                  UserId = Convert.ToInt32(actbid.UserID),
                                  Price = actbid.Price,
                                  Time = actbid.Time,
                                  ReputationStake = Convert.ToDouble(actbid.ReputationStake),
                                  UserName = user.UserName,
                                  AuctionBidID = actbid.AuctionBidID,
                                  NameSurname = user.NameSurname,
                                  UserNote = actbid.AssociateUserNote,
                                  UserType = user.UserType
                              }).ToList();

                    var reputationsTotalJson = Helpers.Request.Post(Program._settings.Service_Reputation_Url + "/UserReputationHistory/GetLastReputationByUserIds", Helpers.Serializers.SerializeJson(result.Select(x => x.UserId)));
                    var reputationsTotal = Helpers.Serializers.DeserializeJson<List<UserReputationHistoryDto>>(reputationsTotalJson);

                    foreach (var bid in result)
                    {
                        if (reputationsTotal.Count(x => x.UserID == bid.UserId) > 0)
                        {
                            bid.UsersTotalReputation = reputationsTotal.First(x => x.UserID == bid.UserId).LastTotal;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, Enums.LogTypes.ApplicationError, true);
            }
            return result;
        }

        /// <summary>
        /// Get user's bids for active auctions
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        [Route("GetUserBids")]
        [HttpGet]
        public List<AuctionBidDto> GetUserBids(int userid)
        {
            List<AuctionBidDto> result = new List<AuctionBidDto>();
            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    result = (from actbid in db.AuctionBids
                              join auction in db.Auctions on actbid.AuctionID equals auction.AuctionID
                              where actbid.UserID == userid && (auction.Status == Helpers.Constants.Enums.AuctionStatusTypes.PublicBidding || auction.Status == Helpers.Constants.Enums.AuctionStatusTypes.InternalBidding)
                              select new AuctionBidDto
                              {
                                  UserId = actbid.UserID,
                                  AuctionBidID = actbid.AuctionBidID,
                                  AuctionID = auction.AuctionID,
                                  CreateDate = actbid.CreateDate,
                                  Price = actbid.Price,
                                  ReputationStake = actbid.ReputationStake,
                                  Time = actbid.Time,
                                  AssociateUserNote = actbid.AssociateUserNote
                              }).ToList();
                }
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, Enums.LogTypes.ApplicationError, true);
            }
            return result;
        }

        #endregion

        #region Dashboard

        /// <summary>
        /// Get dashboard for admin 
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        [Route("GetDashBoardAdmin")]
        [HttpGet]
        public DashBoardViewModelAdmin GetDashBoardAdmin(int userid)
        {
            DashBoardViewModelAdmin res = new DashBoardViewModelAdmin();
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
                            (Auctions, AuctionModel) => new AdminDashboardCardModel
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
                           (Votings, VotingModel) => new AdminDashboardCardModel
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
                Program.monitizer.AddException(ex, Enums.LogTypes.ApplicationError, true);
            }
            return res;
        }


        /// <summary>
        /// Get dashboard for voting associate
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        [Route("GetDashBoardVA")]
        [HttpGet]
        public DashBoardViewModelVA GetDashBoardVA(int userid)
        {
            DashBoardViewModelVA res = new DashBoardViewModelVA();
            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {

                    //Get job post model from GetVoteJobsByProgressTypes function
                    res.MyDoerJobs = (from job in db.JobPosts
                                      join Auction in db.Auctions on job.JobID equals Auction.JobID
                                      join AuctionBid in db.AuctionBids on Auction.AuctionID equals AuctionBid.AuctionID
                                      join User in db.Users on job.UserID equals User.UserId
                                      where job.JobDoerUserID == userid && AuctionBid.UserID == userid
                                      orderby job.CreateDate descending
                                      select new DashboardJobCardModel
                                      {
                                          JobID = job.JobID,
                                          CreateDate = job.CreateDate,
                                          UserName = User.UserName,
                                          Title = job.Title,
                                          Amount = AuctionBid.Price,
                                          Status = job.Status,
                                          EndDate = Auction.PublicAuctionEndDate.Value.AddDays(Convert.ToInt32(AuctionBid.Time))
                                      }).ToList();


                    res.MyOwnerJobs = (from job in db.JobPosts
                                       join Auction in db.Auctions on job.JobID equals Auction.JobID
                                       join AuctionBid in db.AuctionBids on Auction.AuctionID equals AuctionBid.AuctionID
                                       join User in db.Users on job.UserID equals User.UserId
                                       where job.UserID == userid && AuctionBid.UserID == userid
                                       orderby job.CreateDate descending
                                       select new DashboardJobCardModel
                                       {
                                           JobID = job.JobID,
                                           CreateDate = job.CreateDate,
                                           UserName = User.UserName,
                                           Title = job.Title,
                                           Amount = AuctionBid.Price,
                                           Status = job.Status,
                                           EndDate = Auction.PublicAuctionEndDate.Value.AddDays(Convert.ToInt32(AuctionBid.Time))
                                       }).ToList();

                    //Get Last 10 Comments
                    res.LastComments = db.JobPostComments.OrderByDescending(x => x.JobPostCommentID).Take(10)
                        .Join(db.Users,
                            c => c.UserID,
                            cm => cm.UserId,
                            (c, cm) => new LastCommentsDto
                            {
                                UserName = cm.UserName,
                                ProfileImage = cm.ProfileImage,
                                Comment = c.Comment,
                                Date = c.Date,
                                JobID = c.JobID
                            }
                        )
                        .ToList();

                    var date = DateTime.Now.AddMonths(-1);
                    var date2 = DateTime.Now.AddMonths(-2);

                    //Get Trend Jobs
                    res.PopularJobs = db.JobPostComments.Where(x => x.Date > date)
                                         .GroupBy(x => x.JobID)
                                         .Select(g => new { name = g.Key, count = g.Count() })
                                         .OrderByDescending(g => g.count)
                                         .Take(5)
                                         .Join(db.JobPosts,
                                             c => c.name,
                                             cm => cm.JobID,
                                             (c, cm) => new
                                             {
                                                 Title = cm.Title,
                                                 JobDescription = cm.JobDescription,
                                                 JobID = cm.JobID,
                                                 Status = cm.Status,
                                                 userID = cm.UserID,
                                                 CreateDate = cm.CreateDate
                                             }
                                         )
                                         .Join(db.Users,
                                         cx => cx.userID,
                                         cmx => cmx.UserId,
                                         (cx, cmx) => new PopularJobsDto
                                         {
                                             ProfileImage = cmx.ProfileImage,
                                             UserName = cmx.UserName,
                                             JobDescription = cx.JobDescription,
                                             Status = cx.Status,
                                             Title = cx.Title,
                                             CreateDate = cx.CreateDate,
                                             JobID = cx.JobID,
                                         }
                                         )
                                         .ToList();

                    //Get job post count
                    res.MyJobCount = db.JobPosts.Count(x => x.UserID == userid || x.JobDoerUserID == userid);

                    //Get auction count
                    //Get model from Voting_Engine_Url
                    res.MyAuctionCount = db.Auctions.Count(x => x.JobPosterUserID == userid);

                    //Get voting count
                    //Get model from Voting_Engine_Url
                    var VotingModel = Helpers.Serializers.DeserializeJson<List<VoteDto>>(Helpers.Request.Get(Program._settings.Voting_Engine_Url + "/Vote/GetAllVotesByUserId?userid="+userid));

                    res.MyVotesCount = VotingModel.Count();

                    //Get job ratio from the comparison of the last two months
                    var JobPreviousCount = db.JobPosts.Count(x => x.CreateDate > date2 && x.CreateDate < date && x.UserID == userid);
                    var JobCount = db.JobPosts.Count(x => x.CreateDate > date && x.UserID == userid);


                    //Get auction ratio from the comparison of the last two months
                    var AuctionPreviousCount = db.Auctions.Count(x => x.CreateDate > date2 && x.CreateDate < date && x.JobPosterUserID == userid);
                    var AuctionCount = db.Auctions.Count(x => x.CreateDate > date && x.JobPosterUserID == userid);

                }
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, Enums.LogTypes.ApplicationError, true);
            }
            return res;
        }


        /// <summary>
        /// Get dashboard for associate
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        [Route("GetDashBoardAssociate")]
        [HttpGet]
        public DashBoardViewModelVA GetDashBoardAssociate(int userid)
        {
            DashBoardViewModelVA res = new DashBoardViewModelVA();
            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    //Get job post model from GetVoteJobsByProgressTypes function

                    res.MyDoerJobs = (from job in db.JobPosts
                                      join Auction in db.Auctions on job.JobID equals Auction.JobID
                                      join AuctionBid in db.AuctionBids on Auction.AuctionID equals AuctionBid.AuctionID
                                      join User in db.Users on job.UserID equals User.UserId
                                      where job.JobDoerUserID == userid && AuctionBid.UserID == userid
                                      orderby job.CreateDate descending
                                      select new DashboardJobCardModel
                                      {
                                          JobID = job.JobID,
                                          CreateDate = job.CreateDate,
                                          UserName = User.UserName,
                                          Title = job.Title,
                                          Amount = AuctionBid.Price,
                                          Status = job.Status,
                                          EndDate = Auction.PublicAuctionEndDate.Value.AddDays(Convert.ToInt32(AuctionBid.Time))
                                      }).ToList();


                    res.MyOwnerJobs = (from job in db.JobPosts
                                       join Auction in db.Auctions on job.JobID equals Auction.JobID
                                       join AuctionBid in db.AuctionBids on Auction.AuctionID equals AuctionBid.AuctionID
                                       join User in db.Users on job.UserID equals User.UserId
                                       where job.UserID == userid && AuctionBid.UserID == userid
                                       orderby job.CreateDate descending
                                       select new DashboardJobCardModel
                                       {
                                           JobID = job.JobID,
                                           CreateDate = job.CreateDate,
                                           UserName = User.UserName,
                                           Title = job.Title,
                                           Amount = AuctionBid.Price,
                                           Status = job.Status,
                                           EndDate = Auction.PublicAuctionEndDate.Value.AddDays(Convert.ToInt32(AuctionBid.Time))
                                       }).ToList();

                    //Get Last 10 Comments
                    res.LastComments = db.JobPostComments.OrderByDescending(x => x.JobPostCommentID).Take(10)
                       .Join(db.Users,
                           c => c.UserID,
                           cm => cm.UserId,
                           (c, cm) => new LastCommentsDto
                           {
                               UserName = cm.UserName,
                               ProfileImage = cm.ProfileImage,
                               Comment = c.Comment,
                               Date = c.Date,
                               JobID = c.JobID
                           }
                       )
                       .ToList();

                    //Get users registered in the last mounth
                    var date = DateTime.Now.AddMonths(-1);
                    var date2 = DateTime.Now.AddMonths(-2);

                    //Get Trend Jobs
                    res.PopularJobs = db.JobPostComments.Where(x => x.Date > date)
                        .GroupBy(x => x.JobID)
                        .Select(g => new { name = g.Key, count = g.Count() })
                        .OrderByDescending(g => g.count)
                        .Take(5)
                        .Join(db.JobPosts,
                            c => c.name,
                            cm => cm.JobID,
                            (c, cm) => new
                            {
                                Title = cm.Title,
                                JobDescription = cm.JobDescription,
                                JobID = cm.JobID,
                                Status = cm.Status,
                                userID = cm.UserID,
                                CreateDate = cm.CreateDate
                            }
                        )
                        .Join(db.Users,
                        cx => cx.userID,
                        cmx => cmx.UserId,
                        (cx, cmx) => new PopularJobsDto
                        {
                            ProfileImage = cmx.ProfileImage,
                            UserName = cmx.UserName,
                            JobDescription = cx.JobDescription,
                            Status = cx.Status,
                            Title = cx.Title,
                            CreateDate = cx.CreateDate,
                            JobID = cx.JobID,
                        }
                        )
                        .ToList();

                    //Get job post count
                    res.MyJobCount = db.JobPosts.Count(x => x.UserID == userid || x.JobDoerUserID == userid);

                    //Get auction count
                    //Get model from Voting_Engine_Url
                    res.MyAuctionCount = db.Auctions.Count(x => x.JobPosterUserID == userid);

                    //Get voting count
                    //Get model from Voting_Engine_Url
                    var VotingModel = Helpers.Serializers.DeserializeJson<List<VoteDto>>(Helpers.Request.Get(Program._settings.Voting_Engine_Url + "/Vote/GetAllVotesByUserId?userid="+userid));
                    res.MyVotesCount = VotingModel.Count();

                    //Get job ratio from the comparison of the last two months                   
                    var JobCount = db.JobPosts.Count(x => x.CreateDate > date && x.UserID == userid);


                    //Get auction ratio from the comparison of the last two months                    
                    var AuctionCount = db.Auctions.Count(x => x.CreateDate > date && x.JobPosterUserID == userid);
                }
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, Enums.LogTypes.ApplicationError, true);
            }
            return res;
        }

        #endregion

        #region Vote

        /// <summary>
        /// Get votings by status
        /// </summary>
        /// <param name="status">Voting status enum</param>
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
                    string votingsJson = Helpers.Request.Get(Program._settings.Voting_Engine_Url + "/Voting/GetVotingByStatus?status=" + status);
                    List<VotingDto> model = Helpers.Serializers.DeserializeJson<List<VotingDto>>(votingsJson);

                    res = (from voting in model
                           join job in db.JobPosts on voting.JobID equals job.JobID

                           join usr in db.Users on job.JobDoerUserID equals usr.UserId into user
                           from pUser in user.DefaultIfEmpty(new User() { UserId = 0, UserName = "" })
                           join act in db.Auctions on job.JobID equals act.JobID into auction
                           from pAuction in auction.DefaultIfEmpty(new Auction() { AuctionID = 0 })
                           join actbid in db.AuctionBids on pAuction.AuctionID equals actbid.AuctionID into auctionbid
                           from pAuctionBid in auctionbid.DefaultIfEmpty(new AuctionBid() { AuctionBidID = 0, Price = 0 })

                           where (status == null || voting.Status == status) && (pUser == null || pAuctionBid.UserID == pUser.UserId)
                           orderby voting.CreateDate descending
                           select new VotingViewModel
                           {
                               JobID = job.JobID,
                               VotingID = voting.VotingID,
                               IsFormal = voting.IsFormal,
                               CreateDate = voting.CreateDate,
                               StartDate = voting.StartDate,
                               EndDate = voting.EndDate,
                               Title = job.Title,
                               Status = voting.Status,
                               StakedAgainst = voting.StakedAgainst,
                               StakedFor = voting.StakedFor,
                               VoteCount = voting.VoteCount,
                               QuorumCount = voting.QuorumCount,
                               JobDoerUserID = job.JobDoerUserID,
                               JobOwnerUserID = job.UserID,
                               JobDoerUsername = pUser.UserName,
                               WinnerBidPrice = pAuctionBid.Price,
                               EligibleUserCount = voting.EligibleUserCount
                           }).ToList();
                }
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, Enums.LogTypes.ApplicationError, true);
            }
            return res;
        }

        #endregion

        #region Payment History

        /// <summary>
        /// Get user payment history
        /// </summary>
        /// <param name="userid"></param>
        /// <returns>List<PaymentHistoryViewModel></returns>
        [Route("PaymentHistoryByUserId")]
        [HttpGet]
        public PaymentHistoryViewModel PaymentHistoryByUserId(int userid)
        {
            PaymentHistoryViewModel result = new PaymentHistoryViewModel();
            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    result.UserPaymentHistoryList = (from payment in db.PaymentHistories
                                                     join job in db.JobPosts on payment.JobID equals job.JobID
                                                     where payment.UserID == userid
                                                     select new UserPaymentHistoryItem
                                                     {
                                                         Title = job.Title,
                                                         IBAN = payment.IBAN,
                                                         JobID = payment.JobID,
                                                         JobAmount = job.Amount,
                                                         PaymentAmount = payment.Amount,
                                                         WalletAddress = payment.WalletAddress,
                                                         CreateDate = payment.CreateDate,
                                                         Explanation = payment.Explanation
                                                     }).ToList();

                    result.TotalAmount = result.UserPaymentHistoryList.Sum(x => x.PaymentAmount);

                    //Get last months payments
                    result.LastMonthAmount = 0;
                    DateTime lastMonth = DateTime.Now.AddMonths(-1);
                    if (result.UserPaymentHistoryList.Count(x => x.CreateDate >= lastMonth) > 0)
                    {
                        result.LastMonthAmount = result.UserPaymentHistoryList.Where(x => x.CreateDate >= lastMonth).Sum(x => x.PaymentAmount);
                    }
                }
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, Enums.LogTypes.ApplicationError, true);
            }
            return result;
        }

        #endregion


    }
}

