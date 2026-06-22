using System;
using System.Collections.Generic;
using System.Text;

namespace MyEcommerce.Public.Products
{
    public class ProductListFilterDto : BaseListFilterDto
    {
        public Guid? CategoryId { get; set; }
    }
}
