using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WebMagazines.Models
{
    // ShoppingCart class represents an item in the shopping cart
    public class ShoppingCart
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        [ValidateNever]
        public Product? Product { get; set; }

        [Range(1, 1000, ErrorMessage = "Please enter a value between 1 and 1000")]
        public int Count { get; set; }

        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }

        // Price property calculates the price based on the count of products in the cart
        [NotMapped]
        public double Price
        {
            get
            {
                if (Product == null) return 0;

                if (Count <= 50)
                {
                    return Product.Price;
                }
                else if (Count <= 100)
                {
                    return Product.Price50;
                }
                else
                {
                    return Product.Price100;
                }
            }

        }
    }
}
