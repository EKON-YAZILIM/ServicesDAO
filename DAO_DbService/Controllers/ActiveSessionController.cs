using DAO_DbService.Contexts;
using DAO_DbService.Models;
using Helpers.Models.DtoModels.MainDbDto;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Helpers.Constants.Enums;
using static DAO_DbService.Mapping.AutoMapperBase;
using Helpers.Models.SharedModels;
using DAO_DbService.Mapping;
using PagedList.Core;

namespace DAO_DbService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ActiveSessionController : Controller
    {
        [Route("Get")]
        [HttpGet]
        public IEnumerable<ActiveSessionDto> Get()
        {
            List<ActiveSession> model = new List<ActiveSession>();

            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    model = db.ActiveSessions.ToList();
                }
            }
            catch (Exception ex)
            {
                model = new List<ActiveSession>();
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return _mapper.Map<List<ActiveSession>, List<ActiveSessionDto>>(model).ToArray();
        }

        [Route("GetId")]
        [HttpGet]
        public ActiveSessionDto GetId(int id)
        {
            ActiveSession model = new ActiveSession();

            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    model = db.ActiveSessions.Find(id);
                }
            }
            catch (Exception ex)
            {
                model = new ActiveSession();
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return _mapper.Map<ActiveSession, ActiveSessionDto>(model);
        }

        [Route("Post")]
        [HttpPost]
        public ActiveSessionDto Post([FromBody] ActiveSessionDto model)
        {
            try
            {
                ActiveSession item = _mapper.Map<ActiveSessionDto, ActiveSession>(model);
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    db.ActiveSessions.Add(item);
                    db.SaveChanges();
                }
                return _mapper.Map<ActiveSession, ActiveSessionDto>(item);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return new ActiveSessionDto();
            }
        }

        [Route("PostMultiple")]
        [HttpPost]
        public List<ActiveSessionDto> PostMultiple([FromBody] List<ActiveSessionDto> model)
        {
            try
            {
                List<ActiveSession> item = _mapper.Map<List<ActiveSessionDto>, List<ActiveSession>>(model);
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    db.ActiveSessions.AddRange(item);
                    db.SaveChanges();
                }
                return _mapper.Map<List<ActiveSession>, List<ActiveSessionDto>>(item);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return new List<ActiveSessionDto>();
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
                    ActiveSession item = db.ActiveSessions.FirstOrDefault(s => s.ActiveSessionID == ID);
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
        public ActiveSessionDto Update([FromBody] ActiveSessionDto model)
        {
            try
            {
                ActiveSession item = _mapper.Map<ActiveSessionDto, ActiveSession>(model);
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    db.Entry(item).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    db.SaveChanges();
                }
                return _mapper.Map<ActiveSession, ActiveSessionDto>(item);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return new ActiveSessionDto();
            }
        }

        [Route("GetPaged")]
        [HttpGet]
        public PaginationEntity<ActiveSessionDto> GetPaged(int page = 1, int pageCount = 30)
        {
            PaginationEntity<ActiveSessionDto> res = new PaginationEntity<ActiveSessionDto>();

            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {

                    IPagedList<ActiveSessionDto> lst = AutoMapperBase.ToMappedPagedList<ActiveSession, ActiveSessionDto>(db.ActiveSessions.OrderByDescending(x => x.ActiveSessionID).ToPagedList(page, pageCount));

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

        [Route("PostOrUpdate")]
        [HttpPost]
        public ActiveSessionDto PostOrUpdate([FromBody] ActiveSessionDto model)
        {
            try
            {
                ActiveSession item = _mapper.Map<ActiveSessionDto, ActiveSession>(model);

                using (dao_maindb_context db = new dao_maindb_context())
                {
                    if(db.ActiveSessions.Count(x => x.UserID == model.UserID) > 0)
                    {
                        item = db.ActiveSessions.First(x => x.UserID == model.UserID);
                        item.Token = model.Token;
                        item.LoginDate = model.LoginDate;
                        db.Entry(item).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        db.ActiveSessions.Add(item);
                        db.SaveChanges();
                    }
                }

                return _mapper.Map<ActiveSession, ActiveSessionDto>(item);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return new ActiveSessionDto();
            }
        }

        [Route("DeleteByUserId")]
        [HttpDelete]
        public bool DeleteByUserId(int userid)
        {
            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    var records = db.ActiveSessions.Where(x => x.UserID == userid).ToList();
                    foreach (var item in records)
                    {
                        db.ActiveSessions.Remove(item);
                    }
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

    }
}
