using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TrustNTA.Data_Access;


namespace TrustNTA.Controllers
{
    public class EmployerController : Controller
    {
        // GET: Employer
        public ActionResult Index()
        {
             return View();
        }

        public ActionResult JobManagement()
        {
            return View(Database.GetEmployerVacancies("1"));
        }

        public ActionResult Chat()
        {
            return View();
        }
    }
}