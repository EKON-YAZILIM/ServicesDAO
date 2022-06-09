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
using static Helpers.Constants.Enums;

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
                        //string releaseResult = Helpers.Request.Get(Program._settings.Service_Reputation_Url + "/UserReputationStake/ReleaseStakes?referenceProcessID=" + auction.AuctionID + "&reftype=" + Enums.StakeType.Bid);

                        auction.Status = Enums.AuctionStatusTypes.PublicBidding;
                        db.Entry(auction).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                        db.SaveChanges();

                        var job = db.JobPosts.Find(auction.JobID);
                        job.Status = Enums.JobStatusTypes.PublicAuction;
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
                            string releaseResult = Helpers.Request.Get(Program._settings.Service_Reputation_Url + "/UserReputationStake/ReleaseStakes?referenceProcessID=" + auction.AuctionID + "&reftype=" + Enums.StakeType.Bid);

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
            //Check completed informal votings and update job status accordingly
            CheckCompletedInformalVotings();

            //Check completed formal votings and update job status accordingly
            CheckCompletedFormalVotings();

            //Check job doer does not provided job evidence and started informal voting within expected time frame
            CheckJobFail();
        }

        /// <summary>
        ///  Checks completed informal votings and updates job status accordingly
        /// </summary>
        private static void CheckCompletedInformalVotings()
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
                    List<VotingDto> completedInformalModel = Helpers.Serializers.DeserializeJson<List<VotingDto>>(informalVotingsCompletedJson).Where(x => x.IsFormal == false).ToList();

                    foreach (var voting in completedInformalModel)
                    {
                        try
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
                                SendNotificationEmail(emailTitle, emailContent.Replace("{name}", jobDoer.NameSurname.Split(' ')[0]), jobDoer.Email);
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
                                SendNotificationEmail(emailTitle, emailContent.Replace("{name}", jobDoer.NameSurname.Split(' ')[0]), jobDoer.Email);
                            }
                        }
                        catch (Exception ex)
                        {
                            Program.monitizer.AddConsole("Exception in timer CheckCompletedInformalVotings. Ex: " + ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Program.monitizer.AddConsole("Exception in timer CheckCompletedInformalVotings. Ex: " + ex.Message);
            }
        }

        /// <summary>
        ///  Checks completed formal votings and updates job status accordingly
        /// </summary>
        private static void CheckCompletedFormalVotings()
        {
            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    //Check completed formal votings and update job status accordingly
                    var formalVotingJobs = db.JobPosts.Where(x => x.Status == Enums.JobStatusTypes.FormalVoting).ToList();

                    //Get completed formal votes from ApiGateway
                    string formalVotingsCompletedJson = Helpers.Request.Post(Program._settings.Voting_Engine_Url + "/Voting/GetCompletedVotingsByJobIds", Helpers.Serializers.SerializeJson(formalVotingJobs.Select(x => x.JobID)));

                    //Parse result
                    List<VotingDto> completedFormalModel = Helpers.Serializers.DeserializeJson<List<VotingDto>>(formalVotingsCompletedJson).Where(x => x.IsFormal == true).ToList();

                    foreach (var voting in completedFormalModel)
                    {
                        try
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
                                    var job = db.JobPosts.Find(voting.JobID);
                                    job.Status = Enums.JobStatusTypes.Completed;
                                    db.SaveChanges();

                                    //Job completion formal vote passed
                                    if (voting.Type == VoteTypes.JobCompletion)
                                    {
                                        var auction = db.Auctions.First(x => x.JobID == voting.JobID);
                                        var auctionWinnerBid = db.AuctionBids.First(x => x.AuctionBidID == auction.WinnerAuctionBidID);
                                        var user = db.Users.Find(auctionWinnerBid.UserID);

                                        bool vaOnboarded = false;

                                        //Get last DAO setting and check if new user should be onboarded as VA automatically
                                        if (db.PlatformSettings.OrderByDescending(x => x.PlatformSettingID).First().VAOnboardingSimpleVote == false)
                                        {
                                            //If job doer is Associate, change the user type to  VA
                                            if (user.UserType == Enums.UserIdentityType.Associate.ToString() && auctionWinnerBid.VaOnboarding == true)
                                            {
                                                user.DateBecameVA = DateTime.Now;
                                                user.UserType = Enums.UserIdentityType.VotingAssociate.ToString();
                                                db.SaveChanges();
                                                vaOnboarded = true;
                                            }
                                        }

                                        if (vaOnboarded || user.UserType != Enums.UserIdentityType.Associate.ToString())
                                        {
                                            CompleteJobVA(voting, job, auctionWinnerBid);
                                        }
                                        else
                                        {
                                            CompleteJobExternal(voting, job, auctionWinnerBid);
                                        }

                                        //Send notification email to job poster and job doer
                                        var jobPoster = db.Users.Find(job.UserID);
                                        var jobDoer = db.Users.Find(job.JobDoerUserID);

                                        //Set email title and content
                                        string emailTitle = "Formal voting finished successfully for job #" + job.JobID;
                                        string emailContent = "Greetings, {name}, <br><br> Congratulations, your job passed the formal voting and job is completed successfully. <br><br> Payment for the job will be visible on the 'Payment History' for job doer.";

                                        //Send email to job poster
                                        SendNotificationEmail(emailTitle, emailContent.Replace("{name}", jobPoster.NameSurname.Split(' ')[0]), jobPoster.Email);
                                        //Send email to job doer
                                        SendNotificationEmail(emailTitle, emailContent.Replace("{name}", jobDoer.NameSurname.Split(' ')[0]), jobDoer.Email);

                                        Program.monitizer.AddApplicationLog(LogTypes.ApplicationLog, "Job completed with vote #"+voting.VotingID);
                                    }
                                    //VA onboarding formal vote passed
                                    else if (voting.Type == VoteTypes.Simple && job.Title.Contains("New VA Onboarding"))
                                    {
                                        string username = job.Title.Split("(")[1].Split(")")[0];
                                        var user = db.Users.First(x => x.UserName == username);
                                        user.UserType = UserIdentityType.VotingAssociate.ToString();
                                        user.DateBecameVA = DateTime.Now;
                                        db.SaveChanges();

                                        Program.monitizer.AddApplicationLog(LogTypes.ApplicationLog, "New user became VA with vote #"+voting.VotingID);
                                    }
                                    //Governance formal vote passed
                                    else if (voting.Type == VoteTypes.Governance)
                                    {
                                        string settingsJson = job.JobDescription.Split("Variables listed below will be applied to DAO")[1];
                                        PlatformSetting settings = Helpers.Serializers.DeserializeJson<PlatformSetting>(settingsJson);
                                        if (settings != null)
                                        {
                                            settings.UserID = job.UserID;
                                            db.PlatformSettings.Add(settings);
                                            db.SaveChanges();

                                            Program.monitizer.AddApplicationLog(LogTypes.ApplicationLog, "Platform settings changed with governance vote #"+voting.VotingID);
                                        }
                                    }
                                }
                                else
                                {
                                    var job = db.JobPosts.Find(voting.JobID);
                                    job.Status = Enums.JobStatusTypes.Failed;
                                    db.SaveChanges();

                                    if (voting.Type == VoteTypes.JobCompletion)
                                    {
                                        //Send notification email to job poster and job doer
                                        var jobPoster = db.Users.Find(job.UserID);
                                        var jobDoer = db.Users.Find(job.JobDoerUserID);

                                        //Set email title and content
                                        string emailTitle = "Formal voting finished AGAINST for job #" + job.JobID;
                                        string emailContent = "Greetings, {name}, <br><br> We are sorry to give you the bad news. <br><br> Your job failed to pass formal voting. Job amount will be refunded to job poster.";

                                        //Send email to job poster
                                        SendNotificationEmail(emailTitle, emailContent.Replace("{name}", jobPoster.NameSurname.Split(' ')[0]), jobPoster.Email);
                                        //Send email to job doer
                                        SendNotificationEmail(emailTitle, emailContent.Replace("{name}", jobDoer.NameSurname.Split(' ')[0]), jobDoer.Email);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Program.monitizer.AddConsole("Exception in timer CheckCompletedFormalVotings. Ex: " + ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Program.monitizer.AddConsole("Exception in timer CheckCompletedFormalVotings. Ex: " + ex.Message);
            }
        }

        private static void CompleteJobVA(VotingDto voting, JobPost job, AuctionBid auctionWinnerBid)
        {
            using (dao_maindb_context db = new dao_maindb_context())
            {
                PlatformSettingController contr = new PlatformSettingController();
                PlatformSettingDto lastSettings = contr.GetLatestSetting();

                //Get reputation stakes from reputation service
                var reputationsJson = Helpers.Request.Get(Program._settings.Service_Reputation_Url + "/UserReputationStake/GetByProcessId?referenceProcessID=" + voting.VotingID + "&reftype=" + StakeType.For);
                var reputations = Helpers.Serializers.DeserializeJson<List<UserReputationStakeDto>>(reputationsJson);

                //Get reputations of voters who
                var participatedUsers = reputations.Where(x => x.Type == Enums.StakeType.For || x.Type == Enums.StakeType.Against).ToList();
                var allVAs =  db.Users.Where(x=>x.UserType == Enums.UserIdentityType.VotingAssociate.ToString()).Select(x=>x.UserId);

                //Add job doer to list
                participatedUsers.Add(new UserReputationStakeDto() { UserID = job.JobDoerUserID });
                var reputationsTotalJson = "";
                var reputationsTotal = new List<UserReputationHistoryDto>();

                List<int> userIds = new List<int>();
                //Create Payment History model for dao members who participated into voting
                if(lastSettings.DistributePaymentWithoutVote)
                {
                    userIds = allVAs.ToList();
                    reputationsTotalJson = Helpers.Request.Post(Program._settings.Service_Reputation_Url + "/UserReputationHistory/GetLastReputationByUserIds", Helpers.Serializers.SerializeJson(allVAs.ToList()));
                    reputationsTotal =Helpers.Serializers.DeserializeJson<List<UserReputationHistoryDto>>(reputationsTotalJson);
                }
                else
                {
                    userIds = participatedUsers.GroupBy(x => x.UserID).Select(x=>x.Key).ToList();
                    reputationsTotalJson = Helpers.Request.Post(Program._settings.Service_Reputation_Url + "/UserReputationHistory/GetLastReputationByUserIds", Helpers.Serializers.SerializeJson(participatedUsers.Select(x => x.UserID)));
                    reputationsTotal =Helpers.Serializers.DeserializeJson<List<UserReputationHistoryDto>>(reputationsTotalJson);
                }

                foreach (var userId in userIds)
                {
                    if (reputationsTotal.Count(x => x.UserID == userId) == 0) continue;

                    double usersRepPerc = reputationsTotal.FirstOrDefault(x => x.UserID == userId).LastTotal / reputationsTotal.Sum(x => x.LastTotal);
                    double memberPayment = auctionWinnerBid.Price * usersRepPerc;

                    var daouser = db.Users.Find(userId);

                    PaymentHistory paymentDaoMember = new PaymentHistory
                    {
                        JobID = job.JobID,
                        Amount = memberPayment,
                        CreateDate = DateTime.Now,
                        IBAN = daouser.IBAN,
                        UserID = daouser.UserId,
                        WalletAddress = daouser.WalletAddress,
                        Explanation = userId == auctionWinnerBid.UserID ? "User received payment for job completion." : "User received payment for DAO policing."
                    };

                    db.PaymentHistories.Add(paymentDaoMember);
                    db.SaveChanges();
                }
            }
        }

        private static void CompleteJobExternal(VotingDto voting, JobPost job, AuctionBid auctionWinnerBid)
        {
            using (dao_maindb_context db = new dao_maindb_context())
            {
                //Get reputation stakes from reputation service
                var reputationsJson = Helpers.Request.Get(Program._settings.Service_Reputation_Url + "/UserReputationStake/GetByProcessId?referenceProcessID=" + voting.VotingID + "&reftype=" + StakeType.For);
                var reputations = Helpers.Serializers.DeserializeJson<List<UserReputationStakeDto>>(reputationsJson);

                //Get reputations of voters
                var participatedUsers = reputations.Where(x => x.Type == Enums.StakeType.For || x.Type == Enums.StakeType.Against).ToList();
                //Add job doer to list
                var reputationsTotalJson = Helpers.Request.Post(Program._settings.Service_Reputation_Url + "/UserReputationHistory/GetLastReputationByUserIds", Helpers.Serializers.SerializeJson(participatedUsers.Select(x => x.UserID)));
                var reputationsTotal = Helpers.Serializers.DeserializeJson<List<UserReputationHistoryDto>>(reputationsTotalJson);

                PlatformSettingController contr = new PlatformSettingController();
                PlatformSettingDto lastSettings = contr.GetLatestSetting();
                
                List<int> userIds = new List<int>();
                //Create Payment History model for dao members who participated into voting
                if(lastSettings.DistributePaymentWithoutVote)
                {
                    userIds = db.Users.Where(x=>x.UserType == Enums.UserIdentityType.VotingAssociate.ToString()).Select(x=>x.UserId).ToList();
                }
                else
                {
                    userIds = participatedUsers.GroupBy(x => x.UserID).Select(x=>x.Key).ToList();
                }

                //Create Payment History model for dao members who participated into voting
                foreach (var userId in userIds)
                {
                    if (reputationsTotal.Count(x => x.UserID == userId) == 0) continue;

                    double usersRepPerc = reputationsTotal.FirstOrDefault(x => x.UserID == userId).LastTotal / reputationsTotal.Sum(x => x.LastTotal);
                    double memberPayment = auctionWinnerBid.Price * usersRepPerc * Convert.ToDouble(lastSettings.DefaultPolicingRate);

                    var daouser = db.Users.Find(userId);

                    PaymentHistory paymentDaoMember = new PaymentHistory
                    {
                        JobID = job.JobID,
                        Amount = memberPayment,
                        CreateDate = DateTime.Now,
                        IBAN = daouser.IBAN,
                        UserID = daouser.UserId,
                        WalletAddress = daouser.WalletAddress,
                        Explanation = "User received payment for DAO policing."
                    };

                    db.PaymentHistories.Add(paymentDaoMember);
                    db.SaveChanges();
                }


                var jobdoeruser = db.Users.Find(job.JobDoerUserID);
                //Create payment of the job doer
                PaymentHistory paymentExternalMember = new PaymentHistory
                {
                    JobID = job.JobID,
                    Amount = auctionWinnerBid.Price - (auctionWinnerBid.Price * Convert.ToDouble(lastSettings.DefaultPolicingRate)),
                    CreateDate = DateTime.Now,
                    IBAN = jobdoeruser.IBAN,
                    UserID = jobdoeruser.UserId,
                    WalletAddress = jobdoeruser.WalletAddress,
                    Explanation = "User received payment for job completion."
                };

                db.PaymentHistories.Add(paymentExternalMember);
                db.SaveChanges();
            }
        }

        /// <summary>
        ///  Checks if job doer does not provided job evidence and started informal voting within expected time frame
        /// </summary>
        private static void CheckJobFail()
        {
            try
            {
                using (dao_maindb_context db = new dao_maindb_context())
                {
                    //Get current active jobs
                    var activeJobs = db.JobPosts.Where(x => x.Status == Enums.JobStatusTypes.AuctionCompleted).ToList();

                    foreach (var job in activeJobs)
                    {
                        try
                        {
                            var auction = db.Auctions.Where(x => x.JobID == job.JobID).OrderByDescending(x => x.CreateDate).First();
                            var winnerBid = db.AuctionBids.First(x => x.AuctionBidID == auction.WinnerAuctionBidID);

                            int expectedDays = Convert.ToInt32(winnerBid.Time);

                            if (expectedDays > 0)
                            {
                                //Job doer didn't post valid evidence and started informal voting within expected time range -> Set job status to Failed
                                if (Convert.ToDateTime(auction.PublicAuctionEndDate).AddDays(expectedDays) < DateTime.Now)
                                {
                                    string releaseResult = Helpers.Request.Get(Program._settings.Service_Reputation_Url + "/UserReputationStake/ReleaseStakes?referenceProcessID=" + job.JobID + "&reftype=" + Enums.StakeType.Mint);

                                    job.Status = Enums.JobStatusTypes.Failed;
                                    db.SaveChanges();


                                    //Send notification email to job poster and job doer
                                    var jobPoster = db.Users.Find(job.UserID);
                                    var jobDoer = db.Users.Find(job.JobDoerUserID);

                                    //Set email title and content
                                    string emailTitle = "Job failed #" + job.JobID;
                                    string emailContent = "Greetings, {name}, <br><br> We are sorry to give you the bad news. <br><br> Your job is failed because job doer didn't post a valid evidence of work and started informal voting process within the expected time frame.";

                                    //Send email to job poster
                                    SendNotificationEmail(emailTitle, emailContent.Replace("{name}", jobPoster.NameSurname.Split(' ')[0]), jobPoster.Email);
                                    //Send email to job doer
                                    SendNotificationEmail(emailTitle, emailContent.Replace("{name}", jobDoer.NameSurname.Split(' ')[0]), jobDoer.Email);

                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Program.monitizer.AddConsole("Exception in timer CheckJobFail. Ex: " + ex.Message);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Program.monitizer.AddConsole("Exception in timer CheckJobFail. Ex: " + ex.Message);
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
