using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using SendGrid.Helpers.Mail;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using Attachment = System.Net.Mail.Attachment;
using Ionic.Zip;


namespace FileSystem.Controllers
{
    public class UploadController : Controller
    {
        //
        // GET: /FileUpload/Upload
        [HttpGet]
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
        public ActionResult Upload(HttpPostedFileBase file, String email, String phoneNumber)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    
                    string password = "";
                    if (file != null)
                    {
                       

                        Console.WriteLine("File type -->", file.ContentType);
                        using (ZipFile zip = new ZipFile())
                        {
                            //Adds AES 256 encryption to the file
                            zip.Encryption = EncryptionAlgorithm.WinZipAes256;
                            

                            //used for creating a password for the zipped file with RNGCryptoServiceProvider library
                            using (RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider())
                            {
                                //sets the size of the password and converts the base64 number to string to be used as password
                                int max_length = 10;
                                byte[] salt = new byte[max_length];
                                rngCsp.GetNonZeroBytes(salt);
                                password = Convert.ToBase64String(salt);
                                Console.Write("Password-->", password);
                                zip.Password = password;


                                string path = Path.Combine(Server.MapPath("~/UploadedFiles"), "packed");
                                file.SaveAs(path);
                                zip.AddFiles(Directory.GetFiles(Server.MapPath("~/UploadedFiles/")), "packed");
                                //write logic to add timestamp

                                //saves file to /ZippedFIles folder in the root director with the origial filename in zipped format
                                zip.Save(Server.MapPath("~/ZippedFiles/" + file.FileName + ".zip"));

                            }

                        }


                        //Send Email Start
                        //Sets the attributes of the MailMessage namely the from email address, to email address and the Subject of the email
                        MailMessage msg = new MailMessage();
                        msg.From = new MailAddress("Adminstration@FileSystem.com");
                        msg.To.Add(new MailAddress(email));
                        msg.Subject = "Please find the attached document";


                        //To attach the zipped file to the mail that is being sent
                        Attachment data = new Attachment(Server.MapPath("~/ZippedFiles/" + file.FileName + ".zip"));


                        //To add additionaly details to the zipped file namely file creation time, last time the file was written and the last time the file was accessed
                        //This is another security feature to determine if someone has tampered with the file in mid-air
                        ContentDisposition disposition = data.ContentDisposition;
                        disposition.CreationDate = System.IO.File.GetCreationTime(file.ToString());
                        disposition.ModificationDate = System.IO.File.GetLastWriteTime(file.ToString());
                        disposition.ReadDate = System.IO.File.GetLastAccessTime(file.ToString());
                        msg.Attachments.Add(data);

                        //Setting SendGrid as the email client
                        SmtpClient smtpClient = new SmtpClient("smtp.sendgrid.net", Convert.ToInt32(587));

                        //Getting the details of the SendGrid account set in the Web.config file, it a bad programming practice and also a security vulnerability to expose these details in the code
                        var sendGridUserName = ConfigurationManager.AppSettings["SendGridUserName"];
                        var sentFrom = ConfigurationManager.AppSettings["SendGridFromEmail"];
                        var sendGridPassword = ConfigurationManager.AppSettings["SendGridPassword"];
                        System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(sendGridUserName, sendGridPassword);
                        smtpClient.Credentials = credentials;
                        //Setting the SMTP client to use SSL
                        smtpClient.EnableSsl = true;
                        try
                        {
                            //Sends the email
                            smtpClient.Send(msg);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Exception caught in CreateMessageWithAttachment(): {0}",
                                ex.ToString());
                        }
                        //Send Email End

                        // Twilio Begin
                        var accountSid = ConfigurationManager.AppSettings["SMSAccountIdentification"];
                        var authToken = ConfigurationManager.AppSettings["SMSAccountPassword"];
                        var fromNumber = ConfigurationManager.AppSettings["SMSAccountFrom"];


                        TwilioClient.Init(accountSid, authToken);

                        MessageResource result = MessageResource.Create(
                            new PhoneNumber(phoneNumber),
                            from: new PhoneNumber(fromNumber),
                            body: "The password for the zip sent to " + email + " \n" + password
                        );

                        //Twillio End
                    }

                    ViewBag.FileStatus = "File uploaded successfully. -->" + password;
                }
                catch (Exception)
                {

                    ViewBag.FileStatus = "Error while file uploading.--->";
                }

            }

            return View("Upload");
        }
    }
}