using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace Joint.Web.Areas.Admin.Controllers
{
    public class PartialController : Controller
    {
        //[OutputCache(Duration = 3000)]
        // GET: Admin/Partial

        //[OutputCache(Duration = 60)]
        public ActionResult Sidebar()
        {
            //ViewBag.Test = DateTime.Now.ToString();
            return View("_Sidebar");
        }


        //[OutputCache(Duration = 60)]
        public ActionResult Navbar()
        {
            return View("_Navbar");
        }
    }
}