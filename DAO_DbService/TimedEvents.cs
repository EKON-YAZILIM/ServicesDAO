﻿using DAO_DbService.Contexts;
using DAO_DbService.Controllers;
using DAO_DbService.Models;
using Helpers.Constants;
using Helpers.Models.DtoModels.MainDbDto;
using Helpers.Models.DtoModels.ReputationDbDto;
using Helpers.Models.DtoModels.VoteDbDto;
using Helpers.Models.NotificationModels;
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


                        //Send notification email to job poster
                        var jobPoster = db.Users.Find(auction.JobPosterUserID);

                        //Set email title and content
                        string emailTitle = "Your job is in public bidding phase.";
                        string emailContent = "Greetings, " + jobPoster.NameSurname.Split(' ')[0] + ", <br><br> Internal auction phase is finished for your job. There are no winning bids selected. <br><br> Your job will be in public bidding phase until " + Convert.ToDateTime(auction.PublicAuctionEndDate).ToString("MM.dd.yyyy HH:mm");
                        //Send email
                        SendEmailModel emailModel = new SendEmailModel() { Subject = emailTitle, Content = emailContent, To = new List<string> { jobPoster.Email } };
                        Program.rabbitMq.Publish(Helpers.Constants.FeedNames.NotificationFeed, "email", Helpers.Serializers.Serialize(emailModel));
                    }


                    //Check if auction public bidding ended without any winner -> Set auction status to Expired
                    var expiredAuctions = db.Auctions.Where(x => x.Status == Enums.AuctionStatusTypes.PublicBidding && x.PublicAuctionEndDate < DateTime.Now).ToList();

                    foreach (var auction in expiredAuctions)
                    {
                        //No winners selected. Auction expired. -> Set auction and job status to Expired
                        if (auction.WinnerAuctionBidID == null)
                        {
                            string releaseResult = Helpers.Request.Get(Program._settings.Service_Reputation_Url + "/UserReputationStake/ReleaseStakes?referenceProcessID=" + auction.AuctionID + "&reftype=" + Enums.StakeType.Mint);

                            auction.Status = Enums.AuctionStatusTypes.Expired;
                            db.Entry(auction).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            db.SaveChanges();

                            var job = db.JobPosts.Find(auction.JobID);
                            job.Status = Enums.JobStatusTypes.Expired;
                            db.Entry(auction).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            db.SaveChanges();

                            //Send notification email to job poster
                            var jobPoster = db.Users.Find(auction.JobPosterUserID);

                            //Set email title and content
                            string emailTitle = "Your job is expired.";
                            string emailContent = "Greetings, " + jobPoster.NameSurname.Split(' ')[0] + ", <br><br> Public auction phase is finished for your job. There are no winning bids selected. <br><br> Your job status is now expired and won't be listed in the active auctions anymore.";
                            //Send email
                            SendEmailModel emailModel = new SendEmailModel() { Subject = emailTitle, Content = emailContent, To = new List<string> { jobPoster.Email } };
                            Program.rabbitMq.Publish(Helpers.Constants.FeedNames.NotificationFeed, "email", Helpers.Serializers.Serialize(emailModel));
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

                            //Send notification email to job poster and job doer
                            var jobPoster = db.Users.Find(job.UserID);
                            var jobDoer = db.Users.Find(job.JobDoerUserID);

                            //Set email title and content
                            string emailTitle = "Informal voting finished without quorum for job #" + job.JobID;
                            string emailContent = "Greetings, {name}, <br><br> Informal voting process for your job finished without the quorum. <br><br> Your job status is now expired. Please contact with system admin.";

                            //Send email to job poster
                            SendNotificationEmail(emailTitle, emailContent.Replace("{name}", jobPoster.NameSurname.Split(' ')[0]), jobPoster.Email);
                            //Send email to job doer
                            SendNotificationEmail(emailTitle, emailContent.Replace("{name}", jobDoer.NameSurname.Split(' ')[0]), jobPoster.Email);
                        }
                        //Informal voting completed -> Set job status according to vote result
                        else
                        {
                            var job = db.JobPosts.Find(voting.JobID);
                            job.Status = Enums.JobStatusTypes.FormalVoting;
                            db.SaveChanges();

                            //Send notification email to job poster and job doer
                            var jobPoster = db.Users.Find(job.UserID);
                            var jobDoer = db.Users.Find(job.JobDoerUserID);

                            //Set email title and content
                            string emailTitle = "Formal voting started for your job #" + job.JobID;
                            string emailContent = "Greetings, {name}, <br><br> Informal voting process for your job completed successfully. <br><br> Your job is now on formal voting.";

                            //Send email to job poster
                            SendNotificationEmail(emailTitle, emailContent.Replace("{name}", jobPoster.NameSurname.Split(' ')[0]), jobPoster.Email);
                            //Send email to job doer
                            SendNotificationEmail(emailTitle, emailContent.Replace("{name}", jobDoer.NameSurname.Split(' ')[0]), jobPoster.Email);
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
                            //Find winning side
                            if (voting.StakedFor > voting.StakedAgainst)
                            {
                                //Create payment
                                var auction = db.Auctions.First(x => x.JobID == voting.JobID);
                                var auctionWinnerBid = db.AuctionBids.First(x => x.AuctionBidID == auction.WinnerAuctionBidID);
                                var user = db.Users.Find(auctionWinnerBid.UserID);
                                var job = db.JobPosts.Find(auction.JobID);

                                //Create Payment History model
                                PaymentHistory model = new PaymentHistory
                                {
                                    JobID = job.JobID,
                                    Amount = job.Amount,
                                    CreateDate = DateTime.Now,
                                    IBAN = user.IBAN,
                                    UserID = user.UserId,
                                    WalletAddress = user.WalletAddress,
                                };
                                db.PaymentHistories.Add(model);
                                job.Status = Enums.JobStatusTypes.Completed;
                                db.SaveChanges();


                                //Send notification email to job poster and job doer
                                var jobPoster = db.Users.Find(job.UserID);
                                var jobDoer = db.Users.Find(job.JobDoerUserID);

                                //Set email title and content
                                string emailTitle = "Formal voting finished successfully for job #" + job.JobID;
                                string emailContent = "Greetings, {name}, <br><br> Congratulations, your job passed the formal voting and job is completed successfully. <br><br> Payment for the job will be visible on the 'Payment History' for job doer.";

                                //Send email to job poster
                                SendNotificationEmail(emailTitle, emailContent.Replace("{name}", jobPoster.NameSurname.Split(' ')[0]), jobPoster.Email);
                                //Send email to job doer
                                SendNotificationEmail(emailTitle, emailContent.Replace("{name}", jobDoer.NameSurname.Split(' ')[0]), jobPoster.Email);
                            }
                            else
                            {
                                var job = db.JobPosts.Find(voting.JobID);
                                job.Status = Enums.JobStatusTypes.Failed;
                                db.SaveChanges();

                                //Send notification email to job poster and job doer
                                var jobPoster = db.Users.Find(job.UserID);
                                var jobDoer = db.Users.Find(job.JobDoerUserID);

                                //Set email title and content
                                string emailTitle = "Formal voting finished AGAINST for job #" + job.JobID;
                                string emailContent = "Greetings, {name}, <br><br> We are sorry to give you the bad news. <br><br> Your job failed to pass formal voting. Job amount will be refunded to job poster.";

                                //Send email to job poster
                                SendNotificationEmail(emailTitle, emailContent.Replace("{name}", jobPoster.NameSurname.Split(' ')[0]), jobPoster.Email);
                                //Send email to job doer
                                SendNotificationEmail(emailTitle, emailContent.Replace("{name}", jobDoer.NameSurname.Split(' ')[0]), jobPoster.Email);
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

        /// <summary>
        ///  Helper function for sending notification emails
        /// </summary>
        public static void SendNotificationEmail(string title, string content, string email)
        {
            SendEmailModel emailModel = new SendEmailModel() { Subject = title, Content = content, To = new List<string> { email } };
            Program.rabbitMq.Publish(Helpers.Constants.FeedNames.NotificationFeed, "email", Helpers.Serializers.Serialize(emailModel));
        }
    }
}
