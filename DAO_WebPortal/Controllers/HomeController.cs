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
        [Route("My-Jobs")]
        public IActionResult My_Jobs()
        {
            return View();
        }
        [Route("All-Jobs")]
        public IActionResult All_Jobs()
        {
            return View();
        }

        [Route("Auctions")]
        public IActionResult Auctions()
        {
            return View();
        }
        [Route("Votes")]
        public IActionResult Votes()
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
        [Route("User-Profile")]
        public IActionResult User_Profile()
        {
            return View();
        }
        [Route("Contact-Help")]
        public IActionResult Contact_Help()
        {
            return View();
        }

        [Route("Job-Detail/{Job}")]
        public IActionResult Job_Detail(int Job)
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
