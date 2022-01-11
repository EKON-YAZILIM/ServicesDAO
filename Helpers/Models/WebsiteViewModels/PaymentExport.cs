using System;
using System.Collections.Generic;
using System.Text;
using Helpers.Models.DtoModels.MainDbDto;
using static Helpers.Constants.Enums;

namespace Helpers.Models.WebsiteViewModels
{
    public class PaymentExport
    {
       public JobPostDto job {get;set;}
       public AuctionBidItemModel winnerBid { get; set;}

    }    
}
