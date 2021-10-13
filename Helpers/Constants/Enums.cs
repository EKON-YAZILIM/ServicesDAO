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

    }
}
