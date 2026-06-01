using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyEcommerce.Products;
using MyEcommerce.Repositories;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
namespace MyEcommerce.Admin.Products
{
    public class ProductsAppService : CrudAppService<
        Product,
        ProductDto,
        Guid,
        PagedResultRequestDto,
        CreateUpdateProductDto,
        CreateUpdateProductDto>, IProductsAppService
    {
        private readonly IProductRepository _productRepository;
        private readonly ProductManager _productManager;
        public ProductsAppService(IProductRepository productRepository, ProductManager productManager)
            : base(productRepository)
        {
            _productRepository = productRepository;
            _productManager = productManager;
        }

        public async Task DeleteMultipleAsync(IEnumerable<Guid> ids)
        {
            await _productRepository.DeleteManyAsync(ids);
        }

        public async Task<List<ProductInListDto>> GetListAllAsync()
        {
            var query = await _productRepository.GetQueryableAsync();
            query = query.Where(x => x.IsActive == true);
            var data = await AsyncExecuter.ToListAsync(query);
            return ObjectMapper.Map<List<Product>, List<ProductInListDto>>(data);
        }

        public async Task<PagedResultDto<ProductInListDto>> GetListFilterAsync(ProductListFilterDto input)
        {
            var totalCount = await _productRepository.GetCountAsync(input.Keyword, input.CategoryId);
            var data = await _productRepository.GetListFilterAsync(
                input.Keyword,
                input.CategoryId,
                input.SkipCount,
                input.MaxResultCount);
            return new PagedResultDto<ProductInListDto>(totalCount, ObjectMapper.Map<List<Product>, List<ProductInListDto>>(data));
        }

        public override async Task<ProductDto> CreateAsync(CreateUpdateProductDto input)
        {
            var product = await _productManager.CreateAsync(input.ManufacturerId, input.Name, input.Code, input.Slug, input.ProductType, input.SKU, input.SortOrder, input.Visibility, input.IsActive, input.CategoryId, input.SeoMetaDescription, input.Description, input.ThumbnailPicture, input.SellPrice);
            await _productRepository.InsertAsync(product);
            return ObjectMapper.Map<Product, ProductDto>(product);
        }

        public override async Task<ProductDto> UpdateAsync(Guid id, CreateUpdateProductDto input)
        {
            var product = await Repository.GetAsync(id);
            if (product == null)
                throw new BusinessException(MyEcommerceDomainErrorCodes.ProductIsNotExists);
            product.ManufacturerId = input.ManufacturerId;
            product.Name = input.Name;
            product.Code = input.Code;
            product.Slug = input.Slug;
            product.ProductType = input.ProductType;
            product.SKU = input.SKU;
            product.SortOrder = input.SortOrder;
            product.Visibility = input.Visibility;
            product.IsActive = input.IsActive;
            await _productManager.ChangeCategoryAsync(product, input.CategoryId);
            product.SeoMetaDescription = input.SeoMetaDescription;
            product.Description = input.Description;
            product.ThumbnailPicture = input.ThumbnailPicture;
            product.SellPrice = input.SellPrice;
            await Repository.UpdateAsync(product);
            return ObjectMapper.Map<Product, ProductDto>(product);
        }
    }
}