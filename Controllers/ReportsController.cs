using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using MANOFR.Models;

namespace MANOFR.Controllers
{
    public class SRBR_Result {
        public int Total { get; set; }
        public int Closed { get; set; }
        public int Unclosed { get; set; }
    }
    public class SRBS_Result_Item
    {
        public string Status { get; set; }
        public string Color { get; set; }
        public int Count { get; set; }
    }
    public class SRBP_Result_Item
    {
        public string Priority { get; set; }
        public string Color { get; set; }
        public int Count { get; set; }
    }
    public class RepObj_Result_Item
    {
        public string Name { get; set; }
        public string Color { get; set; }
        public int Count { get; set; }
    }
    public class ASIS_Result_Item { 
        public string ItemName { get; set; }
        public IDictionary<string, int> StatusMap { get; set; }
        public int AVG { get; set; }
    }

    public class ReportsController : Controller
    {
        // GET: Reports
        //public ActionResult Index()
        //{
        //    if (Request.Cookies["userFullname"] == null)
        //    {
        //        return RedirectToAction("Index", "Login");
        //    }
        //    else
        //    {
        //        return View();
        //    }
        //}

        //global vars
        ManofrSqlDbModelDataContext dbObj;
        AjaxReturnMSG arm;

        [HttpPost]
        public string Generate_SRBR_Report(UserStory userstory, int? assignedTo)
        {
            try
            {

                arm = new AjaxReturnMSG();
                dbObj = new ManofrSqlDbModelDataContext();
                SRBR_Result frResult = new SRBR_Result();
                SRBR_Result nfrResult = new SRBR_Result();

                if (assignedTo == null || assignedTo == 0)
                {
                    var frTotal = (from x in dbObj.UserStories
                                   where
                                   x.ProjectID == userstory.ProjectID && x.US_Requirement_Type.Type_Value == "Functional"
                                   && (userstory.RelatedTo == 0 ? 1 == 1 : x.RelatedTo == userstory.RelatedTo)
                                   && (userstory.ModuleID == 0 ? 1 == 1 : x.ModuleID == userstory.ModuleID)
                                   && (userstory.EpicID == 0 ? 1 == 1 : x.EpicID == userstory.EpicID)
                                   && (userstory.CreatedBy == 0 ? 1 == 1 : x.CreatedBy == userstory.CreatedBy)
                                   select new { x.ID }
                                  ).ToList();

                    var frClosed = (from x in dbObj.UserStories
                                    where
                                    x.ProjectID == userstory.ProjectID && x.US_Requirement_Type.Type_Value == "Functional" && x.ComponentsStatus.Status == "Closed"
                                    && (userstory.RelatedTo == 0 ? 1 == 1 : x.RelatedTo == userstory.RelatedTo)
                                    && (userstory.ModuleID == 0 ? 1 == 1 : x.ModuleID == userstory.ModuleID)
                                    && (userstory.EpicID == 0 ? 1 == 1 : x.EpicID == userstory.EpicID)
                                    && (userstory.CreatedBy == 0 ? 1 == 1 : x.CreatedBy == userstory.CreatedBy)
                                    select new { x.ID }
                                  ).ToList();

                    var frUnclosed = (from x in dbObj.UserStories
                                      where
                                      x.ProjectID == userstory.ProjectID && x.US_Requirement_Type.Type_Value == "Functional" && x.ComponentsStatus.Status != "Closed"
                                      && (userstory.RelatedTo == 0 ? 1 == 1 : x.RelatedTo == userstory.RelatedTo)
                                      && (userstory.ModuleID == 0 ? 1 == 1 : x.ModuleID == userstory.ModuleID)
                                      && (userstory.EpicID == 0 ? 1 == 1 : x.EpicID == userstory.EpicID)
                                      && (userstory.CreatedBy == 0 ? 1 == 1 : x.CreatedBy == userstory.CreatedBy)
                                      select new { x.ID }
                                  ).ToList();

                    frResult.Total = frTotal.Count;
                    frResult.Closed = frClosed.Count;
                    frResult.Unclosed = frUnclosed.Count;

                    var nfrTotal = (from x in dbObj.UserStories
                                    where
                                    x.ProjectID == userstory.ProjectID && x.US_Requirement_Type.Type_Value == "None Functional"
                                    && (userstory.RelatedTo == 0 ? 1 == 1 : x.RelatedTo == userstory.RelatedTo)
                                    && (userstory.ModuleID == 0 ? 1 == 1 : x.ModuleID == userstory.ModuleID)
                                    && (userstory.EpicID == 0 ? 1 == 1 : x.EpicID == userstory.EpicID)
                                    && (userstory.CreatedBy == 0 ? 1 == 1 : x.CreatedBy == userstory.CreatedBy)
                                    select new { x.ID }
                                  ).ToList();

                    var nfrClosed = (from x in dbObj.UserStories
                                     where
                                     x.ProjectID == userstory.ProjectID && x.US_Requirement_Type.Type_Value == "None Functional" && x.ComponentsStatus.Status == "Closed"
                                     && (userstory.RelatedTo == 0 ? 1 == 1 : x.RelatedTo == userstory.RelatedTo)
                                     && (userstory.ModuleID == 0 ? 1 == 1 : x.ModuleID == userstory.ModuleID)
                                     && (userstory.EpicID == 0 ? 1 == 1 : x.EpicID == userstory.EpicID)
                                     && (userstory.CreatedBy == 0 ? 1 == 1 : x.CreatedBy == userstory.CreatedBy)
                                     select new { x.ID }
                                  ).ToList();

                    var nfrUnclosed = (from x in dbObj.UserStories
                                       where
                                       x.ProjectID == userstory.ProjectID && x.US_Requirement_Type.Type_Value == "None Functional" && x.ComponentsStatus.Status != "Closed"
                                       && (userstory.RelatedTo == 0 ? 1 == 1 : x.RelatedTo == userstory.RelatedTo)
                                       && (userstory.ModuleID == 0 ? 1 == 1 : x.ModuleID == userstory.ModuleID)
                                       && (userstory.EpicID == 0 ? 1 == 1 : x.EpicID == userstory.EpicID)
                                       && (userstory.CreatedBy == 0 ? 1 == 1 : x.CreatedBy == userstory.CreatedBy)
                                       select new { x.ID }
                                  ).ToList();

                    nfrResult.Total = nfrTotal.Count;
                    nfrResult.Closed = nfrClosed.Count;
                    nfrResult.Unclosed = nfrUnclosed.Count;
                }
                else {
                    var frTotal = (from x in dbObj.UserStories
                                   join u in dbObj.UserStory_Assigned_Users on x.ID equals u.UserStory
                                   where
                                   x.ProjectID == userstory.ProjectID && x.US_Requirement_Type.Type_Value == "Functional"
                                   && u.AssignedUser == assignedTo
                                   && (userstory.RelatedTo == 0 ? 1 == 1 : x.RelatedTo == userstory.RelatedTo)
                                   && (userstory.ModuleID == 0 ? 1 == 1 : x.ModuleID == userstory.ModuleID)
                                   && (userstory.EpicID == 0 ? 1 == 1 : x.EpicID == userstory.EpicID)
                                   && (userstory.CreatedBy == 0 ? 1 == 1 : x.CreatedBy == userstory.CreatedBy)
                                   select new { x.ID }
                                      ).ToList();

                    var frClosed = (from x in dbObj.UserStories
                                    join u in dbObj.UserStory_Assigned_Users on x.ID equals u.UserStory
                                    where
                                    x.ProjectID == userstory.ProjectID && x.US_Requirement_Type.Type_Value == "Functional" && x.ComponentsStatus.Status == "Closed"
                                    && u.AssignedUser == assignedTo
                                    && (userstory.RelatedTo == 0 ? 1 == 1 : x.RelatedTo == userstory.RelatedTo)
                                    && (userstory.ModuleID == 0 ? 1 == 1 : x.ModuleID == userstory.ModuleID)
                                    && (userstory.EpicID == 0 ? 1 == 1 : x.EpicID == userstory.EpicID)
                                    && (userstory.CreatedBy == 0 ? 1 == 1 : x.CreatedBy == userstory.CreatedBy)
                                    select new { x.ID }
                                  ).ToList();

                    var frUnclosed = (from x in dbObj.UserStories
                                      join u in dbObj.UserStory_Assigned_Users on x.ID equals u.UserStory
                                      where
                                      x.ProjectID == userstory.ProjectID && x.US_Requirement_Type.Type_Value == "Functional" && x.ComponentsStatus.Status != "Closed"
                                      && u.AssignedUser == assignedTo
                                      && (userstory.RelatedTo == 0 ? 1 == 1 : x.RelatedTo == userstory.RelatedTo)
                                      && (userstory.ModuleID == 0 ? 1 == 1 : x.ModuleID == userstory.ModuleID)
                                      && (userstory.EpicID == 0 ? 1 == 1 : x.EpicID == userstory.EpicID)
                                      && (userstory.CreatedBy == 0 ? 1 == 1 : x.CreatedBy == userstory.CreatedBy)
                                      select new { x.ID }
                                  ).ToList();

                    frResult.Total = frTotal.Count;
                    frResult.Closed = frClosed.Count;
                    frResult.Unclosed = frUnclosed.Count;

                    var nfrTotal = (from x in dbObj.UserStories
                                    join u in dbObj.UserStory_Assigned_Users on x.ID equals u.UserStory
                                    where
                                    x.ProjectID == userstory.ProjectID && x.US_Requirement_Type.Type_Value == "None Functional"
                                    && u.AssignedUser == assignedTo
                                    && (userstory.RelatedTo == 0 ? 1 == 1 : x.RelatedTo == userstory.RelatedTo)
                                    && (userstory.ModuleID == 0 ? 1 == 1 : x.ModuleID == userstory.ModuleID)
                                    && (userstory.EpicID == 0 ? 1 == 1 : x.EpicID == userstory.EpicID)
                                    && (userstory.CreatedBy == 0 ? 1 == 1 : x.CreatedBy == userstory.CreatedBy)
                                    select new { x.ID }
                                  ).ToList();

                    var nfrClosed = (from x in dbObj.UserStories
                                     join u in dbObj.UserStory_Assigned_Users on x.ID equals u.UserStory
                                     where
                                     x.ProjectID == userstory.ProjectID && x.US_Requirement_Type.Type_Value == "None Functional" && x.ComponentsStatus.Status == "Closed"
                                     && u.AssignedUser == assignedTo
                                     && (userstory.RelatedTo == 0 ? 1 == 1 : x.RelatedTo == userstory.RelatedTo)
                                     && (userstory.ModuleID == 0 ? 1 == 1 : x.ModuleID == userstory.ModuleID)
                                     && (userstory.EpicID == 0 ? 1 == 1 : x.EpicID == userstory.EpicID)
                                     && (userstory.CreatedBy == 0 ? 1 == 1 : x.CreatedBy == userstory.CreatedBy)
                                     select new { x.ID }
                                  ).ToList();

                    var nfrUnclosed = (from x in dbObj.UserStories
                                       join u in dbObj.UserStory_Assigned_Users on x.ID equals u.UserStory
                                       where
                                       x.ProjectID == userstory.ProjectID && x.US_Requirement_Type.Type_Value == "None Functional" && x.ComponentsStatus.Status != "Closed"
                                       && u.AssignedUser == assignedTo
                                       && (userstory.RelatedTo == 0 ? 1 == 1 : x.RelatedTo == userstory.RelatedTo)
                                       && (userstory.ModuleID == 0 ? 1 == 1 : x.ModuleID == userstory.ModuleID)
                                       && (userstory.EpicID == 0 ? 1 == 1 : x.EpicID == userstory.EpicID)
                                       && (userstory.CreatedBy == 0 ? 1 == 1 : x.CreatedBy == userstory.CreatedBy)
                                       select new { x.ID }
                                  ).ToList();

                    nfrResult.Total = nfrTotal.Count;
                    nfrResult.Closed = nfrClosed.Count;
                    nfrResult.Unclosed = nfrUnclosed.Count;
                }



                arm.status = 1;
                arm.successFlag = true;
                arm.successMsg = "Status List Was Generated Successfully!";
                arm.returnedObject = frResult;
                arm.extraObject = nfrResult;

            }
            catch (Exception e)
            {
                arm.status = 0;
                arm.successFlag = false;
                arm.errorMsg = "Couldn't Generate The Report!\nError: " + e.Message.ToString();
            }

            return new JavaScriptSerializer().Serialize(arm);
        }

