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
using PagedList.Core;
using DAO_DbService.Mapping;

namespace DAO_DbService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuctionController : Controller
    {
        [Route("Get")]
        [HttpGet]
        public IEnumerable<AuctionDto> Get()
        {
            List<Auction> model = new List<Auction>();

            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    model = db.Auctions.ToList();
                }
            }
            catch (Exception ex)
            {
                model = new List<Auction>();
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return _mapper.Map<List<Auction>, List<AuctionDto>>(model).ToArray();
        }
      
        [Route("GetId")]
        [HttpGet]
        public AuctionDto GetId(int id)
        {
            Auction model = new Auction();

            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    model = db.Auctions.Find(id);
                }
            }
            catch (Exception ex)
            {
                model = new Auction();
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return _mapper.Map<Auction, AuctionDto>(model);
        }

        [Route("Post")]
        [HttpPost]
        public AuctionDto Post([FromBody] AuctionDto model)
        {
            try
            {
                Auction item = _mapper.Map<AuctionDto, Auction>(model);
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    db.Auctions.Add(item);
                    db.SaveChanges();
                }
                return _mapper.Map<Auction, AuctionDto>(item);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return new AuctionDto();
            }
        }

        [Route("PostMultiple")]
        [HttpPost]
        public List<AuctionDto> PostMultiple([FromBody] List<AuctionDto> model)
        {
            try
            {
                List<Auction> item = _mapper.Map<List<AuctionDto>, List<Auction>>(model);
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    db.Auctions.AddRange(item);
                    db.SaveChanges();
                }
                return _mapper.Map<List<Auction>, List<AuctionDto>>(item);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return new List<AuctionDto>();
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
                    Auction item = db.Auctions.FirstOrDefault(s => s.AuctionID == ID);
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
        public AuctionDto Update([FromBody] AuctionDto model)
        {
            try
            {
                Auction item = _mapper.Map<AuctionDto, Auction>(model);
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    db.Entry(item).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    db.SaveChanges();
                }
                return _mapper.Map<Auction, AuctionDto>(item);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return new AuctionDto();
            }
        }

        [Route("GetPaged")]
        [HttpGet]
        public PaginationEntity<AuctionDto> GetPaged(int page = 1, int pageCount = 30)
        {
            PaginationEntity<AuctionDto> res = new PaginationEntity<AuctionDto>();

            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {

                    IPagedList<AuctionDto> lst = AutoMapperBase.ToMappedPagedList<Auction, AuctionDto>(db.Auctions.OrderByDescending(x => x.AuctionID).ToPagedList(page, pageCount));

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
