using System;
using System.Threading.Tasks;
using MyEcommerce.ProductCategories;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace MyEcommerce.Products
{
    public class ProductCategoryManager : DomainService
    {
        private readonly IRepository<ProductCategory, Guid> _productCategoryRepository;
        public ProductCategoryManager(IRepository<ProductCategory, Guid> productCategoryRepository)
        {
            _productCategoryRepository = productCategoryRepository;
        }
        public async Task<ProductCategory> CreateAsync(string name, string code, string slug, int sortOrder, string coverPicture, bool visibility, bool isActive, Guid? parentId, string seoMetaDescription)
        {
            if (await _productCategoryRepository.AnyAsync(x => x.Name == name))
                throw new BusinessException(MyEcommerceDomainErrorCodes.ProductCategoryNameAlreadyExists);
            if (await _productCategoryRepository.AnyAsync(x => x.Code == code))
                throw new BusinessException(MyEcommerceDomainErrorCodes.ProductCategoryCodeAlreadyExists);
            return new ProductCategory(Guid.NewGuid(), name, code, slug, sortOrder, coverPicture, visibility, isActive, parentId, seoMetaDescription);
        }

        public async Task ChangeNameAsync(ProductCategory category, string newName)
        {
            if (category.Name == newName)
                return;
            if (await _productCategoryRepository.AnyAsync(x => x.Name == newName && x.Id != category.Id))
            {
                throw new BusinessException(MyEcommerceDomainErrorCodes.ProductCategoryNameAlreadyExists);
            }
            category.Name = newName;
        }
    }
}