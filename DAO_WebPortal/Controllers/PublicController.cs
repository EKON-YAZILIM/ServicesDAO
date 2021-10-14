using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAO_WebPortal.Controllers
{
    public class PublicController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [Route("Privacy-Policy")]
        public IActionResult Privacy_Policy()
        {
            return View();
        }
        [Route("Contact")]
        public IActionResult Contact()
        {
            return View();
        }
        [Route("RFP")]
        public IActionResult RFP()
        {
            return View();
        }
    }
}
