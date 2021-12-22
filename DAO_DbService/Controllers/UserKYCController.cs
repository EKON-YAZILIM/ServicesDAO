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
using DAO_DbService.Mapping;
using Helpers.Models.SharedModels;
using PagedList.Core;
using Helpers.Models.KYCModels;

namespace DAO_DbService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserKYCController : Controller
    {
        [Route("Get")]
        [HttpGet]
        public IEnumerable<UserKYCDto> Get()
        {
            List<UserKYC> model = new List<UserKYC>();

            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    model = db.UserKYCs.ToList();
                }
            }
            catch (Exception ex)
            {
                model = new List<UserKYC>();
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return _mapper.Map<List<UserKYC>, List<UserKYCDto>>(model).ToArray();
        }

        [Route("UserKYCSearch")]
        [HttpGet]
        public IEnumerable<UserKYCDto> UserKYCSearch(string query)
        {
            List<UserKYC> res = new List<UserKYC>();

            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    res = db.UserKYCs.Where(x => x.UserType.Contains(query)).ToList();
                }

            }
            catch (Exception ex)
            {
                res = new List<UserKYC>();
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }
            return _mapper.Map<List<UserKYC>, List<UserKYCDto>>(res).ToArray();
        }

        [Route("GetId")]
        [HttpGet]
        public UserKYCDto GetId(int id)
        {
            UserKYC model = new UserKYC();

            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    model = db.UserKYCs.Find(id);
                }
            }
            catch (Exception ex)
            {
                model = new UserKYC();
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return _mapper.Map<UserKYC, UserKYCDto>(model);
        }

        [Route("GetUserId")]
        [HttpGet]
        public UserKYCDto GetUserId(int id)
        {
            UserKYC model = new UserKYC();

            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    model = db.UserKYCs.FirstOrDefault(x => x.UserID == id);
                }
            }
            catch (Exception ex)
            {
                model = new UserKYC();
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return _mapper.Map<UserKYC, UserKYCDto>(model);
        }

        [Route("GetApplicantId")]
        [HttpGet]
        public UserKYCDto GetApplicantId(string id)
        {
            UserKYC model = new UserKYC();

            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    model = db.UserKYCs.FirstOrDefault(x => x.ApplicantId == id);
                }
            }
            catch (Exception ex)
            {
                model = new UserKYC();
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return _mapper.Map<UserKYC, UserKYCDto>(model);
        }
     

        [Route("Post")]
        [HttpPost]
        public UserKYCDto Post([FromBody] UserKYCDto model)
        {
            try
            {
                UserKYC item = _mapper.Map<UserKYCDto, UserKYC>(model);
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    db.UserKYCs.Add(item);
                    db.SaveChanges();
                }
                return _mapper.Map<UserKYC, UserKYCDto>(item);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return new UserKYCDto();
            }
        }

        [Route("PostMultiple")]
        [HttpPost]
        public List<UserKYCDto> PostMultiple([FromBody] List<UserKYCDto> model)
        {
            try
            {
                List<UserKYC> item = _mapper.Map<List<UserKYCDto>, List<UserKYC>>(model);
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    db.UserKYCs.AddRange(item);
                    db.SaveChanges();
                }
                return _mapper.Map<List<UserKYC>, List<UserKYCDto>>(item);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return new List<UserKYCDto>();
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
                    UserKYC item = db.UserKYCs.FirstOrDefault(s => s.UserKYCID == ID);
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
        public UserKYCDto Update([FromBody] UserKYCDto model)
        {
            try
            {
                UserKYC item = _mapper.Map<UserKYCDto, UserKYC>(model);
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    db.Entry(item).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    db.SaveChanges();
                }
                return _mapper.Map<UserKYC, UserKYCDto>(item);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return new UserKYCDto();
            }
        }

        [Route("GetPaged")]
        [HttpGet]
        public PaginationEntity<UserKYCDto> GetPaged(int page = 1, int pageCount = 30)
        {
            PaginationEntity<UserKYCDto> res = new PaginationEntity<UserKYCDto>();

            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {

                    IPagedList<UserKYCDto> lst = AutoMapperBase.ToMappedPagedList<UserKYC, UserKYCDto>(db.UserKYCs.OrderByDescending(x => x.UserKYCID).ToPagedList(page, pageCount));

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
        public PaginationEntity<UserKYCDto> Search(string query, int page = 1, int pageCount = 30)
        {
            PaginationEntity<UserKYCDto> res = new PaginationEntity<UserKYCDto>();

            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    IPagedList<UserKYCDto> lst = AutoMapperBase.ToMappedPagedList<UserKYC, UserKYCDto>(db.UserKYCs.Where(x => x.UserType.Contains(query)).ToPagedList(page, pageCount));

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
