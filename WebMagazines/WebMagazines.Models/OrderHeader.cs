using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WebMagazines.Models
{
    // OrderHeader class represents the header information of an order
    public class OrderHeader
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }

        public DateTime OrderDate { get; set; }
        public DateTime ShippingDate { get; set; }

        public double OrderTotal { get; set; }

        public string? OrderStatus { get; set; }

        public string? TrackingNumber { get; set; }
        public string? Carrier { get; set; }


        public string? SessionId { get; set; }
        public string? PaymentIntentId { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string StreetAddress { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public string PostalCode { get; set; }
        [Required]
        public string Name { get; set; }

        // Navigation property for the related OrderDetails
        [ValidateNever]
        public IEnumerable<OrderDetails> OrderDetails { get; set; }
    }
}
