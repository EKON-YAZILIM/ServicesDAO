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
    public class UserReputationController : Controller
    {
        [Route("Get")]
        [HttpGet]
        public IEnumerable<UserReputationDto> Get()
        {
            List<UserReputation> model = new List<UserReputation>();

            try
            {
                using (dao_reputationserv_context db = new dao_reputationserv_context())
                {
                    model = db.UserReputations.ToList();
                }
            }
            catch (Exception ex)
            {
                model = new List<UserReputation>();
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return _mapper.Map<List<UserReputation>, List<UserReputationDto>>(model).ToArray();
        }

        [Route("GetId")]
        [HttpGet]
        public UserReputationDto GetId(int id)
        {
            UserReputation model = new UserReputation();

            try
            {
                using (dao_reputationserv_context db = new dao_reputationserv_context())
                {
                    model = db.UserReputations.Find(id);
                }
            }
            catch (Exception ex)
            {
                model = new UserReputation();
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return _mapper.Map<UserReputation, UserReputationDto>(model);
        }
        
        [Route("Post")]
        [HttpPost]
        public UserReputationDto Post([FromBody] UserReputationDto model)
        {
            try
            {
                UserReputation item = _mapper.Map<UserReputationDto, UserReputation>(model);
                using (dao_reputationserv_context db = new dao_reputationserv_context())
                {
                    db.UserReputations.Add(item);
                    db.SaveChanges();
                }
                return _mapper.Map<UserReputation, UserReputationDto>(item);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return new UserReputationDto();
            }
        }
       
        [Route("PostMultiple")]
        [HttpPost]
        public List<UserReputationDto> PostMultiple([FromBody] List<UserReputationDto> model)
        {
            try
            {
                List<UserReputation> item = _mapper.Map<List<UserReputationDto>, List<UserReputation>>(model);
                using (dao_reputationserv_context db = new dao_reputationserv_context())
                {
                    db.UserReputations.AddRange(item);
                    db.SaveChanges();
                }
                return _mapper.Map<List<UserReputation>, List<UserReputationDto>>(item);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return new List<UserReputationDto>();
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
                    UserReputation item = db.UserReputations.FirstOrDefault(s => s.UserReputationID == ID);
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
        public UserReputationDto Update([FromBody] UserReputationDto model)
        {
            try
            {
                UserReputation item = _mapper.Map<UserReputationDto, UserReputation>(model);
                using (dao_reputationserv_context db = new dao_reputationserv_context())
                {
                    db.Entry(item).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    db.SaveChanges();
                }
                return _mapper.Map<UserReputation, UserReputationDto>(item);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return new UserReputationDto();
            }
        }
       
        [Route("GetPaged")]
        [HttpGet]
        public PaginationEntity<UserReputationDto> GetPaged(int page = 1, int pageCount = 30)
        {
            PaginationEntity<UserReputationDto> res = new PaginationEntity<UserReputationDto>();

            try
            {
                using (dao_reputationserv_context db = new dao_reputationserv_context())
                {

                    IPagedList<UserReputationDto> lst = AutoMapperBase.ToMappedPagedList<UserReputation, UserReputationDto>(db.UserReputations.OrderByDescending(x => x.UserReputationID).ToPagedList(page, pageCount));

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
