using System;
using MyEcommerce.Public.Products;

namespace MyEcommerce.Public.Web.Models
{
    public class CartItem
    {
        public ProductInListDto Product { get; set; } = new ProductInListDto();
        public int Quantity { get; set; }
    }
}