using DAO_RFPService.Contexts;
using DAO_RFPService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Helpers.Constants.Enums;
using Helpers.Models.DtoModels.RfpDbDto;
using static DAO_RFPService.Mapping.AutoMapperBase;
using Helpers.Models.SharedModels;
using PagedList.Core;
using DAO_RFPService.Mapping;
using Helpers.Constants;

namespace DAO_RFPService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RfpController : ControllerBase
    {
        /// <summary>
        ///  Returns list of RFPs in the database by status.
        ///  Returns all records in the database if status parameter is null or empty
        /// </summary>
        /// <param name="status">Status of the RFP</param>
        /// <returns>RFP List</returns>
        [Route("GetRfpsByStatus")]
        [HttpGet]
        public List<RfpDto> GetRfpsByStatus(string status)
        {
            List<Rfp> model = new List<Rfp>();

            try
            {
                using (dao_rfpdb_context db = new dao_rfpdb_context())
                {
                    if (!string.IsNullOrEmpty(status))
                    {
                        model = db.Rfps.Where(x => x.Status == status).ToList();
                    }
                    else
                    {
                        model = db.Rfps.ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return _mapper.Map<List<Rfp>, List<RfpDto>>(model).ToList();
        }

        /// <summary>
        ///  Returns list of RFPs in the database by status with pagination.
        ///  Returns all paginated records in the database if status parameter is null or empty.
        /// </summary>
        /// <param name="status">Status of the RFP</param>
        /// <returns>RFP List</returns>
        [Route("GetRfpsByStatusPaged")]
        [HttpGet]
        public PaginationEntity<RfpDto> GetRfpsByStatusPaged(string status, int page = 1, int pageCount = 30)
        {
            PaginationEntity<RfpDto> res = new PaginationEntity<RfpDto>();

            try
            {
                using (dao_rfpdb_context db = new dao_rfpdb_context())
                {
                    if (!string.IsNullOrEmpty(status))
                    {
                        IPagedList<RfpDto> lst = AutoMapperBase.ToMappedPagedList<Rfp, RfpDto>(db.Rfps.Where(x => x.Status == status).OrderByDescending(x => x.RfpID).ToPagedList(page, pageCount));

                        res.Items = lst;
                        res.MetaData = new PaginationMetaData() { Count = lst.Count, FirstItemOnPage = lst.FirstItemOnPage, HasNextPage = lst.HasNextPage, HasPreviousPage = lst.HasPreviousPage, IsFirstPage = lst.IsFirstPage, IsLastPage = lst.IsLastPage, LastItemOnPage = lst.LastItemOnPage, PageCount = lst.PageCount, PageNumber = lst.PageNumber, PageSize = lst.PageSize, TotalItemCount = lst.TotalItemCount };
                    }
                    else
                    {
                        IPagedList<RfpDto> lst = AutoMapperBase.ToMappedPagedList<Rfp, RfpDto>(db.Rfps.OrderByDescending(x => x.RfpID).ToPagedList(page, pageCount));

                        res.Items = lst;
                        res.MetaData = new PaginationMetaData() { Count = lst.Count, FirstItemOnPage = lst.FirstItemOnPage, HasNextPage = lst.HasNextPage, HasPreviousPage = lst.HasPreviousPage, IsFirstPage = lst.IsFirstPage, IsLastPage = lst.IsLastPage, LastItemOnPage = lst.LastItemOnPage, PageCount = lst.PageCount, PageNumber = lst.PageNumber, PageSize = lst.PageSize, TotalItemCount = lst.TotalItemCount };
                    }
                }
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return res;
        }

        /// <summary>
        ///  Returns list of RFP bids for given RFP by identity.
        /// </summary>
        /// <param name="rfpid">RFP identity (Rfps table primary key)</param>
        /// <returns>RFP Bid List</returns>
        [Route("GetRfpBidsByRfpId")]
        [HttpGet]
        public List<RfpBidDto> GetRfpBidsByRfpId(int rfpid)
        {
            List<RfpBid> model = new List<RfpBid>();

            try
            {
                using (dao_rfpdb_context db = new dao_rfpdb_context())
                {

                    model = db.RfpBids.Where(x => x.RfpID == rfpid).ToList();
                }
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return _mapper.Map<List<RfpBid>, List<RfpBidDto>>(model).ToList();
        }

        /// <summary>
        ///  Post RFP to database
        /// </summary>
        /// <param name="model">Rfp model</param>
        /// <returns></returns>
        [Route("SubmitRfpForm")]
        [HttpPost]
        public AjaxResponse SubmitRfpForm(Rfp model)
        {
            try
            {
                using (dao_rfpdb_context db = new dao_rfpdb_context())
                {
                    model.CreateDate = DateTime.Now;
                    db.Rfps.Add(model);
                    db.SaveChanges();

                    return new AjaxResponse() { Success = true, Message = "Rfp form succesfully posted.", Content = model };
                }
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return new AjaxResponse() { Success = false, Message = "An error occured while proccesing your request." };
            }
        }

        /// <summary>
        ///  Post RFP Bid to database
        /// </summary>
        /// <param name="model">RfpBid model</param>
        /// <returns></returns>
        [Route("SubmitBid")]
        [HttpPost]
        public AjaxResponse SubmitBid(RfpBid model)
        {
            try
            {
                using (dao_rfpdb_context db = new dao_rfpdb_context())
                {
                    var rfp = db.Rfps.Find(model.RfpID);

                    if (rfp == null || rfp.RfpID <= 0)
                    {
                        return new AjaxResponse() { Success = false, Message = "Invalid RfpID. Please post an existing RfpID." };
                    }

                    if (rfp.Status != Helpers.Constants.Enums.JobStatusTypes.Active.ToString())
                    {
                        return new AjaxResponse() { Success = false, Message = "Rfp status must be 'Active' in order to post bid. Current Rfp status: " + rfp.Status };
                    }

                    if(db.RfpBids.Count(x=>x.UserId == model.UserId && x.RfpID == model.RfpID) > 0)
                    {
                        return new AjaxResponse() { Success = false, Message = "Bid already exists for given UserID. Please delete your bid first." };
                    }

                    model.CreateDate = DateTime.Now;
                    db.RfpBids.Add(model);
                    db.SaveChanges();

                    return new AjaxResponse() { Success = true, Message = "Rfp bid succesfully posted.", Content = model };
                }
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return new AjaxResponse() { Success = false, Message = "An error occured while proccesing your request." };
            }
        }

        /// <summary>
        ///  Delete RFP Bid record from database
        /// </summary>
        /// <param name="rfpbidid">RfpBid identity</param>
        /// <returns></returns>
        [Route("DeleteBid")]
        [HttpDelete]
        public AjaxResponse DeleteBid([FromQuery]int RfpBidID)
        {
            try
            {
                using (dao_rfpdb_context db = new dao_rfpdb_context())
                {
                    var rfpbid = db.RfpBids.Find(RfpBidID);

                    if (rfpbid == null || rfpbid.RfpBidID <= 0)
                    {
                        return new AjaxResponse() { Success = false, Message = "Invalid RfpBidID. Please post an existing RfoBidID." };
                    }

                    db.RfpBids.Remove(rfpbid);
                    db.SaveChanges();

                    return new AjaxResponse() { Success = true, Message = "Rfp bid succesfully deleted." };
                }
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return new AjaxResponse() { Success = false, Message = "An error occured while proccesing your request." };
            }
        }

        /// <summary>
        ///  Changes the status of the given Rfp record
        /// </summary>
        /// <param name="model">Rfp model</param>
        /// <returns></returns>
        [Route("ChangeRfpStatus")]
        [HttpPut]
        public AjaxResponse ChangeRfpStatus(Rfp model)
        {
            try
            {
                using (dao_rfpdb_context db = new dao_rfpdb_context())
                {
                    var rfp = db.Rfps.Find(model.RfpID);

                    if (rfp == null || rfp.RfpID <= 0)
                    {
                        return new AjaxResponse() { Success = false, Message = "Invalid RfpID. Please post an existing RfpID." };
                    }

                    try
                    {
                        Enums.JobStatusTypes type = (Enums.JobStatusTypes)Enum.Parse(typeof(Enums.JobStatusTypes), model.Status);
                    }
                    catch
                    {
                        return new AjaxResponse() { Success = false, Message = "Invalid status. Please post a valid status. Valid status codes: Pending, Active, Waiting, Completed" };
                    }

                    rfp.Status = model.Status;
                    
                    db.Entry(rfp).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    db.SaveChanges();

                    return new AjaxResponse() { Success = true, Message = "Rfp status succesfully updated.", Content = model };
                }
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return new AjaxResponse() { Success = false, Message = "An error occured while proccesing your request." };
            }
        }

        /// <summary>
        ///  Choose winning bid from Rfp Bids with RfpBidID
        /// </summary>
        /// <param name="rfpbidid">Identity of the RfpBid</param>
        /// <returns></returns>
        [Route("ChooseWinningBid")]
        [HttpPut]
        public AjaxResponse ChooseWinningBid([FromQuery]int RfpBidID)
        {

            try
            {
                using (dao_rfpdb_context db = new dao_rfpdb_context())
                {
                    var rfpbid = db.RfpBids.Find(RfpBidID);
                    if (rfpbid == null || rfpbid.RfpID <= 0)
                    {
                        return new AjaxResponse() { Success = false, Message = "Invalid RfpBidID. Please post an existing RfpBidID." };
                    }

                    var rfp = db.Rfps.Find(rfpbid.RfpID);
                    if (rfp == null || rfp.RfpID <= 0)
                    {
                        return new AjaxResponse() { Success = false, Message = "Invalid RfpID. Please post an existing RfpID." };
                    }

                    rfp.WinnerRfpBidID = RfpBidID;
                    rfp.Status = Enums.JobStatusTypes.Completed.ToString();

                    db.Entry(rfp).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    db.SaveChanges();

                    return new AjaxResponse() { Success = true, Message = "Rfp winning bid and status succesfully updated.", Content = rfp };
                }
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return new AjaxResponse() { Success = false, Message = "An error occured while proccesing your request." };
            }
        }
    }
}
