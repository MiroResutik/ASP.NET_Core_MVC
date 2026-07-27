using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace WebMagazines.Models
{
    public class Category
    {
        //[Key]
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        [Display(Name = "Category Name")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Display Order")]
        //[ValidateNever]
        [Required]
        // Add validation for display order to be between 0 and 100
        [Range(0, 100, ErrorMessage ="Display order must be between 0 and 100!!!")] 
        public int? DisplayOrder { get; set; }
    }
}
