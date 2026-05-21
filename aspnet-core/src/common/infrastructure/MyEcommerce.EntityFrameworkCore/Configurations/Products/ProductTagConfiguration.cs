using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MyEcommerce.Products
{
    public class ProductTagConfiguration : IEntityTypeConfiguration<ProductTag>
    {
        public void Configure(EntityTypeBuilder<ProductTag> builder)
        {
            builder.ToTable(MyEcommerceConsts.DbTablePrefix + "ProductTags");
            builder.HasKey(x => new { x.ProductId, x.TagId });
        }
    }
}