        [HttpPost]
        public string Generate_SRBS_Report(UserStory userstory)
        {
            try
            {
                var statusColorsDictionary = new Dictionary<string, string> { { "Open", "#2F78CF" }, { "In-Progress", "#E2E25B" }, { "Pending", "#B287C2" }, { "Closed", "#73D55B" }, { "Overdue", "#F5490D" } };

                arm = new AjaxReturnMSG();
                dbObj = new ManofrSqlDbModelDataContext();
                List<SRBS_Result_Item> srbsResList = new List<SRBS_Result_Item>();

                var statusList = (from s in dbObj.ComponentsStatus
                                  select s).ToList();

                SRBS_Result_Item tmpItemSRBS = null;

                foreach (var stat in statusList) {

                    tmpItemSRBS = new SRBS_Result_Item();
                    tmpItemSRBS.Status = stat.Status;
                    tmpItemSRBS.Color = statusColorsDictionary[stat.Status];
                    tmpItemSRBS.Count = (from x in dbObj.UserStories
                                         where
                                            x.ProjectID == userstory.ProjectID &&
                                            x.StatusID == stat.ID &&
                                            (userstory.RequirementType == 0 ? 1 == 1 : x.RequirementType == userstory.RequirementType)
                                         select new { x.ID }
                                        ).ToList().Count;

                    srbsResList.Add(tmpItemSRBS);
                }

                arm.status = 1;
                arm.successFlag = true;
                arm.successMsg = "Status List Was Generated Successfully!";
                arm.returnedObject = srbsResList;

            }
            catch (Exception e)
            {
                arm.status = 0;
                arm.successFlag = false;
                arm.errorMsg = "Couldn't Generate The Report!\nError: " + e.Message.ToString();
            }

            return new JavaScriptSerializer().Serialize(arm);
        }

