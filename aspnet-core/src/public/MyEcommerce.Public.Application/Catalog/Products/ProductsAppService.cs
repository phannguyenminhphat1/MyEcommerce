using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyEcommerce.ProductAttributes;
using MyEcommerce.ProductCategories;
using MyEcommerce.Products;
using MyEcommerce.Public.Products.Attributes;
using MyEcommerce.Repositories;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.BlobStoring;
using Volo.Abp.Domain.Repositories;

namespace MyEcommerce.Public.Products
{
    public class ProductsAppService : ReadOnlyAppService<
        Product,
        ProductDto,
        Guid,
        PagedResultRequestDto>, IProductsAppService
    {
        private readonly IProductRepository _productRepository;
        private readonly ProductManager _productManager;
        private readonly IBlobContainer<ProductThumbnailPictureContainer> _blobContainer;
        private readonly IRepository<ProductAttribute> _productAttributeRepository;
        private readonly IRepository<ProductAttributeDateTime> _productAttributeDateTimeRepository;
        private readonly IRepository<ProductAttributeInt> _productAttributeIntRepository;
        private readonly IRepository<ProductAttributeDecimal> _productAttributeDecimalRepository;
        private readonly IRepository<ProductAttributeVarchar> _productAttributeVarcharRepository;
        private readonly IRepository<ProductAttributeText> _productAttributeTextRepository;


        public ProductsAppService(
            IProductRepository productRepository,
            ProductManager productManager,
            IBlobContainer<ProductThumbnailPictureContainer> blobContainer,
            IRepository<ProductAttribute> productAttributeRepository,
            IRepository<ProductAttributeDateTime> productAttributeDateTimeRepository,
            IRepository<ProductAttributeInt> productAttributeIntRepository,
            IRepository<ProductAttributeDecimal> productAttributeDecimalRepository,
            IRepository<ProductAttributeVarchar> productAttributeVarcharRepository,
            IRepository<ProductAttributeText> productAttributeTextRepository
              )
            : base(productRepository)
        {
            _productRepository = productRepository;
            _productManager = productManager;
            _blobContainer = blobContainer;
            _productAttributeRepository = productAttributeRepository;
            _productAttributeDateTimeRepository = productAttributeDateTimeRepository;
            _productAttributeIntRepository = productAttributeIntRepository;
            _productAttributeDecimalRepository = productAttributeDecimalRepository;
            _productAttributeVarcharRepository = productAttributeVarcharRepository;
            _productAttributeTextRepository = productAttributeTextRepository;
        }

        #region GET LIST PRODUCTS
        public async Task<List<ProductInListDto>> GetListAllAsync()
        {
            var query = await _productRepository.GetQueryableAsync();
            query = query.Where(x => x.IsActive == true);
            var data = await AsyncExecuter.ToListAsync(query);
            return ObjectMapper.Map<List<Product>, List<ProductInListDto>>(data);
        }
        #endregion

        #region GET LIST PRODUCTS WITH FILTER
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
        #endregion

        #region GET THUMBNAIL IMAGE
        public async Task<string?> GetThumbnailImageAsync(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return null;
            }
            var thumbnailContent = await _blobContainer.GetAllBytesOrNullAsync(fileName);
            if (thumbnailContent is null)
            {
                return null;
            }
            var result = Convert.ToBase64String(thumbnailContent);
            return result;
        }
        #endregion

