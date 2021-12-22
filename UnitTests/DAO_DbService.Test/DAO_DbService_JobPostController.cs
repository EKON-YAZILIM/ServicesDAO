using Xunit;
using FluentAssertions;
using Helpers.Models.DtoModels.MainDbDto;
using System;
using DAO_DbService.Models;
using System.Collections.Generic;
using System.Linq;
using PagedList.Core;
using Microsoft.EntityFrameworkCore;
using DAO_DbService.Tests.Util;

namespace DAO_DbService.Tests
{
    ///<summary>
    /// Tests of DAO_DbService.JobPostController class methods.
    /// DAO_DbService microservice is only responsible for database CRUD operations.
    /// Methods:
    /// IEnumerable<JobPostDto> Get() +
    /// IEnumerable<JobPostDto> JobPostSearch(string query) +
    /// JobPostDto GetId(int id) +
    /// public JobPostDto Post([FromBody] JobPostDto model) +
    /// JobPostDto Update([FromBody] JobPostDto model) +
    /// JobPostDto ChangeJobStatus(int jobid, JobStatusTypes status)

    ///</summary>
    [Collection("Sequential")]
    public class DAO_DbService_JobPostController_Test
    {
        PostTestController controllers = new PostTestController();

        // IEnumerable<JobPostDto> Get()
        // IEnumerable<JobPostDto> JobPostSearch(string query)
        // JobPostDto GetId(int id)
        // public JobPostDto Post([FromBody] JobPostDto model)
        // public List<JobPostDto> PostMultiple([FromBody] List<JobPostDto> model)
        // bool Delete(int? ID)
        // JobPostDto Update([FromBody] JobPostDto model)
        // JobPostDto ChangeJobStatus(int jobid, JobStatusTypes status)
        // List<JobPostDto> GetByUserId(int userid)
        [Fact]
        public void JobPostController_Get_Test()
        {
            //Arrange
            TestDbInitializer.ClearDatabase();
            var jobs = TestDbInitializer.SeedJobs();
            //Act
            var getJobs = controllers.jobPostController.Get();
            //Assert
            // (getJobs.Count()).Should().HaveCount(jobs.Count());
            getJobs.Count().Should().Be(jobs.Count());
        }

        /// Test of DAO_DbService.JobPostController.jobPostSearch(query)
        /// Tested for 2 use cases :: searching for one existing and one non-existing user        
        [Fact]
        public void JobPostController_JobPostSearch()
        {
            var jobs = TestDbInitializer.SeedJobs();
            //Arrange
            string searchkey = "#6";
            //Act
            IEnumerable<JobPostDto> job = controllers.jobPostController.JobPostSearch(searchkey);
            //Assert
            (job.FirstOrDefault().Title.Contains(searchkey) || job.FirstOrDefault().JobDescription.Contains(searchkey)).Should().Be(true);

            //Arrange
            string wrongSearchKey = "wrongkey";
            //Act
            IEnumerable<JobPostDto> non_existant_job = controllers.jobPostController.JobPostSearch(wrongSearchKey);
            //Assert
            non_existant_job.Should().HaveCount(0);
        }

        /// Test of DAO_DbService.jobPostController.GetId(id)
        /// Finds a job by its id
        [Fact]
        public void JobPostController_JobPostSearch_GetId_Test()
        {
            //Arrange
            var jobs_list = TestDbInitializer.SeedJobs();
            Random random = new Random();
            int selected_Job = random.Next(1, jobs_list.Count);
            JobPost job = jobs_list.ElementAt(selected_Job);
            //Act
            JobPostDto job_called_by_id = controllers.jobPostController.GetId(job.JobID);
            //Assert
            job_called_by_id.JobID.Should().Be(job.JobID);
        }

        /// Test of DAO_DbService.jobPostController.Post(UserDto)
        /// Adds job to database
        [Fact]
        public void JobPostController_Post_Test()
        {
            // Arrange
            // Creates a jobpost  
            var jobs_list = TestDbInitializer.SeedJobs();          
            Random random = new Random();
            int id = random.Next(1,5);
            IEnumerable<UserDto> users = controllers.usersController.Get().OrderBy(x => Guid.NewGuid()).Take(1);

            JobPostDto post = new JobPostDto{
                    CreateDate     = DateTime.Now.AddDays(random.Next(1,5)), 
                    UserID         = users.FirstOrDefault().UserId,
                    JobDoerUserID  = 0,
                    Title          = "Job #" + id,
                    JobDescription = "Description of Job #" + id,
                    Amount         = 1000*random.Next(7,100),
                    TimeFrame      = "65",
                    LastUpdate     = DateTime.Now.AddDays(40),
                    Status         = Helpers.Constants.Enums.JobStatusTypes.AdminApprovalPending,
                    DosFeePaid     = true                    
            };
            // Act
            var postedJob = controllers.jobPostController.Post(post);
            // Assert
            postedJob.JobDescription.Should().Be(post.JobDescription);
        }

        /// Test of DAO_DbService.JobPostController.Update(JobPostDto model)
        /// Updates jobPost model 
        [Fact]
        public void JobPostController_Update_Test()
        {
            Random rand = new Random();
            //Arrange
            var jobs_list = TestDbInitializer.SeedJobs();
            int selected_job = rand.Next(1, jobs_list.Count());
            JobPost job = jobs_list.ElementAt(selected_job);
            JobPostDto job_to_be_updated = controllers.jobPostController.GetId(job.JobID);

            job_to_be_updated.JobDescription = "description_is_updated";
            //Act
            JobPostDto updated_job = controllers.jobPostController.Update(job_to_be_updated);
            //Assert
            (updated_job.JobDescription == job_to_be_updated.JobDescription && updated_job.JobDescription == "description_is_updated").Should().Be(true);
        }
    }
}