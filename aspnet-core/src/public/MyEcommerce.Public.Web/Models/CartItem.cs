using System;
using MyEcommerce.Public.Products;

namespace MyEcommerce.Public.Web.Models
{
    public class CartItem
    {
        public ProductDto Product { get; set; } = new ProductDto();
        public int Quantity { get; set; }
        public bool Selected { get; set; } = true;
    }
}