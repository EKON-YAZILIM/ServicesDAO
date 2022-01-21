using DAO_LogService.Models;
using Helpers.Models.DtoModels.LogDbDto;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Helpers.Constants.Enums;
using static DAO_LogService.Mapping.AutoMapperBase;
using DAO_LogService.Contexts;
using Helpers.Models.SharedModels;
using PagedList.Core;
using DAO_LogService.Mapping;

namespace DAO_LogService.Controllers
{
    /// <summary>
    ///  ErrorLogsController contains error logs operation methods
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class ErrorLogController : Controller
    {
        /// <summary>
        ///  Get error logs list
        /// </summary>
        /// <returns>ErrorLog List</returns>
        [Route("Get")]
        [HttpGet]
        public IEnumerable<ErrorLogDto> Get()
        {
            List<ErrorLog> model = new List<ErrorLog>();

            try
            {
                using (dao_logsdb_context db = new dao_logsdb_context())
                {
                    model = db.ErrorLogs.ToList();
                }
            }
            catch (Exception ex)
            {
                model = new List<ErrorLog>();
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return _mapper.Map<List<ErrorLog>, List<ErrorLogDto>>(model).ToArray();
        }

        /// <summary>
        /// Get error logs by id
        /// </summary>
        [Route("GetId")]
        [HttpGet]
        public ErrorLogDto GetId(int id)
        {
            ErrorLog model = new ErrorLog();

            try
            {
                using (dao_logsdb_context db = new dao_logsdb_context())
                {
                    model = db.ErrorLogs.Find(id);
                }
            }
            catch (Exception ex)
            {
                model = new ErrorLog();
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return _mapper.Map<ErrorLog, ErrorLogDto>(model);
        }

        /// <summary>
        /// Saves the ErrorLog using the post method.
        /// </summary>
        [Route("Post")]
        [HttpPost]
        public ErrorLogDto Post([FromBody] ErrorLogDto model)
        {
            try
            {
                ErrorLog item = _mapper.Map<ErrorLogDto, ErrorLog>(model);
                using (dao_logsdb_context db = new dao_logsdb_context())
                {
                    db.ErrorLogs.Add(item);
                    db.SaveChanges();
                }
                return _mapper.Map<ErrorLog, ErrorLogDto>(item);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return new ErrorLogDto();
            }
        }

        /// <summary>
        /// Saves the list of errorLogs model using post method
        /// </summary>
        /// <returns>List of errorLogs class</returns>
        [Route("PostMultiple")]
        [HttpPost]
        public List<ErrorLogDto> PostMultiple([FromBody] List<ErrorLogDto> model)
        {
            try
            {
                List<ErrorLog> item = _mapper.Map<List<ErrorLogDto>, List<ErrorLog>>(model);
                using (dao_logsdb_context db = new dao_logsdb_context())
                {
                    db.ErrorLogs.AddRange(item);
                    db.SaveChanges();
                }
                return _mapper.Map<List<ErrorLog>, List<ErrorLogDto>>(item);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return new List<ErrorLogDto>();
            }
        }

        /// <summary>
        /// Deletes the errorlog by id using delete method
        /// </summary>
        [Route("Delete")]
        [HttpDelete]
        public bool Delete(int? ID)
        {
            try
            {
                using (dao_logsdb_context db = new dao_logsdb_context())
                {
                    ErrorLog item = db.ErrorLogs.FirstOrDefault(s => s.ErrorLogId == ID);
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
        /// Updates the errorlogs by model using put method
        /// </summary>
        [Route("Update")]
        [HttpPut]
        public ErrorLogDto Update([FromBody] ErrorLogDto model)
        {
            try
            {
                ErrorLog item = _mapper.Map<ErrorLogDto, ErrorLog>(model);
                using (dao_logsdb_context db = new dao_logsdb_context())
                {
                    db.Entry(item).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    db.SaveChanges();
                }
                return _mapper.Map<ErrorLog, ErrorLogDto>(item);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return new ErrorLogDto();
            }
        }

        /// <summary>
        /// Brings up the errorLogs pages.
        /// The selected page is fetched. Not all pages are returned
        /// </summary>
        [Route("GetPaged")]
        [HttpGet]
        public PaginationEntity<ErrorLogDto> GetPaged(int page = 1, int pageCount = 30)
        {
            PaginationEntity<ErrorLogDto> res = new PaginationEntity<ErrorLogDto>();

            try
            {
                using (dao_logsdb_context db = new dao_logsdb_context())
                {

                    IPagedList<ErrorLogDto> lst = AutoMapperBase.ToMappedPagedList<ErrorLog, ErrorLogDto>(db.ErrorLogs.OrderByDescending(x => x.ErrorLogId).ToPagedList(page, pageCount));

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
    }
}
