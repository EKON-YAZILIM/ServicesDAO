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
        public List<PaymentExport> ExportPaymentHistory()
        {

            List<PaymentExport> model = new List<PaymentExport>();


            using (dao_maindb_context db = new dao_maindb_context())
            {
                foreach (var job in db.JobPosts.Where(x => x.Status == Enums.JobStatusTypes.Completed).ToList())
                {
                    PaymentExport item = new PaymentExport();
                    item.job = _mapper.Map<JobPost, JobPostDto>(job);
                    item.job.JobDescription = "";
                    var auction = db.Auctions.First(x => x.JobID == item.job.JobID);
                    WebsiteController cont = new WebsiteController();
                    var bids = cont.GetAuctionBids(auction.AuctionID);
                    item.winnerBid = bids.First(x => x.AuctionBidID == auction.WinnerAuctionBidID);
                    model.Add(item);
                }

            }

            return model;
        }

        /// <summary>
        ///  Deletes all records from payment history table and recreate records from completed jobs.
        /// </summary>
        /// <returns>New payment history records</returns>
        [Route("RecreatePaymentHistory")]
        [HttpGet]
        public List<PaymentHistoryDto> RecreatePaymentHistory()
        {

            List<PaymentHistory> model = new List<PaymentHistory>();

            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    db.PaymentHistories.RemoveRange(db.PaymentHistories);
                    db.SaveChanges();

                    List<UserReputationHistoryDto> usersHistory = new List<UserReputationHistoryDto>();

                    foreach (var job in db.JobPosts.Where(x => x.Status == Enums.JobStatusTypes.Completed).ToList())
                    {

                        //Create payment
                        var auction = db.Auctions.First(x => x.JobID == job.JobID);
                        var auctionWinnerBid = db.AuctionBids.First(x => x.AuctionBidID == auction.WinnerAuctionBidID);
                        var user = db.Users.Find(auctionWinnerBid.UserID);

                        //Get completed informal votes from ApiGateway
                        string votingsCompletedJson = Helpers.Request.Post(Program._settings.Voting_Engine_Url + "/Voting/GetCompletedVotingsByJobIds", Helpers.Serializers.SerializeJson(new List<int>() { job.JobID }));

                        //Parse result
                        List<VotingDto> completedFormalModel = Helpers.Serializers.DeserializeJson<List<VotingDto>>(votingsCompletedJson).Where(x => x.IsFormal == true).ToList();

                        //Get reputation stakes from reputation service
                        var reputationsJson = Helpers.Request.Get(Program._settings.Service_Reputation_Url + "/UserReputationStake/GetByProcessId?referenceProcessID=" + completedFormalModel[0].VotingID + "&reftype=" + StakeType.For);
                        var reputations = Helpers.Serializers.DeserializeJson<List<UserReputationStakeDto>>(reputationsJson);

                        //Get reputations of voters who voted FOR
                        var forReps = reputations.Where(x => x.Type == Enums.StakeType.For).ToList();
                        //Add job doer to list
                        forReps.Add(new UserReputationStakeDto() { UserID = job.JobDoerUserID });

                        //Find latest reputation records until vote end date
                        List<UserReputationHistoryDto> reputationsTotal = new List<UserReputationHistoryDto>();
                        foreach (var item in forReps)
                        {
                            if(usersHistory.Count(x=>x.UserID == item.UserID) == 0){
                                var userRepJson = Helpers.Request.Get(Program._settings.Service_Reputation_Url + "/UserReputationHistory/GetByUserId?userid=" + item.UserID);
                                var userReps = Helpers.Serializers.DeserializeJson<List<UserReputationHistoryDto>>(userRepJson);
                                usersHistory.AddRange(userReps);
                                usersHistory = usersHistory.OrderBy(x=>x.Date).ToList();
                            }

                            var lastHistoricalRepOfUser = usersHistory.Where(x=>x.UserID == item.UserID && x.Date <= completedFormalModel[0].EndDate).Last();
                            reputationsTotal.Add(lastHistoricalRepOfUser);
                        }

                        //Create Payment History model for dao members who participated into voting
                        foreach (var group in forReps.GroupBy(x => x.UserID))
                        {
                            if (reputationsTotal.Count(x => x.UserID == group.Key) == 0) continue;

                            double usersRepPerc = reputationsTotal.FirstOrDefault(x => x.UserID == group.Key).LastTotal / reputationsTotal.Sum(x => x.LastTotal);
                            double memberPayment = Math.Round(auctionWinnerBid.Price * usersRepPerc, 2);

                            var daouser = db.Users.Find(group.Key);

                            PaymentHistory paymentDaoMember = new PaymentHistory
                            {
                                JobID = job.JobID,
                                Amount = memberPayment,
                                CreateDate = DateTime.Now,
                                IBAN = daouser.IBAN,
                                UserID = daouser.UserId,
                                WalletAddress = daouser.WalletAddress,
                                Explanation = "User received payment from DAO policing. Job #" + job.JobID
                            };

                            model.Add(paymentDaoMember);

                            db.PaymentHistories.Add(paymentDaoMember);
                            db.SaveChanges();
                        }
                        
                    }
                }
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, Enums.LogTypes.ApplicationError, true);
                return null;
            }

            return _mapper.Map<List<PaymentHistory>, List<PaymentHistoryDto>>(model).ToList();

        }

    }
}
