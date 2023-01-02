using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using MANOFR.Models;

namespace MANOFR.Controllers
{
    public class ModulesController : Controller
    {
        //global vars
        ManofrSqlDbModelDataContext dbObj;
        AjaxReturnMSG arm;
        // GET: Modules
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
        public string AddModule(int domain, string module)
        {
            try
            {
                arm = new AjaxReturnMSG();
                dbObj = new ManofrSqlDbModelDataContext();

                List<Module> moduleCheck = dbObj.Modules.Where(x => x.Domain == domain && x.Module1 == module).ToList();
                if (moduleCheck.Count > 0)
                {
                    arm.status = 0;
                    arm.successFlag = false;
                    arm.errorMsg = "Couldn't Add Module!\nError: Module Already Exists.";

                }
                else
                {

                    Module newModule= new Module();
                    newModule.Domain = domain;
                    newModule.Module1 = module;
                    dbObj.Modules.InsertOnSubmit(newModule);
                    dbObj.SubmitChanges();

                    arm.status = 1;
                    arm.successFlag = true;
                    arm.successMsg = "Module Added Successfully!";

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
        public string GetModulesList(int? domain, string module)
        {
            try
            {

                arm = new AjaxReturnMSG();
                dbObj = new ManofrSqlDbModelDataContext();

                var list = (from ls in dbObj.Modules
                            where (domain == null ? 1 == 1 : ls.Domain == domain) &&
                            (module == "" ? 1 == 1 : ls.Module1.Contains(module))
                            select new { ls.ID, Domain = ls.Domain1.Domain1, Module = ls.Module1 }).ToList();

                arm.status = 1;
                arm.successFlag = true;
                arm.successMsg = "";
                arm.returnedObject = list;

            }
            catch (Exception e)
            {
                arm.status = 0;
                arm.successFlag = false;
                arm.errorMsg = "Couldn't Get Modules List!\nError: " + e.Message.ToString();
            }

            return new JavaScriptSerializer().Serialize(arm);
        }

        [HttpPost]
        public string GetProjectModules(int projectID)
        {
            try
            {

                arm = new AjaxReturnMSG();
                dbObj = new ManofrSqlDbModelDataContext();

                var assignedModules = (from mod in dbObj.ProjectsModules
                                     where mod.Project == projectID
                                     select new { mod.Module1.ID, Module = mod.Module1.Module1, Domain = mod.Module1.Domain1.Domain1}).ToList();

                var unassignedModules = (from module in dbObj.Modules
                                       where !dbObj.ProjectsModules.Any(f => f.Module == module.ID && f.Project == projectID) &&
                                       module.Domain == dbObj.Projects.Where(x=> x.ID == projectID).Select(s=> s.Domain).SingleOrDefault()
                                       select new { module.ID, Module = module.Module1, Domain = module.Domain1.Domain1 }).ToList();

                arm.status = 1;
                arm.successFlag = true;
                arm.successMsg = "Modules Was Generated Successfully!";
                arm.returnedObject = assignedModules;
                arm.extraObject = unassignedModules;

            }
            catch (Exception e)
            {
                arm.status = 0;
                arm.successFlag = false;
                arm.errorMsg = "Couldn't Get Modules List!\nError: " + e.Message.ToString();
            }

            return new JavaScriptSerializer().Serialize(arm);
        }

        [HttpPost]
        public string GetModulesForEpic(int projectID)
        {
            try
            {

                arm = new AjaxReturnMSG();
                dbObj = new ManofrSqlDbModelDataContext();

                var list = (from pm in dbObj.ProjectsModules 
                            join pp in dbObj.Projects on pm.Project equals pp.ID
                            join mm in dbObj.Modules on pm.Module equals mm.ID
                            where pm.Project == projectID && pp.Domain == mm.Domain
                            select new { mm.ID, Domain = mm.Domain1.Domain1, Module = mm.Module1 }).ToList();

                arm.status = 1;
                arm.successFlag = true;
                arm.successMsg = "Modules List Was Generated Successfully!";
                arm.returnedObject = list;

            }
            catch (Exception e)
            {
                arm.status = 0;
                arm.successFlag = false;
                arm.errorMsg = "Couldn't Get Modules List!\nError: " + e.Message.ToString();
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
        public string UpdateModule(Module updatedModule)
        {
            try
            {
                arm = new AjaxReturnMSG();
                dbObj = new ManofrSqlDbModelDataContext();

                Module moduleToUpdate = dbObj.Modules.Where(md => md.ID == updatedModule.ID).SingleOrDefault();
                List<Module> moduleDomainDuplicate= dbObj.Modules.Where(mdd => mdd.Domain == updatedModule.Domain && mdd.Module1 == updatedModule.Module1 && mdd.ID != updatedModule.ID).ToList();

                if (moduleToUpdate != null && moduleDomainDuplicate.Count <= 0)
                {

                    moduleToUpdate.Domain = updatedModule.Domain;
                    moduleToUpdate.Module1 = updatedModule.Module1;

                    dbObj.SubmitChanges();

                    arm.status = 1;
                    arm.successFlag = true;
                    arm.successMsg = "Module Updated Successfully!";

                }
                else if (moduleToUpdate != null && moduleDomainDuplicate.Count > 0)
                {
                    arm.status = 0;
                    arm.successFlag = false;
                    arm.errorMsg = "Another Module Has The Same Name And Domain Exists!";
                }
                else if (moduleToUpdate == null)
                {

                    arm.status = 0;
                    arm.successFlag = false;
                    arm.errorMsg = "Module Not Found In The Database!";

                }

            }
            catch (Exception e)
            {
                arm.status = 0;
                arm.successFlag = false;
                arm.errorMsg = "Failed Updating Module!\nError: " + e.Message.ToString();
            }

            return new JavaScriptSerializer().Serialize(arm);
        }

        [HttpPost]
        public string DeleteModule(int moduleID)
        {
            try
            {
                arm = new AjaxReturnMSG();
                dbObj = new ManofrSqlDbModelDataContext();

                Module moduleToDelete = dbObj.Modules.Where(md => md.ID == moduleID).SingleOrDefault();

                if (moduleToDelete != null)
                {
                    dbObj.Modules.DeleteOnSubmit(moduleToDelete);
                    dbObj.SubmitChanges();

                    arm.status = 1;
                    arm.successFlag = true;
                    arm.successMsg = "Module Deleted Successfully!";
                }
                else
                {

                    arm.status = 0;
                    arm.successFlag = false;
                    arm.errorMsg = "Module Not Found In The Database!";

                }

            }
            catch (Exception e)
            {
                arm.status = 0;
                arm.successFlag = false;
                if (e.Message.Contains("conflicted with the REFERENCE constraint"))
                {
                    arm.errorMsg = "Failed Deleting Module!\nError: It's being used by another item.";
                }
                else
                {
                    arm.errorMsg = "Failed Deleting Module!\nError: " + e.Message.ToString();
                }
            }

            return new JavaScriptSerializer().Serialize(arm);
        }
    }
}