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
    ///  JobPostCommentController contains job post comment operation methods
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class JobPostCommentController : Controller
    {
        /// <summary>
        /// Gets JobPostComment List
        /// </summary>
        /// <returns></returns>
        [Route("Get")]
        [HttpGet]
        public IEnumerable<JobPostCommentDto> Get()
        {
            List<JobPostComment> model = new List<JobPostComment>();

            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    model = db.JobPostComments.ToList();
                }
            }
            catch (Exception ex)
            {
                model = new List<JobPostComment>();
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return _mapper.Map<List<JobPostComment>, List<JobPostCommentDto>>(model).ToArray();
        }

        /// <summary>
        /// returns a list of all job post comment by using title or descrpition as search key
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [Route("JobPostCommentSearch")]
        [HttpGet]
        public IEnumerable<JobPostCommentDto> JobPostCommentSearch(string query)
        {
            List<JobPostComment> res = new List<JobPostComment>();

            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    res = db.JobPostComments.Where(x => x.Comment.Contains(query)).ToList();
                }

            }
            catch (Exception ex)
            {
                res = new List<JobPostComment>();
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }
            return _mapper.Map<List<JobPostComment>, List<JobPostCommentDto>>(res).ToArray();
        }

        /// <summary>
        /// Get JobPostComment by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("GetId")]
        [HttpGet]
        public JobPostCommentDto GetId(int id)
        {
            JobPostComment model = new JobPostComment();

            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    model = db.JobPostComments.Find(id);
                }
            }
            catch (Exception ex)
            {
                model = new JobPostComment();
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return _mapper.Map<JobPostComment, JobPostCommentDto>(model);
        }

        /// <summary>
        /// Saves job post comment model using post method
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("Post")]
        [HttpPost]
        public JobPostCommentDto Post([FromBody] JobPostCommentDto model)
        {
            try
            {
                JobPostComment item = _mapper.Map<JobPostCommentDto, JobPostComment>(model);
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    db.JobPostComments.Add(item);
                    db.SaveChanges();
                }
                return _mapper.Map<JobPostComment, JobPostCommentDto>(item);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return new JobPostCommentDto();
            }
        }

        /// <summary>
        /// Saves job post comment list using post method
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("PostMultiple")]
        [HttpPost]
        public List<JobPostCommentDto> PostMultiple([FromBody] List<JobPostCommentDto> model)
        {
            try
            {
                List<JobPostComment> item = _mapper.Map<List<JobPostCommentDto>, List<JobPostComment>>(model);
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    db.JobPostComments.AddRange(item);
                    db.SaveChanges();
                }
                return _mapper.Map<List<JobPostComment>, List<JobPostCommentDto>>(item);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return new List<JobPostCommentDto>();
            }
        }

        /// <summary>
        /// Removes job post comment by id
        /// Ends job post comment
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
                    JobPostComment item = db.JobPostComments.FirstOrDefault(s => s.JobPostCommentID == ID);
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
        /// Updates jobPostComment using put method
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("Update")]
        [HttpPut]
        public JobPostCommentDto Update([FromBody] JobPostCommentDto model)
        {
            try
            {
                JobPostComment item = _mapper.Map<JobPostCommentDto, JobPostComment>(model);
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    db.Entry(item).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    db.SaveChanges();
                }
                return _mapper.Map<JobPostComment, JobPostCommentDto>(item);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return new JobPostCommentDto();
            }
        }

        /// <summary>
        /// Brings up the jobPostComment pages.
        /// The selected page is fetched. Not all pages are returned
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageCount"></param>
        /// <returns></returns>
        [Route("GetPaged")]
        [HttpGet]
        public PaginationEntity<JobPostCommentDto> GetPaged(int page = 1, int pageCount = 30)
        {
            PaginationEntity<JobPostCommentDto> res = new PaginationEntity<JobPostCommentDto>();

            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {

                    IPagedList<JobPostCommentDto> lst = AutoMapperBase.ToMappedPagedList<JobPostComment, JobPostCommentDto>(db.JobPostComments.OrderByDescending(x => x.JobPostCommentID).ToPagedList(page, pageCount));

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
        /// returns a list containing desired amount of found job post comment by using title or descrpition as search key
        /// </summary>
        /// <param name="query"></param>
        /// <param name="page"></param>
        /// <param name="pageCount"></param>
        /// <returns></returns>
        [Route("Search")]
        [HttpGet]
        public PaginationEntity<JobPostCommentDto> Search(string query, int page = 1, int pageCount = 30)
        {
            PaginationEntity<JobPostCommentDto> res = new PaginationEntity<JobPostCommentDto>();

            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    IPagedList<JobPostCommentDto> lst = AutoMapperBase.ToMappedPagedList<JobPostComment, JobPostCommentDto>(db.JobPostComments.Where(x => x.Comment.Contains(query)).ToPagedList(page, pageCount));

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

    }
}
