using DAO_DbService.Contexts;
using Helpers.Constants;
using Helpers.Models.DtoModels.ReputationDbDto;
using Helpers.Models.DtoModels.VoteDbDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace DAO_DbService
{
    public class TimedEvents
    {
        /// <summary>
        /// Auction status control timer
        /// Checks the end date of the auction as follows:
        /// If internal auction end date reached without any internal bids -> Start public auction
        /// If public auction end date reached without any public bids -> Set auction and job status to Expired
        /// </summary>
        public static System.Timers.Timer auctionStatusTimer;

        /// <summary>
        ///  Job status control timer
        ///  Checks the results of voting as follows:
        ///  If formal voting ended as AGAINST -> Set job status to Failed
        ///  If formal voting ended as FOR -> Set job status to Completed
        /// </summary>
        public static System.Timers.Timer jobStatusTimer;

        /// <summary>
        ///  Starts timer controls of the application
        /// </summary>
        public static void StartTimers()
        {
            CheckAuctionStatus(null, null);
            CheckJobStatus(null, null);

            //Auction status timer
            auctionStatusTimer = new System.Timers.Timer(10000);
            auctionStatusTimer.Elapsed += CheckAuctionStatus;
            auctionStatusTimer.AutoReset = true;
            auctionStatusTimer.Enabled = true;

            //Job status timer
            //It sends request to VotinEngine so it has longer interval
            jobStatusTimer = new System.Timers.Timer(60000);
            jobStatusTimer.Elapsed += CheckJobStatus;
            jobStatusTimer.AutoReset = true;
            jobStatusTimer.Enabled = true;
        }

        /// <summary>
        ///  Checks auction status 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private static void CheckAuctionStatus(Object source, ElapsedEventArgs e)
        {
            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    //Check if auction internal bidding ended -> Start public bidding
                    var publicAuctions = db.Auctions.Where(x => x.Status == Enums.AuctionStatusTypes.InternalBidding && x.InternalAuctionEndDate < DateTime.Now && x.WinnerAuctionBidID == null).ToList();

                    foreach (var auction in publicAuctions)
                    {
                        string releaseResult = Helpers.Request.Get(Program._settings.Service_Reputation_Url + "/UserReputationStake/ReleaseStakes?referenceProcessID=" + auction.AuctionID + "&reftype=" + Enums.StakeType.Bid);

                        auction.Status = Enums.AuctionStatusTypes.PublicBidding;
                        db.Entry(auction).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                        db.SaveChanges();
                    }


                    //Check if auction public bidding ended without any winner -> Set auction status to Expired
                    var expiredAuctions = db.Auctions.Where(x => x.Status == Enums.AuctionStatusTypes.PublicBidding && x.PublicAuctionEndDate < DateTime.Now).ToList();

                    foreach (var auction in expiredAuctions)
                    {
                        //No winners selected. Auction expired. -> Set auction and job status to Expired
                        if (auction.WinnerAuctionBidID == null)
                        {
                            auction.Status = Enums.AuctionStatusTypes.Expired;
                            db.Entry(auction).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            db.SaveChanges();

                            var job = db.JobPosts.Find(auction.JobID);
                            job.Status = Enums.JobStatusTypes.Expired;
                            db.Entry(auction).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Program.monitizer.AddConsole("Exception in timer CheckAuctionStatus. Ex: " + ex.Message);
            }
        }

        /// <summary>
        ///  Checks job status 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private static void CheckJobStatus(Object source, ElapsedEventArgs e)
        {
            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    //Check completed informal votings and update job status accordingly
                    var informalVotingJobs = db.JobPosts.Where(x => x.Status == Enums.JobStatusTypes.InformalVoting).ToList();

                    //Get completed informal votes from ApiGateway
                    string informalVotingsCompletedJson = Helpers.Request.Post(Program._settings.Voting_Engine_Url + "/Voting/GetCompletedVotingsByJobIds", Helpers.Serializers.SerializeJson(informalVotingJobs.Select(x => x.JobID)));

                    //Parse result
                    List<VotingDto> completedInformalModel = Helpers.Serializers.DeserializeJson<List<VotingDto>>(informalVotingsCompletedJson);

                    foreach (var voting in completedInformalModel)
                    {
                        //Informal voting finished without quorum -> Set job status to Expired
                        if (voting.Status == Enums.VoteStatusTypes.Expired)
                        {
                            var job = db.JobPosts.Find(voting.JobID);
                            job.Status = Enums.JobStatusTypes.Expired;
                            db.SaveChanges();
                        }
                        //Informal voting completed -> Set job status according to vote result
                        else
                        {
                            var job = db.JobPosts.Find(voting.JobID);
                            job.Status = Enums.JobStatusTypes.FormalVoting;
                            db.SaveChanges();
                        }
                    }


                    //Check completed formal votings and update job status accordingly
                    var formalVotingJobs = db.JobPosts.Where(x => x.Status == Enums.JobStatusTypes.FormalVoting).ToList();

                    //Get completed formal votes from ApiGateway
                    string formalVotingsCompletedJson = Helpers.Request.Post(Program._settings.Voting_Engine_Url + "/Voting/GetCompletedVotingsByJobIds", Helpers.Serializers.SerializeJson(formalVotingJobs.Select(x => x.JobID)));

                    //Parse result
                    List<VotingDto> completedFormalModel = Helpers.Serializers.DeserializeJson<List<VotingDto>>(formalVotingsCompletedJson);

                    foreach (var voting in completedFormalModel)
                    {
                        //Formal voting finished without quorum -> Set job status to Expired
                        if (voting.Status == Enums.VoteStatusTypes.Expired)
                        {
                            var job = db.JobPosts.Find(voting.JobID);
                            job.Status = Enums.JobStatusTypes.Expired;
                            db.SaveChanges();
                        }
                        //Formal voting completed -> Set job status according to vote result
                        else if (voting.Status == Enums.VoteStatusTypes.Completed)
                        {
                            //Get reputation stakes
                            string reputationStakesJson = Helpers.Request.Get(Program._settings.Service_Reputation_Url + "/UserReputationStake/GetByProcessId?referenceProcessID=" + voting.VotingID + "&reftype=" + Enums.StakeType.For);
                            List<UserReputationStakeDto> stakeList = Helpers.Serializers.DeserializeJson<List<UserReputationStakeDto>>(reputationStakesJson);

                            //Find winning side
                            Enums.StakeType winnerSide = Enums.StakeType.For;
                            double forReps = stakeList.Where(x => x.Type == Enums.StakeType.For).Sum(x => x.Amount);
                            double againstReps = stakeList.Where(x => x.Type == Enums.StakeType.Against).Sum(x => x.Amount);
                            if (againstReps > forReps)
                            {
                                winnerSide = Enums.StakeType.Against;
                            }

                            if (winnerSide == Enums.StakeType.For)
                            {
                                //Create payment

                                var job = db.JobPosts.Find(voting.JobID);
                                job.Status = Enums.JobStatusTypes.Completed;
                                db.SaveChanges();
                            }
                            else if (winnerSide == Enums.StakeType.Against)
                            {
                                var job = db.JobPosts.Find(voting.JobID);
                                job.Status = Enums.JobStatusTypes.Failed;
                                db.SaveChanges();
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Program.monitizer.AddConsole("Exception in timer CheckJobStatus. Ex: " + ex.Message);
            }
        }
    }
}
