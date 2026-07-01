using System.Collections.Generic;
using MyEcommerce.Public.ProductCategories;
using MyEcommerce.Public.Products;

namespace MyEcommerce.Public.Web.Models
{
    public class HeaderCacheItem
    {
        public List<ProductCategoryInListDto>? Categories { set; get; }
        public List<CartItem>? CartItems { get; set; }
    }
}