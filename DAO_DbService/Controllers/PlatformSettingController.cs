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
    public class PlatformSettingController : Controller
    {
        [Route("Get")]
        [HttpGet]
        public IEnumerable<PlatformSettingDto> Get()
        {
            List<PlatformSetting> model = new List<PlatformSetting>();

            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    model = db.PlatformSettings.ToList();
                }
            }
            catch (Exception ex)
            {
                model = new List<PlatformSetting>();
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return _mapper.Map<List<PlatformSetting>, List<PlatformSettingDto>>(model).ToArray();
        }

        [Route("GetId")]
        [HttpGet]
        public PlatformSettingDto GetId(int id)
        {
            PlatformSetting model = new PlatformSetting();

            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    model = db.PlatformSettings.Find(id);
                }
            }
            catch (Exception ex)
            {
                model = new PlatformSetting();
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return _mapper.Map<PlatformSetting, PlatformSettingDto>(model);
        }

        [Route("Post")]
        [HttpPost]
        public PlatformSettingDto Post([FromBody] PlatformSettingDto model)
        {
            try
            {
                PlatformSetting item = _mapper.Map<PlatformSettingDto, PlatformSetting>(model);
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    db.PlatformSettings.Add(item);
                    db.SaveChanges();
                }
                return _mapper.Map<PlatformSetting, PlatformSettingDto>(item);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return new PlatformSettingDto();
            }
        }

        [Route("PostMultiple")]
        [HttpPost]
        public List<PlatformSettingDto> PostMultiple([FromBody] List<PlatformSettingDto> model)
        {
            try
            {
                List<PlatformSetting> item = _mapper.Map<List<PlatformSettingDto>, List<PlatformSetting>>(model);
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    db.PlatformSettings.AddRange(item);
                    db.SaveChanges();
                }
                return _mapper.Map<List<PlatformSetting>, List<PlatformSettingDto>>(item);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return new List<PlatformSettingDto>();
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
                    PlatformSetting item = db.PlatformSettings.FirstOrDefault(s => s.PlatformSettingID == ID);
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
        public PlatformSettingDto Update([FromBody] PlatformSettingDto model)
        {
            try
            {
                PlatformSetting item = _mapper.Map<PlatformSettingDto, PlatformSetting>(model);
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    db.Entry(item).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    db.SaveChanges();
                }
                return _mapper.Map<PlatformSetting, PlatformSettingDto>(item);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return new PlatformSettingDto();
            }
        }

        [Route("GetPaged")]
        [HttpGet]
        public PaginationEntity<PlatformSettingDto> GetPaged(int page = 1, int pageCount = 30)
        {
            PaginationEntity<PlatformSettingDto> res = new PaginationEntity<PlatformSettingDto>();

            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {

                    IPagedList<PlatformSettingDto> lst = AutoMapperBase.ToMappedPagedList<PlatformSetting, PlatformSettingDto>(db.PlatformSettings.OrderByDescending(x => x.PlatformSettingID).ToPagedList(page, pageCount));

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

        [Route("GetLatestSetting")]
        [HttpGet]
        public PlatformSettingDto GetLatestSetting()
        {
            try
            {

                using (dao_maindb_context db = new dao_maindb_context())
                {
                    if (db.PlatformSettings.Count() > 0)
                    {
                        PlatformSetting result = db.PlatformSettings.OrderByDescending(x => x.PlatformSettingID).First();

                        return _mapper.Map<PlatformSetting, PlatformSettingDto>(result);
                    }
                }
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return null;
        }

    }
}
