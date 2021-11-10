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
    public class UserCommentVoteController : Controller
    {
        [Route("Get")]
        [HttpGet]
        public IEnumerable<UserCommentVoteDto> Get()
        {
            List<UserCommentVote> model = new List<UserCommentVote>();

            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    model = db.UserCommentVotes.ToList();
                }
            }
            catch (Exception ex)
            {
                model = new List<UserCommentVote>();
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return _mapper.Map<List<UserCommentVote>, List<UserCommentVoteDto>>(model).ToArray();
        }

        [Route("GetId")]
        [HttpGet]
        public UserCommentVoteDto GetId(int id)
        {
            UserCommentVote model = new UserCommentVote();

            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    model = db.UserCommentVotes.Find(id);
                }
            }
            catch (Exception ex)
            {
                model = new UserCommentVote();
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return _mapper.Map<UserCommentVote, UserCommentVoteDto>(model);
        }

        [Route("Post")]
        [HttpPost]
        public UserCommentVoteDto Post([FromBody] UserCommentVoteDto model)
        {
            try
            {
                UserCommentVote item = _mapper.Map<UserCommentVoteDto, UserCommentVote>(model);
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    db.UserCommentVotes.Add(item);
                    db.SaveChanges();
                }
                return _mapper.Map<UserCommentVote, UserCommentVoteDto>(item);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return new UserCommentVoteDto();
            }
        }

        [Route("PostMultiple")]
        [HttpPost]
        public List<UserCommentVoteDto> PostMultiple([FromBody] List<UserCommentVoteDto> model)
        {
            try
            {
                List<UserCommentVote> item = _mapper.Map<List<UserCommentVoteDto>, List<UserCommentVote>>(model);
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    db.UserCommentVotes.AddRange(item);
                    db.SaveChanges();
                }
                return _mapper.Map<List<UserCommentVote>, List<UserCommentVoteDto>>(item);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return new List<UserCommentVoteDto>();
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
                    UserCommentVote item = db.UserCommentVotes.FirstOrDefault(s => s.UserCommentVoteID == ID);
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

        [Route("DeleteMultiple")]
        [HttpDelete]
        public bool DeleteMultiple(int? ID)
        {
            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    List<UserCommentVote> item = db.UserCommentVotes.Where(s => s.JobPostCommentID == ID).ToList();
                    db.UserCommentVotes.RemoveRange(item);                 
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
        public UserCommentVoteDto Update([FromBody] UserCommentVoteDto model)
        {
            try
            {
                UserCommentVote item = _mapper.Map<UserCommentVoteDto, UserCommentVote>(model);
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    db.Entry(item).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    db.SaveChanges();
                }
                return _mapper.Map<UserCommentVote, UserCommentVoteDto>(item);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return new UserCommentVoteDto();
            }
        }

        [Route("GetPaged")]
        [HttpGet]
        public PaginationEntity<UserCommentVoteDto> GetPaged(int page = 1, int pageCount = 30)
        {
            PaginationEntity<UserCommentVoteDto> res = new PaginationEntity<UserCommentVoteDto>();

            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {

                    IPagedList<UserCommentVoteDto> lst = AutoMapperBase.ToMappedPagedList<UserCommentVote, UserCommentVoteDto>(db.UserCommentVotes.OrderByDescending(x => x.UserCommentVoteID).ToPagedList(page, pageCount));

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

        [Route("GetByUserId")]
        [HttpGet]
        public List<UserCommentVoteDto> GetByUserId(int UserId)
        {
            List<UserCommentVote> model = new List<UserCommentVote>();

            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    model = db.UserCommentVotes.Where(x => x.UserId == UserId).ToList();
                }
            }
            catch (Exception ex)
            {
                model = new List<UserCommentVote>();
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return _mapper.Map<List<UserCommentVote>, List<UserCommentVoteDto>>(model);
        }

        [Route("GetByCommentId")]
        [HttpGet]
        public List<UserCommentVoteDto> GetByCommentId(int CommentId)
        {
            List<UserCommentVote> model = new List<UserCommentVote>();

            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    model = db.UserCommentVotes.Where(x => x.JobPostCommentID == CommentId).ToList();
                }
            }
            catch (Exception ex)
            {
                model = new List<UserCommentVote>();
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return _mapper.Map<List<UserCommentVote>, List<UserCommentVoteDto>>(model);
        }


    }
}