        [HttpPost]
        public string Generate_SRBP_Report(UserStory userstory)
        {
            try
            {
                arm = new AjaxReturnMSG();
                dbObj = new ManofrSqlDbModelDataContext();
                List<SRBP_Result_Item> srbpResList = new List<SRBP_Result_Item>();

                var priorityList = (from s in dbObj.US_Priorities
                                    select s).ToList();

                SRBP_Result_Item tmpItemSRBP = null;

                foreach (var prior in priorityList)
                {

                    tmpItemSRBP = new SRBP_Result_Item();
                    tmpItemSRBP.Priority = prior.Priority_Value;
                    tmpItemSRBP.Count = (from x in dbObj.UserStories
                                         where
                                            x.ProjectID == userstory.ProjectID &&
                                            x.UsPriority == prior.ID &&
                                            (userstory.RequirementType == 0 ? 1 == 1 : x.RequirementType == userstory.RequirementType)
                                         select new { x.ID }
                                        ).ToList().Count;

                    srbpResList.Add(tmpItemSRBP);
                }

                arm.status = 1;
                arm.successFlag = true;
                arm.successMsg = "Priority List Was Generated Successfully!";
                arm.returnedObject = srbpResList;

            }
            catch (Exception e)
            {
                arm.status = 0;
                arm.successFlag = false;
                arm.errorMsg = "Couldn't Generate The Report!\nError: " + e.Message.ToString();
            }

            return new JavaScriptSerializer().Serialize(arm);
        }

