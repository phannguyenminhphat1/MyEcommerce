using System.Collections.Generic;
using MyEcommerce.Admin.Manufacturers;
using MyEcommerce.Admin.ProductAttributes;
using MyEcommerce.Admin.ProductCategories;
using MyEcommerce.Admin.Products;
using MyEcommerce.Admin.Roles;
using MyEcommerce.Manufacturers;
using MyEcommerce.ProductAttributes;
using MyEcommerce.ProductCategories;
using MyEcommerce.Products;
using MyEcommerce.Roles;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Identity;
using Volo.Abp.Mapperly;

namespace MyEcommerce.Admin;

#region PRODUCT CATEGORY
[Mapper]
public partial class ProductCategoryToProductCategoryDtoMapper : MapperBase<ProductCategory, ProductCategoryDto>
{
    public override partial ProductCategoryDto Map(ProductCategory source);
    public override partial void Map(ProductCategory source, ProductCategoryDto destination);
}

[Mapper]
public partial class CreateUpdateProductCategoryDtoToProductCategoryMapper : MapperBase<CreateUpdateProductCategoryDto, ProductCategory>
{
    public override partial ProductCategory Map(CreateUpdateProductCategoryDto source);
    public override partial void Map(CreateUpdateProductCategoryDto source, ProductCategory destination);
}

[Mapper]
public partial class ProductCategoryToProductCategoryInListDto : MapperBase<ProductCategory, ProductCategoryInListDto>
{
    public override partial ProductCategoryInListDto Map(ProductCategory source);

    public override partial void Map(ProductCategory source, ProductCategoryInListDto destination);
}
#endregion


#region PRODUCT
[Mapper]
public partial class ProductToProductDtoMapper : MapperBase<Product, ProductDto>
{
    public override partial ProductDto Map(Product source);
    public override partial void Map(Product source, ProductDto destination);
}

// [Mapper]
// public partial class CreateUpdateProductDtoToProductMapper : MapperBase<CreateUpdateProductDto, Product>
// {
//     public override partial Product Map(CreateUpdateProductDto source);
//     public override partial void Map(CreateUpdateProductDto source, Product destination);
// }

[Mapper]
public partial class ProductToProductInListDtoMapper : MapperBase<Product, ProductInListDto>
{
    [MapProperty("Category.Name", "CategoryName")]
    [MapProperty("Category.Slug", "CategorySlug")]
    public override partial ProductInListDto Map(Product source);

    public override partial void Map(
        Product source,
        ProductInListDto destination
    );
}
#endregion

#region MANUFACTURER
[Mapper]
public partial class ManufacturerToManufacturerDtoMapper : MapperBase<Manufacturer, ManufacturerDto>
{
    public override partial ManufacturerDto Map(Manufacturer source);
    public override partial void Map(Manufacturer source, ManufacturerDto destination);
}

[Mapper]
public partial class CreateUpdateManufacturerDtoToManufacturerMapper : MapperBase<CreateUpdateManufacturerDto, Manufacturer>
{
    public override partial Manufacturer Map(CreateUpdateManufacturerDto source);
    public override partial void Map(CreateUpdateManufacturerDto source, Manufacturer destination);
}

[Mapper]
public partial class ManufacturerToManufacturerInListDtoMapper : MapperBase<Manufacturer, ManufacturerInListDto>
{
    public override partial ManufacturerInListDto Map(Manufacturer source);

    public override partial void Map(
        Manufacturer source,
        ManufacturerInListDto destination
    );
}
#endregion

#region PRODUCT ATTRIBUTE
[Mapper]
public partial class ProductAttributeToProductAttributeDtoMapper : MapperBase<ProductAttribute, ProductAttributeDto>
{
    public override partial ProductAttributeDto Map(ProductAttribute source);
    public override partial void Map(ProductAttribute source, ProductAttributeDto destination);
}

[Mapper]
public partial class CreateUpdateProductAttributeDtoToProductAttributeMapper : MapperBase<CreateUpdateProductAttributeDto, ProductAttribute>
{
    public override partial ProductAttribute Map(CreateUpdateProductAttributeDto source);
    public override partial void Map(CreateUpdateProductAttributeDto source, ProductAttribute destination);
}

[Mapper]
public partial class ProductAttributeToProductAttributeInListDtoMapper : MapperBase<ProductAttribute, ProductAttributeInListDto>
{
    public override partial ProductAttributeInListDto Map(ProductAttribute source);

    public override partial void Map(
        ProductAttribute source,
        ProductAttributeInListDto destination
    );
}

#endregion

#region ROLE
[Mapper]
public partial class RoleToRoleDtoMapper : MapperBase<IdentityRole, RoleDto>
{
    [MapProperty(nameof(IdentityRole.ExtraProperties), nameof(RoleDto.Description), Use = nameof(MapDescription))]
    public override partial RoleDto Map(IdentityRole source);

    public override partial void Map(IdentityRole source, RoleDto destination);

    private static string? MapDescription(Dictionary<string, object?> extraProperties)
    {
        return extraProperties.TryGetValue(RoleConsts.DescriptionFieldName, out var value)
            ? value?.ToString()
            : null;
    }
}

[Mapper]
public partial class RoleToRoleInListDtoMapper : MapperBase<IdentityRole, RoleInListDto>
{
    [MapProperty(nameof(IdentityRole.ExtraProperties), nameof(RoleDto.Description), Use = nameof(MapDescription))]
    public override partial RoleInListDto Map(IdentityRole source);

    public override partial void Map(IdentityRole source, RoleInListDto destination);

    private static string? MapDescription(Dictionary<string, object?> extraProperties)
    {
        return extraProperties.TryGetValue(RoleConsts.DescriptionFieldName, out var value)
            ? value?.ToString()
            : null;
    }
}
#endregion


