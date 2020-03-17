using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TrustNTA.Models;
using TrustNTA.Data_Access;
using TrustNTA.Utilities;

namespace TrustNTA.Controllers
{
    public class AccountController : Controller
    {
        public const string USER_TYPE_EMPLOYER = "EMPLOYER";
        public const string USER_TYPE_SEEKER = "SEEKER";

        [HttpPost]
        [ActionName("seeker-login")]
        public ActionResult SeekerLogin(Seeker seeker)
        {
            if (Database.CheckSeekerExist(seeker.identifier))
            {
                Seeker user = Database.GetSeekerViaIdentifier(seeker.identifier);
                if (user.isVerified)
                {
                    if (PasswordManagement.VerifyPassword(seeker.password, user.password))
                    {
                        SetupSeekerSession(user);
                        Response.StatusCode = 200;
                        return Json(new { status = "user_authenticated" });
                    }
                    else 
                    {
                        Response.StatusCode = 200;
                        return Json(new { status = "invalid_password" });
                    }
                }
                else 
                {
                    Response.StatusCode = 403;
                    return Json(new { status = "not_verified" });
                }
            }
            else 
            {
                Response.StatusCode = 403;
                return Json(new { status = "invalid_user" });
            }
        }

        [HttpPost]
        [ActionName("employer-login")]
        public ActionResult EmployerLogin(Employer employer)
        {
            var result = Miscellaneous.SendEmail();
            if (Database.CheckEmployerExist(employer.username))
            {
                Employer user = Database.GetEmployerViaUsername(employer.username);
                
                    if (PasswordManagement.VerifyPassword(employer.password, user.password))
                    {
                        SetupEmployerSession(user);
                        Response.StatusCode = 200;
                        return Json(new { status = "user_authenticated" });
                    }
                    else
                    {
                        Response.StatusCode = 200;
                        return Json(new { status = "invalid_password" });
                    }
            }
            else
            {
                Response.StatusCode = 403;
                return Json(new { status = "invalid_user" });
            }
        }

        [HttpPost]
        [ActionName("register-employer")]
        public ActionResult CreateEmployerAccount(EmployerAccount account) 
        {
            if (!Database.CheckEmployerExist(account.username))
            {
                account.accountType = account.accountType;
                account.firstName = account.firstName;
                account.lastName = account.lastName;
                account.companyAssociated = account.companyAssociated;
                account.address = account.address;
                account.companyOptional = account.companyOptional;
                account.emailAddress = account.emailAddress;
                account.telephone = account.telephone;
                account.username = account.username;
                Database.NewEmployer(account);

                Response.StatusCode = 200;
                return Json(new { status = "user_created" });
            }
            else 
            {
                Response.StatusCode = 403;
                return Json(new { status = "user_exist"});
            }
           
        }

        [HttpPost]
        [ActionName("add-seeker")]
        public ActionResult AddSeeker(SeekerAccount account)
        {
            if (!Database.CheckSeekerExist(account.identifier))
            {
                account.userId = account.userId;
                account.dateCreated = account.dateCreated;
                account.trn = account.trn;
                account.firstName = account.firstName;
                account.middleName = account.middleName;
                account.lastName = account.lastName;
                account.email = account.email;
                account.isGraduate = account.isGraduate;
                account.isVerified = account.isVerified;
                account.identifier = account.identifier;
                Database.NewSeeker(account);
                Response.StatusCode = 200;
                return Json(new { status = "user_created" });
            }
            else 
            {
                Response.StatusCode = 403;
                return Json(new { status = "user_exist" });
            }
        }

        [HttpPost]
        [ActionName("seeker-activation")]
        public ActionResult ActivateGraduateAccount(string identifier) 
        {
            if (Database.CheckSeekerExist(identifier)) 
            {
                Seeker user = Database.GetSeekerViaIdentifier(identifier);
                Database.NewSeekerActivationCode(user.userId, Miscellaneous.GenerateActivationCode(), DateTime.Now.AddDays(1));
                Response.StatusCode = 200;
                return Json(new { status = "code_generated" });
            }
            else
            {
                Response.StatusCode = 403;
                return Json(new { status = "invalid_user" });
            }
        }

        [HttpPost]
        [ActionName("seeker-activation-verify")]
        public ActionResult VerifySeekerActivation(string activationCode, string password)
        {
            SeekerActivation activation = Database.GetActivation(activationCode);
            if (activation != null)
            {
                if (activation.activationCode == activationCode && DateTime.Now <= activation.validUntil)
                {
                    Seeker user = Database.GetSeekerViaUID(activation.userId);
                    if (user != null)
                    {
                        Database.CompleteSeekerActivation(user.userId, activationCode, PasswordManagement.HashPassword(password));
                        Response.StatusCode = 200;
                        return Json(new { status = "activation_successful" });
                    }
                    else 
                    {
                        Response.StatusCode = 403;
                        return Json(new { status = "invalid_user" });
                    }
                }
                else 
                {
                    Response.StatusCode = 403;
                    return Json(new { status = "invalid_activation" });
                }
            }
            else 
            {
                Response.StatusCode = 403;
                return Json(new { status = "invalid_activation" });
            }
        }

        [HttpPost]
        [ActionName("add-company")]
        public ActionResult AddCompany(Company company) 
        {
            if (!Database.CheckCompanyExist(company.companyName))
            {
                string _companyId = Database.AddCompany(company);
                Response.StatusCode = 200;
                return Json(new { status = "company_added", companyId = _companyId });
            }
            else 
            {
                Response.StatusCode = 403;
                return Json(new { status = "company_exist" });
            }
        }

        private void SetupEmployerSession(Employer employer) 
        {
            Session["userId"] = employer.userId;
            Session["firstName"] = employer.firstName;
            Session["lastName"] = employer.lastName;
            Session["username"] = employer.username;
            Session["accountType"] = employer.accountType;
            Session["userType"] = USER_TYPE_EMPLOYER;
        }

        private void SetupSeekerSession(Seeker seeker) 
        {
            Session["userId"] = seeker.userId;
            Session["firstName"] = seeker.firstName;
            Session["lastName"] = seeker.lastName;
            Session["identifier"] = seeker.identifier;
            Session["userType"] = USER_TYPE_SEEKER;
        }

        public ActionResult Register(string utype)
        {
            ViewData["utype"] = utype;
            return View(Database.GetCompanies());
        }

        public ActionResult Login(string utype)
        {
            ViewData["utype"] = utype;
            return View();
        }

    }
}