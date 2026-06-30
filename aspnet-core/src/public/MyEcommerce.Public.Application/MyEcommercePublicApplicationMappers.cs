using MyEcommerce.Manufacturers;
using MyEcommerce.Orders;
using MyEcommerce.ProductAttributes;
using MyEcommerce.ProductCategories;
using MyEcommerce.Products;
using MyEcommerce.Public.Manufacturers;
using MyEcommerce.Public.Orders;
using MyEcommerce.Public.ProductAttributes;
using MyEcommerce.Public.ProductCategories;
using MyEcommerce.Public.Products;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace MyEcommerce.Public;

[Mapper]
public partial class MyEcommercePublicApplicationMappers
{
    /* You can configure your Mapperly mapping configuration here.
     * Alternatively, you can split your mapping configurations
     * into multiple mapper classes for a better organization. */
    #region PRODUCT CATEGORY
    [Mapper]
    public partial class ProductCategoryToProductCategoryDtoMapper : MapperBase<ProductCategory, ProductCategoryDto>
    {
        public override partial ProductCategoryDto Map(ProductCategory source);
        public override partial void Map(ProductCategory source, ProductCategoryDto destination);
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
    public partial class ProductAttributeToProductAttributeInListDtoMapper : MapperBase<ProductAttribute, ProductAttributeInListDto>
    {
        public override partial ProductAttributeInListDto Map(ProductAttribute source);

        public override partial void Map(
            ProductAttribute source,
            ProductAttributeInListDto destination
        );
    }
    #endregion

    #region ORDER
    [Mapper]
    public partial class OrderToOrderDtoMapper : MapperBase<Order, OrderDto>
    {
        public override partial OrderDto Map(Order source);
        public override partial void Map(Order source, OrderDto destination);
    }
    #endregion
}
