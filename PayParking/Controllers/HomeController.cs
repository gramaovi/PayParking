using PayParking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

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
        public ActionResult ParkCar(int? id)
       
        {
            using (DatabaseEntities2 de = new DatabaseEntities2())
            {
                if (!id.HasValue)
                    return HttpNotFound();

                Parking park = de.Parkings.Find(id);
                if (null == park)
                {
                    return HttpNotFound();
                }
                return View(park);


            }
        }
        [HttpPost]
        public ActionResult ParkCar(Parking park)
        {
            string email=System.Web.HttpContext.Current.User.Identity.Name;

            using (DatabaseEntities1 de = new DatabaseEntities1())
            {
                User user = de.Users.Where(x => x.Email == email).FirstOrDefault<User>();
                using (DatabaseEntities2 de2 = new DatabaseEntities2())
                {
                    park.LastName = user.LastName;
                    park.FirstName = user.FirstName;
                    park.CheckIn = DateTime.Now;
                    park.IsFree = false;
                    park.LicencePlate = user.LicencePlate;
                    de2.Entry(park).State = System.Data.Entity.EntityState.Modified;
                    de2.SaveChanges();
                    return RedirectToAction("GetParkings");


                }
            }
 
        }

    }
}