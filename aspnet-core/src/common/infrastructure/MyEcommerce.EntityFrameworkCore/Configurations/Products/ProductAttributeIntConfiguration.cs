using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MyEcommerce.Products
{
    public class ProductAttributeIntConfiguration : IEntityTypeConfiguration<ProductAttributeInt>
    {
        public void Configure(EntityTypeBuilder<ProductAttributeInt> builder)
        {
            builder.ToTable(MyEcommerceConsts.DbTablePrefix + "ProductAttributeInts");
            builder.HasKey(x => x.Id);
        }
    }
}
