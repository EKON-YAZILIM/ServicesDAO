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
    [Route("[controller]")]
    [ApiController]
    public class UserLogController : Controller
    {
        [Route("Get")]
        [HttpGet]
        public IEnumerable<UserLogDto> Get()
        {
            List<UserLog> model = new List<UserLog>();

            try
            {
                using (dao_logsdb_context db = new dao_logsdb_context())
                {
                    model = db.UserLogs.ToList();
                }
            }
            catch (Exception ex)
            {
                model = new List<UserLog>();
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return _mapper.Map<List<UserLog>, List<UserLogDto>>(model).ToArray();
        }

        [Route("GetId")]
        [HttpGet]
        public UserLogDto GetId(int id)
        {
            UserLog model = new UserLog();

            try
            {
                using (dao_logsdb_context db = new dao_logsdb_context())
                {
                    model = db.UserLogs.Find(id);
                }
            }
            catch (Exception ex)
            {
                model = new UserLog();
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return _mapper.Map<UserLog, UserLogDto>(model);
        }

        [Route("Post")]
        [HttpPost]
        public UserLogDto Post([FromBody] UserLogDto model)
        {
            try
            {
                UserLog item = _mapper.Map<UserLogDto, UserLog>(model);
                using (dao_logsdb_context db = new dao_logsdb_context())
                {
                    db.UserLogs.Add(item);
                    db.SaveChanges();
                }
                return _mapper.Map<UserLog, UserLogDto>(item);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return new UserLogDto();
            }
        }

        [Route("PostMultiple")]
        [HttpPost]
        public List<UserLogDto> PostMultiple([FromBody] List<UserLogDto> model)
        {
            try
            {
                List<UserLog> item = _mapper.Map<List<UserLogDto>, List<UserLog>>(model);
                using (dao_logsdb_context db = new dao_logsdb_context())
                {
                    db.UserLogs.AddRange(item);
                    db.SaveChanges();
                }
                return _mapper.Map<List<UserLog>, List<UserLogDto>>(item);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return new List<UserLogDto>();
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
                    UserLog item = db.UserLogs.FirstOrDefault(s => s.UserLogId == ID);
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
        public UserLogDto Update([FromBody] UserLogDto model)
        {
            try
            {
                UserLog item = _mapper.Map<UserLogDto, UserLog>(model);
                using (dao_logsdb_context db = new dao_logsdb_context())
                {
                    db.Entry(item).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    db.SaveChanges();
                }
                return _mapper.Map<UserLog, UserLogDto>(item);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return new UserLogDto();
            }
        }

        [Route("GetPaged")]
        [HttpGet]
        public PaginationEntity<UserLogDto> GetPaged(int page = 1, int pageCount = 30)
        {
            PaginationEntity<UserLogDto> res = new PaginationEntity<UserLogDto>();

            try
            {
                using (dao_logsdb_context db = new dao_logsdb_context())
                {

                    IPagedList<UserLogDto> lst = AutoMapperBase.ToMappedPagedList<UserLog, UserLogDto>(db.UserLogs.OrderByDescending(x => x.UserLogId).ToPagedList(page, pageCount));

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
        public IEnumerable<UserLogDto> GetLastWithCount(int count =20)
        {
            List<UserLog> model = new List<UserLog>();

            try
            {
                using (dao_logsdb_context db = new dao_logsdb_context())
                {
                    model = db.UserLogs.OrderByDescending(x=>x.UserLogId).Take(count).ToList();
                }
            }
            catch (Exception ex)
            {
                model = new List<UserLog>();
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return _mapper.Map<List<UserLog>, List<UserLogDto>>(model).ToArray();
        }
    }
}
