using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TrustNTA.Models;
using TrustNTA.Data_Access;

namespace TrustNTA.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Register() 
        {
            return View();
        }

        [HttpPost]
        public ViewResult CreateEmployerAccount(EmployerAccount account) 
        {
            if (!Database.CheckEmployerUserExist(account.username))
            {
                Database.NewEmployer(account);
            }
            return null;
        }

        [HttpPost]
        public ViewResult ActivateGraduateAccount(GraduateAccount account) 
        {
            return null;
        }
    }
}