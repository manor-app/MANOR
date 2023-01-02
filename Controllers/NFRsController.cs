using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using MANOFR.Models;

namespace MANOFR.Controllers
{
    public class NFRsController : Controller
    {
        //global vars
        ManofrSqlDbModelDataContext dbObj;
        AjaxReturnMSG arm;
        // GET: NFRs
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
        public string AddNFR(string nfr)
        {
            try
            {
                arm = new AjaxReturnMSG();
                dbObj = new ManofrSqlDbModelDataContext();

                List<UsersNFR> usersNFR = dbObj.UsersNFRs.Where(x => x.NFR == nfr).ToList();
                List<SystemNFR> sysNFR = dbObj.SystemNFRs.Where(x => x.NFR == nfr).ToList();

                if (usersNFR.Count > 0 || sysNFR.Count > 0)
                {
                    arm.status = 0;
                    arm.successFlag = false;
                    arm.errorMsg = "Couldn't Add NFR!\nError: NFR Already Exists.";

                }
                else
                {

                    UsersNFR newNFR = new UsersNFR();
                    newNFR.NFR = nfr;
                    dbObj.UsersNFRs.InsertOnSubmit(newNFR);
                    dbObj.SubmitChanges();

                    arm.status = 1;
                    arm.successFlag = true;
                    arm.successMsg = "NFR Added Successfully!";

                }

            }
            catch (Exception e)
            {
                arm.status = 0;
                arm.successFlag = false;
                arm.errorMsg = "Couldn't Add Module!\nError: " + e.Message.ToString();
            }

            return new JavaScriptSerializer().Serialize(arm);
        }

        [HttpPost]
        public string GetNFRsList(string nfr)
        {
            try
            {

                arm = new AjaxReturnMSG();
                dbObj = new ManofrSqlDbModelDataContext();

                var sysNFRList = (from sysNF in dbObj.SystemNFRs
                            where (nfr == "" ? 1 == 1 : sysNF.NFR.Contains(nfr))
                            select new { sysNF.ID, sysNF.NFR }).ToList();

                var usrNFRList = (from usrNF in dbObj.UsersNFRs
                                  where (nfr == "" ? 1 == 1 : usrNF.NFR.Contains(nfr))
                                  select new { usrNF.ID, usrNF.NFR }).ToList();

                arm.status = 1;
                arm.successFlag = true;
                arm.successMsg = "";
                arm.returnedObject = sysNFRList;
                arm.extraObject = usrNFRList;

            }
            catch (Exception e)
            {
                arm.status = 0;
                arm.successFlag = false;
                arm.errorMsg = "Couldn't Get NFRs List!\nError: " + e.Message.ToString();
            }

            return new JavaScriptSerializer().Serialize(arm);
        }

        [HttpPost]
        public string GetNFRsForProject(int projectID)
        {
            try
            {

                arm = new AjaxReturnMSG();
                dbObj = new ManofrSqlDbModelDataContext();

                var assignedNFRs = (from asNFR in dbObj.ProjectsNFRs
                                    where asNFR.ProjectID == projectID
                                    select new { asNFR.ID,asNFR.ProjectID, asNFR.NFR }).ToList();

                var sysNFRs = (from sysNF in dbObj.SystemNFRs
                               where !dbObj.ProjectsNFRs.Any(f => f.NFR == sysNF.NFR && f.ProjectID == projectID)
                               select new { sysNF.ID, sysNF.NFR }).ToList();
                var userNFRs = (from usrNF in dbObj.UsersNFRs
                                where !dbObj.ProjectsNFRs.Any(f => f.NFR == usrNF.NFR && f.ProjectID == projectID)
                                select new { usrNF.ID, usrNF.NFR }).ToList();

                List<DummyNFR> unassignedNFRs = new List<DummyNFR>();
                DummyNFR tmpNFR;
                foreach (var nfr in sysNFRs) {
                    tmpNFR = new DummyNFR();
                    tmpNFR.ID = nfr.ID;
                    tmpNFR.NFR = nfr.NFR;
                    unassignedNFRs.Add(tmpNFR);
                }
                foreach (var nfr2 in userNFRs)
                {
                    tmpNFR = new DummyNFR();
                    tmpNFR.ID = nfr2.ID;
                    tmpNFR.NFR = nfr2.NFR;
                    unassignedNFRs.Add(tmpNFR);
                }

                arm.status = 1;
                arm.successFlag = true;
                arm.successMsg = "";
                arm.returnedObject = assignedNFRs;
                arm.extraObject = unassignedNFRs;

            }
            catch (Exception e)
            {
                arm.status = 0;
                arm.successFlag = false;
                arm.errorMsg = "Couldn't Get NFRs List!\nError: " + e.Message.ToString();
            }

            return new JavaScriptSerializer().Serialize(arm);
        }

