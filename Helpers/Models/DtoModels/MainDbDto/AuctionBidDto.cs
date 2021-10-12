using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Helpers.Models.DtoModels.MainDbDto
{
    [DataContract]
    [Serializable]
    public partial class AuctionBidDto
    {
        [DataMember]
        public int AuctionBidID { get; set; }

        [DataMember]
        public int AuctionID { get; set; }

        [DataMember]
        public int UserId { get; set; }

        [DataMember]
        public double Price { get; set; }

        [DataMember]
        public string Time { get; set; }

        [DataMember]
        public bool IsInternal { get; set; }

        [DataMember]
        public double ReputationStake { get; set; }
  
    }
}
