using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using MANOFR.Models;
using System.Diagnostics;

namespace MANOFR.Controllers
{
    public class UsersController : Controller
    {
        //global vars
        ManofrSqlDbModelDataContext dbObj;
        AjaxReturnMSG arm;
        // GET: Users
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
        public string CreateUser(User userObj, int project) {
            try {

                arm = new AjaxReturnMSG();
                dbObj = new ManofrSqlDbModelDataContext();

                List<User> newUserCheck = dbObj.Users.Where(x => x.Email == userObj.Email).ToList();
                if (newUserCheck.Count > 0) 
                {
                    arm.status = 0;
                    arm.successFlag = false;
                    arm.errorMsg = "This Email Already Exists!";
                } else
                {
                    User newUser = new User();
                    newUser.Email = userObj.Email;
                    newUser.Fullname = userObj.Fullname;
                    newUser.Title = userObj.Title;
                    newUser.FirstTimeLogin = true;

                    string generatedPwd = userObj.Email + DateTime.Now.ToString();
                    generatedPwd = generatedPwd.Replace(" ", "");
                    generatedPwd = GlobalMethods.ScrambleString(generatedPwd);

                    byte[] data = System.Text.Encoding.ASCII.GetBytes(generatedPwd);
                    data = new System.Security.Cryptography.SHA256Managed().ComputeHash(data);
                    string hashedPwd = System.Text.Encoding.ASCII.GetString(data);
                    newUser.Password = hashedPwd;

                    //Debug.WriteLine(generatedPwd);

                    dbObj.Users.InsertOnSubmit(newUser);
                    dbObj.SubmitChanges();

                    ProjectsUser projuser = new ProjectsUser();
                    projuser.Project = project;
                    projuser.SysUser = newUser.ID;

                    dbObj.ProjectsUsers.InsertOnSubmit(projuser);
                    dbObj.SubmitChanges();

                    AppMail mailObj = new AppMail();
                    mailObj.toEmail = newUser.Email;
                    mailObj.toName = newUser.Fullname;
                    mailObj.subject = "Your MANOFR Password";
                    mailObj.body = "Please Find Your New Default Below, Please Consider Changing It At The First Login Attempt:\nPassword: " + generatedPwd;

                    AjaxReturnMSG mailSent = GlobalMethods.SendEmail(mailObj);

                    if (!mailSent.successFlag)
                    {
                        arm.errorMsg = mailSent.errorMsg;
                    }

                    arm.status = 1;
                    arm.successFlag = true;
                    arm.successMsg = "User Was Added Successfully!";
                }

            } catch (Exception e) {
                arm.status = 0;
                arm.successFlag = false;
                arm.errorMsg = "User Wasn't Added Successfully!\nError: " + e.Message.ToString();
            }

            return new JavaScriptSerializer().Serialize(arm);
        }

