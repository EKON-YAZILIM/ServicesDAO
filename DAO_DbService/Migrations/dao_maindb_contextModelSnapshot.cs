﻿// <auto-generated />
using System;
using DAO_DbService.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DAO_DbService.Migrations
{
    [DbContext(typeof(dao_maindb_context))]
    partial class dao_maindb_contextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 64)
                .HasAnnotation("ProductVersion", "5.0.10");

            modelBuilder.Entity("DAO_DbService.Models.ActiveSession", b =>
                {
                    b.Property<int>("ActiveSessionID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("LoginDate")
                        .HasColumnType("datetime");

                    b.Property<string>("Token")
                        .HasColumnType("text");

                    b.Property<int>("UserID")
                        .HasColumnType("int");

                    b.HasKey("ActiveSessionID");

                    b.ToTable("ActiveSessions");
                });

            modelBuilder.Entity("DAO_DbService.Models.Auction", b =>
                {
                    b.Property<int>("AuctionID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("InternalAuctionEndDate")
                        .HasColumnType("datetime");

                    b.Property<int?>("JobID")
                        .HasColumnType("int");

                    b.Property<int?>("JobPosterUserID")
                        .HasColumnType("int");

                    b.Property<DateTime?>("PublicAuctionEndDate")
                        .HasColumnType("datetime");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<int?>("WinnerAuctionBidID")
                        .HasColumnType("int");

                    b.HasKey("AuctionID");

                    b.ToTable("Auctions");
                });

            modelBuilder.Entity("DAO_DbService.Models.AuctionBid", b =>
                {
                    b.Property<int>("AuctionBidID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("AssociateUserNote")
                        .HasColumnType("text");

                    b.Property<int>("AuctionID")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime");

                    b.Property<string>("GithubLink")
                        .HasColumnType("text");

                    b.Property<double>("Price")
                        .HasColumnType("double");

                    b.Property<string>("Referrer")
                        .HasColumnType("text");

                    b.Property<double>("ReputationStake")
                        .HasColumnType("double");

                    b.Property<string>("ResumeLink")
                        .HasColumnType("text");

                    b.Property<string>("Time")
                        .HasColumnType("text");

                    b.Property<int>("UserID")
                        .HasColumnType("int");

                    b.Property<bool>("VaOnboarding")
                        .HasColumnType("tinyint(1)");

                    b.HasKey("AuctionBidID");

                    b.ToTable("AuctionBids");
                });

            modelBuilder.Entity("DAO_DbService.Models.JobPost", b =>
                {
                    b.Property<int>("JobID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<double>("Amount")
                        .HasColumnType("double");

                    b.Property<string>("CodeUrl")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime");

                    b.Property<bool?>("DosFeePaid")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("JobDescription")
                        .HasColumnType("text");

                    b.Property<int>("JobDoerUserID")
                        .HasColumnType("int");

                    b.Property<DateTime>("LastUpdate")
                        .HasColumnType("datetime");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("Tags")
                        .HasColumnType("text");

                    b.Property<string>("TimeFrame")
                        .HasColumnType("text");

                    b.Property<string>("Title")
                        .HasColumnType("text");

                    b.Property<int>("UserID")
                        .HasColumnType("int");

                    b.HasKey("JobID");

                    b.ToTable("JobPosts");
                });

            modelBuilder.Entity("DAO_DbService.Models.JobPostComment", b =>
                {
                    b.Property<int>("JobPostCommentID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Comment")
                        .HasColumnType("text");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime");

                    b.Property<int>("DownVote")
                        .HasColumnType("int");

                    b.Property<bool?>("IsFlagged")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool?>("IsPinned")
                        .HasColumnType("tinyint(1)");

                    b.Property<int>("JobID")
                        .HasColumnType("int");

                    b.Property<int>("SubCommentID")
                        .HasColumnType("int");

                    b.Property<int>("UpVote")
                        .HasColumnType("int");

                    b.Property<int>("UserID")
                        .HasColumnType("int");

                    b.HasKey("JobPostCommentID");

                    b.ToTable("JobPostComments");
                });

            modelBuilder.Entity("DAO_DbService.Models.PaymentHistory", b =>
                {
                    b.Property<int>("PaymentHistoryID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<double>("Amount")
                        .HasColumnType("double");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime");

                    b.Property<string>("Explanation")
                        .HasColumnType("text");

                    b.Property<string>("IBAN")
                        .HasColumnType("text");

                    b.Property<int>("JobID")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<int>("UserID")
                        .HasColumnType("int");

                    b.Property<string>("WalletAddress")
                        .HasColumnType("text");

                    b.HasKey("PaymentHistoryID");

                    b.ToTable("PaymentHistories");
                });

            modelBuilder.Entity("DAO_DbService.Models.PlatformSetting", b =>
                {
                    b.Property<int>("PlatformSettingID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("AuctionTimeType")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime");

                    b.Property<double?>("DefaultPolicingRate")
                        .HasColumnType("double");

                    b.Property<bool>("DistributePaymentWithoutVote")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("DosCurrencies")
                        .HasColumnType("text");

                    b.Property<string>("DosFees")
                        .HasColumnType("text");

                    b.Property<bool>("ForumKYCRequired")
                        .HasColumnType("tinyint(1)");

                    b.Property<double?>("GovernancePaymentRatio")
                        .HasColumnType("double");

                    b.Property<string>("GovernanceWallet")
                        .HasColumnType("text");

                    b.Property<double?>("InternalAuctionTime")
                        .HasColumnType("double");

                    b.Property<double?>("MaxPolicingRate")
                        .HasColumnType("double");

                    b.Property<double?>("MinPolicingRate")
                        .HasColumnType("double");

                    b.Property<double?>("PublicAuctionTime")
                        .HasColumnType("double");

                    b.Property<double?>("QuorumRatio")
                        .HasColumnType("double");

                    b.Property<double?>("ReputationConversionRate")
                        .HasColumnType("double");

                    b.Property<double?>("SimpleVotingTime")
                        .HasColumnType("double");

                    b.Property<string>("SimpleVotingTimeType")
                        .HasColumnType("text");

                    b.Property<int?>("UserID")
                        .HasColumnType("int");

                    b.Property<bool>("VAOnboardingSimpleVote")
                        .HasColumnType("tinyint(1)");

                    b.Property<double?>("VotingTime")
                        .HasColumnType("double");

                    b.Property<string>("VotingTimeType")
                        .HasColumnType("text");

                    b.HasKey("PlatformSettingID");

                    b.ToTable("PlatformSettings");
                });

            modelBuilder.Entity("DAO_DbService.Models.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime");

                    b.Property<DateTime>("DateBecameVA")
                        .HasColumnType("datetime");

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<int?>("FailedLoginCount")
                        .HasColumnType("int");

                    b.Property<string>("IBAN")
                        .HasColumnType("text");

                    b.Property<bool>("IsActive")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("IsBlocked")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("KYCStatus")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("NameSurname")
                        .HasColumnType("text");

                    b.Property<bool>("Newsletter")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Password")
                        .HasColumnType("text");

                    b.Property<string>("ProfileImage")
                        .HasColumnType("text");

                    b.Property<string>("UserName")
                        .HasColumnType("text");

                    b.Property<string>("UserType")
                        .HasColumnType("text");

                    b.Property<string>("WalletAddress")
                        .HasColumnType("text");

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("DAO_DbService.Models.UserCommentVote", b =>
                {
                    b.Property<int>("UserCommentVoteID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<bool>("IsUpVote")
                        .HasColumnType("tinyint(1)");

                    b.Property<int>("JobPostCommentID")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("UserCommentVoteID");

                    b.ToTable("UserCommentVotes");
                });

            modelBuilder.Entity("DAO_DbService.Models.UserKYC", b =>
                {
                    b.Property<int>("UserKYCID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("ApplicantId")
                        .HasColumnType("text");

                    b.Property<string>("Comment")
                        .HasColumnType("text");

                    b.Property<string>("DocumentId")
                        .HasColumnType("text");

                    b.Property<string>("FileId1")
                        .HasColumnType("text");

                    b.Property<string>("FileId2")
                        .HasColumnType("text");

                    b.Property<string>("KYCStatus")
                        .HasColumnType("text");

                    b.Property<int>("UserID")
                        .HasColumnType("int");

                    b.Property<string>("UserType")
                        .HasColumnType("text");

                    b.Property<string>("VerificationId")
                        .HasColumnType("text");

                    b.Property<bool>("Verified")
                        .HasColumnType("tinyint(1)");

                    b.HasKey("UserKYCID");

                    b.ToTable("UserKYCs");
                });
#pragma warning restore 612, 618
        }
    }
}
