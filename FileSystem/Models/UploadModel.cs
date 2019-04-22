using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ionic.Zip;
using Microsoft.Owin.Security.DataHandler;
using Newtonsoft.Json;


namespace FileSystem.Models
{

    public class UploadModel
    {
        [DataType(DataType.Upload)]
        [Display(Name = "File")]
        [Required, FileExtensions(Extensions = ".pdf", ErrorMessage = "Incorrect file format")]
        public HttpPostedFileBase file { get; set; }

        [Required(ErrorMessage = "Invalid Email Address", AllowEmptyStrings = false)]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Invalid Phone Number", AllowEmptyStrings = false)]
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

    }

}