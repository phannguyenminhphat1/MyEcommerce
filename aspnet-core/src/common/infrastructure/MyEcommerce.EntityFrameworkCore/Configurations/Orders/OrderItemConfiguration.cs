using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MyEcommerce.Orders
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable(MyEcommerceConsts.DbTablePrefix + "OrderItems");
            builder.HasKey(x => new { x.ProductId, x.OrderId });
            builder.Property(x => x.SKU)
                 .HasMaxLength(50)
                 .IsUnicode(false)
                 .IsRequired();
        }
    }
}
