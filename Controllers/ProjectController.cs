using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MANOFR.Models;
using System.Web.Script.Serialization;
using System.Diagnostics;
using System.IO;

namespace MANOFR.Controllers
{
    class USUser { 
        public int ID { get; set; }
        public string Name { get; set; }
    }
    class AssignedUsersToUS {
        public List<USUser> AssignedUsersList { get; set; }
        public AssignedUsersToUS()
        {
            this.AssignedUsersList = new List<USUser>();
        }
    }
    class USStory
    {
        public int ID { get; set; }
        public string Title { get; set; }
    }
    class AssignedStoriesToUS
    {
        public List<USStory> AssignedStoriesList { get; set; }
        public AssignedStoriesToUS()
        {
            this.AssignedStoriesList = new List<USStory>();
        }
    }
    class AssignedItemsToUS { 
        public List<AssignedUsersToUS> AssignedUsers { get; set; }
        public List<AssignedStoriesToUS> AssignedStories { get; set; }

        public AssignedItemsToUS() {
            this.AssignedUsers = new List<AssignedUsersToUS>();
            this.AssignedStories = new List<AssignedStoriesToUS>();
        }
    }
    public class ProjectController : Controller
    {
        //global vars
        ManofrSqlDbModelDataContext dbObj;
        AjaxReturnMSG arm;

