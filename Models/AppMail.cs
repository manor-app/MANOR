using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MANOFR.Models
{
    public class AppMail
    {
        private string fromEmail { get; set; }
        private string fromName { get; set; }
        public string toEmail { get; set; }
        public string toName { get; set; }
        private string password { get; set; }
        public string subject { get; set; }
        public string body { get; set; }
        public string host { get; set; }
        public int port { get; set; }
        public bool EnableSsl { get; set; }
        public bool UseDefaultCredentials { get; set; }

        public AppMail() {
            fromEmail = "manofr.app@gmail.com";
            fromName = "MANOFR APPLICATION";
            password = "manofr.2021";
            host = "smtp.gmail.com";
            port = 587;
            EnableSsl = true;
            UseDefaultCredentials = false;
        }
        public string GetFromAddress() {
            return fromEmail;
        }
        public string GetFromName()
        {
            return fromName;
        }
        public string GetPWD()
        {
            return password;
        }
    }
}