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

        /// <summary>
        ///  Returns list of Auctions in the database by status.
        ///  Returns all records in the database if status parameter is null or empty
        /// </summary>
        /// <param name="status">Status of the Auction</param>
        /// <returns>Auction List</returns>
        [Route("GetAuctionsByStatus")]
        [HttpGet]
        public List<Auction> GetAuctionsByStatus(AuctionStatusTypes? status)
        {
            List<Auction> model = new List<Auction>();

            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    if (status != null)
                    {
                        model = db.Auctions.Where(x => x.Status == status).ToList();
                    }
                    else
                    {
                        model = db.Auctions.ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return model;
        }

        /// <summary>
        ///  Returns list of Auctions in the database by status with pagination.
        ///  Returns all paginated records in the database if status parameter is null or empty.
        /// </summary>
        /// <param name="status">Status of the Auction</param>
        /// <returns>Auction List with pagination entity</returns>
        [Route("GetAuctionsByStatusPaged")]
        [HttpGet]
        public PagedList.Core.IPagedList<Auction> GetAuctionsByStatusPaged(AuctionStatusTypes? status, int page = 1, int pageCount = 30)
        {
            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    if (status != null)
                    {
                        IPagedList<Auction> lst = db.Auctions.Where(x => x.Status == status).OrderByDescending(x => x.AuctionID).ToPagedList(page, pageCount);
                        return lst;
                    }
                    else
                    {
                        IPagedList<Auction> lst = db.Auctions.OrderByDescending(x => x.AuctionID).ToPagedList(page, pageCount);
                        return lst;
                    }
                }
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return new PagedList<Auction>(null, 1, 1);
        }


        [Route("GetByJobId")]
        [HttpGet]
        public AuctionDto GetByJobId(int jobid)
        {
            AuctionDto res = new AuctionDto();

            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    Auction ac = db.Auctions.OrderByDescending(x=>x.AuctionID).FirstOrDefault(x => x.JobID == jobid);
                    res = _mapper.Map<Auction, AuctionDto>(ac);
                }
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return res;
        }


        [Route("SetWinnerBid")]
        [HttpGet]
        public bool SetWinnerBid(int bidId)
        {
            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    AuctionBid auctionBid = db.AuctionBids.Find(bidId);

                    Auction auction = db.Auctions.Find(auctionBid.AuctionID);
                    auction.WinnerAuctionBidID = bidId;
                    auction.Status = AuctionStatusTypes.Completed;
                    db.Entry(auction).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    db.SaveChanges();

                    JobPost job = db.JobPosts.Find(auction.JobID);
                    job.JobDoerUserID = auctionBid.UserID;
                    db.Entry(job).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
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