        // GET: Project
        public ActionResult CreateProject()
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
        public ActionResult ProjectPage(int? projectID)
        {
            if (Request.Cookies["userFullname"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                if (!ValidUrlProjectID(projectID))
                    return RedirectToAction("Index", "Home");

                ViewBag.ProjectID = projectID;
                return View();
            }
        }

        public bool ValidUrlProjectID(int? ID)
        {
            bool valid = true;
            try
            {
                dbObj = new ManofrSqlDbModelDataContext();
                Project proj = dbObj.Projects.Where(x => x.ID == ID).SingleOrDefault();

                if (ID == null)
                    valid = false;
                if (ID == 0)
                    valid = false;
                if (proj == null)
                    valid = false;

            }
            catch (Exception)
            {
                valid = false;
            }

            return valid;
        }

        [HttpPost]
        public string AssignUnAssignUserToProject(bool assignFlag, int userID, int projectID) {

            string resultMSG = "", errorMsg = "";

            try {
                dbObj = new ManofrSqlDbModelDataContext();
                arm = new AjaxReturnMSG();

                User usr = new User();
                ProjectsUser projUser = new ProjectsUser();
                if (assignFlag)
                {
                    projUser = new ProjectsUser();
                    projUser.Project = projectID;
                    projUser.SysUser = userID;
                    dbObj.ProjectsUsers.InsertOnSubmit(projUser);
                    dbObj.SubmitChanges();
                    resultMSG = "User Assigned Successfully!";
                    errorMsg = "Something Went Wrong While Assigning User!";
                }
                else {
                    projUser = dbObj.ProjectsUsers.Where(u => u.SysUser == userID && u.Project == projectID).SingleOrDefault();
                    dbObj.ProjectsUsers.DeleteOnSubmit(projUser);
                    dbObj.SubmitChanges();
                    resultMSG = "User Unassigned Successfully!";
                    errorMsg = "Something Went Wrong While Unassigning User!";
                }

                arm.status = 1;
                arm.successFlag = true;
                arm.successMsg = resultMSG;

            } catch (Exception e) {
                arm.status = 0;
                arm.successFlag = false;
                arm.errorMsg = errorMsg + "\nError: " + e.Message.ToString();
            }

            return new JavaScriptSerializer().Serialize(arm);
        }

        [HttpPost]
        public string AssignUnAssignModuleToProject(bool assignFlag, int moduleID, int projectID)
        {

            string resultMSG = "", errorMsg = "";

            try
            {
                dbObj = new ManofrSqlDbModelDataContext();
                arm = new AjaxReturnMSG();

                Module mod = new Module();
                ProjectsModule projModule= new ProjectsModule();
                if (assignFlag)
                {
                    projModule = new ProjectsModule();
                    projModule.Project = projectID;
                    projModule.Module = moduleID;
                    dbObj.ProjectsModules.InsertOnSubmit(projModule);
                    dbObj.SubmitChanges();
                    resultMSG = "Module Assigned Successfully!";
                    errorMsg = "Something Went Wrong While Assigning Module!";
                }
                else
                {
                    projModule = dbObj.ProjectsModules.Where(u => u.Module == moduleID && u.Project == projectID).SingleOrDefault();
                    dbObj.ProjectsModules.DeleteOnSubmit(projModule);
                    dbObj.SubmitChanges();
                    resultMSG = "Module Unassigned Successfully!";
                    errorMsg = "Something Went Wrong While Unassigning Module!";
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
        public string CreateNewProject(Project proj)
        {
            try
            {
                arm = new AjaxReturnMSG();
                dbObj = new ManofrSqlDbModelDataContext();

                List<Project> projectCheck = dbObj.Projects.Where(x => x.Name == proj.Name && x.Domain == proj.Domain).ToList();

                if (projectCheck.Count > 0)
                {
                    arm.status = 0;
                    arm.successFlag = false;
                    arm.errorMsg = "Couldn't Create Project!\nError: Project Already Exists With The Same Name & Domain.";

                }
                else
                {

                    //Debug.WriteLine(int.Parse(Request.Cookies["userID"].Value));
                    Project newProject = new Project();
                    newProject.Name = proj.Name;
                    newProject.Domain = proj.Domain;
                    newProject.SystemType = proj.SystemType;
                    newProject.Status = proj.Status;

                    newProject.CustomerName = proj.CustomerName;
                    newProject.CustomerPhone = proj.CustomerPhone;
                    newProject.CustomerEmail = proj.CustomerEmail;
                    newProject.StartDate = proj.StartDate;
                    newProject.EndDate = proj.EndDate;
                    newProject.Description = proj.Description;
                    newProject.CreationDate = DateTime.Now;
                    newProject.CreatedBy = int.Parse(Request.Cookies["userID"].Value);
                    dbObj.Projects.InsertOnSubmit(newProject);
                    dbObj.SubmitChanges();

                    arm.status = 1;
                    arm.successFlag = true;
                    arm.successMsg = "Project Created Successfully!";

                }

            }
            catch (Exception e)
            {
                arm.status = 0;
                arm.successFlag = false;
                arm.errorMsg = "Couldn't Create Project!\nError: " + e.Message.ToString();
            }

            return new JavaScriptSerializer().Serialize(arm);
        }

        [HttpPost]
        public string EraseProject(int proj)
        {
            try
            {
                arm = new AjaxReturnMSG();
                dbObj = new ManofrSqlDbModelDataContext();

                Project projectCheck = dbObj.Projects.Where(x => x.ID == proj).SingleOrDefault();

                if (projectCheck == null)
                {
                    arm.status = 0;
                    arm.successFlag = false;
                    arm.errorMsg = "Couldn't Delete Project!\nError: Project Cannot Be Found!.";

                }
                else
                {
                    projectCheck.isSuspended = true;
                    dbObj.SubmitChanges();

                    arm.status = 1;
                    arm.successFlag = true;
                    arm.successMsg = "Project Deleted Successfully!";

                }

            }
            catch (Exception e)
            {
                arm.status = 0;
                arm.successFlag = false;
                arm.errorMsg = "Couldn't Delete Project!\nError: " + e.Message.ToString();
            }

            return new JavaScriptSerializer().Serialize(arm);
        }

        [HttpPost]
        public string CreateNewSprint(Sprint sprint, int projectID)
        {
            try
            {
                arm = new AjaxReturnMSG();
                dbObj = new ManofrSqlDbModelDataContext();

                List<Sprint> sprintCheck = dbObj.Sprints.Where(x => x.Name == sprint.Name && x.ProjectID == projectID).ToList();

                if (sprintCheck.Count > 0)
                {
                    arm.status = 0;
                    arm.successFlag = false;
                    arm.errorMsg = "Couldn't Create Sprint!\nError: Sprint Name Already Exists In This Project.";

                }
                else
                {

                    Sprint newSprint= new Sprint();
                    newSprint.ProjectID = projectID;
                    newSprint.Name = sprint.Name;
                    newSprint.StartDate = sprint.StartDate;
                    newSprint.EndDate = sprint.EndDate;
                    newSprint.Goal = sprint.Goal;
                    newSprint.Status = sprint.Status;
                    newSprint.CreationDate = DateTime.Now;
                    newSprint.CreatedBy = int.Parse(Request.Cookies["userID"].Value);

                    dbObj.Sprints.InsertOnSubmit(newSprint);
                    dbObj.SubmitChanges();

                    arm.status = 1;
                    arm.successFlag = true;
                    arm.successMsg = "Sprint Created Successfully!";

                }

            }
            catch (Exception e)
            {
                arm.status = 0;
                arm.successFlag = false;
                arm.errorMsg = "Couldn't Create Sprint!\nError: " + e.Message.ToString();
            }

            return new JavaScriptSerializer().Serialize(arm);
        }

        [HttpPost]
        public string CreateNewEpic(Epic epic, int projectID)
        {
            try
            {
                arm = new AjaxReturnMSG();
                dbObj = new ManofrSqlDbModelDataContext();

                List<Epic> epicCheck = dbObj.Epics.Where(x => x.Name == epic.Name && x.ProjectID == projectID).ToList();

                if (epicCheck.Count > 0)
                {
                    arm.status = 0;
                    arm.successFlag = false;
                    arm.errorMsg = "Couldn't Create Epic!\nError: Epic Name Already Exists In This Project.";

                }
                else
                {

                    Epic newEpic= new Epic();
                    newEpic.ProjectID = projectID;
                    newEpic.Name = epic.Name;
                    if(epic.Module != 0)
                        newEpic.Module = epic.Module;

                    dbObj.Epics.InsertOnSubmit(newEpic);
                    dbObj.SubmitChanges();

                    arm.status = 1;
                    arm.successFlag = true;
                    arm.successMsg = "Epic Created Successfully!";

                }

            }
            catch (Exception e)
            {
                arm.status = 0;
                arm.successFlag = false;
                arm.errorMsg = "Couldn't Create Epic!\nError: " + e.Message.ToString();
            }

            return new JavaScriptSerializer().Serialize(arm);
        }

        [HttpPost]
        public string GetProjectUnAssignedUS(int projectID)
        {
            try
            {

                arm = new AjaxReturnMSG();
                dbObj = new ManofrSqlDbModelDataContext();

                var storiesList = (from us in dbObj.UserStories
                                   where
                                   us.ProjectID == projectID && us.SprintID == null
                                   select new
                                   {us.ID, us.FRS_Title, Status = us.ComponentsStatus.Status, CreatedBy = us.User.Fullname}).ToList();

                arm.status = 1;
                arm.successFlag = true;
                arm.successMsg = "";
                arm.returnedObject = storiesList;

            }
            catch (Exception e)
            {
                arm.status = 0;
                arm.successFlag = false;
                arm.errorMsg = "Couldn't Get User Stories List!\nError: " + e.Message.ToString();
            }

            return new JavaScriptSerializer().Serialize(arm);
        }

        [HttpPost]
        public string CreateNewUserStory(UserStory userStory, List<int> relatedStories, List<int> assignedUsers, bool hasFiles)
        {
            try
            {
                arm = new AjaxReturnMSG();
                dbObj = new ManofrSqlDbModelDataContext();

                List<UserStory> userStoryCheck = dbObj.UserStories.Where(x => x.FRS_Title == userStory.FRS_Title && x.ProjectID == userStory.ProjectID).ToList();

                if (userStoryCheck.Count > 0)
                {
                    arm.status = 0;
                    arm.successFlag = false;
                    arm.errorMsg = "Couldn't Create User Story!\nError: User Story With The Same Title Already Exists In This Project.";

                }
                else
                {

                    UserStory newStory = new UserStory();
                    newStory = userStory;
                    newStory.CreationDate = DateTime.Now;
                    newStory.CreatedBy = int.Parse(Request.Cookies["userID"].Value);

                    dbObj.UserStories.InsertOnSubmit(newStory);
                    dbObj.SubmitChanges();

                    if (hasFiles)
                    {
                        newStory.AttachmentsUrl = "/Uploads/User-Stories-Files/" + newStory.ID;
                        dbObj.SubmitChanges();
                    }

                    if (relatedStories != null) {
                        UserStory_Related_Story relStory = new UserStory_Related_Story();
                        for (int i = 0; i < relatedStories.Count; i++)
                        {
                            relStory = new UserStory_Related_Story();
                            relStory.UserStory = newStory.ID;
                            relStory.RelatedToStory = relatedStories[i];
                            dbObj.UserStory_Related_Stories.InsertOnSubmit(relStory);
                        }
                    }


                    if (assignedUsers != null) {
                        UserStory_Assigned_User assiUser = new UserStory_Assigned_User();
                        for (int i = 0; i < assignedUsers.Count; i++)
                        {
                            assiUser = new UserStory_Assigned_User();
                            assiUser.UserStory = newStory.ID;
                            assiUser.AssignedUser = assignedUsers[i];
                            dbObj.UserStory_Assigned_Users.InsertOnSubmit(assiUser);
                        }
                    }

                    dbObj.SubmitChanges();

                    arm.status = 1;
                    arm.successFlag = true;
                    arm.successMsg = "User Story Created Successfully!";
                    arm.returnedObject = @"/Uploads/User-Stories-Files/" + newStory.ID + "/";

                }

            }
            catch (Exception e)
            {
                arm.status = 0;
                arm.successFlag = false;
                arm.errorMsg = "Couldn't Create User Story!\nError: " + e.Message.ToString();
            }

            return new JavaScriptSerializer().Serialize(arm);
        }

        [HttpPost]
        public string UpdateUserStory(UserStory updatedUserStory, List<UserStory_Assigned_User> assignedUsers, List<UserStory_Related_Story> relatedStories, bool hasFiles) {
            try
            {
                arm = new AjaxReturnMSG();
                dbObj = new ManofrSqlDbModelDataContext();

                UserStory usToUpdate = dbObj.UserStories.Where(us => us.ID == updatedUserStory.ID).SingleOrDefault();

                usToUpdate.SprintID = updatedUserStory.SprintID;
                usToUpdate.EpicID = updatedUserStory.EpicID;
                usToUpdate.ModuleID = updatedUserStory.ModuleID;
                usToUpdate.StatusID = updatedUserStory.StatusID;
                usToUpdate.UsPriority = updatedUserStory.UsPriority;
                usToUpdate.FRC_CustomUserStory = updatedUserStory.FRC_CustomUserStory;
                usToUpdate.FRS_Title = updatedUserStory.FRS_Title;
                usToUpdate.FRS_AsA = updatedUserStory.FRS_AsA;
                usToUpdate.FRS_IWish = updatedUserStory.FRS_IWish;
                usToUpdate.FRS_SoThat = updatedUserStory.FRS_SoThat;
                usToUpdate.FRS_AcceptanceCriteria = updatedUserStory.FRS_AcceptanceCriteria;
                usToUpdate.NFR_PosOrNeg = updatedUserStory.NFR_PosOrNeg;
                usToUpdate.NFR_NFRID = updatedUserStory.NFR_NFRID;
                usToUpdate.NFR_Description = updatedUserStory.NFR_Description;
                usToUpdate.NFR_For = updatedUserStory.NFR_For;
                usToUpdate.NFR_IWant = updatedUserStory.NFR_IWant;
                usToUpdate.NFR_ToBe = updatedUserStory.NFR_ToBe;
                usToUpdate.LastUpdateDate = DateTime.Now;
                usToUpdate.UpdatedBy = int.Parse(Request.Cookies["userID"].Value);

                if (hasFiles)
                {
                    usToUpdate.AttachmentsUrl = "/Uploads/User-Stories-Files/" + usToUpdate.ID;
                }

                var olduserslist = dbObj.UserStory_Assigned_Users.Where(usau => usau.UserStory == updatedUserStory.ID).ToList();
                if(olduserslist != null && olduserslist.Count > 0)
                    dbObj.UserStory_Assigned_Users.DeleteAllOnSubmit(olduserslist);

                if (assignedUsers != null) {
                    UserStory_Assigned_User tmpAUser = new UserStory_Assigned_User();
                    foreach (UserStory_Assigned_User auser in assignedUsers)
                    {
                        tmpAUser = new UserStory_Assigned_User();
                        tmpAUser.UserStory = auser.UserStory;
                        tmpAUser.AssignedUser = auser.AssignedUser;
                        dbObj.UserStory_Assigned_Users.InsertOnSubmit(tmpAUser);
                    }
                }

                var oldrelatedstorieslist = dbObj.UserStory_Related_Stories.Where(usau => usau.UserStory == updatedUserStory.ID).ToList();
                if (oldrelatedstorieslist != null && oldrelatedstorieslist.Count > 0)
                    dbObj.UserStory_Related_Stories.DeleteAllOnSubmit(oldrelatedstorieslist);

                if (relatedStories != null) {
                    UserStory_Related_Story tmpRStory = new UserStory_Related_Story();
                    foreach (UserStory_Related_Story rstory in relatedStories)
                    {
                        tmpRStory = new UserStory_Related_Story();
                        tmpRStory.UserStory = rstory.UserStory;
                        tmpRStory.RelatedToStory = rstory.RelatedToStory;
                        dbObj.UserStory_Related_Stories.InsertOnSubmit(tmpRStory);
                    }
                }
                
                dbObj.SubmitChanges();

                arm.status = 1;
                arm.successFlag = true;
                arm.successMsg = "UserStory Updated Successfully!";

            }
            catch (Exception e)
            {
                arm.status = 0;
                arm.successFlag = false;
                arm.errorMsg = "Failed Updating User Story!\nError: " + e.Message.ToString();
            }

            return new JavaScriptSerializer().Serialize(arm);
        }

        [HttpPost]
        public string DeleteUserStory(int storyID)
        {
            try
            {
                arm = new AjaxReturnMSG();
                dbObj = new ManofrSqlDbModelDataContext();

                var assignedusers = dbObj.UserStory_Assigned_Users.Where(x => x.UserStory == storyID).ToList();
                var relatedStories = dbObj.UserStory_Related_Stories.Where(x => x.UserStory == storyID).ToList();

                UserStory storyToDelete = dbObj.UserStories.Where(std => std.ID == storyID).SingleOrDefault();

                if (storyToDelete != null)
                {
                    if (assignedusers != null)
                        dbObj.UserStory_Assigned_Users.DeleteAllOnSubmit(assignedusers);
                    if (relatedStories != null)
                        dbObj.UserStory_Related_Stories.DeleteAllOnSubmit(relatedStories);

                    dbObj.UserStories.DeleteOnSubmit(storyToDelete);
                    dbObj.SubmitChanges();

                    DirectoryInfo di = new DirectoryInfo(Server.MapPath("~/Uploads/User-Stories-Files/" + storyID));
                    if (di.Exists) {
                        foreach (FileInfo fileToDel in di.GetFiles())
                        {
                            fileToDel.Delete();
                        }
                        foreach (DirectoryInfo dir in di.GetDirectories())
                        {
                            dir.Delete(true);

                        }
                        di.Delete(true);
                    }

                    arm.status = 1;
                    arm.successFlag = true;
                    arm.successMsg = "User Story Deleted Successfully!";
                }
                else
                {

                    arm.status = 0;
                    arm.successFlag = false;
                    arm.errorMsg = "User Story Not Found In The Database!";

                }

            }
            catch (Exception e)
            {
                arm.status = 0;
                arm.successFlag = false;

                if (e.Message.Contains("conflicted with the REFERENCE constraint"))
                {
                    arm.errorMsg = "Failed Deleting User Story!\nError: It's being used by another item.";
                }
                else
                {
                    arm.errorMsg = "Failed Deleting User Story!\nError: " + e.Message.ToString();
                }
            }

            return new JavaScriptSerializer().Serialize(arm);
        }

        [HttpPost]
        public string GetProjectUserStories(UserStory story, int projectID)
        {
            try
            {

                arm = new AjaxReturnMSG();
                dbObj = new ManofrSqlDbModelDataContext();

                var storiesList = (from us in dbObj.UserStories
                                  where 

                                  us.ProjectID == projectID &&
                                  (story.ID == 0 ? 1 == 1 : us.ID == story.ID) &&
                                  (story.CreatedBy == 0 ? 1 == 1 : us.CreatedBy == story.CreatedBy) &&
                                  (story.SprintID == null ? 1 == 1 : us.SprintID == story.SprintID) &&
                                  (story.ModuleID == null ? 1 == 1 : us.ModuleID == story.ModuleID) &&
                                  (story.StatusID == 0 ? 1 == 1 : us.StatusID == story.StatusID) 

                                  select new
                                  {
                                      us.ID,us.ProjectID,
                                      Sprint = us.Sprint.Name,
                                      Epic = us.Epic.Name,
                                      Module = us.ModuleID,
                                      Status = us.ComponentsStatus.Status,
                                      USType = us.US_Type.Type_Value,
                                      RequirementType = us.US_Requirement_Type.Type_Value,
                                      RelatedTo = us.US_Relation.Relation_Value,
                                      Priority = us.US_Priority.Priority_Value,
                                      us.AttachmentsUrl,
                                      us.FRC_CustomUserStory,
                                      us.FRS_Title,
                                      us.FRS_AsA,
                                      us.FRS_IWish,
                                      us.FRS_SoThat,
                                      us.FRS_AcceptanceCriteria,
                                      Positivity = us.US_Positivity.PosNeg_Value,
                                      NFR = us.ProjectsNFR.NFR,
                                      us.NFR_For,
                                      us.NFR_IWant,
                                      us.NFR_ToBe,
                                      us.NFR_Description,
                                      CreationDate = us.CreationDate.ToString(),
                                      CreatedBy = us.User.Fullname,
                                      LastUpdate = us.LastUpdateDate.ToString(),
                                      UpdatedBy = us.User1.Fullname
                                  }).ToList();

                AssignedItemsToUS AITU = new AssignedItemsToUS();
                
                foreach ( var us in storiesList ){
                    var assignedusersList = (from usi in dbObj.UserStory_Assigned_Users
                                             where usi.UserStory == us.ID
                                             select new { usi.User.ID, usi.User.Fullname }).ToList();

                    var assignedstoriesList = (from usi in dbObj.UserStory_Related_Stories
                                             where usi.UserStory == us.ID
                                             select new { usi.UserStory1.ID, usi.UserStory1.FRS_Title }).ToList();

                    AssignedUsersToUS tmpAutu = new AssignedUsersToUS();
                    AssignedStoriesToUS tmpAstu = new AssignedStoriesToUS();

                    foreach (var user in assignedusersList) {
                        USUser tmpUser = new USUser();
                        tmpUser.ID = user.ID;
                        tmpUser.Name = user.Fullname;
                        tmpAutu.AssignedUsersList.Add(tmpUser);
                    }

                    foreach (var stry in assignedstoriesList) {
                        USStory tmpStory = new USStory();
                        tmpStory.ID = stry.ID;
                        tmpStory.Title = stry.FRS_Title;
                        tmpAstu.AssignedStoriesList.Add(tmpStory);
                    }

                    AITU.AssignedUsers.Add(tmpAutu);
                    AITU.AssignedStories.Add(tmpAstu);
                }

                //Debug.WriteLine(storiesList.Count + " - " + AITU.AssignedUsers.Count + " - " + AITU.AssignedStories.Count);
                
                arm.status = 1;
                arm.successFlag = true;
                arm.successMsg = "";
                arm.returnedObject = storiesList;
                arm.extraObject = AITU;

            }
            catch (Exception e)
            {
                arm.status = 0;
                arm.successFlag = false;
                arm.errorMsg = "Couldn't Get User Stories List!\nError: " + e.Message.ToString();
            }

            return new JavaScriptSerializer().Serialize(arm);
        }

        [HttpPost]
        public string AssignUserStoriesToSprint(int sprintID, List<int> userStoriesIDs) {
            try
            {

                arm = new AjaxReturnMSG();
                dbObj = new ManofrSqlDbModelDataContext();


                UserStory usToUpdate = new UserStory();
                foreach (int id in userStoriesIDs) {
                    usToUpdate = dbObj.UserStories.Where(us=> us.ID == id && us.SprintID == null).SingleOrDefault();
                    usToUpdate.SprintID = sprintID;
                    dbObj.SubmitChanges();
                }

                arm.status = 1;
                arm.successFlag = true;
                arm.successMsg = "User Stories Assigned Successfully!";

            }
            catch (Exception e)
            {
                arm.status = 0;
                arm.successFlag = false;
                arm.errorMsg = "Couldn't Get User Stories List!\nError: " + e.Message.ToString();
            }

            return new JavaScriptSerializer().Serialize(arm);
        }

        [HttpPost]
        public string CreateNewTask(Task task, List<int> assignedUsers, bool hasFiles)
        {
            try
            {
                arm = new AjaxReturnMSG();
                dbObj = new ManofrSqlDbModelDataContext();

                List<Task> userTaskCheck = dbObj.Tasks.Where(x => x.Title == task.Title && x.ProjectID == task.ProjectID).ToList();

                if (userTaskCheck.Count > 0)
                {
                    arm.status = 0;
                    arm.successFlag = false;
                    arm.errorMsg = "Couldn't Create Task!\nError: Task With The Same Title Already Exists In This Project.";

                }
                else
                {

                    Task newTask = new Task();
                    newTask = task;
                    newTask.CreationDate = DateTime.Now;
                    newTask.CreatedBy = int.Parse(Request.Cookies["userID"].Value);

                    dbObj.Tasks.InsertOnSubmit(newTask);
                    dbObj.SubmitChanges();

                    if (hasFiles) {
                        newTask.AttachmentUrl = "/Uploads/Tasks-Files/" + newTask.ID;
                        dbObj.SubmitChanges();
                    }

                    if (assignedUsers != null)
                    {
                        Tasks_Assigned_User assiUser = new Tasks_Assigned_User();
                        for (int i = 0; i < assignedUsers.Count; i++)
                        {
                            assiUser = new Tasks_Assigned_User();
                            assiUser.TaskID = newTask.ID;
                            assiUser.AssignedUser = assignedUsers[i];
                            dbObj.Tasks_Assigned_Users.InsertOnSubmit(assiUser);
                        }
                    }

                    dbObj.SubmitChanges();

                    arm.status = 1;
                    arm.successFlag = true;
                    arm.successMsg = "Task Created Successfully!";
                    arm.returnedObject = @"/Uploads/Tasks-Files/" + newTask.ID + "/";

                }

            }
            catch (Exception e)
            {
                arm.status = 0;
                arm.successFlag = false;
                arm.errorMsg = "Couldn't Create Task!\nError: " + e.Message.ToString();
            }

            return new JavaScriptSerializer().Serialize(arm);
        }

        [HttpPost]
        public string GetProjectTasks(Task task, int projectID)
        {
            try
            {

                arm = new AjaxReturnMSG();
                dbObj = new ManofrSqlDbModelDataContext();

                var tasksList = (from tk in dbObj.Tasks
                                 where
                                 task.ProjectID == projectID &&
                                 (task.ID == 0 ? 1 == 1 : tk.ID == task.ID) &&
                                 (task.CreatedBy == 0 ? 1 == 1 : tk.CreatedBy == task.CreatedBy) &&
                                 (task.AssignedUserStory == null ? 1 == 1 : tk.AssignedUserStory == task.AssignedUserStory) &&
                                 (task.Status == 0 ? 1 == 1 : tk.Status == task.Status)

                                 select new
                                 {
                                     tk.ID,
                                     tk.ProjectID,
                                     tk.Title,
                                     tk.Description,
                                     Status = tk.ComponentsStatus.Status,
                                     Priority = tk.US_Priority.Priority_Value,
                                     UserstoryID = tk.AssignedUserStory,
                                     Userstory = tk.UserStory.FRS_Title,
                                     ReqTypeID = tk.RequirementType,
                                     ReqType = tk.US_Requirement_Type.Type_Value,
                                     tk.AttachmentUrl,
                                     CreationDate = tk.CreationDate.ToString(),
                                     CreatedBy = tk.User.Fullname,
                                     LastUpdate = tk.LastUpdateDate.ToString(),
                                     UpdatedBy = tk.User1.Fullname
                                 }).ToList();

                AssignedItemsToUS AITU = new AssignedItemsToUS();

                foreach (var tk in tasksList)
                {
                    var assignedusersList = (from tki in dbObj.Tasks_Assigned_Users
                                             where tki.TaskID == tk.ID
                                             select new { tki.User.ID, tki.User.Fullname }).ToList();

                    AssignedUsersToUS tmpAutu = new AssignedUsersToUS();

                    foreach (var user in assignedusersList)
                    {
                        USUser tmpUser = new USUser();
                        tmpUser.ID = user.ID;
                        tmpUser.Name = user.Fullname;
                        tmpAutu.AssignedUsersList.Add(tmpUser);
                    }

                    AITU.AssignedUsers.Add(tmpAutu);
                }

                //Debug.WriteLine(tasksList.Count + " - " + AITU.AssignedUsers.Count + " - " + AITU.AssignedStories.Count);

                arm.status = 1;
                arm.successFlag = true;
                arm.successMsg = "";
                arm.returnedObject = tasksList;
                arm.extraObject = AITU;

            }
            catch (Exception e)
            {
                arm.status = 0;
                arm.successFlag = false;
                arm.errorMsg = "Couldn't Get Tasks List!\nError: " + e.Message.ToString();
            }

            return new JavaScriptSerializer().Serialize(arm);
        }

        [HttpPost]
        public string GetUsersCreatedTasks(int projectID)
        {
            try
            {

                arm = new AjaxReturnMSG();
                dbObj = new ManofrSqlDbModelDataContext();

                var list = (from tk in dbObj.Tasks
                            where tk.ProjectID == projectID
                            select new { tk.User.ID, tk.User.Fullname, tk.User.Email }).ToList().Distinct();

                arm.status = 1;
                arm.successFlag = true;
                arm.successMsg = "Users List Was Generated Successfully!";
                arm.returnedObject = list;

            }
            catch (Exception e)
            {
                arm.status = 0;
                arm.successFlag = false;
                arm.errorMsg = "Couldn't Get Users List!\nError: " + e.Message.ToString();
            }

            return new JavaScriptSerializer().Serialize(arm);
        }

        [HttpPost]
        public string UpdateTask(Task updatedTask, List<Tasks_Assigned_User> assignedUsers, bool hasFiles)
        {
            try
            {
                arm = new AjaxReturnMSG();
                dbObj = new ManofrSqlDbModelDataContext();

                Task tkToUpdate = dbObj.Tasks.Where(us => us.ID == updatedTask.ID).SingleOrDefault();

                tkToUpdate.Title = updatedTask.Title;
                tkToUpdate.Description = updatedTask.Description;
                tkToUpdate.Status = updatedTask.Status;
                tkToUpdate.Priority = updatedTask.Priority;
                tkToUpdate.AssignedUserStory = updatedTask.AssignedUserStory;
                tkToUpdate.RequirementType = updatedTask.RequirementType;
                tkToUpdate.LastUpdateDate = DateTime.Now;
                tkToUpdate.UpdatedBy = int.Parse(Request.Cookies["userID"].Value);

                if (hasFiles)
                {
                    tkToUpdate.AttachmentUrl = "/Uploads/Tasks-Files/" + tkToUpdate.ID;
                }

                var olduserslist = dbObj.Tasks_Assigned_Users.Where(oul => oul.TaskID == updatedTask.ID).ToList();
                if (olduserslist != null && olduserslist.Count > 0)
                    dbObj.Tasks_Assigned_Users.DeleteAllOnSubmit(olduserslist);

                if (assignedUsers != null)
                {
                    Tasks_Assigned_User tmpAUser = new Tasks_Assigned_User();
                    foreach (Tasks_Assigned_User auser in assignedUsers)
                    {
                        tmpAUser = new Tasks_Assigned_User();
                        tmpAUser.TaskID = auser.TaskID;
                        tmpAUser.AssignedUser = auser.AssignedUser;
                        dbObj.Tasks_Assigned_Users.InsertOnSubmit(tmpAUser);
                    }
                }

                dbObj.SubmitChanges();

                arm.status = 1;
                arm.successFlag = true;
                arm.successMsg = "Task Updated Successfully!";

            }
            catch (Exception e)
            {
                arm.status = 0;
                arm.successFlag = false;
                arm.errorMsg = "Failed Updating Task!\nError: " + e.Message.ToString();
            }

            return new JavaScriptSerializer().Serialize(arm);
        }

        [HttpPost]
        public string DeleteTask(int taskID)
        {
            try
            {
                arm = new AjaxReturnMSG();
                dbObj = new ManofrSqlDbModelDataContext();

                var assignedusers = dbObj.Tasks_Assigned_Users.Where(x => x.TaskID == taskID).ToList();
                Task taskToDelete = dbObj.Tasks.Where(std => std.ID == taskID).SingleOrDefault();

                if (taskToDelete != null)
                {
                    if(assignedusers != null)
                        dbObj.Tasks_Assigned_Users.DeleteAllOnSubmit(assignedusers);
                    dbObj.Tasks.DeleteOnSubmit(taskToDelete);
                    dbObj.SubmitChanges();

                    DirectoryInfo di = new DirectoryInfo(Server.MapPath("~/Uploads/Tasks-Files/" + taskID));
                    foreach (FileInfo fileToDel in di.GetFiles())
                    {
                        fileToDel.Delete();
                    }
                    foreach (DirectoryInfo dir in di.GetDirectories())
                    {
                        dir.Delete(true);

                    }
                    di.Delete(true);

                    arm.status = 1;
                    arm.successFlag = true;
                    arm.successMsg = "Task Deleted Successfully!";
                }
                else
                {

                    arm.status = 0;
                    arm.successFlag = false;
                    arm.errorMsg = "Task Not Found In The Database!";

                }

            }
            catch (Exception e)
            {
                arm.status = 0;
                arm.successFlag = false;
                if (e.Message.Contains("conflicted with the REFERENCE constraint"))
                {
                    arm.errorMsg = "Failed Deleting Task!\nError: It's being used by another item.";
                }
                else
                {
                    arm.errorMsg = "Failed Deleting Task!\nError: " + e.Message.ToString();
                }
            }

            return new JavaScriptSerializer().Serialize(arm);
        }

        [HttpGet]
        public string GetUsReqTypes() {
            try
            {

                arm = new AjaxReturnMSG();
                dbObj = new ManofrSqlDbModelDataContext();

                var list = (from cs in dbObj.US_Requirement_Types
                            select new { cs.ID, cs.Type_Value }).ToList();

                arm.status = 1;
                arm.successFlag = true;
                arm.successMsg = "Requirements Types List Was Generated Successfully!";
                arm.returnedObject = list;

            }
            catch (Exception e)
            {
                arm.status = 0;
                arm.successFlag = false;
                arm.errorMsg = "Couldn't Get Requirements Types List!\nError: " + e.Message.ToString();
            }

            return new JavaScriptSerializer().Serialize(arm);
        }

        [HttpGet]
        public string GetUsRelations()
        {
            try
            {

                arm = new AjaxReturnMSG();
                dbObj = new ManofrSqlDbModelDataContext();

                var list = (from cs in dbObj.US_Relations
                            select new { cs.ID, cs.Relation_Value, cs.Req_Type }).ToList();

                arm.status = 1;
                arm.successFlag = true;
                arm.successMsg = "Relations List Was Generated Successfully!";
                arm.returnedObject = list;

            }
            catch (Exception e)
            {
                arm.status = 0;
                arm.successFlag = false;
                arm.errorMsg = "Couldn't Get Relations List!\nError: " + e.Message.ToString();
            }

            return new JavaScriptSerializer().Serialize(arm);
        }

        [HttpGet]
        public string GetUsPriorities()
        {
            try
            {

                arm = new AjaxReturnMSG();
                dbObj = new ManofrSqlDbModelDataContext();

                var list = (from cs in dbObj.US_Priorities
                            select new { cs.ID, cs.Priority_Value }).ToList();

                arm.status = 1;
                arm.successFlag = true;
                arm.successMsg = "Priorities List Was Generated Successfully!";
                arm.returnedObject = list;

            }
            catch (Exception e)
            {
                arm.status = 0;
                arm.successFlag = false;
                arm.errorMsg = "Couldn't Get Priorities List!\nError: " + e.Message.ToString();
            }

            return new JavaScriptSerializer().Serialize(arm);
        }

        [HttpGet]
        public string GetUsTypes()
        {
            try
            {

                arm = new AjaxReturnMSG();
                dbObj = new ManofrSqlDbModelDataContext();

                var list = (from cs in dbObj.US_Types
                            select new { cs.ID, cs.Type_Value }).ToList();

                arm.status = 1;
                arm.successFlag = true;
                arm.successMsg = "Types List Was Generated Successfully!";
                arm.returnedObject = list;

            }
            catch (Exception e)
            {
                arm.status = 0;
                arm.successFlag = false;
                arm.errorMsg = "Couldn't Get Types List!\nError: " + e.Message.ToString();
            }

            return new JavaScriptSerializer().Serialize(arm);
        }

        [HttpGet]
        public string GetUsPositivity()
        {
            try
            {

                arm = new AjaxReturnMSG();
                dbObj = new ManofrSqlDbModelDataContext();

                var list = (from cs in dbObj.US_Positivities
                            select new { cs.ID, cs.PosNeg_Value }).ToList();

                arm.status = 1;
                arm.successFlag = true;
                arm.successMsg = "Positivity List Was Generated Successfully!";
                arm.returnedObject = list;

            }
            catch (Exception e)
            {
                arm.status = 0;
                arm.successFlag = false;
                arm.errorMsg = "Couldn't Get Positivity List!\nError: " + e.Message.ToString();
            }

            return new JavaScriptSerializer().Serialize(arm);
        }

        [HttpPost]
        public string GetProjectsList(string name, int? domain)
        {
            try
            {

                arm = new AjaxReturnMSG();
                dbObj = new ManofrSqlDbModelDataContext();

                var projList = (from pl in dbObj.Projects
                                  where 
                                  (pl.isSuspended == false || pl.isSuspended == null)&&
                                  (name == "" ? 1 == 1 : pl.Name.Contains(name)) &&
                                  (domain == null ? 1 == 1 : pl.Domain == domain)
                                  select new { 
                                      pl.ID, 
                                      pl.Name, 
                                      Domain = pl.Domain1.Domain1, 
                                      SysType = pl.ApplicationsType.Type, 
                                      Status = pl.ComponentsStatus.Status,
                                      pl.CustomerName, 
                                      pl.CustomerPhone, 
                                      pl.CustomerEmail, 
                                      StartDate = pl.StartDate.ToString(), 
                                      EndDate = pl.EndDate.ToString(),
                                      StartDateRaw = pl.StartDate,
                                      EndDateRaw = pl.EndDate,
                                      pl.Description
                                  }).ToList();

                arm.status = 1;
                arm.successFlag = true;
                arm.successMsg = "";
                arm.returnedObject = projList;

            }
            catch (Exception e)
            {
                arm.status = 0;
                arm.successFlag = false;
                arm.errorMsg = "Couldn't Get Projects List!\nError: " + e.Message.ToString();
            }

            return new JavaScriptSerializer().Serialize(arm);
        }

        [HttpPost]
        public string GetProjectSprints(int projectID)
        {
            try
            {

                arm = new AjaxReturnMSG();
                dbObj = new ManofrSqlDbModelDataContext();

                var sprintList = (from sl in dbObj.Sprints
                                where sl.ProjectID == projectID
                                select new
                                { 
                                    sl.ID, 
                                    sl.ProjectID, 
                                    sl.Name, 
                                    StartDate = sl.StartDate.ToString(), 
                                    EndDate = sl.EndDate.ToString(), 
                                    sl.Goal, 
                                    Status = sl.ComponentsStatus.Status
                                }).ToList();

                arm.status = 1;
                arm.successFlag = true;
                arm.successMsg = "";
                arm.returnedObject = sprintList;

            }
            catch (Exception e)
            {
                arm.status = 0;
                arm.successFlag = false;
                arm.errorMsg = "Couldn't Get Sprints List!\nError: " + e.Message.ToString();
            }

            return new JavaScriptSerializer().Serialize(arm);
        }

        [HttpPost]
        public string GetProjUserStoriesList(int projectID)
        {
            try
            {

                arm = new AjaxReturnMSG();
                dbObj = new ManofrSqlDbModelDataContext();

                var usList = (from sl in dbObj.UserStories
                                  where sl.ProjectID == projectID
                                  select new
                                  {
                                      sl.ID,
                                      sl.ProjectID,
                                      Title = sl.FRS_Title
                                  }).ToList();

                arm.status = 1;
                arm.successFlag = true;
                arm.successMsg = "";
                arm.returnedObject = usList;

            }
            catch (Exception e)
            {
                arm.status = 0;
                arm.successFlag = false;
                arm.errorMsg = "Couldn't Get Sprints List!\nError: " + e.Message.ToString();
            }

            return new JavaScriptSerializer().Serialize(arm);
        }

        [HttpPost]
        public string GetProjectEpics(int projectID)
        {
            try
            {

                arm = new AjaxReturnMSG();
                dbObj = new ManofrSqlDbModelDataContext();

                var epicList = (from el in dbObj.Epics
                                  where el.ProjectID == projectID
                                  select new
                                  {
                                      el.ID,
                                      el.ProjectID,
                                      el.Name,
                                      Module = el.Module1.Module1,
                                      Domain = el.Module1.Domain1.Domain1
                                  }).ToList();

                arm.status = 1;
                arm.successFlag = true;
                arm.successMsg = "";
                arm.returnedObject = epicList;

            }
            catch (Exception e)
            {
                arm.status = 0;
                arm.successFlag = false;
                arm.errorMsg = "Couldn't Get Epics List!\nError: " + e.Message.ToString();
            }

            return new JavaScriptSerializer().Serialize(arm);
        }

        [HttpPost]
        public string GetSpecificProject(int projID)
        {
            try
            {

                arm = new AjaxReturnMSG();
                dbObj = new ManofrSqlDbModelDataContext();

                ProjectExtras pe = new ProjectExtras();
                pe.moulesCount = dbObj.ProjectsModules.Where(pm => pm.Project == projID).ToList().Count;
                pe.nfrsCount = dbObj.ProjectsNFRs.Where(pn => pn.ProjectID == projID).ToList().Count;
                pe.sprintsCount = dbObj.Sprints.Where(pm => pm.ProjectID == projID).ToList().Count;

                var proj = (from pr in dbObj.Projects
                                where projID == pr.ID
                                select new
                                {
                                    pr.ID,
                                    pr.Name,
                                    Domain = pr.Domain1.Domain1,
                                    SysType = pr.ApplicationsType.Type,
                                    Status = pr.ComponentsStatus.Status,
                                    pr.CustomerName,
                                    pr.CustomerPhone,
                                    pr.CustomerEmail,
                                    StartDate = pr.StartDate.ToString(),
                                    EndDate = pr.EndDate.ToString(),
                                    pr.Description,
                                    CreationDate = pr.CreationDate.ToString()
                                }).SingleOrDefault();


                arm.status = 1;
                arm.successFlag = true;
                arm.successMsg = "";
                arm.returnedObject = proj;
                arm.extraObject = pe;

            }
            catch (Exception e)
            {
                arm.status = 0;
                arm.successFlag = false;
                arm.errorMsg = "Couldn't Get Project!\nError: " + e.Message.ToString();
            }

            return new JavaScriptSerializer().Serialize(arm);
        }

        [HttpGet]
        public string GetStatus()
        {
            try
            {

                arm = new AjaxReturnMSG();
                dbObj = new ManofrSqlDbModelDataContext();

                var list = (from cs in dbObj.ComponentsStatus
                            select new { cs.ID, cs.Status }).ToList();

                arm.status = 1;
                arm.successFlag = true;
                arm.successMsg = "Status List Was Generated Successfully!";
                arm.returnedObject = list;

            }
            catch (Exception e)
            {
                arm.status = 0;
                arm.successFlag = false;
                arm.errorMsg = "Couldn't Get Status List!\nError: " + e.Message.ToString();
            }

            return new JavaScriptSerializer().Serialize(arm);
        }

        [HttpGet]
        public string GetSystemTypes()
        {
            try
            {

                arm = new AjaxReturnMSG();
                dbObj = new ManofrSqlDbModelDataContext();

                var list = (from at in dbObj.ApplicationsTypes
                            select new { at.ID, at.Type }).ToList();

                arm.status = 1;
                arm.successFlag = true;
                arm.successMsg = "System Types List Was Generated Successfully!";
                arm.returnedObject = list;

            }
            catch (Exception e)
            {
                arm.status = 0;
                arm.successFlag = false;
                arm.errorMsg = "Couldn't Get System Types List!\nError: " + e.Message.ToString();
            }

            return new JavaScriptSerializer().Serialize(arm);
        }

        [HttpPost]
        public string GetUsersCreatedUserStories(int projectID) {
            try
            {

                arm = new AjaxReturnMSG();
                dbObj = new ManofrSqlDbModelDataContext();

                var list = (from us in dbObj.UserStories
                            where us.ProjectID == projectID
                            select new { us.User.ID, us.User.Fullname, us.User.Email }).ToList().Distinct();

                arm.status = 1;
                arm.successFlag = true;
                arm.successMsg = "Users List Was Generated Successfully!";
                arm.returnedObject = list;

            }
            catch (Exception e)
            {
                arm.status = 0;
                arm.successFlag = false;
                arm.errorMsg = "Couldn't Get Users List!\nError: " + e.Message.ToString();
            }

            return new JavaScriptSerializer().Serialize(arm);
        }

        [HttpPost]
        public string GetUsersList(int? filterVal, int projectID)
        {
            try
            {

                arm = new AjaxReturnMSG();
                dbObj = new ManofrSqlDbModelDataContext();

                List<Object> dummyList = new List<object>();

                if (filterVal == 1) {
                    var assignedusers = (from user in dbObj.ProjectsUsers
                                         where user.Project == projectID
                                         select new { user.User.ID, Email = user.User.Email, Fullname = user.User.Fullname, Title = user.User.UsersTitle.Title }).ToList();

                    arm.status = 1;
                    arm.successFlag = true;
                    arm.successMsg = "Users Was Generated Successfully!";
                    arm.returnedObject = assignedusers;
                    arm.extraObject = dummyList;
                }

                if (filterVal == 2) {
                    var unassignedusers = (from user in dbObj.Users
                                           where !dbObj.ProjectsUsers.Any(f => f.SysUser == user.ID && f.Project == projectID)
                                           select new { user.ID, user.Email, user.Fullname, user.UsersTitle.Title }).ToList();

                    arm.status = 1;
                    arm.successFlag = true;
                    arm.successMsg = "Users Was Generated Successfully!";
                    arm.returnedObject = dummyList;
                    arm.extraObject = unassignedusers;
                }

                if (filterVal == 3 || filterVal == null) {

                    var assignedusers = (from user in dbObj.ProjectsUsers
                                         where user.Project == projectID
                                         select new { user.User.ID, Email = user.User.Email, Fullname = user.User.Fullname, Title = user.User.UsersTitle.Title }).ToList();

                    var unassignedusers = (from user in dbObj.Users
                                       where !dbObj.ProjectsUsers.Any(f => f.SysUser == user.ID && f.Project == projectID)
                                       select new { user.ID, user.Email, user.Fullname, user.UsersTitle.Title }).ToList();

                    arm.status = 1;
                    arm.successFlag = true;
                    arm.successMsg = "Users Was Generated Successfully!";
                    arm.returnedObject = assignedusers;
                    arm.extraObject = unassignedusers;
                }

            }
            catch (Exception e)
            {
                arm.status = 0;
                arm.successFlag = false;
                arm.errorMsg = "Couldn't Get Users List!\nError: " + e.Message.ToString();
            }

            return new JavaScriptSerializer().Serialize(arm);
        }

        [HttpPost]
        public string UpdateProject(Project updatedProject)
        {
            try
            {
                arm = new AjaxReturnMSG();
                dbObj = new ManofrSqlDbModelDataContext();

                Project projectToUpdate = dbObj.Projects.Where(pr => pr.ID == updatedProject.ID).SingleOrDefault();
                List<Project> projWithSameNameDomain = dbObj.Projects.Where(x => x.Name == updatedProject.Name && x.Domain == updatedProject.Domain && x.ID != updatedProject.ID).ToList();

                if (projectToUpdate != null && projWithSameNameDomain.Count <= 0)
                {
                    if (updatedProject.StartDate < projectToUpdate.CreationDate) {
                        arm.status = 0;
                        arm.successFlag = false;
                        arm.errorMsg = "Start Date \""+ (updatedProject.StartDate != null ? updatedProject.StartDate.Value.ToString("yyyy-MM-dd") : "n/a") + "\"" + " Must Be After Project Creation Date \""+ projectToUpdate.CreationDate.ToString("yyyy-MM-dd") + "\"";

                        return new JavaScriptSerializer().Serialize(arm);
                    }

                    if (updatedProject.EndDate < projectToUpdate.CreationDate)
                    {
                        arm.status = 0;
                        arm.successFlag = false;
                        arm.errorMsg = "End Date \"" + (updatedProject.EndDate != null ? updatedProject.EndDate.Value.ToString("yyyy-MM-dd") : "n/a") + "\"" + " Must Be After Project Creation Date \"" + projectToUpdate.CreationDate.ToString("yyyy-MM-dd") + "\"";

                        return new JavaScriptSerializer().Serialize(arm);
                    }

                    projectToUpdate.Name = updatedProject.Name;
                    projectToUpdate.Domain = updatedProject.Domain;
                    projectToUpdate.SystemType = updatedProject.SystemType;
                    projectToUpdate.Status = updatedProject.Status;
                    projectToUpdate.CustomerName = updatedProject.CustomerName;
                    projectToUpdate.CustomerPhone = updatedProject.CustomerPhone;
                    projectToUpdate.CustomerEmail = updatedProject.CustomerEmail;
                    projectToUpdate.StartDate = updatedProject.StartDate;
                    projectToUpdate.EndDate = updatedProject.EndDate;
                    projectToUpdate.Description = updatedProject.Description;

                    dbObj.SubmitChanges();

                    arm.status = 1;
                    arm.successFlag = true;
                    arm.successMsg = "Project Info Updated Successfully!";

                }
                else if (projectToUpdate != null && projWithSameNameDomain.Count > 0)
                {
                    arm.status = 0;
                    arm.successFlag = false;
                    arm.errorMsg = "Another Project With The Same Name And Domain Exists!";
                }
                else if (projectToUpdate == null)
                {

                    arm.status = 0;
                    arm.successFlag = false;
                    arm.errorMsg = "Project Not Found In The Database!";

                }

            }
            catch (Exception e)
            {
                arm.status = 0;
                arm.successFlag = false;
                arm.errorMsg = "Failed Updating Project Info!\nError: " + e.Message.ToString();
            }

            return new JavaScriptSerializer().Serialize(arm);
        }

        [HttpPost]
        public string UpdateProjectEpic(Epic updatedEpic)
        {
            try
            {
                arm = new AjaxReturnMSG();
                dbObj = new ManofrSqlDbModelDataContext();

                Epic epicToUpdate = dbObj.Epics.Where(ep => ep.ID == updatedEpic.ID).SingleOrDefault();
                List<Epic> epicNameDuplicate = dbObj.Epics.Where(end => end.Name == updatedEpic.Name && end.ProjectID == updatedEpic.ProjectID && end.ID != updatedEpic.ID).ToList();

                if (epicToUpdate != null && epicNameDuplicate.Count <= 0)
                {

                    epicToUpdate.Name = updatedEpic.Name;
                    epicToUpdate.Module = updatedEpic.Module;

                    dbObj.SubmitChanges();

                    arm.status = 1;
                    arm.successFlag = true;
                    arm.successMsg = "Epic Updated Successfully!";

                }
                else if (epicToUpdate != null && epicNameDuplicate.Count > 0)
                {
                    arm.status = 0;
                    arm.successFlag = false;
                    arm.errorMsg = "Another Epic Has The Same Name in This Project!";
                }
                else if (epicToUpdate == null)
                {

                    arm.status = 0;
                    arm.successFlag = false;
                    arm.errorMsg = "Epic Not Found In The Database!";

                }

            }
            catch (Exception e)
            {
                arm.status = 0;
                arm.successFlag = false;
                arm.errorMsg = "Failed Updating Epic!\nError: " + e.Message.ToString();
            }

            return new JavaScriptSerializer().Serialize(arm);
        }

        [HttpPost]
        public string DeleteEpic(int epicID)
        {
            try
            {
                arm = new AjaxReturnMSG();
                dbObj = new ManofrSqlDbModelDataContext();

                Epic epicToDelete = dbObj.Epics.Where(etd => etd.ID == epicID).SingleOrDefault();

                if (epicToDelete != null)
                {
                    dbObj.Epics.DeleteOnSubmit(epicToDelete);
                    dbObj.SubmitChanges();

                    arm.status = 1;
                    arm.successFlag = true;
                    arm.successMsg = "Epic Deleted Successfully!";
                }
                else
                {

                    arm.status = 0;
                    arm.successFlag = false;
                    arm.errorMsg = "Epic Not Found In The Database!";

                }

            }
            catch (Exception e)
            {
                arm.status = 0;
                arm.successFlag = false;
                if (e.Message.Contains("conflicted with the REFERENCE constraint"))
                {
                    arm.errorMsg = "Failed Deleting Epic!\nError: It's being used by another item.";
                }
                else
                {
                    arm.errorMsg = "Failed Deleting Epic!\nError: " + e.Message.ToString();
                }
            }

            return new JavaScriptSerializer().Serialize(arm);
        }

        [HttpPost]
        public string UpdateProjectSprint(Sprint updatedSprint)
        {
            try
            {
                arm = new AjaxReturnMSG();
                dbObj = new ManofrSqlDbModelDataContext();

                Sprint sprintToUpdate = dbObj.Sprints.Where(stu => stu.ID == updatedSprint.ID).SingleOrDefault();
                List<Sprint> sprintWithSameName = dbObj.Sprints.Where(x => x.Name == updatedSprint.Name && x.ID != updatedSprint.ID && x.ProjectID == updatedSprint.ProjectID).ToList();

                if (sprintToUpdate != null && sprintWithSameName.Count <= 0)
                {
                    if (updatedSprint.StartDate < sprintToUpdate.CreationDate)
                    {
                        arm.status = 0;
                        arm.successFlag = false;
                        arm.errorMsg = "Start Date \"" + updatedSprint.StartDate.ToString("yyyy-MM-dd") + "\"" + " Must Be After Sprint Creation Date \"" + sprintToUpdate.CreationDate.ToString("yyyy-MM-dd") + "\"";

                        return new JavaScriptSerializer().Serialize(arm);
                    }

                    if (updatedSprint.EndDate < sprintToUpdate.CreationDate)
                    {
                        arm.status = 0;
                        arm.successFlag = false;
                        arm.errorMsg = "End Date \"" +  updatedSprint.EndDate.ToString("yyyy-MM-dd") + "\"" + " Must Be After Sprint Creation Date \"" + sprintToUpdate.CreationDate.ToString("yyyy-MM-dd") + "\"";

                        return new JavaScriptSerializer().Serialize(arm);
                    }

                    sprintToUpdate.Name = updatedSprint.Name;
                    sprintToUpdate.StartDate = updatedSprint.StartDate;
                    sprintToUpdate.EndDate = updatedSprint.EndDate;
                    sprintToUpdate.Goal = updatedSprint.Goal;
                    sprintToUpdate.Status = updatedSprint.Status;

                    dbObj.SubmitChanges();

                    arm.status = 1;
                    arm.successFlag = true;
                    arm.successMsg = "Sprint Info Updated Successfully!";

                }
                else if (sprintToUpdate != null && sprintWithSameName.Count > 0)
                {
                    arm.status = 0;
                    arm.successFlag = false;
                    arm.errorMsg = "Another Sprint With The Same Name Exists In This Project!";
                }
                else if (sprintToUpdate == null)
                {

                    arm.status = 0;
                    arm.successFlag = false;
                    arm.errorMsg = "Sprint Not Found In The Database!";

                }

            }
            catch (Exception e)
            {
                arm.status = 0;
                arm.successFlag = false;
                arm.errorMsg = "Failed Updating Sprint Info!\nError: " + e.Message.ToString();
            }

            return new JavaScriptSerializer().Serialize(arm);
        }

        [HttpPost]
        public string DeleteProjectSprint(int sprintID)
        {
            try
            {
                arm = new AjaxReturnMSG();
                dbObj = new ManofrSqlDbModelDataContext();

                Sprint sprintToDelete = dbObj.Sprints.Where(std => std.ID == sprintID).SingleOrDefault();

                if (sprintToDelete != null)
                {
                    dbObj.Sprints.DeleteOnSubmit(sprintToDelete);
                    dbObj.SubmitChanges();

                    arm.status = 1;
                    arm.successFlag = true;
                    arm.successMsg = "Sprint Deleted Successfully!";
                }
                else
                {

                    arm.status = 0;
                    arm.successFlag = false;
                    arm.errorMsg = "Sprint Not Found In The Database!";

                }

            }
            catch (Exception e)
            {
                arm.status = 0;
                arm.successFlag = false;
                if (e.Message.Contains("conflicted with the REFERENCE constraint"))
                {
                    arm.errorMsg = "Failed Deleting Sprint!\nError: It's being used by another item.";
                } else
                {
                    arm.errorMsg = "Failed Deleting Sprint!\nError: " + e.Message.ToString();
                }
            }

            return new JavaScriptSerializer().Serialize(arm);
        }

        [HttpGet]
        public string GetSprintsForCharts() {
            try
            {
                arm = new AjaxReturnMSG();
                dbObj = new ManofrSqlDbModelDataContext();

                var sprintsList = (from spr in dbObj.Sprints
                                   select new { spr.ID, spr.Name,
                                       spr.ProjectID, EndDate = spr.EndDate.ToString(), StatusID = spr.Status,
                                       spr.ComponentsStatus.Status }).ToList().OrderByDescending(x=> x.StatusID);

                arm.status = 1;
                arm.successFlag = true;
                arm.successMsg = "";
                arm.returnedObject = sprintsList;
            }
            catch (Exception e)
            {
                arm.status = 0;
                arm.successFlag = false;
                arm.errorMsg = "Failed Getting Sprints For Chart!\nError: " + e.Message.ToString();
            }

            return new JavaScriptSerializer().Serialize(arm);
        }

        public class NFRRecResult {
            public string NFR { get; set; }
            public double Usage { get; set; }
            public NFRRecResult()
            {
                NFR = "";
                Usage = 0;
            }
        }
        [HttpPost]
        public string GetTopRecomendedNFR(int Top, int ActiveProjectID) {
            try
            {
                arm = new AjaxReturnMSG();
                dbObj = new ManofrSqlDbModelDataContext();
                List<NFRRecResult> nfrResult = new List<NFRRecResult>();
                //List<NFRRecResult> nfrResultSorted = new List<NFRRecResult>();

                int domain = dbObj.Projects.Where(x => x.ID == ActiveProjectID).Select(s => s.Domain).FirstOrDefault();
                int sysType = dbObj.Projects.Where(x => x.ID == ActiveProjectID).Select(s => s.SystemType).FirstOrDefault();
                int totalNFRS = dbObj.Projects.Where(x=> x.Domain == domain && x.SystemType == sysType).ToList().Count;

                var nfrList = (from nfrs in dbObj.ProjectsNFRs
                                        where nfrs.Project.Domain == domain && nfrs.Project.SystemType == sysType
                                        select new
                                        {
                                            nfrs.NFR
                                        }).Distinct();
                NFRRecResult tmp = new NFRRecResult();

                foreach (var nfr in nfrList) {
                    tmp = new NFRRecResult();
                    tmp.NFR = nfr.NFR;
                    double count = dbObj.ProjectsNFRs.Where(x => x.Project.Domain == domain && x.Project.SystemType == sysType && x.NFR == nfr.NFR).ToList().Count;
                    double res = count / totalNFRS;
                    tmp.Usage = Math.Round((res * 100), 1);
                    nfrResult.Add(tmp);
                }

                var nfrResultSorted = nfrResult.OrderByDescending(o => o.Usage).Take(Top);

                arm.status = 1;
                arm.successFlag = true;
                arm.successMsg = "";
                arm.returnedObject = nfrResultSorted;
            }
            catch (Exception e)
            {
                arm.status = 0;
                arm.successFlag = false;
                arm.errorMsg = "Failed Getting NFR List!\nError: " + e.Message.ToString();
            }

            return new JavaScriptSerializer().Serialize(arm);
        }

        [HttpPost]
        public string GetReccomendedNFRsForProject(int projectID)
        {
            try
            {
                arm = new AjaxReturnMSG();
                dbObj = new ManofrSqlDbModelDataContext();

                string domain = dbObj.Projects.Where(x => x.ID == projectID).Select(s => s.Domain1.Domain1).FirstOrDefault();
                string sysType = dbObj.Projects.Where(x => x.ID == projectID).Select(s => s.ApplicationsType.Type).FirstOrDefault();

                var nfrDResult = dbObj.NFR_Rec_Domains.Where(x => x.Domain == domain).Select(s => s.Reccomended_NFRs).FirstOrDefault();
                var nfrATResult = dbObj.NFR_Rec_AppTypes.Where(x => x.AppType == sysType).Select(s => s.Reccomended_NFRs).FirstOrDefault();

                arm.status = 1;
                arm.successFlag = true;
                arm.successMsg = "";
                arm.returnedObject = nfrDResult;
                arm.extraObject = nfrATResult;
            }
            catch (Exception e)
            {
                arm.status = 0;
                arm.successFlag = false;
                arm.errorMsg = "Failed Getting NFR List!\nError: " + e.Message.ToString();
            }

            return new JavaScriptSerializer().Serialize(arm);
        }
    }
}