namespace Helpers.Constants
{
    public class Enums
    {
        /// <summary>
        ///  Enum of application names in the project
        /// </summary>
        public enum AppNames
        {
            DAO_ApiGateway,
            DAO_DbService,
            DAO_IdentityService,
            DAO_LogService,
            DAO_NotificationService,
            DAO_ReputationService,
            DAO_VotingEngine,
            DAO_WebPortal
        }

        /// <summary>
        ///  Enum of log types in the system
        /// </summary>
        public enum LogTypes
        {
            PublicUserLog,
            UserLog,
            AdminLog,
            ApplicationLog,
            ApplicationError
        }

        /// <summary>
        ///  Enum of user log types in the system
        /// </summary>
        public enum UserLogType
        {
            Auth,
            Request,
            Agreement
        }

        /// <summary>
        ///  Enum of notification channels (Only Email is available in the current version)
        /// </summary>
        public enum NotificationTypes
        {
            Email,
            Push,
            SMS,
            Web,
            All
        }

        /// <summary>
        ///  Enum of user authorization types
        /// </summary>
        public enum UserIdentityType
        {
            Admin,
            Associate,
            VotingAssociate
        }

        /// <summary>
        ///  Enum of current progress of a job post
        /// </summary>
        public enum JobStatusTypes
        {
            AdminApprovalPending,
            DoSFeePending,
            KYCPending,
            InternalAuction,
            PublicAuction,
            InformalVoting,
            FormalVoting,            
            Completed,
            Failed,
            Expired
        }

        /// <summary>
        ///  Enum of current status of an auction
        /// </summary>
        public enum AuctionStatusTypes
        {
            AdminApproval,
            InternalBidding,
            PublicBidding,
            Completed,
            Expired
        }

        /// <summary>
        ///  Enum of current status of a voting
        /// </summary>
        public enum VoteStatusTypes
        {
            Pending,
            Active,
            Waiting,
            Completed,
            Expired
        }

        /// <summary>
        ///  Enum of type of a voting
        /// </summary>
        public enum VoteTypes
        {
            Simple,
            Governance,
            Admin,
            JobCompletion,
            JobRefund
        }


        /// <summary>
        ///  Enum of vote directions
        /// </summary>
        public enum VoteDirection
        {
            For,
            Against
        }

        /// <summary>
        ///  Enum of current status of a reputation stake
        /// </summary>
        public enum ReputationStakeStatus
        {
            Staked,
            Released
        }


        /// <summary>
        ///  Enum of current status of a reputation stake
        /// </summary>
        public enum StakeReferenceType
        {
            Auction,
            Voting
        }

    }
}
