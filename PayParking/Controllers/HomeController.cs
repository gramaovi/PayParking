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
        private ParkingdbContext pdb = new ParkingdbContext();
        [Authorize]
        public ActionResult Index()
        {
            using (DatabaseEntities2 de = new DatabaseEntities2())
            {

                return View(de.Parkings.ToList());
            }
        }


    }
}