using System.Collections.Generic;
using MyEcommerce.Public.Manufacturers;
using MyEcommerce.Public.ProductCategories;
using MyEcommerce.Public.Products;

namespace MyEcommerce.Public.Web.Models
{
    public class HomeCacheItem
    {
        public List<ProductCategoryInListDto>? Categories { set; get; }
        public List<ProductInListDto>? TopSellerProducts { set; get; }
        public List<ManufacturerInListDto>? Manufacturers { set; get; }
    }
}