        #region GET LIST PRODUCT ATTRIBUTE
        public async Task<List<ProductAttributeValueDto>> GetListProductAttributeAllAsync(Guid productId)
        {
            var attributeQuery = await _productAttributeRepository.GetQueryableAsync();
            var attributeDateTimeQuery = await _productAttributeDateTimeRepository.GetQueryableAsync();
            var attributeDecimalQuery = await _productAttributeDecimalRepository.GetQueryableAsync();
            var attributeIntQuery = await _productAttributeIntRepository.GetQueryableAsync();
            var attributeVarcharQuery = await _productAttributeVarcharRepository.GetQueryableAsync();
            var attributeTextQuery = await _productAttributeTextRepository.GetQueryableAsync();

            var query = from a in attributeQuery

                        join adate in attributeDateTimeQuery on a.Id equals adate.AttributeId into aDateTimeTable
                        from adate in aDateTimeTable.DefaultIfEmpty()

                        join adecimal in attributeDecimalQuery on a.Id equals adecimal.AttributeId into aDecimalTable
                        from adecimal in aDecimalTable.DefaultIfEmpty()

                        join aint in attributeIntQuery on a.Id equals aint.AttributeId into aIntTable
                        from aint in aIntTable.DefaultIfEmpty()

                        join aVarchar in attributeVarcharQuery on a.Id equals aVarchar.AttributeId into aVarcharTable
                        from aVarchar in aVarcharTable.DefaultIfEmpty()

                        join aText in attributeTextQuery on a.Id equals aText.AttributeId into aTextTable
                        from aText in aTextTable.DefaultIfEmpty()

                        where (adate == null || adate.ProductId == productId)
                        && (adecimal == null || adecimal.ProductId == productId)
                        && (aint == null || aint.ProductId == productId)
                        && (aVarchar == null || aVarchar.ProductId == productId)
                        && (aText == null || aText.ProductId == productId)

                        select new ProductAttributeValueDto
                        {
                            Label = a.Label,
                            AttributeId = a.Id,
                            DataType = a.DataType,
                            Code = a.Code,
                            ProductId = productId,

                            DateTimeValue = adate != null ? adate.Value : null,
                            DecimalValue = adecimal != null ? adecimal.Value : null,
                            IntValue = aint != null ? aint.Value : null,
                            TextValue = aText != null ? aText.Value : null,
                            VarcharValue = aVarchar != null ? aVarchar.Value : null,

                            DateTimeId = adate != null ? adate.Id : null,
                            DecimalId = adecimal != null ? adecimal.Id : null,
                            IntId = aint != null ? aint.Id : null,
                            TextId = aText != null ? aText.Id : null,
                            VarcharId = aVarchar != null ? aVarchar.Id : null
                        };

            query = query.Where(x => x.DateTimeId != null || x.DecimalId != null || x.IntValue != null || x.TextId != null || x.VarcharId != null);
            return await AsyncExecuter.ToListAsync(query);
        }
        #endregion

        #region GET LIST PRODUCT ATTRIBUTE WITH PAGING
        public async Task<PagedResultDto<ProductAttributeValueDto>> GetListProductAttributesAsync(ProductAttributeListFilterDto input)
        {
            var attributeQuery = await _productAttributeRepository.GetQueryableAsync();
            var attributeDateTimeQuery = await _productAttributeDateTimeRepository.GetQueryableAsync();
            var attributeDecimalQuery = await _productAttributeDecimalRepository.GetQueryableAsync();
            var attributeIntQuery = await _productAttributeIntRepository.GetQueryableAsync();
            var attributeVarcharQuery = await _productAttributeVarcharRepository.GetQueryableAsync();
            var attributeTextQuery = await _productAttributeTextRepository.GetQueryableAsync();

            var query = from a in attributeQuery

                        join adate in attributeDateTimeQuery on a.Id equals adate.AttributeId into aDateTimeTable
                        from adate in aDateTimeTable.DefaultIfEmpty()

                        join adecimal in attributeDecimalQuery on a.Id equals adecimal.AttributeId into aDecimalTable
                        from adecimal in aDecimalTable.DefaultIfEmpty()

                        join aint in attributeIntQuery on a.Id equals aint.AttributeId into aIntTable
                        from aint in aIntTable.DefaultIfEmpty()

                        join aVarchar in attributeVarcharQuery on a.Id equals aVarchar.AttributeId into aVarcharTable
                        from aVarchar in aVarcharTable.DefaultIfEmpty()

                        join aText in attributeTextQuery on a.Id equals aText.AttributeId into aTextTable
                        from aText in aTextTable.DefaultIfEmpty()

                        where (adate == null || adate.ProductId == input.ProductId)
                        && (adecimal == null || adecimal.ProductId == input.ProductId)
                        && (aint == null || aint.ProductId == input.ProductId)
                        && (aVarchar == null || aVarchar.ProductId == input.ProductId)
                        && (aText == null || aText.ProductId == input.ProductId)

                        select new ProductAttributeValueDto
                        {
                            Label = a.Label,
                            AttributeId = a.Id,
                            DataType = a.DataType,
                            Code = a.Code,
                            ProductId = input.ProductId,

                            DateTimeValue = adate != null ? adate.Value : null,
                            DecimalValue = adecimal != null ? adecimal.Value : null,
                            IntValue = aint != null ? aint.Value : null,
                            TextValue = aText != null ? aText.Value : null,
                            VarcharValue = aVarchar != null ? aVarchar.Value : null,

                            DateTimeId = adate != null ? adate.Id : null,
                            DecimalId = adecimal != null ? adecimal.Id : null,
                            IntId = aint != null ? aint.Id : null,
                            TextId = aText != null ? aText.Id : null,
                            VarcharId = aVarchar != null ? aVarchar.Id : null
                        };

            query = query.Where(x => x.DateTimeId != null || x.DecimalId != null || x.IntValue != null || x.TextId != null || x.VarcharId != null);
            var totalCount = await AsyncExecuter.LongCountAsync(query);
            var data = await AsyncExecuter.ToListAsync(
                query.OrderByDescending(x => x.Label)
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount)
                );
            return new PagedResultDto<ProductAttributeValueDto>(totalCount, data);
        }
        #endregion
    }
}