        [HttpGet]
        public string Generate_Dashboard_PBST_Report()
        {
            try
            {
                arm = new AjaxReturnMSG();
                dbObj = new ManofrSqlDbModelDataContext();
                List<RepObj_Result_Item> ResList = new List<RepObj_Result_Item>();

                var systypes = (from s in dbObj.ApplicationsTypes
                                    select s).ToList();

                RepObj_Result_Item tmpItem = null;

                foreach (var item in systypes)
                {

                    tmpItem = new RepObj_Result_Item();
                    tmpItem.Name = item.Type;
                    tmpItem.Count = (from x in dbObj.Projects
                                     where x.SystemType == item.ID
                                     select new { x.ID }).ToList().Count;

                    ResList.Add(tmpItem);
                }

                arm.status = 1;
                arm.successFlag = true;
                arm.successMsg = "List Was Generated Successfully!";
                arm.returnedObject = ResList;

            }
            catch (Exception e)
            {
                arm.status = 0;
                arm.successFlag = false;
                arm.errorMsg = "Couldn't Generate The PBST Report!\nError: " + e.Message.ToString();
            }

            return new JavaScriptSerializer().Serialize(arm);
        }

        [HttpGet]
        public string Generate_Dashboard_TFNFRS_Report()
        {
            try
            {
                arm = new AjaxReturnMSG();
                dbObj = new ManofrSqlDbModelDataContext();
                List<int> Counts = new List<int>();
                var top5NFRs = dbObj.ProjectsNFRs
                                    .GroupBy(q => q.NFR)
                                    .OrderByDescending(gp => gp.Count())
                                    .Take(5)
                                    .Select(g => g.Key).ToList();
                var top5NFRsC = dbObj.ProjectsNFRs
                                    .GroupBy(q => q.NFR)
                                    .OrderByDescending(gp => gp.Count())
                                    .Take(5)
                                    .Select(g => g.Count()).ToList();

                foreach (var item in top5NFRs) {
                    Counts.Add(dbObj.ProjectsNFRs.Where(x=> x.NFR == item).ToList().Count);
                }

                arm.status = 1;
                arm.successFlag = true;
                arm.successMsg = "List Was Generated Successfully!";
                arm.returnedObject = top5NFRs;
                arm.extraObject = Counts;

            }
            catch (Exception e)
            {
                arm.status = 0;
                arm.successFlag = false;
                arm.errorMsg = "Couldn't Generate The PBST Report!\nError: " + e.Message.ToString();
            }

            return new JavaScriptSerializer().Serialize(arm);
        }

