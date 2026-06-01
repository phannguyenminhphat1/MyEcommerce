using System;
using System.Threading.Tasks;
using MyEcommerce.ProductCategories;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace MyEcommerce.Products
{
    public class ProductManager : DomainService
    {
        private readonly IRepository<Product, Guid> _productRepository;
        private readonly IRepository<ProductCategory, Guid> _productCategoryRepository;
        public ProductManager(IRepository<Product, Guid> productRepository, IRepository<ProductCategory, Guid> productCategoryRepository)
        {
            _productCategoryRepository = productCategoryRepository;
            _productRepository = productRepository;
        }
        public async Task<Product> CreateAsync(Guid manufacturerId,
            string name, string code, string slug,
            ProductType productType, string sku,
            int sortOrder, bool visibility,
            bool isActive, Guid categoryId,
            string seoMetaDescription, string description,
            string thumbnailPicture, double sellPrice)
        {
            if (await _productRepository.AnyAsync(x => x.Name == name))
                throw new UserFriendlyException("Product Name Already Exists", MyEcommerceDomainErrorCodes.ProductNameAlreadyExists);
            if (await _productRepository.AnyAsync(x => x.Code == code))
                throw new UserFriendlyException("Product Code Already Exists", MyEcommerceDomainErrorCodes.ProductCodeAlreadyExists);
            if (await _productRepository.AnyAsync(x => x.SKU == sku))
                throw new UserFriendlyException("Product SKU Already Exists", MyEcommerceDomainErrorCodes.ProductSKUAlreadyExists);

            await GetValidCategoryAsync(categoryId);

            return new Product(Guid.NewGuid(), manufacturerId, name, code, slug, productType, sku, sortOrder,
                visibility, isActive, categoryId, seoMetaDescription, description, thumbnailPicture, sellPrice);
        }

        private async Task<ProductCategory> GetValidCategoryAsync(Guid categoryId)
        {
            var category = await _productCategoryRepository.FirstOrDefaultAsync(x => x.Id == categoryId);

            if (category == null)
                throw new BusinessException(MyEcommerceDomainErrorCodes.ProductCategoryNotExists);

            if (!category.IsActive)
                throw new BusinessException(MyEcommerceDomainErrorCodes.ProductCategoryInactive);

            return category;
        }

        public async Task ChangeCategoryAsync(Product product, Guid categoryId)
        {
            if (product.CategoryId == categoryId)
                return;
            await GetValidCategoryAsync(categoryId);
            product.CategoryId = categoryId;
        }
    }
}