using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MyEcommerce.Products
{
    public class TagConfiguration : IEntityTypeConfiguration<Tag>
    {
        public void Configure(EntityTypeBuilder<Tag> builder)
        {
            builder.ToTable(MyEcommerceConsts.DbTablePrefix + "Tags");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
             .HasMaxLength(50)
             .IsRequired();
            builder.Property(x => x.Name)
               .HasMaxLength(50)
               .IsRequired();

        }
    }
}
