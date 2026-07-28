using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WebMagazines.Models.ViewModels
{
    public class RegisterVM
    {
        // Properties for user registration
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }


        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;
        [DisplayName("Street Address")]
        public string? StreetAddress { get; set; }
        [DisplayName("City")]
        public string? City { get; set; }
        [DisplayName("Country")]
        public string? State { get; set; }
        [DisplayName("Post Code")]
        public string? PostCode { get; set; }
        [DisplayName("Phone Number")]
        public string? PhoneNumber { get; set; }

        // Role property to hold the selected role for the user
        public string? Role { get; set; }

        [DisplayName("Role")]
        [ValidateNever]
        public IEnumerable<SelectListItem> RoleList { get; set; }
    }
}
