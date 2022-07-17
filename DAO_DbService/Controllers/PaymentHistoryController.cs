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
using Helpers.Models.WebsiteViewModels;
using Helpers.Constants;
using Helpers.Models.DtoModels.ReputationDbDto;
using Helpers.Models.DtoModels.VoteDbDto;

namespace DAO_DbService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PaymentHistoryController : Controller
    {

        [Route("Get")]
        [HttpGet]
        public IEnumerable<PaymentHistoryDto> Get()
        {
            List<PaymentHistory> model = new List<PaymentHistory>();

            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    model = db.PaymentHistories.ToList();
                }
            }
            catch (Exception ex)
            {
                model = new List<PaymentHistory>();
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return _mapper.Map<List<PaymentHistory>, List<PaymentHistoryDto>>(model).ToArray();
        }

        [Route("GetId")]
        [HttpGet]
        public PaymentHistoryDto GetId(int id)
        {
            PaymentHistory model = new PaymentHistory();

            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    model = db.PaymentHistories.Find(id);
                }
            }
            catch (Exception ex)
            {
                model = new PaymentHistory();
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return _mapper.Map<PaymentHistory, PaymentHistoryDto>(model);
        }

        [Route("Post")]
        [HttpPost]
        public PaymentHistoryDto Post([FromBody] PaymentHistoryDto model)
        {
            try
            {
                PaymentHistory item = _mapper.Map<PaymentHistoryDto, PaymentHistory>(model);
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    db.PaymentHistories.Add(item);
                    db.SaveChanges();
                }
                return _mapper.Map<PaymentHistory, PaymentHistoryDto>(item);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return new PaymentHistoryDto();
            }
        }

        [Route("PostMultiple")]
        [HttpPost]
        public List<PaymentHistoryDto> PostMultiple([FromBody] List<PaymentHistoryDto> model)
        {
            try
            {
                List<PaymentHistory> item = _mapper.Map<List<PaymentHistoryDto>, List<PaymentHistory>>(model);
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    db.PaymentHistories.AddRange(item);
                    db.SaveChanges();
                }
                return _mapper.Map<List<PaymentHistory>, List<PaymentHistoryDto>>(item);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return new List<PaymentHistoryDto>();
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
                    PaymentHistory item = db.PaymentHistories.FirstOrDefault(s => s.PaymentHistoryID == ID);
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
        public PaymentHistoryDto Update([FromBody] PaymentHistoryDto model)
        {
            try
            {
                PaymentHistory item = _mapper.Map<PaymentHistoryDto, PaymentHistory>(model);
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    db.Entry(item).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    db.SaveChanges();
                }
                return _mapper.Map<PaymentHistory, PaymentHistoryDto>(item);
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
                return new PaymentHistoryDto();
            }
        }

        [Route("GetPaymentHistoryByUserId")]
        [HttpGet]
        public IEnumerable<PaymentHistoryDto> GetPaymentHistoryByUserId(int userid)
        {
            List<PaymentHistory> model = new List<PaymentHistory>();

            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    model = db.PaymentHistories.Where(x => x.UserID == userid).ToList();
                }
            }
            catch (Exception ex)
            {
                model = new List<PaymentHistory>();
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return _mapper.Map<List<PaymentHistory>, List<PaymentHistoryDto>>(model).ToArray();
        }


        /// <summary>
        /// Get job and payment details for completed jobs
        /// </summary>
        /// <returns>List<PaymentExport></returns>
        [Route("ExportPaymentHistory")]
        [HttpGet]
        public List<PaymentExport> ExportPaymentHistory(int? userid)
        {

            List<PaymentExport> model = new List<PaymentExport>();


            using (dao_maindb_context db = new dao_maindb_context())
            {
                foreach (var job in db.JobPosts.Where(x => x.Status == Enums.JobStatusTypes.Completed && (userid == null || x.JobDoerUserID == userid)).ToList())
                {
                    PaymentExport item = new PaymentExport();
                    item.job = _mapper.Map<JobPost, JobPostDto>(job);
                    item.job.JobDescription = "";
                    if(db.Auctions.Count(x => x.JobID == item.job.JobID) > 0)
                    {
                        var auction = db.Auctions.First(x => x.JobID == item.job.JobID);
                        WebsiteController cont = new WebsiteController();
                        var bids = cont.GetAuctionBids(auction.AuctionID);
                        item.winnerBid = bids.First(x => x.AuctionBidID == auction.WinnerAuctionBidID);
                    }
                    item.paymentHistory = new PaymentHistoryDto();
                    if(userid != null && db.PaymentHistories.Count(x=> x.JobID == job.JobID && x.UserID == userid) > 0){
                        item.paymentHistory = _mapper.Map<PaymentHistory, PaymentHistoryDto>(db.PaymentHistories.First(x=> x.JobID == job.JobID && (userid == null || x.UserID == userid)));
                    }
                    model.Add(item);
                }

            }

            return model;
        }


        /// <summary>
        /// Get job and payment details for completed jobs of the user between two dates
        /// </summary>
        /// <returns>List<PaymentExport></returns>
        [Route("ExportPaymentHistoryByDate")]
        [HttpGet]
        public List<PaymentExport> ExportPaymentHistoryByDate(int? userid, DateTime? start, DateTime? end)
        {

            List<PaymentExport> model = new List<PaymentExport>();


            using (dao_maindb_context db = new dao_maindb_context())
            {
                var result = (from paymentHist in db.PaymentHistories
                              join job in db.JobPosts on paymentHist.JobID equals job.JobID
                              join jobDoer in db.Users on job.JobDoerUserID equals jobDoer.UserId
                              join jobPoster in db.Users on job.UserID equals jobPoster.UserId
                              join paymentUser in db.Users on paymentHist.UserID equals paymentUser.UserId
                              where
                              (userid == null || userid == paymentHist.UserID)
                              && (start == null || paymentHist.CreateDate >= start)
                              && (end == null || paymentHist.CreateDate <= end)
                              select new
                              {
                                jobPost = job,
                                payment = paymentHist,
                                jobDoer = jobDoer.UserName,
                                jobPoster = jobPoster.UserName,
                                paymentUser = paymentUser.UserName
                              }).ToList();

                
                foreach (var item in result)
                {
                    PaymentExport export = new PaymentExport();
                    export.job = _mapper.Map<JobPost, JobPostDto>(item.jobPost);
                    export.job.JobDescription = "";
                    if(db.Auctions.Count(x => x.JobID == export.job.JobID) > 0)
                    {
                        var auction = db.Auctions.First(x => x.JobID == export.job.JobID);
                        WebsiteController cont = new WebsiteController();
                        var bids = cont.GetAuctionBids(auction.AuctionID);
                        export.winnerBid = bids.First(x => x.AuctionBidID == auction.WinnerAuctionBidID);
                    }
                    export.paymentHistory = _mapper.Map<PaymentHistory, PaymentHistoryDto>(item.payment);
                    export.JobDoerUsername = item.jobDoer;
                    export.JobPosterUsername = item.jobPoster;
                    export.PaymentUsername = item.paymentUser;
                    model.Add(export);
                }

            }

            return model;
        }


        [Route("ChangeStatusMulti")]
        [HttpPost]
        public List<PaymentHistoryDto> ChangeStatusMulti(List<int> ids, Enums.PaymentType status)
        {
            List<PaymentHistoryDto> model = new List<PaymentHistoryDto>();

            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    foreach (var item in db.PaymentHistories.Where(x => ids.Contains(x.PaymentHistoryID)).ToList())
                    {
                        PaymentHistory pm = db.PaymentHistories.Find(item.PaymentHistoryID);
                        pm.Status = status;               
                        db.SaveChanges();
                        model.Add(_mapper.Map<PaymentHistory, PaymentHistoryDto>(item));
                    }

                  
                }
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError, true);
            }

            return model;
        }

    }
}
