using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyEcommerce.Products;
using MyEcommerce.Repositories;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

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
        public ProductsAppService(IProductRepository productRepository)
            : base(productRepository)
        {
            _productRepository = productRepository;
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
    }
}