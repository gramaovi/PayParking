using PayParking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PayParking.Controllers
{
    public class UserController : Controller
    {
       //register
       [HttpGet]
       public ActionResult Registration()
        {
            return View();
        }
        [HttpGet]
        [ValidateAntiForgeryToken]
        public ActionResult Registration([Bind(Exclude ="IsEmailVerified,ActivationCode")]User user)
        {
            return View(user);
        }
    }
}