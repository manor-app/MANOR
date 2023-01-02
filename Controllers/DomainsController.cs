using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using MANOFR.Models;

namespace MANOFR.Controllers
{
    public class DomainsController : Controller
    {
        //global vars
        ManofrSqlDbModelDataContext dbObj;
        AjaxReturnMSG arm;
        // GET: Domains
        public ActionResult Index()
        {
            if (Request.Cookies["userFullname"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public string GetDomainsList(string domain)
        {
            try {

                arm = new AjaxReturnMSG();
                dbObj = new ManofrSqlDbModelDataContext();

                var list = (from ls in dbObj.Domains
                            where (domain == "" ?  1==1 : ls.Domain1.Contains(domain))
                            select new { ls.ID, Domain = ls.Domain1 }).ToList();

                arm.status = 1;
                arm.successFlag = true;
                arm.successMsg = "";
                arm.returnedObject = list;

            } catch (Exception e) {
                arm.status = 0;
                arm.successFlag = false;
                arm.errorMsg = "Couldn't Get Domains List!\nError: " + e.Message.ToString();
            }

            return new JavaScriptSerializer().Serialize(arm);
        }

        [HttpPost]
        public string AddDomain(string domain)
        {
            try
            {
                arm = new AjaxReturnMSG();
                dbObj = new ManofrSqlDbModelDataContext();

                List<Domain> domainCheck = dbObj.Domains.Where(x=> x.Domain1 == domain).ToList();
                if (domainCheck.Count > 0)
                {
                    arm.status = 0;
                    arm.successFlag = false;
                    arm.errorMsg = "Couldn't Add Domain!\nError: Domain Already Exists.";

                } else {

                    Domain newDomain = new Domain();
                    newDomain.Domain1 = domain;
                    dbObj.Domains.InsertOnSubmit(newDomain);
                    dbObj.SubmitChanges();

                    arm.status = 1;
                    arm.successFlag = true;
                    arm.successMsg = "Domain Added Successfully!";

                }

            }
            catch (Exception e)
            {
                arm.status = 0;
                arm.successFlag = false;
                //arm.errorMsg = "Couldn't Add Domain!\nError: " + e.Message.ToString();
                arm.errorMsg = "Something Went wrong";
            }

            return new JavaScriptSerializer().Serialize(arm);
        }

        [HttpGet]
        public string GetDomains()
        {
            try
            {

                arm = new AjaxReturnMSG();
                dbObj = new ManofrSqlDbModelDataContext();

                var list = (from ut in dbObj.Domains
                            select new { ut.ID, Domain = ut.Domain1 }).ToList();

                arm.status = 1;
                arm.successFlag = true;
                arm.successMsg = "Domains List Was Generated Successfully!";
                arm.returnedObject = list;

            }
            catch (Exception e)
            {
                arm.status = 0;
                arm.successFlag = false;
                arm.errorMsg = "Couldn't Get Domains List!\nError: " + e.Message.ToString();
            }

            return new JavaScriptSerializer().Serialize(arm);
        }

        [HttpPost]
        public string UpdateDomain(int domainID, string newDomainText)
        {
            try {
                arm = new AjaxReturnMSG();
                dbObj = new ManofrSqlDbModelDataContext();

                Domain domainToUpdate = dbObj.Domains.Where(dom => dom.ID == domainID).SingleOrDefault();

                if (domainToUpdate != null) {

                    domainToUpdate.Domain1 = newDomainText;
                    dbObj.SubmitChanges();

                    arm.status = 1;
                    arm.successFlag = true;
                    arm.successMsg = "Domain Updated Successfully!";
                } else {

                    arm.status = 0;
                    arm.successFlag = false;
                    arm.errorMsg = "Domain Not Found In The Database!";

                }
            
            } catch (Exception e) {
                arm.status = 0;
                arm.successFlag = false;
                arm.errorMsg = "Failed Updating Domain!\nError: "+e.Message.ToString();
            }

            return new JavaScriptSerializer().Serialize(arm);
        }

        [HttpPost]
        public string DeleteDomain(int domainID)
        {
            try
            {
                arm = new AjaxReturnMSG();
                dbObj = new ManofrSqlDbModelDataContext();

                Domain domainToUpdate = dbObj.Domains.Where(dom => dom.ID == domainID).SingleOrDefault();

                if (domainToUpdate != null)
                {
                    dbObj.Domains.DeleteOnSubmit(domainToUpdate);
                    dbObj.SubmitChanges();

                    arm.status = 1;
                    arm.successFlag = true;
                    arm.successMsg = "Domain Deleted Successfully!";
                }
                else
                {

                    arm.status = 0;
                    arm.successFlag = false;
                    arm.errorMsg = "Domain Not Found In The Database!";

                }

            }
            catch (Exception e)
            {
                arm.status = 0;
                arm.successFlag = false;
                if (e.Message.Contains("conflicted with the REFERENCE constraint"))
                {
                    arm.errorMsg = "Failed Deleting Domain!\nError: It's being used by another item.";
                }
                else
                {
                    arm.errorMsg = "Failed Deleting Domain!\nError: " + e.Message.ToString();
                }
            }

            return new JavaScriptSerializer().Serialize(arm);
        }
    }
}