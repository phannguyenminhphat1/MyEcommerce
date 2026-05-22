using System.Collections.Generic;
using MyEcommerce.Admin.ProductCategories;
using MyEcommerce.ProductCategories;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace MyEcommerce.Admin;


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

