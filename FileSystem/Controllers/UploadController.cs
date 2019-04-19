using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ionic.Zip;

namespace FileSystem.Controllers
{
    public class UploadController : Controller
    {
        //
        // GET: /FileUpload/Upload
        [Authorize]
        public ActionResult Upload()
        {
            return View();
        }

        //
        // POST: /FileUpload/Upload
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //Method 1 Get file details from current request    
                    //Uncomment following code if you wants to use method 1    
                    //if (Request.Files.Count > 0)    
                    // {    
                    //     var Inputfile = Request.Files[0];    

                    //     if (Inputfile != null && Inputfile.ContentLength > 0)    
                    //     {    
                    //         var filename = Path.GetFileName(Inputfile.FileName);    
                    //       var path = Path.Combine(Server.MapPath("~/uploadedfile/"), filename);    
                    //        Inputfile.SaveAs(path);    
                    //    }    
                    // }    

                    //Method 2 Get file details from HttpPostedFileBase class    

                    if (file != null)
                    {
                        
                        Console.WriteLine("File type -->", file.ContentType);
                        using (ZipFile zip = new ZipFile())
                        {
                            zip.Encryption = EncryptionAlgorithm.WinZipAes256;

                            zip.Password = "123456";


                            string path = Path.Combine(Server.MapPath("~/UploadedFiles"), "packed");
                            file.SaveAs(path);
                            zip.AddFiles(Directory.GetFiles(Server.MapPath("~/UploadedFiles/")), "packed");
                            zip.Save(Server.MapPath("~/Encoded.zip"));

                        }
                    }

                    ViewBag.FileStatus = "File uploaded successfully.";
                }
                catch (Exception)
                {

                    ViewBag.FileStatus = "Error while file uploading.";
                }

            }

            return View("Upload");
        }
    }
}