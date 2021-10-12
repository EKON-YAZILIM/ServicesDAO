using DAO_WebPortal.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DAO_WebPortal.Controllers
{
    public class HomeController : Controller
    {
        #region Views
        [Route("Home")]
        public IActionResult Index()
        {
            return View();
        }
        [Route("Jobs")]
        public IActionResult Jobs()
        {
            return View();
        }
        [Route("New-Job")]
        public IActionResult New_Job()
        {
            return View();
        }
        [Route("Forum")]
        public IActionResult Forum()
        {
            return View();
        }
        [Route("Forum-Detail")]
        public IActionResult Forum_Detail()
        {
            return View();
        }
        [Route("Auctions")]
        public IActionResult Auctions()
        {
            return View();
        }
        [Route("Auction-Detail")]
        public IActionResult Auction_Detail()
        {
            return View();
        }
        [Route("Voting")]
        public IActionResult Voting()
        {
            return View();
        }
        [Route("Voting-Detail")]
        public IActionResult Voting_Detail()
        {
            return View();
        }
        [Route("User-Profile")]
        public IActionResult User_Profile()
        {
            return View();
        }
        [Route("Reputation-History")]
        public IActionResult Reputation_History()
        {
            return View();
        }
        [Route("Payments-History")]
        public IActionResult Payments_History()
        {
            return View();
        }
        [Route("Contact-Help")]
        public IActionResult Contact_Help()
        {
            return View();
        }

        #endregion

        #region UserSerttings
        [HttpGet]
        public JsonResult SetCookie(string src)
        {
            CookieOptions cookies = new CookieOptions();
            cookies.Expires = DateTime.Now.AddDays(90);
            Response.Cookies.Append("theme", src, cookies);
            return Json("");
        }
        #endregion
    }
}
