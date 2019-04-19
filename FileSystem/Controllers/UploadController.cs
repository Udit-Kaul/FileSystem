using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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
                  string pass = "";
                    if (file != null)
                    {
                       
                        
                        Console.WriteLine("File type -->", file.ContentType);
                        using (ZipFile zip = new ZipFile())
                        {
                            //Adds AES 256 encryption to the file
                            zip.Encryption = EncryptionAlgorithm.WinZipAes256;
                            //  zip.Password = "123456";

                            //used for creating a password for the zipped file with RNGCryptoServiceProvider library
                            using (RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider())
                            {
                                //sets the size of the password and converts the base64 number to string to be used as password
                                int max_length = 10;
                                byte[] salt = new byte[max_length];
                                rngCsp.GetNonZeroBytes(salt);
                                String password = Convert.ToBase64String(salt);
                                Console.Write("Password-->", password);
                                pass = zip.Password = password;


                                string path = Path.Combine(Server.MapPath("~/UploadedFiles"), "packed");
                                file.SaveAs(path);
                                zip.AddFiles(Directory.GetFiles(Server.MapPath("~/UploadedFiles/")), "packed");
                             //write logic to add timestamp

                                 //saves file to /ZippedFIles folder in the root director with the origial filename in zipped format
                                zip.Save(Server.MapPath("~/ZippedFiles/"+file.FileName+".zip"));
                            }

                        }
                    }

                    ViewBag.FileStatus = "File uploaded successfully. -->" + pass+"-->"+file.ContentType;
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