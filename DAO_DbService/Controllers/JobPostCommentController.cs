using DAO_DbService.Contexts;
using DAO_DbService.Mapping;
using DAO_DbService.Models;
using Helpers.Models.DtoModels.MainDbDto;
using Helpers.Models.SharedModels;
using Microsoft.AspNetCore.Mvc;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static DAO_DbService.Mapping.AutoMapperBase;
using static Helpers.Constants.Enums;

namespace DAO_DbService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class JobPostCommentController : Controller
    {
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
