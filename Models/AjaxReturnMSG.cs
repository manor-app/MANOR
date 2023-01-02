using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MANOFR.Models
{
    public class AjaxReturnMSG
    {
        public int status { get; set; }
        public bool successFlag { get; set; }
        public string successMsg { get; set; }
        public string errorMsg { get; set; }
        public Object returnedObject { get; set; }
        public Object extraObject { get; set; }
    }
}