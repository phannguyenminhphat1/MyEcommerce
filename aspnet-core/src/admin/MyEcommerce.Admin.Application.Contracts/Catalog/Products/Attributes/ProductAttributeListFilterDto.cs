using System;
using System.Collections.Generic;
using System.Text;

namespace MyEcommerce.Admin.Products.Attributes
{
    public class ProductAttributeListFilterDto : BaseListFilterDto
    {
        public Guid ProductId { get; set; }
    }
}
