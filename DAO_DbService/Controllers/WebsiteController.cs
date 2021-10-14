using DAO_DbService.Contexts;
using Helpers.Models.WebsiteViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAO_DbService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class WebsiteController : Controller
    {
        [Route("GetAllJobs")]
        [HttpGet]
        public List<JobPostWebsiteModel> GetAllJobs()
        {
            List<JobPostWebsiteModel> result = new List<JobPostWebsiteModel>();
            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    result = (from job in db.JobPosts
                              join user in db.Users on job.UserID equals user.UserId
                              let explenation = job.JobDescription.Substring(0, 100)
                              let count = db.JobPostComments.Count(x => x.JobID == job.JobID)
                              where job.Status == Helpers.Constants.Enums.JobStatusTypes.Active
                              select new JobPostWebsiteModel
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
                    result.JobPostWebsiteModel = new JobPostWebsiteModel
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
                    result.JobPostCommentMode = GetJobComment(jobid);
                }

            }
            catch (Exception ex)
            {

            }
            return result;
        }
    }
}
