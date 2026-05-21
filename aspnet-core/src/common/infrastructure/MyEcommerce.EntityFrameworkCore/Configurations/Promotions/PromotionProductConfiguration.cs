using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MyEcommerce.Promotions
{
    public class PromotionProductConfiguration : IEntityTypeConfiguration<PromotionProduct>
    {
        public void Configure(EntityTypeBuilder<PromotionProduct> builder)
        {
            builder.ToTable(MyEcommerceConsts.DbTablePrefix + "PromotionProducts");
            builder.HasKey(x => x.Id);
        }
    }
}
