using PayParking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registration([Bind(Exclude = "IsEmailVerified,ActivationCode")] User user)
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
                user.Password = Cryptography.Hash(user.Password);
                user.ConfirmPassword = Cryptography.Hash(user.ConfirmPassword);

                user.IsEmailVerified = false;
                
                //Save to db
                using (DatabaseEntities1 de=new DatabaseEntities1())
                {
                    de.Users.Add(user);
                    de.SaveChanges();

                }
                //Send verification code to email

                SendVerificationLinkEmail(user.Email, user.ActivationCode.ToString());
                message = "Registration successfully done. Activation link has been sent to your email :" + user.Email;
                Status = true;
            }
            else
            {
                message = "Invalid Request";
            }
            ViewBag.Message = message;
            ViewBag.Status = Status;
            return View(user);
        }

        [HttpGet]
        public ActionResult VerifyAccount(string id)
        {
            bool Status = false;
            using (DatabaseEntities1 de=new DatabaseEntities1())
            {
                de.Configuration.ValidateOnSaveEnabled = false;//avoid confirm password does not match

                var v = de.Users.Where(a => a.ActivationCode == new Guid(id)).FirstOrDefault();
                if(v!=null)
                {
                    v.IsEmailVerified = true;
                    de.SaveChanges();
                    Status = true;
                }
                else
                {
                    ViewBag.Message = "Invalid Request";
                }
                ViewBag.Status = Status;
                return View();
            }
        }
        //login

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UserLogin login,string ReturnUrl="")
        {
            string message = "";
            using (DatabaseEntities1 dc = new DatabaseEntities1())
            {
                var v = dc.Users.Where(a => a.Email == login.Email).FirstOrDefault();
                if (v != null)
                {
                    if (!v.IsEmailVerified)
                    {
                        ViewBag.Message = "Please verify your email first";
                        return View();
                    }

                    if (string.Compare(Cryptography.Hash(login.Password), v.Password) == 0)
                    {
                        int timeout = login.RememberMe ? 525600 : 20; // 525600 min = 1 year
                        var ticket = new FormsAuthenticationTicket(login.Email, login.RememberMe, timeout);
                        string encrypted = FormsAuthentication.Encrypt(ticket);
                        var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
                        cookie.Expires = DateTime.Now.AddMinutes(timeout);
                        cookie.HttpOnly = true;
                        Response.Cookies.Add(cookie);


                        if (Url.IsLocalUrl(ReturnUrl))
                        {
                            return Redirect(ReturnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                        message = "Invalid credential provided";
                    }
                }
                else
                {
                    message = "Invalid credential provided";
                }
            }
            ViewBag.Message = message;
            return View();
        }
        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "User");
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
        [NonAction]
        public void SendVerificationLinkEmail(string email,string activationCode)
        {
            var verifyUrl = "/User/VerifyAccount/" + activationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);
            var fromEmail = new MailAddress("matrix.software.recruit.test@gmail.com", "Matrix Software House");
            var toEmail = new MailAddress(email);
            var fromEmailPassword = "matrixqwerty123";
            string subject = "Your account was succesfully created";
            string body = "<br/><br/> You re Parking Pay service account was successfully created. Click on the link below to verify your account " + link + "<br/><br/><a href='" + link + "'>" + link + "</a>";
            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)

            };
            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);
        }
    }
}