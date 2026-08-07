using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebMagazines.Models.ViewModels
{
    public class ProductVM
    {
        // This property represents the product being created or edited
        public Product Product { get; set; }

        // This property holds a list of categories for the product, used for dropdown selection in the view
        [ValidateNever]
        public IEnumerable<SelectListItem> CategoryList { get; set; }
    }
}
