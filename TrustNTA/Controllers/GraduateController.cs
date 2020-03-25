using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TrustNTA.Models;
using TrustNTA.Data_Access;
using System.IO;

namespace TrustNTA.Controllers
{
    public class GraduateController : Controller
    {
        public ActionResult Dashboard()
        {
            if (Session["userId"] != null)
            {
                ViewBag.Title = "Job vacancies that suit your qualifications...";
                var userId = Session["userId"].ToString();
                SeekerDashboardData dashboardData = new SeekerDashboardData
                {
                    companies = Database.GetCompanies(),
                    jobLocations = Database.GetJobLocations(),
                    specificVacancies = Database.GetSpecificVacanciesForSeeker(userId),
                    jobTypes = Database.GetJobTypes(),
                    vacancyFilterData = Data_Access.Database.GetVacancyFilterData(0, 4, "", "", "", "", "", DateTime.MinValue, DateTime.MinValue),
                    jobsCreated = Database.GetSeekerJobsCreated(Session["userId"].ToString()),
                    jobTypesInterested = Database.GetSeekerJobTypeInterests(Session["userId"].ToString()),
                    locationsAvailability = Database.GetSeekerLocationsAvailability(Session["userId"].ToString())
                };
                return View(dashboardData);
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult UploadResume()
        {
            try
            {
                if (HttpContext.Request.Files.AllKeys.Any()) 
                {
                    var postedFile = HttpContext.Request.Files[0];
                    if (postedFile != null)
                    {
                        var baseDir = (HttpContext.Server.MapPath("~/Uploads/"));
                        if (!Directory.Exists(baseDir)) 
                        {
                            Directory.CreateDirectory(baseDir);
                        }

                        var extenstion = Path.GetExtension(postedFile.FileName);
                        var name = Path.GetFileNameWithoutExtension(postedFile.FileName);
                        var fileName = Utilities.Miscellaneous.GenerateResumePrefix()+"-"+name+extenstion;

                        var fileSavePath = baseDir + fileName;
                        postedFile.SaveAs(fileSavePath);
                        Response.StatusCode = 200;
                        return Json(new { status = "file_uploaded", fileName = fileName }, JsonRequestBehavior.AllowGet);
                    }
                    else 
                    {
                        Response.StatusCode = 403;
                        return Json(new { status = "file_not_uploaded" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    Response.StatusCode = 403;
                    return Json(new { status = "file_not_uploaded" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch(Exception e)
            {
                Response.StatusCode = 403;
                return Json(new { status = "file_not_uploaded" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult CreateSeekerJob(SeekerJobCreated job) 
        {
            if (job != null && Session["userId"] !=null)
            {
                job.userId = Session["userId"].ToString();
                var createdJob = Database.CreateSeekerJob(job);
                Response.StatusCode = 200;
                return Json(new { status = "created", createdJob = createdJob }, JsonRequestBehavior.AllowGet);
            }
            else 
            {
                Response.StatusCode = 403;
                return Json(new { status = "not_created" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult DeleteSeekerJob(string jobId) 
        {
            if (jobId != null)
            {
                Database.UpdateSeekerJobVisibility(jobId, false);
                Response.StatusCode = 200;
                return Json(new { status = "updated" }, JsonRequestBehavior.AllowGet);
            }
            else 
            {
                Response.StatusCode = 403;
                return Json(new { status = "not_updated" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult UpdateSeekerJob(SeekerJobCreated job)
        {
            if (job != null)
            {
                var result = Database.UpdateSeekerJob(job);
                Response.StatusCode = 200;
                return Json(new { status = "updated", updatedJob = result }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                Response.StatusCode = 403;
                return Json(new { status = "update_fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult JobManagement() 
        {
            if (Session["userId"] == null)
            {
                return RedirectToAction("login", "account", new { utype = "seeker" });
            }
            ViewBag.Title = "Create, update or view the job types you are interested in...";

            SeekerDashboardData data = new SeekerDashboardData
            {
                companies = Database.GetCompanies(),
                jobLocations = Database.GetJobLocations(),
                specificVacancies = Database.GetSpecificVacanciesForSeeker(Session["userId"].ToString()),
                jobTypes = Database.GetJobTypes(),
                vacancyFilterData = Data_Access.Database.GetVacancyFilterData(0, 4, "", "", "", "", "", DateTime.MinValue, DateTime.MinValue),
                jobsCreated = Database.GetSeekerJobsCreated(Session["userId"].ToString()),
                jobTypesInterested = Database.GetSeekerJobTypeInterests(Session["userId"].ToString()),
                locationsAvailability = Database.GetSeekerLocationsAvailability(Session["userId"].ToString())
            };
            return View(data);
        }

        [HttpGet]
        [ActionName("restore-joblocs")]
        public ActionResult RestoreJobLocs(string jobId) 
        {
            if (jobId != null)
            {
                Response.StatusCode = 200;
                return Json(new { status = "retrieved", job = Database.GetSeekerJobCreated(jobId) }, JsonRequestBehavior.AllowGet);
            }
            else 
            {
                Response.StatusCode = 403;
                return Json(new { status = "id_not_present" }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}