using System.Collections.Generic;
using MyEcommerce.Admin.ProductCategories;

namespace MyEcommerce.Admin.Products
{
    public class HomeCacheItem
    {
        public List<ProductCategoryInListDto>? Categories { set; get; }
        public List<ProductInListDto>? TopSellerProducts { set; get; }
    }
}