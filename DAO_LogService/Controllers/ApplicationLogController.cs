using DAO_LogService.Contexts;
using DAO_LogService.Models;
using Helpers.Models.DtoModels.LogDbDto;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Helpers.Constants.Enums;
using static DAO_LogService.Mapping.AutoMapperBase;
using PagedList.Core;
using DAO_LogService.Mapping;
using Helpers.Models.SharedModels;

namespace DAO_LogService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ApplicationLogController : Controller
    {
        [Route("Get")]
        [HttpGet]
        public IEnumerable<ApplicationLogDto> Get()
        {
            List<ApplicationLog> model = new List<ApplicationLog>();

            try
            {
                using (dao_logsdb_context db = new dao_logsdb_context())
                {
                    model = db.ApplicationLogs.ToList();
                }
            }
            catch (Exception ex)
            {
                model = new List<ApplicationLog>();
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return _mapper.Map<List<ApplicationLog>, List<ApplicationLogDto>>(model).ToArray();
        }

        [Route("GetId")]
        [HttpGet]
        public ApplicationLogDto GetId(int id)
        {
            ApplicationLog model = new ApplicationLog();

            try
            {
                using (dao_logsdb_context db = new dao_logsdb_context())
                {
                    model = db.ApplicationLogs.Find(id);
                }
            }
            catch (Exception ex)
            {
                model = new ApplicationLog();
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return _mapper.Map<ApplicationLog, ApplicationLogDto>(model);
        }

        [Route("Post")]
        [HttpPost]
        public ApplicationLogDto Post([FromBody] ApplicationLogDto model)
        {
            try
            {
                ApplicationLog item = _mapper.Map<ApplicationLogDto, ApplicationLog>(model);
                using (dao_logsdb_context db = new dao_logsdb_context())
                {
                    db.ApplicationLogs.Add(item);
                    db.SaveChanges();
                }
                return _mapper.Map<ApplicationLog, ApplicationLogDto>(item);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return new ApplicationLogDto();
            }
        }

        [Route("PostMultiple")]
        [HttpPost]
        public List<ApplicationLogDto> PostMultiple([FromBody] List<ApplicationLogDto> model)
        {
            try
            {
                List<ApplicationLog> item = _mapper.Map<List<ApplicationLogDto>, List<ApplicationLog>>(model);
                using (dao_logsdb_context db = new dao_logsdb_context())
                {
                    db.ApplicationLogs.AddRange(item);
                    db.SaveChanges();
                }
                return _mapper.Map<List<ApplicationLog>, List<ApplicationLogDto>>(item);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return new List<ApplicationLogDto>();
            }
        }

        [Route("Delete")]
        [HttpDelete]
        public bool Delete(int? ID)
        {
            try
            {
                using (dao_logsdb_context db = new dao_logsdb_context())
                {
                    ApplicationLog item = db.ApplicationLogs.FirstOrDefault(s => s.ApplicationLogID == ID);
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
        public ApplicationLogDto Update([FromBody] ApplicationLogDto model)
        {
            try
            {
                ApplicationLog item = _mapper.Map<ApplicationLogDto, ApplicationLog>(model);
                using (dao_logsdb_context db = new dao_logsdb_context())
                {
                    db.Entry(item).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    db.SaveChanges();
                }
                return _mapper.Map<ApplicationLog, ApplicationLogDto>(item);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return new ApplicationLogDto();
            }
        }

        [Route("GetPaged")]
        [HttpGet]
        public PaginationEntity<ApplicationLogDto> GetPaged(int page = 1, int pageCount = 30)
        {
            PaginationEntity<ApplicationLogDto> res = new PaginationEntity<ApplicationLogDto>();

            try
            {
                using (dao_logsdb_context db = new dao_logsdb_context())
                {

                    IPagedList<ApplicationLogDto> lst = AutoMapperBase.ToMappedPagedList<ApplicationLog, ApplicationLogDto>(db.ApplicationLogs.OrderByDescending(x => x.ApplicationLogID).ToPagedList(page, pageCount));

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

        [Route("GetLastWithCount")]
        [HttpGet]
        public IEnumerable<ApplicationLogDto> GetLastWithCount(int count = 20)
        {
            List<ApplicationLog> model = new List<ApplicationLog>();

            try
            {
                using (dao_logsdb_context db = new dao_logsdb_context())
                {
                    model = db.ApplicationLogs.OrderByDescending(x => x.ApplicationLogID).Take(count).ToList();
                }
            }
            catch (Exception ex)
            {
                model = new List<ApplicationLog>();
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return _mapper.Map<List<ApplicationLog>, List<ApplicationLogDto>>(model).ToArray();
        }
    }
}