        [HttpPost]
        public string GetUsersList(string email, string fName, int? title) {
            try
            {

                arm = new AjaxReturnMSG();
                dbObj = new ManofrSqlDbModelDataContext();

                var usersList = (from u in dbObj.Users
                                 where (email == "" ?  1==1 : u.Email.Contains(email)) &&
                                 (fName == "" ? 1 == 1 : u.Fullname.Contains(fName)) &&
                                 (title == null ? 1==1 : u.Title == title)
                                 select new { u.ID, u.Email, u.Fullname, Title = u.UsersTitle.Title, TitleID = u.Title }).ToList();

                arm.status = 1;
                arm.successFlag = true;
                arm.successMsg = "Users Was Generated Successfully!";
                arm.returnedObject = usersList;

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
        public string GetUserAssignedProjects(int userID)
        {
            try
            {

                arm = new AjaxReturnMSG();
                dbObj = new ManofrSqlDbModelDataContext();

                
                var projectsList = (from pu in dbObj.ProjectsUsers 
                                    where pu.SysUser == userID
                                    select new { pu.ID, pu.User.Email, pu.SysUser, ProjectName = pu.Project1.Name }).ToList();

                arm.status = 1;
                arm.successFlag = true;
                arm.successMsg = "User Projects List Was Generated Successfully!";
                arm.returnedObject = projectsList;

            }
            catch (Exception e)
            {
                arm.status = 0;
                arm.successFlag = false;
                arm.errorMsg = "Couldn't Get User Projects List!\nError: " + e.Message.ToString();
            }

            return new JavaScriptSerializer().Serialize(arm);
        }

        [HttpGet]
        public string GetUsersTitles()
        {
            try
            {

                arm = new AjaxReturnMSG();
                dbObj = new ManofrSqlDbModelDataContext();

                var usersTitles = (from ut in dbObj.UsersTitles
                                 select new { ut.ID, ut.Title }).ToList();

                arm.status = 1;
                arm.successFlag = true;
                arm.successMsg = "Users Titles Was Generated Successfully!";
                arm.returnedObject = usersTitles;

            }
            catch (Exception e)
            {
                arm.status = 0;
                arm.successFlag = false;
                arm.errorMsg = "Couldn't Get Users Titles List!\nError: " + e.Message.ToString();
            }

            return new JavaScriptSerializer().Serialize(arm);
        }

        [HttpGet]
        public string GetProjects()
        {
            try
            {

                arm = new AjaxReturnMSG();
                dbObj = new ManofrSqlDbModelDataContext();

                var usersTitles = (from pj in dbObj.Projects
                                   select new { pj.ID, pj.Name }).ToList();

                arm.status = 1;
                arm.successFlag = true;
                arm.successMsg = "Projects List Was Generated Successfully!";
                arm.returnedObject = usersTitles;

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
        public string UpdateUser(User updatedUser)
        {
            try
            {
                arm = new AjaxReturnMSG();
                dbObj = new ManofrSqlDbModelDataContext();

                User userToUpdate = dbObj.Users.Where(us => us.ID == updatedUser.ID).SingleOrDefault();
                List<User> usersWithSameEmail = dbObj.Users.Where(us => us.Email == updatedUser.Email && us.ID != userToUpdate.ID).ToList();

                if (userToUpdate != null && usersWithSameEmail.Count <= 0)
                {

                    userToUpdate.Email = updatedUser.Email;
                    userToUpdate.Fullname = updatedUser.Fullname;
                    userToUpdate.Title = updatedUser.Title;

                    dbObj.SubmitChanges();

                    arm.status = 1;
                    arm.successFlag = true;
                    arm.successMsg = "User Updated Successfully!";

                } else if (userToUpdate != null && usersWithSameEmail.Count > 0) {
                    arm.status = 0;
                    arm.successFlag = false;
                    arm.errorMsg = "Another User Is Using This Email!";
                }
                else if(userToUpdate == null)
                {

                    arm.status = 0;
                    arm.successFlag = false;
                    arm.errorMsg = "User Not Found In The Database!";

                }

            }
            catch (Exception e)
            {
                arm.status = 0;
                arm.successFlag = false;
                arm.errorMsg = "Failed Updating User!\nError: " + e.Message.ToString();
            }

            return new JavaScriptSerializer().Serialize(arm);
        }

        [HttpPost]
        public string DeleteUser(int userID)
        {
            try
            {
                arm = new AjaxReturnMSG();
                dbObj = new ManofrSqlDbModelDataContext();

                User userToDelete = dbObj.Users.Where(us => us.ID == userID).SingleOrDefault();

                if (userToDelete != null)
                {
                    dbObj.Users.DeleteOnSubmit(userToDelete);
                    dbObj.SubmitChanges();

                    arm.status = 1;
                    arm.successFlag = true;
                    arm.successMsg = "User Deleted Successfully!";
                }
                else
                {

                    arm.status = 0;
                    arm.successFlag = false;
                    arm.errorMsg = "User Not Found In The Database!";

                }

            }
            catch (Exception e)
            {
                arm.status = 0;
                arm.successFlag = false;
                if (e.Message.Contains("conflicted with the REFERENCE constraint"))
                {
                    arm.errorMsg = "Failed Deleting User!\nError: It's being used by another item.";
                }
                else
                {
                    arm.errorMsg = "Failed Deleting User!\nError: " + e.Message.ToString();
                }
            }

            return new JavaScriptSerializer().Serialize(arm);
        }
    }
}