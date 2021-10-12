using DAO_ReputationService.Contexts;
using DAO_ReputationService.Models;
using Helpers.Models.DtoModels.VoteDbDto;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Helpers.Constants.Enums;
using static DAO_ReputationService.Mapping.AutoMapperBase;
using PagedList;
using Helpers.Models.SharedModels;
using DAO_ReputationService.Mapping;

namespace DAO_ReputationService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserReputationHistoryController : Controller
    {
        [Route("Get")]
        [HttpGet]
        public IEnumerable<UserReputationHistoryDto> Get()
        {
            List<UserReputationHistory> model = new List<UserReputationHistory>();

            try
            {
                using (dao_reputationserv_context db = new dao_reputationserv_context())
                {
                    model = db.UserReputationHistories.ToList();
                }
            }
            catch (Exception ex)
            {
                model = new List<UserReputationHistory>();
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return _mapper.Map<List<UserReputationHistory>, List<UserReputationHistoryDto>>(model).ToArray();
        }

        [Route("GetId")]
        [HttpGet]
        public UserReputationHistoryDto GetId(int id)
        {
            UserReputationHistory model = new UserReputationHistory();

            try
            {
                using (dao_reputationserv_context db = new dao_reputationserv_context())
                {
                    model = db.UserReputationHistories.Find(id);
                }
            }
            catch (Exception ex)
            {
                model = new UserReputationHistory();
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return _mapper.Map<UserReputationHistory, UserReputationHistoryDto>(model);
        }

        [Route("Post")]
        [HttpPost]
        public UserReputationHistoryDto Post([FromBody] UserReputationHistoryDto model)
        {
            try
            {
                UserReputationHistory item = _mapper.Map<UserReputationHistoryDto, UserReputationHistory>(model);
                using (dao_reputationserv_context db = new dao_reputationserv_context())
                {
                    db.UserReputationHistories.Add(item);
                    db.SaveChanges();
                }
                return _mapper.Map<UserReputationHistory, UserReputationHistoryDto>(item);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return new UserReputationHistoryDto();
            }
        }

        [Route("PostMultiple")]
        [HttpPost]
        public List<UserReputationHistoryDto> PostMultiple([FromBody] List<UserReputationHistoryDto> model)
        {
            try
            {
                List<UserReputationHistory> item = _mapper.Map<List<UserReputationHistoryDto>, List<UserReputationHistory>>(model);
                using (dao_reputationserv_context db = new dao_reputationserv_context())
                {
                    db.UserReputationHistories.AddRange(item);
                    db.SaveChanges();
                }
                return _mapper.Map<List<UserReputationHistory>, List<UserReputationHistoryDto>>(item);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return new List<UserReputationHistoryDto>();
            }
        }

        [Route("Delete")]
        [HttpDelete]
        public bool Delete(int? ID)
        {
            try
            {
                using (dao_reputationserv_context db = new dao_reputationserv_context())
                {
                    UserReputationHistory item = db.UserReputationHistories.FirstOrDefault(s => s.UserReputationHistoryID == ID);
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
        public UserReputationHistoryDto Update([FromBody] UserReputationHistoryDto model)
        {
            try
            {
                UserReputationHistory item = _mapper.Map<UserReputationHistoryDto, UserReputationHistory>(model);
                using (dao_reputationserv_context db = new dao_reputationserv_context())
                {
                    db.Entry(item).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    db.SaveChanges();
                }
                return _mapper.Map<UserReputationHistory, UserReputationHistoryDto>(item);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return new UserReputationHistoryDto();
            }
        }

        [Route("GetPaged")]
        [HttpGet]
        public PaginationEntity<UserReputationHistoryDto> GetPaged(int page = 1, int pageCount = 30)
        {
            PaginationEntity<UserReputationHistoryDto> res = new PaginationEntity<UserReputationHistoryDto>();

            try
            {
                using (dao_reputationserv_context db = new dao_reputationserv_context())
                {

                    IPagedList<UserReputationHistoryDto> lst = AutoMapperBase.ToMappedPagedList<UserReputationHistory, UserReputationHistoryDto>(db.UserReputationHistories.OrderByDescending(x => x.UserReputationHistoryID).ToPagedList(page, pageCount));

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

        [Route("UserReputationHistorySearch")]
        [HttpGet]
        public IEnumerable<UserReputationHistoryDto> UserReputationHistorySearch(string query)
        {
            List<UserReputationHistory> res = new List<UserReputationHistory>();

            try
            {
                using (dao_reputationserv_context db = new dao_reputationserv_context())
                {
                    res = db.UserReputationHistories.Where(x => x.Explanation.Contains(query)).ToList();
                }

            }
            catch (Exception ex)
            {
                res = new List<UserReputationHistory>();
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }
            return _mapper.Map<List<UserReputationHistory>, List<UserReputationHistoryDto>>(res).ToArray();
        }

        [Route("Search")]
        [HttpGet]
        public PaginationEntity<UserReputationHistoryDto> Search(string query, int page = 1, int pageCount = 30)
        {
            PaginationEntity<UserReputationHistoryDto> res = new PaginationEntity<UserReputationHistoryDto>();

            try
            {
                using (dao_reputationserv_context db = new dao_reputationserv_context())
                {
                    IPagedList<UserReputationHistoryDto> lst = AutoMapperBase.ToMappedPagedList<UserReputationHistory, UserReputationHistoryDto>(db.UserReputationHistories.Where(x =>x.Explanation.Contains(query)).ToPagedList(page, pageCount));

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
