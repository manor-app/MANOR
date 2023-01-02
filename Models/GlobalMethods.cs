using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MANOFR.Models;
using System.Net;
using System.Net.Mail;
using System.Web.Script.Serialization;
using System.Text;
using NPOI.SS.Formula.Functions;

namespace MANOFR.Models
{
    public class GlobalMethods
    {
        public static AjaxReturnMSG SendEmail(AppMail mailObj) {
            AjaxReturnMSG arm = new AjaxReturnMSG();

            try {
                var fromAddressObj = new MailAddress(mailObj.GetFromAddress(), mailObj.GetFromName());
                var toAddressObj = new MailAddress(mailObj.toEmail, mailObj.toName);

                var smtp = new SmtpClient
                {
                    Host = mailObj.host,
                    Port = mailObj.port,
                    EnableSsl = mailObj.EnableSsl,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = mailObj.UseDefaultCredentials,
                    Credentials = new NetworkCredential(fromAddressObj.Address, mailObj.GetPWD())
                };
                using (var message = new MailMessage(fromAddressObj, toAddressObj)
                {
                    Subject = mailObj.subject,
                    Body = mailObj.body
                })
                {
                    smtp.Send(message);
                }

                arm.status = 1;
                arm.successFlag = true;
                arm.successMsg = "Mail Sent Successfully!";
                arm.returnedObject = mailObj.subject;

            }
            catch (Exception e)
            {
                arm.status = 0;
                arm.successFlag = false;
                arm.errorMsg = "Somehing Went Wrong!\nError: " + e.Message.ToString();
            }

            return arm;
        }

        public static string ScrambleString(string text) {
            char[] chars = new char[text.Length];
            Random rand = new Random(10000);
            int index = 0;
            while (text.Length > 0)
            { // Get a random number between 0 and the length of the word. 
                int next = rand.Next(0, text.Length - 1); // Take the character from the random position 
                                                          //and add to our char array. 
                chars[index] = text[next];                // Remove the character from the word. 
                text = text.Substring(0, next) + text.Substring(next + 1);
                ++index;
            }
            return new String(chars);
        }
    }
}