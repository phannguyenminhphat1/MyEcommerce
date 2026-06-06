using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MyEcommerce.IdentitySettings
{
    public class IdentitySettingConfiguration : IEntityTypeConfiguration<IdentitySetting>
    {
        public void Configure(EntityTypeBuilder<IdentitySetting> builder)
        {
            builder.ToTable(MyEcommerceConsts.DbTablePrefix + "IdentitySettings", MyEcommerceConsts.DbSchema);
            builder.HasKey(x => x.Id);
            builder.Property(e => e.Name).IsRequired().HasMaxLength(200);
            builder.Property(e => e.Prefix).IsRequired().HasMaxLength(50);
            builder.Property(e => e.CurrentNumber).IsRequired();
            builder.Property(e => e.StepNumber).IsRequired();
        }
    }
}