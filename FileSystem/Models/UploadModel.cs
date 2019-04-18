using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace FileSystem.Models
{
    public class UploadModel
    {
        [DataType(DataType.Upload)]
        [Display(Name = "File")]
        [Required, FileExtensions(Extensions = "application/pdf", ErrorMessage = "Incorrect file format")]
        public string file { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

    }
}