using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TrustNTA.Models;
using TrustNTA.Data_Access;

namespace TrustNTA.Controllers
{
    public class GraduateController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Dashboard() 
        {
            
            if (Session["userId"] != null)
            {
                var userId = Session["userId"].ToString();
                return View();
            }
            else 
            {
                return View();
            }
        }
    }
}