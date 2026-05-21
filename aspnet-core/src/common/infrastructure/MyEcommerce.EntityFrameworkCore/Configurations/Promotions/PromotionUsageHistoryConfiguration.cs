using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyEcommerce.Promotions;

namespace MyEcommerce.Promotions
{
    public class PromotionUsageHistoryConfiguration : IEntityTypeConfiguration<PromotionUsageHistory>
    {
        public void Configure(EntityTypeBuilder<PromotionUsageHistory> builder)
        {
            builder.ToTable(MyEcommerceConsts.DbTablePrefix + "PromotionUsageHistories");
            builder.HasKey(x => x.Id);
        }
    }
}
