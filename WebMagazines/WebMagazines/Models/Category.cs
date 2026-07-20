using System.ComponentModel.DataAnnotations;

namespace WebMagazines.Models
{
    public class Category
    {
        //[Key]
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        // Add validation for display order to be between 0 and 100
        [Range(0, 100, ErrorMessage ="Display order must be between 0 and 100!!!")] 
        public int? DisplayOrder { get; set; }
    }
}
