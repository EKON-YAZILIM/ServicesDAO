using DAO_DbService.Contexts;
using DAO_DbService.Models;
using Helpers.Models.DtoModels.MainDbDto;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAO_DbService.Mapping;
using static DAO_DbService.Mapping.AutoMapperBase;
using Helpers.Models.SharedModels;
using PagedList.Core;
using static Helpers.Constants.Enums;

namespace DAO_DbService.Controllers
{
    /// <summary>
    ///  UsersController contains user operation methods
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class UsersController : Controller
    {
        /// <summary>
        /// Gets users list
        /// </summary>
        /// <returns></returns>
        [Route("Get")]
        [HttpGet]
        public IEnumerable<UserDto> Get()
        {
            List<User> model = new List<User>();

            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    model = db.Users.ToList();
                }
            }
            catch (Exception ex)
            {
                model = new List<User>();
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return _mapper.Map<List<User>, List<UserDto>>(model).ToArray();
        }

        /// <summary>
        /// returns a list of all users by using title or descrpition as search key
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [Route("UserSearch")]
        [HttpGet]
        public IEnumerable<UserDto> UserSearch(string query)
        {
            List<User> res = new List<User>();

            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    res = db.Users.Where(x => x.NameSurname.Contains(query) || x.Email.Contains(query) || x.UserName.Contains(query)).ToList();
                }

            }
            catch (Exception ex)
            {
                res = new List<User>();
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }
            return _mapper.Map<List<User>, List<UserDto>>(res).ToArray();
        }

        /// <summary>
        /// Get user by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("GetId")]
        [HttpGet]
        public UserDto GetId(int id)
        {
            User model = new User();

            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    model = db.Users.Find(id);
                }
            }
            catch (Exception ex)
            {
                model = new User();
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return _mapper.Map<User, UserDto>(model);
        }

        /// <summary>
        /// Saves user model using post method
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("Post")]
        [HttpPost]
        public UserDto Post([FromBody] UserDto model)
        {
            try
            {
                User item = _mapper.Map<UserDto, User>(model);
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    db.Users.Add(item);
                    db.SaveChanges();
                }
                return _mapper.Map<User, UserDto>(item);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return new UserDto();
            }
        }

        /// <summary>
        /// Saves users list using post method
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("PostMultiple")]
        [HttpPost]
        public List<UserDto> PostMultiple([FromBody] List<UserDto> model)
        {
            try
            {
                List<User> item = _mapper.Map<List<UserDto>, List<User>>(model);
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    db.Users.AddRange(item);
                    db.SaveChanges();
                }
                return _mapper.Map<List<User>, List<UserDto>>(item);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return new List<UserDto>();
            }
        }

        /// <summary>
        /// Removes users by id
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [Route("Delete")]
        [HttpDelete]
        public bool Delete(int? ID)
        {
            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    User item = db.Users.FirstOrDefault(s => s.UserId == ID);
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
        /// Updates users using put method
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("Update")]
        [HttpPut]
        public UserDto Update([FromBody] UserDto model)
        {
            try
            {
                User item = _mapper.Map<UserDto, User>(model);
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    db.Entry(item).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    db.SaveChanges();
                }
                return _mapper.Map<User, UserDto>(item);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return new UserDto();
            }
        }

        /// <summary>
        /// Brings up the users pages.
        /// The selected page is fetched. Not all pages are returned
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageCount"></param>
        /// <returns></returns>
        [Route("GetPaged")]
        [HttpGet]
        public PaginationEntity<UserDto> GetPaged(int page = 1, int pageCount = 30)
        {
            PaginationEntity<UserDto> res = new PaginationEntity<UserDto>();

            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {

                    IPagedList<UserDto> lst = AutoMapperBase.ToMappedPagedList<User, UserDto>(db.Users.OrderByDescending(x => x.UserId).ToPagedList(page, pageCount));

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

        /// <summary>
        /// returns a list containing desired amount of found users by using title or descrpition as search key
        /// </summary>
        /// <param name="query"></param>
        /// <param name="page"></param>
        /// <param name="pageCount"></param>
        /// <returns></returns>
        [Route("Search")]
        [HttpGet]
        public PaginationEntity<UserDto> Search(string query, int page = 1, int pageCount = 30)
        {
            PaginationEntity<UserDto> res = new PaginationEntity<UserDto>();

            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    IPagedList<UserDto> lst = AutoMapperBase.ToMappedPagedList<User, UserDto>(db.Users.Where(x => x.Email.Contains(query) || x.NameSurname.Contains(query) || x.UserName.Contains(query)).ToPagedList(page, pageCount));

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

        /// <summary>
        /// Get users by email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [Route("GetByEmail")]
        [HttpGet]
        public UserDto GetByEmail(string email)
        {
            UserDto model = new UserDto();
            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    model = _mapper.Map<User, UserDto>(db.Users.FirstOrDefault(x => x.Email == email));
                }
            }
            catch (Exception ex)
            {
                model = new UserDto();
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }
            return model;
        }

        /// <summary>
        /// Get users by username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [Route("GetByUsername")]
        [HttpGet]
        public UserDto GetByUsername(string username)
        {
            UserDto model = new UserDto();
            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    model = _mapper.Map<User, UserDto>(db.Users.FirstOrDefault(x => x.UserName == username));
                }
            }
            catch (Exception ex)
            {
                model = new UserDto();
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }
            return model;
        }

        /// <summary>
        /// Get all admin users
        /// </summary>
        /// <returns></returns>
        [Route("GetAdminUsers")]
        [HttpGet]
        public List<UserDto> GetAdminUsers()
        {
            List<UserDto> model = new List<UserDto>();
            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    model = _mapper.Map<List<User>, List<UserDto>>(db.Users.Where(x => x.UserType == Helpers.Constants.Enums.UserIdentityType.Admin.ToString()).ToList());
                }
            }
            catch (Exception ex)
            {
                model = new List<UserDto>();
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }
            return model;
        }

        /// <summary>
        /// Get users count by type
        /// if types are not equal Admin , Associate or VotingAssociate
        /// returns all users count
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [Route("GetCount")]
        [HttpGet]
        public int? GetCount(UserIdentityType? type)
        {
            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    if (type == UserIdentityType.Admin)
                    {
                        return db.Users.Count(x => x.UserType == UserIdentityType.Admin.ToString());
                    }
                    else if (type == UserIdentityType.Associate)
                    {
                        return db.Users.Count(x => x.UserType == UserIdentityType.Associate.ToString());
                    }
                    else if (type == UserIdentityType.VotingAssociate)
                    {
                        return db.Users.Count(x => x.UserType == UserIdentityType.VotingAssociate.ToString());
                    }
                    else
                    {
                        return db.Users.Count();
                    }
                }
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return null;
        }

        /// <summary>
        /// Get usernames by userIds
        /// </summary>
        /// <param name="userids"></param>
        /// <returns></returns>
        [Route("GetUsernamesByUserIds")]
        [HttpPost]
        public List<string> GetUsernamesByUserIds(List<int> userids)
        {
            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    var usrs = db.Users.Where(x => userids.Contains(x.UserId));
                    List<string> usernames = new List<string>();

                    foreach (var item in userids)
                    {
                        usernames.Add(usrs.First(x => x.UserId == item).UserName);
                    }

                    return usernames;
                }
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return null;
        }

        /// <summary>
        /// Get users by type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [Route("GetUsersByType")]
        [HttpGet]
        public List<UserDto> GetUsersByType(Helpers.Constants.Enums.UserIdentityType type)
        {
            List<UserDto> model = new List<UserDto>();
            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    model = _mapper.Map<List<User>, List<UserDto>>(db.Users.Where(x => x.UserType == type.ToString()).ToList());
                }
            }
            catch (Exception ex)
            {
                model = new List<UserDto>();
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }
            return model;
        }
    
        /// <summary>
        /// Get users Id by type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [Route("GetUserIdsByType")]
        [HttpGet]
        public List<int> GetUserIdsByType(Helpers.Constants.Enums.UserIdentityType type)
        {
            List<int> model = new List<int>();
            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    model = db.Users.Where(x => x.UserType == type.ToString()).Select(x=>x.UserId).ToList();
                }
            }
            catch (Exception ex)
            {
                model = new List<int>();
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }
            return model;
        }
    
    }
}
