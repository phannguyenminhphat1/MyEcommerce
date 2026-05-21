using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MyEcommerce.Promotions
{
    public class PromotionManufacturerConfiguration : IEntityTypeConfiguration<PromotionManufacturer>
    {
        public void Configure(EntityTypeBuilder<PromotionManufacturer> builder)
        {
            builder.ToTable(MyEcommerceConsts.DbTablePrefix + "PromotionManufacturers");
            builder.HasKey(x => x.Id);
        }
    }
}
