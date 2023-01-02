using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO.Compression;
using System.Net.NetworkInformation;
using System.Net;
using System.Management;

namespace MANOFR.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
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
        public ActionResult UploadFiles()
        {
            // Checking no of files injected in Request object  
            if (Request.Files.Count > 0)
            {
                try
                {
                    //  Get all files from Request object  
                    HttpFileCollectionBase files = Request.Files;
                    string filePath = Request.Form["FilePath"];
                    //Debug.WriteLine(filePath);

                    //create if path doesnt exist
                    if (!Directory.Exists(Server.MapPath("~" + filePath)))
                        Directory.CreateDirectory(Server.MapPath("~" + filePath));
                    else
                    {
                        DirectoryInfo di = new DirectoryInfo(Server.MapPath("~" + filePath));
                        foreach (FileInfo fileToDel in di.GetFiles())
                        {
                            fileToDel.Delete();
                        }
                        foreach (DirectoryInfo dir in di.GetDirectories())
                        {
                            dir.Delete(true);

                        }
                    }

                    using (FileStream zipFile = System.IO.File.Open(Server.MapPath("~" + filePath) + "Files-Archive.zip", FileMode.Create))
                    {
                        using (ZipArchive archive = new ZipArchive(zipFile, ZipArchiveMode.Update))
                        {
                            for (int i = 0; i < files.Count; i++)
                            {
                                //string path = AppDomain.CurrentDomain.BaseDirectory + "Uploads/";  
                                //string filename = Path.GetFileName(Request.Files[i].FileName);  

                                HttpPostedFileBase file = files[i];
                                string fname;

                                // Checking for Internet Explorer  
                                if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                                {
                                    string[] testfiles = file.FileName.Split(new char[] { '\\' });
                                    fname = testfiles[testfiles.Length - 1];
                                }
                                else
                                {
                                    fname = file.FileName;
                                }

                                // Get the complete folder path and store the file inside it.  
                                fname = Path.Combine(Server.MapPath("~" + filePath), fname);
                                file.SaveAs(fname);

                                // Add files to the archive
                                archive.CreateEntryFromFile(fname, file.FileName);
                                
                            }
                        }

                    }

                    DirectoryInfo di2 = new DirectoryInfo(Server.MapPath("~" + filePath));
                    foreach (FileInfo fileToDel in di2.GetFiles())
                    {
                        if(fileToDel.Name != "Files-Archive.zip")
                            fileToDel.Delete();
                    }
                    // Returns message that successfully uploaded  
                    return Json("File Uploaded Successfully!");
                }
                catch (Exception ex)
                {
                    return Json("Error occurred. Error details: " + ex.Message);
                }
            }
            else
            {
                return Json("No files selected.");
            }
        }
    }
}