        [HttpGet]
        public string Generate_Dashboard_ASIS_Report()
        {
            try
            {
                arm = new AjaxReturnMSG();
                dbObj = new ManofrSqlDbModelDataContext();

                List<ASIS_Result_Item> result = new List<ASIS_Result_Item>();

                var statusList = (from s in dbObj.ComponentsStatus
                                  select s).ToList();

                ASIS_Result_Item tmpItem = new ASIS_Result_Item();


                int tmpCount = 0;
                tmpItem.ItemName = "Projects";
                tmpItem.StatusMap = new Dictionary<string, int>();
                foreach (var stat in statusList)
                {
                    tmpCount = dbObj.Projects.Where(x => x.Status == stat.ID).ToList().Count;
                    tmpItem.StatusMap.Add( stat.Status, tmpCount);
                    tmpItem.AVG += tmpCount;
                }
                tmpItem.AVG = tmpItem.AVG / statusList.Count;
                result.Add(tmpItem);

                tmpItem = new ASIS_Result_Item();
                tmpItem.ItemName = "Sprints";
                tmpItem.StatusMap = new Dictionary<string, int>();
                foreach (var stat in statusList)
                {
                    tmpCount = dbObj.Sprints.Where(x => x.Status == stat.ID).ToList().Count;
                    tmpItem.StatusMap.Add(stat.Status, tmpCount);
                    tmpItem.AVG += tmpCount;
                }
                tmpItem.AVG = tmpItem.AVG / statusList.Count;
                result.Add(tmpItem);

                tmpItem = new ASIS_Result_Item();
                tmpItem.ItemName = "User Stories";
                tmpItem.StatusMap = new Dictionary<string, int>();
                foreach (var stat in statusList)
                {
                    tmpCount = dbObj.UserStories.Where(x => x.StatusID == stat.ID).ToList().Count;
                    tmpItem.StatusMap.Add(stat.Status, tmpCount);
                    tmpItem.AVG += tmpCount;
                }
                tmpItem.AVG = tmpItem.AVG / statusList.Count;
                result.Add(tmpItem);

                tmpItem = new ASIS_Result_Item();
                tmpItem.ItemName = "Tasks";
                tmpItem.StatusMap = new Dictionary<string, int>();
                foreach (var stat in statusList)
                {
                    tmpCount = dbObj.Tasks.Where(x => x.Status == stat.ID).ToList().Count;
                    tmpItem.StatusMap.Add(stat.Status, tmpCount);
                    tmpItem.AVG += tmpCount;
                }
                tmpItem.AVG = tmpItem.AVG / statusList.Count;
                result.Add(tmpItem);

                arm.status = 1;
                arm.successFlag = true;
                arm.successMsg = "List Was Generated Successfully!";
                arm.returnedObject = result;

            }
            catch (Exception e)
            {
                arm.status = 0;
                arm.successFlag = false;
                arm.errorMsg = "Couldn't Generate The ASIS Report!\nError: " + e.Message.ToString();
            }

            return new JavaScriptSerializer().Serialize(arm);
        }
    }
}