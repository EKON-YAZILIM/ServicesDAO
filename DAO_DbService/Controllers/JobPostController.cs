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
    public class JobPostController : Controller
    {
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
    }
}
