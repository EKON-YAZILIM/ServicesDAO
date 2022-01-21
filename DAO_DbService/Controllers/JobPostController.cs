using DAO_DbService.Contexts;
using DAO_DbService.Mapping;
using DAO_DbService.Models;
using Helpers.Models.DtoModels.MainDbDto;
using Helpers.Models.SharedModels;
using Microsoft.AspNetCore.Mvc;
using PagedList.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static DAO_DbService.Mapping.AutoMapperBase;
using static Helpers.Constants.Enums;

namespace DAO_DbService.Controllers
{
    /// <summary>
    ///  JobPostController contains job post operation methods
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class JobPostController : Controller
    {
        /// <summary>
        /// Get JobPost List
        /// </summary>
        /// <returns></returns>
        [Route("Get")]
        [HttpGet]
        public IEnumerable<JobPostDto> Get()
        {
            List<JobPost> model = new List<JobPost>();

            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    model = db.JobPosts.ToList();
                }
            }
            catch (Exception ex)
            {
                model = new List<JobPost>();
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return _mapper.Map<List<JobPost>, List<JobPostDto>>(model).ToArray();
        }

        /// <summary>
        /// returns a list of all job post by using title or descrpition as search key
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [Route("JobPostSearch")]
        [HttpGet]
        public IEnumerable<JobPostDto> JobPostSearch(string query)
        {
            List<JobPost> res = new List<JobPost>();

            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    res = db.JobPosts.Where(x => x.Title.Contains(query) || x.JobDescription.Contains(query)).ToList();
                }

            }
            catch (Exception ex)
            {
                res = new List<JobPost>();
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }
            return _mapper.Map<List<JobPost>, List<JobPostDto>>(res).ToArray();
        }

        /// <summary>
        /// Get JobPost by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("GetId")]
        [HttpGet]
        public JobPostDto GetId(int id)
        {
            JobPost model = new JobPost();

            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    model = db.JobPosts.Find(id);
                }
            }
            catch (Exception ex)
            {
                model = new JobPost();
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return _mapper.Map<JobPost, JobPostDto>(model);
        }

        /// <summary>
        ///  Saves job post model using post method
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("Post")]
        [HttpPost]
        public JobPostDto Post([FromBody] JobPostDto model)
        {
            try
            {
                JobPost item = _mapper.Map<JobPostDto, JobPost>(model);
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    db.JobPosts.Add(item);
                    db.SaveChanges();
                }
                return _mapper.Map<JobPost, JobPostDto>(item);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return new JobPostDto();
            }
        }

        /// <summary>
        /// Saves job post list using post method
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("PostMultiple")]
        [HttpPost]
        public List<JobPostDto> PostMultiple([FromBody] List<JobPostDto> model)
        {
            try
            {
                List<JobPost> item = _mapper.Map<List<JobPostDto>, List<JobPost>>(model);
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    db.JobPosts.AddRange(item);
                    db.SaveChanges();
                }
                return _mapper.Map<List<JobPost>, List<JobPostDto>>(item);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return new List<JobPostDto>();
            }
        }

        /// <summary>
        /// Removes job post by id
        /// Ends active sessions
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [Route("Delete")]
        [HttpDelete]
        public bool Delete(int? ID)
        {
            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    JobPost item = db.JobPosts.FirstOrDefault(s => s.JobID == ID);
                    db.Entry(item).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
                    db.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return false;
            }
        }

        /// <summary>
        ///  Update jobPost using put method
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("Update")]
        [HttpPut]
        public JobPostDto Update([FromBody] JobPostDto model)
        {
            try
            {
                JobPost item = _mapper.Map<JobPostDto, JobPost>(model);
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    db.Entry(item).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    db.SaveChanges();
                }
                return _mapper.Map<JobPost, JobPostDto>(item);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return new JobPostDto();
            }
        }

        /// <summary>
        /// Brings up the jobPost pages.
        /// The selected page is fetched. Not all pages are returned
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageCount"></param>
        /// <returns></returns>
        [Route("GetPaged")]
        [HttpGet]
        public PaginationEntity<JobPostDto> GetPaged(int page = 1, int pageCount = 30)
        {
            PaginationEntity<JobPostDto> res = new PaginationEntity<JobPostDto>();

            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {

                    IPagedList<JobPostDto> lst = AutoMapperBase.ToMappedPagedList<JobPost, JobPostDto>(db.JobPosts.OrderByDescending(x => x.JobID).ToPagedList(page, pageCount));

                    res.Items = lst;
                    res.MetaData = new PaginationMetaData() { Count = lst.Count, FirstItemOnPage = lst.FirstItemOnPage, HasNextPage = lst.HasNextPage, HasPreviousPage = lst.HasPreviousPage, IsFirstPage = lst.IsFirstPage, IsLastPage = lst.IsLastPage, LastItemOnPage = lst.LastItemOnPage, PageCount = lst.PageCount, PageNumber = lst.PageNumber, PageSize = lst.PageSize, TotalItemCount = lst.TotalItemCount };



                    return res;
                }
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return res;
        }

        /// <summary>
        /// returns a list containing desired amount of found job post by using title or descrpition as search key
        /// </summary>
        /// <param name="query"></param>
        /// <param name="page"></param>
        /// <param name="pageCount"></param>
        /// <returns></returns>
        [Route("Search")]
        [HttpGet]
        public PaginationEntity<JobPostDto> Search(string query, int page = 1, int pageCount = 30)
        {
            PaginationEntity<JobPostDto> res = new PaginationEntity<JobPostDto>();

            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    IPagedList<JobPostDto> lst = AutoMapperBase.ToMappedPagedList<JobPost, JobPostDto>(db.JobPosts.Where(x => x.Title.Contains(query) || x.JobDescription.Contains(query)).ToPagedList(page, pageCount));

                    res.Items = lst;
                    res.MetaData = new PaginationMetaData() { Count = lst.Count, FirstItemOnPage = lst.FirstItemOnPage, HasNextPage = lst.HasNextPage, HasPreviousPage = lst.HasPreviousPage, IsFirstPage = lst.IsFirstPage, IsLastPage = lst.IsLastPage, LastItemOnPage = lst.LastItemOnPage, PageCount = lst.PageCount, PageNumber = lst.PageNumber, PageSize = lst.PageSize, TotalItemCount = lst.TotalItemCount };
                }

                return res;
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return res;
            }

        }

        /// <summary>
        /// Updates ChangeJobStatus by jobid and status
        /// </summary>
        /// <param name="jobid"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [Route("ChangeJobStatus")]
        [HttpGet]
        public JobPostDto ChangeJobStatus(int jobid, JobStatusTypes status)
        {
            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    JobPost item = db.JobPosts.Find(jobid);
                    item.Status = status;
                    db.Entry(item).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    db.SaveChanges();
                    return _mapper.Map<JobPost, JobPostDto>(item);
                }
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return new JobPostDto();
        }

        /// <summary>
        /// gets jobPost List by userId
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        [Route("GetByUserId")]
        [HttpGet]
        public List<JobPostDto> GetByUserId(int userid)
        {
            List<JobPost> model = new List<JobPost>();

            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    model = db.JobPosts.Where(x=>x.UserID == userid).ToList();
                }
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return _mapper.Map<List<JobPost>, List<JobPostDto>>(model);
        }
    }
}
