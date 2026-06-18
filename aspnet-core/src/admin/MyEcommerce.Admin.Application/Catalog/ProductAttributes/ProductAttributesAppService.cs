using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using MyEcommerce.Admin.Permissions;
using MyEcommerce.ProductAttributes;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace MyEcommerce.Admin.ProductAttributes
{
    [Authorize(MyEcommercePermissions.Attribute.Default, Policy = "AdminOnly")]
    public class ProductAttributesAppService : CrudAppService<
        ProductAttribute,
        ProductAttributeDto,
        Guid,
        PagedResultRequestDto,
        CreateUpdateProductAttributeDto,
        CreateUpdateProductAttributeDto>, IProductAttributesAppService
    {
        private readonly ProductAttributeCodeGenerator _productAttributeCodeGenerator;
        public ProductAttributesAppService(IRepository<ProductAttribute, Guid> repository, ProductAttributeCodeGenerator productAttributeCodeGenerator)
            : base(repository)
        {
            _productAttributeCodeGenerator = productAttributeCodeGenerator;
            GetPolicyName = MyEcommercePermissions.Attribute.Default;
            GetListPolicyName = MyEcommercePermissions.Attribute.Default;
            CreatePolicyName = MyEcommercePermissions.Attribute.Create;
            UpdatePolicyName = MyEcommercePermissions.Attribute.Update;
            DeletePolicyName = MyEcommercePermissions.Attribute.Delete;
        }

        [Authorize(MyEcommercePermissions.Attribute.Delete)]
        public async Task DeleteMultipleAsync(IEnumerable<Guid> ids)
        {
            await Repository.DeleteManyAsync(ids);
        }

        [Authorize(MyEcommercePermissions.Attribute.Default)]
        public async Task<List<ProductAttributeInListDto>> GetListAllAsync()
        {
            var query = await Repository.GetQueryableAsync();
            query = query.Where(x => x.IsActive == true);
            var data = await AsyncExecuter.ToListAsync(query);
            return ObjectMapper.Map<List<ProductAttribute>, List<ProductAttributeInListDto>>(data);
        }

        [Authorize(MyEcommercePermissions.Attribute.Default)]
        public async Task<PagedResultDto<ProductAttributeInListDto>> GetListFilterAsync(BaseListFilterDto input)
        {
            var query = await Repository.GetQueryableAsync();
            query = query.WhereIf(!string.IsNullOrWhiteSpace(input.Keyword), x => x.Label.Contains(input.Keyword!));
            var totalCount = await AsyncExecuter.LongCountAsync(query);
            var data = await AsyncExecuter.ToListAsync(query.Skip(input.SkipCount).Take(input.MaxResultCount));
            return new PagedResultDto<ProductAttributeInListDto>(totalCount, ObjectMapper.Map<List<ProductAttribute>, List<ProductAttributeInListDto>>(data));
        }

        [Authorize(MyEcommercePermissions.Attribute.Default)]
        public async Task<string> GetSuggestNewCodeAsync()
        {
            return await _productAttributeCodeGenerator.GenerateAsync();
        }
    }
}
