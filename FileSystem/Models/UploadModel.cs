using System;
using System.ComponentModel.DataAnnotations;
using Ionic.Zip;
using Newtonsoft.Json;


namespace FileSystem.Models
{

    public class UploadModel
    {
        [DataType(DataType.Upload)]
        [Display(Name = "File")]
        [Required, FileExtensions(Extensions = ".pdf", ErrorMessage = "Incorrect file format")]
        public string file { get; set; }

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