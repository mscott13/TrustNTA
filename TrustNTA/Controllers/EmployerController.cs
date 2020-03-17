using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TrustNTA.Data_Access;
using TrustNTA.Models;


namespace TrustNTA.Controllers
{
    public class EmployerController : Controller
    {
        // GET: Employer
        public ActionResult Index()
        {
            if (Session["userId"] == null)
            {
                return RedirectToAction("login", "account");
            }
            return View();
        }

        public ActionResult JobManagement(string intent)
        {
            if (Session["userId"] == null)
            {
                return RedirectToAction("login", "account", new { utype="employer" });
            }

            JobManagementObjects jmgr = new JobManagementObjects();
            if (intent != null && intent == "create")
            {
                ViewData["create_job_immediate"] = true;
            }
            if (Session["userId"] != null)
            {
                jmgr.employerVacancies = Database.GetEmployerVacancies(Session["userId"].ToString());
                jmgr.jobLocations = Database.GetJobLocations();
                jmgr.jobs = Database.GetJobTypes();
                return View(jmgr);
            }
            else
            {
                jmgr.employerVacancies = null;
                jmgr.jobLocations = Database.GetJobLocations();
                jmgr.jobs = Database.GetJobTypes();
                return View(jmgr);
            }
        }


        [HttpGet]
        [ActionName("seekers-search")]
        public ActionResult GetSeekersByName(string q)
        {
            List<SeekerSearch> seekers = Database.GetSeekersByName(q);
            Response.StatusCode = 200;
            return Json(seekers, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ActionName("add-vacancy")]
        public ActionResult AddVcancy(EmployerVacancy vacancy) 
        {
            string userId = Session["userId"].ToString();
            vacancy.userId = userId;

            EmployerVacancy result = Database.CreateVacancy(vacancy);
            Response.StatusCode = 200;
            return Json(new {status="vacancy_created", vacancy=result }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [ActionName("restore-joblocs")]
        public ActionResult RestoreJobLocs(string jobId) 
        {
            if (jobId != null)
            {
                JobLocs_JobType jobLocs_JobType = new JobLocs_JobType
                {
                    jobLocations = Database.GetLocationsForJob(jobId),
                    jobType = Database.GetJobTypeForEmployerJob(jobId),
                    vacancy = Database.GetEmployerVacancy(jobId)
                };
                Response.StatusCode = 200;
                return Json(jobLocs_JobType, JsonRequestBehavior.AllowGet);
            }
            else 
            {
                Response.StatusCode = 403;
                return Json(new { msg = "provide a jobId"}, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ActionName("update-vacancy")]
        public ActionResult UpdateVacancy(EmployerVacancy vacancy) 
        {
            Database.UpdateJobVacancy(vacancy);
            Response.StatusCode = 200;
            return Json(new {status = "job_updated", vacancy}, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ActionName("delete-vacancy")]
        public ActionResult DeleteVacancy(string jobId) 
        {
            Database.DeleteJobVacancy(jobId);
            return Json(new { status = "job_deleted" }, JsonRequestBehavior.AllowGet);
        }
    }
}