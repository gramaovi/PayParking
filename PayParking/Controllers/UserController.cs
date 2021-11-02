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
            bool Status = false;
            string message = "";
            //Model validation
            if(ModelState.IsValid)
            {
                //Email already exists
                var isExist = IsEmaiExist(user.Email);
                if(isExist)
                {
                    ModelState.AddModelError("EmailExist", " Email already exist");
                    return View(user);
                }
                //Generate activation code
                user.ActivationCode = Guid.NewGuid();

                //Password hashing

            }
            else
            {
                message = "Invalid Request";
            }
            return View(user);
        }
        [NonAction]
        public bool IsEmaiExist(string email)
        {
            using (DatabaseEntities1 de=new DatabaseEntities1())
            {
                var x = de.Users.Where(a => a.Email == email).FirstOrDefault();
                return x != null;
            }
        }
    }
}