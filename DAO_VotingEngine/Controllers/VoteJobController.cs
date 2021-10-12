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
    public class VoteJobController : Controller
    {
        [Route("Get")]
        [HttpGet]
        public IEnumerable<VoteJobDto> Get()
        {
            List<VoteJob> model = new List<VoteJob>();

            try
            {
                using (dao_votesdb_context db = new dao_votesdb_context())
                {
                    model = db.VoteJobs.ToList();
                }
            }
            catch (Exception ex)
            {
                model = new List<VoteJob>();
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return _mapper.Map<List<VoteJob>, List<VoteJobDto>>(model).ToArray();
        }
        
        [Route("GetId")]
        [HttpGet]
        public VoteJobDto GetId(int id)
        {
            VoteJob model = new VoteJob();

            try
            {
                using (dao_votesdb_context db = new dao_votesdb_context())
                {
                    model = db.VoteJobs.Find(id);
                }
            }
            catch (Exception ex)
            {
                model = new VoteJob();
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return _mapper.Map<VoteJob, VoteJobDto>(model);
        }

        [Route("Post")]
        [HttpPost]
        public VoteJobDto Post([FromBody] VoteJobDto model)
        {
            try
            {
                VoteJob item = _mapper.Map<VoteJobDto, VoteJob>(model);
                using (dao_votesdb_context db = new dao_votesdb_context())
                {
                    db.VoteJobs.Add(item);
                    db.SaveChanges();
                }
                return _mapper.Map<VoteJob, VoteJobDto>(item);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return new VoteJobDto();
            }
        }
       
        [Route("PostMultiple")]
        [HttpPost]
        public List<VoteJobDto> PostMultiple([FromBody] List<VoteJobDto> model)
        {
            try
            {
                List<VoteJob> item = _mapper.Map<List<VoteJobDto>, List<VoteJob>>(model);
                using (dao_votesdb_context db = new dao_votesdb_context())
                {
                    db.VoteJobs.AddRange(item);
                    db.SaveChanges();
                }
                return _mapper.Map<List<VoteJob>, List<VoteJobDto>>(item);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return new List<VoteJobDto>();
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
                    VoteJob item = db.VoteJobs.FirstOrDefault(s => s.VoteJobID == ID);
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
        public VoteJobDto Update([FromBody] VoteJobDto model)
        {
            try
            {
                VoteJob item = _mapper.Map<VoteJobDto, VoteJob>(model);
                using (dao_votesdb_context db = new dao_votesdb_context())
                {
                    db.Entry(item).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    db.SaveChanges();
                }
                return _mapper.Map<VoteJob, VoteJobDto>(item);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return new VoteJobDto();
            }
        }
       
        [Route("GetPaged")]
        [HttpGet]
        public PaginationEntity<VoteJobDto> GetPaged(int page = 1, int pageCount = 30)
        {
            PaginationEntity<VoteJobDto> res = new PaginationEntity<VoteJobDto>();

            try
            {
                using (dao_votesdb_context db = new dao_votesdb_context())
                {

                    IPagedList<VoteJobDto> lst = AutoMapperBase.ToMappedPagedList<VoteJob, VoteJobDto>(db.VoteJobs.OrderByDescending(x => x.VoteJobID).ToPagedList(page, pageCount));

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
