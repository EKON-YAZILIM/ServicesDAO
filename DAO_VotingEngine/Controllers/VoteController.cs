using DAO_VotingEngine.Contexts;
using DAO_VotingEngine.Models;
using Helpers.Models.DtoModels.VoteDbDto;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Helpers.Constants.Enums;
using static DAO_VotingEngine.Mapping.AutoMapperBase;
using PagedList;
using DAO_VotingEngine.Mapping;
using Helpers.Models.SharedModels;

namespace DAO_VotingEngine.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class VoteController : Controller
    {
        [Route("Get")]
        [HttpGet]
        public IEnumerable<VoteDto> Get()
        {
            List<Vote> model = new List<Vote>();

            try
            {
                using (dao_votesdb_context db = new dao_votesdb_context())
                {
                    model = db.Votes.ToList();
                }
            }
            catch (Exception ex)
            {
                model = new List<Vote>();
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return _mapper.Map<List<Vote>, List<VoteDto>>(model).ToArray();
        }
       
        [Route("GetId")]
        [HttpGet]
        public VoteDto GetId(int id)
        {
            Vote model = new Vote();

            try
            {
                using (dao_votesdb_context db = new dao_votesdb_context())
                {
                    model = db.Votes.Find(id);
                }
            }
            catch (Exception ex)
            {
                model = new Vote();
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return _mapper.Map<Vote, VoteDto>(model);
        }

        [Route("Post")]
        [HttpPost]
        public VoteDto Post([FromBody] VoteDto model)
        {
            try
            {
                Vote item = _mapper.Map<VoteDto, Vote>(model);
                using (dao_votesdb_context db = new dao_votesdb_context())
                {
                    db.Votes.Add(item);
                    db.SaveChanges();
                }
                return _mapper.Map<Vote, VoteDto>(item);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return new VoteDto();
            }
        }
     
        [Route("PostMultiple")]
        [HttpPost]
        public List<VoteDto> PostMultiple([FromBody] List<VoteDto> model)
        {
            try
            {
                List<Vote> item = _mapper.Map<List<VoteDto>, List<Vote>>(model);
                using (dao_votesdb_context db = new dao_votesdb_context())
                {
                    db.Votes.AddRange(item);
                    db.SaveChanges();
                }
                return _mapper.Map<List<Vote>, List<VoteDto>>(item);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return new List<VoteDto>();
            }
        }
      
        [Route("Delete")]
        [HttpDelete]
        public bool Delete(int? ID)
        {
            try
            {
                using (dao_votesdb_context db = new dao_votesdb_context())
                {
                    Vote item = db.Votes.FirstOrDefault(s => s.VoteId == ID);
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
        public VoteDto Update([FromBody] VoteDto model)
        {
            try
            {
                Vote item = _mapper.Map<VoteDto, Vote>(model);
                using (dao_votesdb_context db = new dao_votesdb_context())
                {
                    db.Entry(item).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    db.SaveChanges();
                }
                return _mapper.Map<Vote, VoteDto>(item);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return new VoteDto();
            }
        }
      
        [Route("GetPaged")]
        [HttpGet]
        public PaginationEntity<VoteDto> GetPaged(int page = 1, int pageCount = 30)
        {
            PaginationEntity<VoteDto> res = new PaginationEntity<VoteDto>();

            try
            {
                using (dao_votesdb_context db = new dao_votesdb_context())
                {

                    IPagedList<VoteDto> lst = AutoMapperBase.ToMappedPagedList<Vote, VoteDto>(db.Votes.OrderByDescending(x => x.VoteId).ToPagedList(page, pageCount));

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
