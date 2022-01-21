using Helpers.Models.DtoModels.VoteDbDto;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Helpers.Constants.Enums;
using static DAO_DbService.Mapping.AutoMapperBase;
using PagedList.Core;
using DAO_DbService.Mapping;
using Helpers.Models.SharedModels;
using DAO_DbService.Models;
using Helpers.Models.DtoModels.MainDbDto;
using DAO_DbService.Contexts;

namespace DAO_DbService.Controllers
{
    /// <summary>
    ///  AuctionBidController contains auction bid operation methods
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class AuctionBidController : Controller
    {
        /// <summary>
        /// Get Auction Bid List
        /// </summary>
        /// <returns></returns>
        [Route("Get")]
        [HttpGet]
        public IEnumerable<AuctionBidDto> Get()
        {
            List<AuctionBid> model = new List<AuctionBid>();

            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    model = db.AuctionBids.ToList();
                }
            }
            catch (Exception ex)
            {
                model = new List<AuctionBid>();
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return _mapper.Map<List<AuctionBid>, List<AuctionBidDto>>(model).ToArray();
        }

        /// <summary>
        /// Get Auction bid by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("GetId")]
        [HttpGet]
        public AuctionBidDto GetId(int id)
        {
            AuctionBid model = new AuctionBid();

            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    model = db.AuctionBids.Find(id);
                }
            }
            catch (Exception ex)
            {
                model = new AuctionBid();
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return _mapper.Map<AuctionBid, AuctionBidDto>(model);
        }

        /// <summary>
        /// Saves Auction bid model using post method
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("Post")]
        [HttpPost]
        public AuctionBidDto Post([FromBody] AuctionBidDto model)
        {
            try
            {
                AuctionBid item = _mapper.Map<AuctionBidDto, AuctionBid>(model);
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    db.AuctionBids.Add(item);
                    db.SaveChanges();
                }
                return _mapper.Map<AuctionBid, AuctionBidDto>(item);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return new AuctionBidDto();
            }
        }

        /// <summary>
        ///  Saves Auction bid list using post method
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("PostMultiple")]
        [HttpPost]
        public List<AuctionBidDto> PostMultiple([FromBody] List<AuctionBidDto> model)
        {
            try
            {
                List<AuctionBid> item = _mapper.Map<List<AuctionBidDto>, List<AuctionBid>>(model);
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    db.AuctionBids.AddRange(item);
                    db.SaveChanges();
                }
                return _mapper.Map<List<AuctionBid>, List<AuctionBidDto>>(item);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return new List<AuctionBidDto>();
            }
        }

        /// <summary>
        /// Removes auction bid by id
        /// Ends auction bid
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
                    AuctionBid item = db.AuctionBids.FirstOrDefault(s => s.AuctionBidID == ID);
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
        ///  Update auction bid
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("Update")]
        [HttpPut]
        public AuctionBidDto Update([FromBody] AuctionBidDto model)
        {
            try
            {
                AuctionBid item = _mapper.Map<AuctionBidDto, AuctionBid>(model);
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    db.Entry(item).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    db.SaveChanges();
                }
                return _mapper.Map<AuctionBid, AuctionBidDto>(item);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return new AuctionBidDto();
            }
        }

        /// <summary>
        /// Brings up the auction bid pages.
        /// The selected page is fetched. Not all pages are returned
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageCount"></param>
        /// <returns></returns>
        [Route("GetPaged")]
        [HttpGet]
        public PaginationEntity<AuctionBidDto> GetPaged(int page = 1, int pageCount = 30)
        {
            PaginationEntity<AuctionBidDto> res = new PaginationEntity<AuctionBidDto>();

            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {

                    IPagedList<AuctionBidDto> lst = AutoMapperBase.ToMappedPagedList<AuctionBid, AuctionBidDto>(db.AuctionBids.OrderByDescending(x => x.AuctionBidID).ToPagedList(page, pageCount));

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
        /// Get auction bid by auctionid
        /// </summary>
        /// <param name="auctionid"></param>
        /// <returns></returns>
        [Route("GetByAuctionId")]
        [HttpGet]
        public List<AuctionBidDto> GetByAuctionId(int auctionid)
        {
            List<AuctionBid> model = new List<AuctionBid>();

            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    model = db.AuctionBids.Where(x=>x.AuctionID == auctionid).ToList();
                }
            }
            catch (Exception ex)
            {
                model = new List<AuctionBid>();
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return _mapper.Map<List<AuctionBid>, List<AuctionBidDto>>(model).ToList();
        }

        /// <summary>
        /// Removes auction bid by auctionId
        /// Ends active sessions
        /// </summary>
        /// <param name="auctionId"></param>
        /// <returns></returns>
        [Route("DeleteByAuctionID")]
        [HttpDelete]
        public bool DeleteByAuctionID(int auctionId)
        {
            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    var bids = db.AuctionBids.Where(x => x.AuctionID == auctionId).ToList();
                    db.AuctionBids.RemoveRange(bids);
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
