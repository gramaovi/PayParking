using PayParking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PayParking.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetParkings()
        {
            using (DatabaseEntities2 de = new DatabaseEntities2())
            {

                return View(de.Parkings.ToList());
            }
        }


    }
}