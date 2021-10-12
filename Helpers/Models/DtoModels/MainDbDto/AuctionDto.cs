using System;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace Helpers.Models.DtoModels.MainDbDto
{
    [DataContract]
    [Serializable]
    public partial class AuctionDto
    {
        [DataMember]
        public int AuctionID { get; set; }

        [DataMember]
        public int JobID { get; set; }

        [DataMember]
        public DateTime StartDate { get; set; }

        [DataMember]
        public DateTime EndDate { get; set; }

        [DataMember]
        public DateTime CreateDate { get; set; }

        [DataMember]
        public int JobPosterUserId { get; set; }

        [DataMember]
        public int WinnerAuctionBidID { get; set; }

        [DataMember]
        public string Status { get; set; }
    }
}
