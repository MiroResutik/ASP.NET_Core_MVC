using System;
using System.Collections.Generic;
using System.Text;

namespace WebMagazines.Models.ViewModels
{
    public class ShoppingCartVM
    {
        // ShoppingCartList property to hold a collection of ShoppingCart objects
        public IEnumerable<ShoppingCart> ShoppingCartList { get; set; }

        // OrderHeader property to hold an instance of OrderHeader
        public OrderHeader OrderHeader { get; set; }
    }
}
