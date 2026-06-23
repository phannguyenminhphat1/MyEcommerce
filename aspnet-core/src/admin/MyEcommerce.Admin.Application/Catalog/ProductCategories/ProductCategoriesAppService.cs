using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using MyEcommerce.Admin.Permissions;
using MyEcommerce.ProductCategories;
using MyEcommerce.Products;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.BlobStoring;
using Volo.Abp.Domain.Repositories;

namespace MyEcommerce.Admin.ProductCategories
{
    [Authorize(MyEcommercePermissions.ProductCategory.Default, Policy = "AdminOnly")]
    public class ProductCategoriesAppService : CrudAppService<
        ProductCategory,
        ProductCategoryDto,
        Guid,
        PagedResultRequestDto,
        CreateUpdateProductCategoryDto,
        CreateUpdateProductCategoryDto>, IProductCategoriesAppService
    {
        private readonly ProductCategoryCodeGenerator _productCategoryCodeGenerator;
        private readonly IBlobContainer<ProductCategoryCoverPictureContainer> _blobContainer;
        private readonly ProductCategoryManager _productCategoryManager;
        private readonly IRepository<ProductCategory, Guid> _productCategoryRepository;
        public ProductCategoriesAppService(
            IRepository<ProductCategory, Guid> repository,
            ProductCategoryCodeGenerator productCategoryCodeGenerator,
            IBlobContainer<ProductCategoryCoverPictureContainer> blobContainer,
            ProductCategoryManager productCategoryManager,
            IRepository<ProductCategory, Guid> productCategoryRepository)
            : base(repository)
        {
            GetPolicyName = MyEcommercePermissions.ProductCategory.Default;
            GetListPolicyName = MyEcommercePermissions.ProductCategory.Default;
            CreatePolicyName = MyEcommercePermissions.ProductCategory.Create;
            UpdatePolicyName = MyEcommercePermissions.ProductCategory.Update;
            DeletePolicyName = MyEcommercePermissions.ProductCategory.Delete;
            _productCategoryCodeGenerator = productCategoryCodeGenerator;
            _blobContainer = blobContainer;
            _productCategoryManager = productCategoryManager;
            _productCategoryRepository = productCategoryRepository;

        }

        [Authorize(MyEcommercePermissions.ProductCategory.Default)]
        public async Task<PagedResultDto<ProductCategoryInListDto>> GetListFilterAsync(BaseListFilterDto input)
        {
            var query = await Repository.GetQueryableAsync();
            query = query.WhereIf(!string.IsNullOrWhiteSpace(input.Keyword), x => x.Name.Contains(input.Keyword!));
            var totalCount = await AsyncExecuter.LongCountAsync(query);
            var data = await AsyncExecuter.ToListAsync(query.Skip(input.SkipCount).Take(input.MaxResultCount));
            return new PagedResultDto<ProductCategoryInListDto>(totalCount, ObjectMapper.Map<List<ProductCategory>, List<ProductCategoryInListDto>>(data));
        }

        [Authorize(MyEcommercePermissions.ProductCategory.Delete)]
        public async Task DeleteMultipleAsync(IEnumerable<Guid> ids)
        {
            await Repository.DeleteManyAsync(ids);
        }

        [Authorize(MyEcommercePermissions.ProductCategory.Default)]
        public async Task<List<ProductCategoryInListDto>> GetListAllAsync()
        {
            var query = await Repository.GetQueryableAsync();
            query = query.Where(x => x.IsActive == true);
            var data = await AsyncExecuter.ToListAsync(query);
            return ObjectMapper.Map<List<ProductCategory>, List<ProductCategoryInListDto>>(data);
        }

        [Authorize(MyEcommercePermissions.ProductCategory.Default)]
        public async Task<string> GetSuggestNewCodeAsync()
        {
            return await _productCategoryCodeGenerator.GenerateAsync();
        }

        #region SAVE COVER PICTURE
        [Authorize(MyEcommercePermissions.ProductCategory.Default)]
        private async Task SaveCoverPictureAsync(string fileName, string base64)
        {
            Regex regex = new Regex(@"^[\w/\:.-]+;base64,");
            base64 = regex.Replace(base64, string.Empty);
            byte[] bytes = Convert.FromBase64String(base64);
            await _blobContainer.SaveAsync(fileName, bytes, overrideExisting: true);
        }
        #endregion

        #region GET COVER PICTURE
        [Authorize(MyEcommercePermissions.ProductCategory.Default)]
        public async Task<string?> GetCoverPictureAsync(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return null;
            }
            var coverPictureContent = await _blobContainer.GetAllBytesOrNullAsync(fileName);
            if (coverPictureContent is null)
            {
                return null;
            }
            var result = Convert.ToBase64String(coverPictureContent);
            return result;
        }
        #endregion

        #region CREATE PRODUCT CATEGORY
        [Authorize(MyEcommercePermissions.ProductCategory.Create)]
        public override async Task<ProductCategoryDto> CreateAsync(CreateUpdateProductCategoryDto input)
        {
            var productCategory = await _productCategoryManager.CreateAsync(input.Name, input.Code, input.Slug, input.SortOrder, "", input.Visibility, input.IsActive, input.ParentId, input.SeoMetaDescription);
            if (input.CoverPictureContent != null && input.CoverPictureContent.Length > 0)
            {
                await SaveCoverPictureAsync(input.CoverPictureName, input.CoverPictureContent);
                productCategory.CoverPicture = input.CoverPictureName;
            }
            await _productCategoryRepository.InsertAsync(productCategory);
            return ObjectMapper.Map<ProductCategory, ProductCategoryDto>(productCategory);
        }
        #endregion
    }
}
