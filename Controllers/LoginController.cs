using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using MANOFR.Models;

namespace MANOFR.Controllers
{
    public class LoginController : Controller
    {
        //global vars
        AjaxReturnMSG arm;
        ManofrSqlDbModelDataContext dbObj;
        // GET: Login
        public ActionResult Index()
        {
            if (Request.Cookies["userFullname"] != null) {
                return RedirectToAction("Index","Home");
            } else
            {
                return View();
            }
        }

        [HttpPost]
        public int LogUserOut() {
            Response.Cookies["userFullname"].Expires = DateTime.Now.AddDays(-1);
            Response.Cookies["userID"].Expires = DateTime.Now.AddDays(-1);
            return 1;
        }

        [HttpPost]
        public string ResetUserPassword(string userEmail)
        {

            try
            {
                arm = new AjaxReturnMSG();
                dbObj = new ManofrSqlDbModelDataContext();

                User user = dbObj.Users.Where(u => u.Email == userEmail).SingleOrDefault();

                if (user != null)
                {
                    string generatedPwd = userEmail + DateTime.Now.ToString();
                    generatedPwd = generatedPwd.Replace(" ", "");
                    generatedPwd = GlobalMethods.ScrambleString(generatedPwd);

                    byte[] data = System.Text.Encoding.ASCII.GetBytes(generatedPwd);
                    data = new System.Security.Cryptography.SHA256Managed().ComputeHash(data);
                    string hashedPwd = System.Text.Encoding.ASCII.GetString(data);
                    user.Password = hashedPwd;
                    user.FirstTimeLogin = true;

                    dbObj.SubmitChanges();

                    AppMail mailObj = new AppMail();
                    mailObj.toEmail = user.Email;
                    mailObj.toName = user.Fullname;
                    mailObj.subject = "MANOFR Password Reset";
                    mailObj.body = "Please Find Your New Password Below:\nPassword: " + generatedPwd;

                    AjaxReturnMSG mailSent = GlobalMethods.SendEmail(mailObj);

                    if (!mailSent.successFlag)
                    {
                        arm.errorMsg = mailSent.errorMsg;
                    }
                    else {
                        arm.errorMsg = "Sent";
                    }

                    arm.status = 1;
                    arm.successFlag = true;
                    arm.successMsg = "Password Reset Done Successfully!\nAn Email Sent With The New Password.";
                }
                else {
                    arm.status = 0;
                    arm.successFlag = false;
                    arm.errorMsg = "This Email Doesn't Exist In The System!.";
                }
                
            }
            catch (Exception e)
            {
                arm.status = 0;
                arm.successFlag = false;
                arm.errorMsg = "Something Went Wrong While Resetting Password!\nError: " + e.Message.ToString();
            }

            return new JavaScriptSerializer().Serialize(arm);
        }

        [HttpPost]
        public string AuthenticateUser(string username, string pwd) {

            try {
                arm = new AjaxReturnMSG();
                dbObj = new ManofrSqlDbModelDataContext();

                byte[] data = System.Text.Encoding.ASCII.GetBytes(pwd);
                data = new System.Security.Cryptography.SHA256Managed().ComputeHash(data);
                string hashedPW = System.Text.Encoding.ASCII.GetString(data);

                var user = dbObj.Users.Where(u => u.Email == username && u.Password == pwd).Select(usr => new { usr.ID, usr.Email, usr.Fullname, usr.FirstTimeLogin }).SingleOrDefault();

                if (user != null)
                {
                    Response.Cookies["userFullname"].Value = user.Fullname;
                    Response.Cookies["userFullname"].Expires = DateTime.Now.AddDays(1); // add expiry time

                    Response.Cookies["userID"].Value = user.ID.ToString();
                    Response.Cookies["userID"].Expires = DateTime.Now.AddDays(1); // add expiry time

                    arm.status = 1;
                    arm.successFlag = true;
                    arm.successMsg = "";
                    arm.returnedObject = user;
                }
                else
                {
                    arm.status = 0;
                    arm.successFlag = false;
                    arm.errorMsg = "Wrong Email Or Password!";

                }
            }catch (Exception e) {
                arm.status = 0;
                arm.successFlag = false;
                arm.errorMsg = "Something Went Wrong While Authenticating!\nError: "+ e.Message.ToString();
            }

            return new JavaScriptSerializer().Serialize(arm);
        }

        [HttpPost]
        public string SubmitUserNewPassword(string userEmail, string userPWD)
        {
            try {
                arm = new AjaxReturnMSG();
                dbObj = new ManofrSqlDbModelDataContext();

                User user = dbObj.Users.Where(usr => usr.Email == userEmail).SingleOrDefault();

                byte[] data = System.Text.Encoding.ASCII.GetBytes(@userPWD);
                data = new System.Security.Cryptography.SHA256Managed().ComputeHash(data);
                string hashedPwd = System.Text.Encoding.ASCII.GetString(data);
                user.Password = hashedPwd;
                user.FirstTimeLogin = false;

                dbObj.SubmitChanges();

                arm.status = 1;
                arm.successFlag = true;
                arm.successMsg = "";

            } catch (Exception e) {
                arm.status = 0;
                arm.successFlag = false;
                arm.errorMsg = "Something Went Wrong While Changing Password!\nError: " + e.Message.ToString();
            }

            return new JavaScriptSerializer().Serialize(arm);
        }

        private void dummy() {

            //MailMessage mail = new MailMessage();

            //mail.To.Add("mg.mustafagamal@gmail.com");
            //mail.From = new MailAddress("manofr.app@gmail.com");
            //mail.Subject = "test";
            //mail.Body = "test body";
            //mail.IsBodyHtml = true;
            //SmtpClient smtp = new SmtpClient();
            ////smtp.Host = "172.29.0.132";
            //smtp.Host = "smtp.gmail.com";
            //smtp.EnableSsl = true;
            //smtp.Port = 587;
            //smtp.UseDefaultCredentials = false;
            //smtp.Credentials = new System.Net.NetworkCredential("manofr.app@gmail.com", "manofr.2021");
            //smtp.Send(mail);

            //byte[] data = System.Text.Encoding.ASCII.GetBytes(inputString);
            //data = new System.Security.Cryptography.SHA256Managed().ComputeHash(data);
            //String hash = System.Text.Encoding.ASCII.GetString(data);


            //try {
            //testdbEntities testcontext = new testdbEntities();
            //order o = new order
            //{
            //    OrderID = 3,
            //    OrderNumber = 3,
            //    PersonID = 1
            //};
            //testcontext.orders.Add(o);
            //testcontext.SaveChanges();

            //order ord = testcontext.orders.First(i => i.OrderID == 3);
            //{
            //    ord.OrderNumber = 102;
            //    testcontext.SaveChanges();
            //};

            //order ord = testcontext.orders.First(i => i.OrderID == 3);
            //{
            //    testcontext.orders.Remove(ord);
            //    testcontext.SaveChanges();
            //};

            //var load = (from g in testcontext.testtables
            //                join c in testcontext.orders on g.id equals c.PersonID
            //                select new { c.testtable.id, c.OrderID, c.OrderNumber, g.name }).OrderBy(x => x.OrderID).ToList();

            //    foreach (var l in load)
            //        //System.Diagnostics.Debug.WriteLine(l.id + " - " +l.name)
            //        System.Diagnostics.Debug.WriteLine(l.OrderID + " - " + l.OrderNumber + " - " + l.id + " - " + l.name);
            // } catch (Exception e) {
            //     System.Diagnostics.Debug.WriteLine(e.Message.ToString());
            // }
        }
    }
}