        [HttpPost]
        public string AssignUnAssignNFRToProject(bool assignFlag, string nfr, int projectID)
        {

            string resultMSG = "", errorMsg = "";

            try
            {
                dbObj = new ManofrSqlDbModelDataContext();
                arm = new AjaxReturnMSG();

                ProjectsNFR projNFR= new ProjectsNFR();
                if (assignFlag)
                {
                    projNFR = new ProjectsNFR();
                    projNFR.ProjectID = projectID;
                    projNFR.NFR = nfr;
                    dbObj.ProjectsNFRs.InsertOnSubmit(projNFR);
                    dbObj.SubmitChanges();
                    resultMSG = "NFR Assigned Successfully!";
                    errorMsg = "Something Went Wrong While Assigning NFR!";
                }
                else
                {
                    projNFR = dbObj.ProjectsNFRs.Where(u => u.NFR == nfr && u.ProjectID == projectID).SingleOrDefault();
                    dbObj.ProjectsNFRs.DeleteOnSubmit(projNFR);
                    dbObj.SubmitChanges();
                    resultMSG = "NFR Unassigned Successfully!";
                    errorMsg = "Something Went Wrong While Unassigning NFR!";
                }

                arm.status = 1;
                arm.successFlag = true;
                arm.successMsg = resultMSG;

            }
            catch (Exception e)
            {
                arm.status = 0;
                arm.successFlag = false;
                arm.errorMsg = errorMsg + "\nError: " + e.Message.ToString();
            }

            return new JavaScriptSerializer().Serialize(arm);
        }

        [HttpPost]
        public string UpdateNFR(int nfrID, string newNfrText, bool isSysNFR)
        {
            try
            {
                arm = new AjaxReturnMSG();
                dbObj = new ManofrSqlDbModelDataContext();

                if (isSysNFR) {

                    SystemNFR nfrToUpdate = dbObj.SystemNFRs.Where(nf => nf.ID == nfrID).SingleOrDefault();

                    if (nfrToUpdate != null)
                    {

                        nfrToUpdate.NFR = newNfrText;
                        dbObj.SubmitChanges();

                        arm.status = 1;
                        arm.successFlag = true;
                        arm.successMsg = "NFR Updated Successfully!";
                    }
                    else
                    {

                        arm.status = 0;
                        arm.successFlag = false;
                        arm.errorMsg = "NFR Not Found In The Database!";

                    }

                } else {

                    UsersNFR nfrToUpdate = dbObj.UsersNFRs.Where(nf => nf.ID == nfrID).SingleOrDefault();

                    if (nfrToUpdate != null)
                    {

                        nfrToUpdate.NFR = newNfrText;
                        dbObj.SubmitChanges();

                        arm.status = 1;
                        arm.successFlag = true;
                        arm.successMsg = "NFR Updated Successfully!";
                    }
                    else
                    {

                        arm.status = 0;
                        arm.successFlag = false;
                        arm.errorMsg = "NFR Not Found In The Database!";

                    }

                }

            }
            catch (Exception e)
            {
                arm.status = 0;
                arm.successFlag = false;
                arm.errorMsg = "Failed Updating NFR!\nError: " + e.Message.ToString();
            }

            return new JavaScriptSerializer().Serialize(arm);
        }

        [HttpPost]
        public string DeleteNFR(int nfrID, bool isSysNFR)
        {
            try
            {
                arm = new AjaxReturnMSG();
                dbObj = new ManofrSqlDbModelDataContext();

                if (isSysNFR)
                {

                    SystemNFR nfrToUpdate = dbObj.SystemNFRs.Where(nf => nf.ID == nfrID).SingleOrDefault();

                    if (nfrToUpdate != null)
                    {

                        dbObj.SystemNFRs.DeleteOnSubmit(nfrToUpdate);
                        dbObj.SubmitChanges();

                        arm.status = 1;
                        arm.successFlag = true;
                        arm.successMsg = "NFR Deleted Successfully!";
                    }
                    else
                    {

                        arm.status = 0;
                        arm.successFlag = false;
                        arm.errorMsg = "NFR Not Found In The Database!";

                    }

                }
                else
                {

                    UsersNFR nfrToUpdate = dbObj.UsersNFRs.Where(nf => nf.ID == nfrID).SingleOrDefault();

                    if (nfrToUpdate != null)
                    {

                        dbObj.UsersNFRs.DeleteOnSubmit(nfrToUpdate);
                        dbObj.SubmitChanges();

                        arm.status = 1;
                        arm.successFlag = true;
                        arm.successMsg = "NFR Deleted Successfully!";
                    }
                    else
                    {

                        arm.status = 0;
                        arm.successFlag = false;
                        arm.errorMsg = "NFR Not Found In The Database!";

                    }

                }

            }
            catch (Exception e)
            {
                arm.status = 0;
                arm.successFlag = false;
                if (e.Message.Contains("conflicted with the REFERENCE constraint"))
                {
                    arm.errorMsg = "Failed Deleting NFR!\nError: It's being used by another item.";
                }
                else
                {
                    arm.errorMsg = "Failed Deleting NFR!\nError: " + e.Message.ToString();
                }
            }

            return new JavaScriptSerializer().Serialize(arm);
        }
    }
}