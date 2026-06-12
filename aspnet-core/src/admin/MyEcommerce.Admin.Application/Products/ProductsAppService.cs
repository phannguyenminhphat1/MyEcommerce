using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using MyEcommerce.Admin.Products.Attributes;
using MyEcommerce.ProductAttributes;
using MyEcommerce.Products;
using MyEcommerce.Repositories;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.BlobStoring;
using Volo.Abp.Domain.Repositories;
namespace MyEcommerce.Admin.Products
{
    [Authorize]
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
        private readonly IBlobContainer<ProductThumbnailPictureContainer> _blobContainer;
        private readonly ProductCodeGenerator _productCodeGenerator;
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
            ProductCodeGenerator productCodeGenerator,
            IRepository<ProductAttribute> productAttributeRepository,
            IRepository<ProductAttributeDateTime> productAttributeDateTimeRepository,
            IRepository<ProductAttributeInt> productAttributeIntRepository,
            IRepository<ProductAttributeDecimal> productAttributeDecimalRepository,
            IRepository<ProductAttributeVarchar> productAttributeVarcharRepository,
            IRepository<ProductAttributeText> productAttributeTextRepository) : base(productRepository)
        {
            _productRepository = productRepository;
            _productManager = productManager;
            _blobContainer = blobContainer;
            _productCodeGenerator = productCodeGenerator;
            _productAttributeRepository = productAttributeRepository;
            _productAttributeDateTimeRepository = productAttributeDateTimeRepository;
            _productAttributeIntRepository = productAttributeIntRepository;
            _productAttributeDecimalRepository = productAttributeDecimalRepository;
            _productAttributeVarcharRepository = productAttributeVarcharRepository;
            _productAttributeTextRepository = productAttributeTextRepository;
        }

        #region DELETE MULTIPLE PRODUCTS
        public async Task DeleteMultipleAsync(IEnumerable<Guid> ids)
        {
            await _productRepository.DeleteManyAsync(ids);
        }
        #endregion

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

        #region CREATE PRODUCT
        public override async Task<ProductDto> CreateAsync(CreateUpdateProductDto input)
        {
            var product = await _productManager.CreateAsync(input.ManufacturerId, input.Name, input.Code, input.Slug, input.ProductType, input.SKU, input.SortOrder, input.Visibility, input.IsActive, input.CategoryId, input.SeoMetaDescription, input.Description, "", input.SellPrice);
            if (input.ThumbnailPictureContent != null && input.ThumbnailPictureContent.Length > 0)
            {
                await SaveThumbnailImageAsync(input.ThumbnailPictureName, input.ThumbnailPictureContent);
                product.ThumbnailPicture = input.ThumbnailPictureName;
            }
            await _productRepository.InsertAsync(product);
            return ObjectMapper.Map<Product, ProductDto>(product);
        }
        #endregion

        #region UPDATE PRODUCT
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
            if (input.ThumbnailPictureContent != null && input.ThumbnailPictureContent.Length > 0)
            {
                await SaveThumbnailImageAsync(input.ThumbnailPictureName, input.ThumbnailPictureContent);
                product.ThumbnailPicture = input.ThumbnailPictureName;
            }
            product.SellPrice = input.SellPrice;
            await Repository.UpdateAsync(product);
            return ObjectMapper.Map<Product, ProductDto>(product);
        }
        #endregion

        #region SAVE THUMBNAIL IMAGE
        private async Task SaveThumbnailImageAsync(string fileName, string base64)
        {
            Regex regex = new Regex(@"^[\w/\:.-]+;base64,");
            base64 = regex.Replace(base64, string.Empty);
            byte[] bytes = Convert.FromBase64String(base64);
            await _blobContainer.SaveAsync(fileName, bytes, overrideExisting: true);
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

        #region GET SUGGEST NEW CODE
        public async Task<string> GetSuggestNewCodeAsync()
        {
            return await _productCodeGenerator.GenerateAsync();
        }
        #endregion

        #region ADD PRODUCT ATTRIBUTE
        public async Task<ProductAttributeValueDto> AddProductAttributeAsync(AddUpdateProductAttributeDto input)
        {
            var product = await Repository.GetAsync(input.ProductId);
            if (product == null)
                throw new BusinessException(MyEcommerceDomainErrorCodes.ProductIsNotExists);

            var attribute = await _productAttributeRepository.GetAsync(x => x.Id == input.AttributeId);
            if (attribute == null)
                throw new BusinessException(MyEcommerceDomainErrorCodes.ProductAttributeNotExists);

            var newAttributeId = Guid.NewGuid();
            switch (attribute.DataType)
            {
                case AttributeType.Date:
                    if (input.DateTimeValue == null)
                    {
                        throw new BusinessException(MyEcommerceDomainErrorCodes.ProductAttributeValueIsNotValid);
                    }
                    var productAttributeDateTime = new ProductAttributeDateTime(newAttributeId, input.AttributeId, input.ProductId, input.DateTimeValue);
                    await _productAttributeDateTimeRepository.InsertAsync(productAttributeDateTime);
                    break;

                case AttributeType.Int:
                    if (input.IntValue == null)
                    {
                        throw new BusinessException(MyEcommerceDomainErrorCodes.ProductAttributeValueIsNotValid);
                    }
                    var productAttributeInt = new ProductAttributeInt(newAttributeId, input.AttributeId, input.ProductId, input.IntValue.Value);
                    await _productAttributeIntRepository.InsertAsync(productAttributeInt);
                    break;

                case AttributeType.Decimal:
                    if (input.DecimalValue == null)
                    {
                        throw new BusinessException(MyEcommerceDomainErrorCodes.ProductAttributeValueIsNotValid);
                    }
                    var productAttributeDecimal = new ProductAttributeDecimal(newAttributeId, input.AttributeId, input.ProductId, input.DecimalValue.Value);
                    await _productAttributeDecimalRepository.InsertAsync(productAttributeDecimal);
                    break;

                case AttributeType.Varchar:
                    if (input.VarcharValue == null)
                    {
                        throw new BusinessException(MyEcommerceDomainErrorCodes.ProductAttributeValueIsNotValid);
                    }
                    var productAttributeVarchar = new ProductAttributeVarchar(newAttributeId, input.AttributeId, input.ProductId, input.VarcharValue);
                    await _productAttributeVarcharRepository.InsertAsync(productAttributeVarchar);
                    break;

                case AttributeType.Text:
                    if (input.TextValue == null)
                    {
                        throw new BusinessException(MyEcommerceDomainErrorCodes.ProductAttributeValueIsNotValid);
                    }
                    var productAttributeText = new ProductAttributeText(newAttributeId, input.AttributeId, input.ProductId, input.TextValue);
                    await _productAttributeTextRepository.InsertAsync(productAttributeText);
                    break;
            }
            return new ProductAttributeValueDto()
            {
                AttributeId = input.AttributeId,
                Code = attribute.Code,
                DataType = attribute.DataType,
                DateTimeValue = input.DateTimeValue,
                DecimalValue = input.DecimalValue,
                Id = newAttributeId,
                IntValue = input.IntValue,
                Label = attribute.Label,
                ProductId = input.ProductId,
                TextValue = input.TextValue
            };
        }
        #endregion

        #region UPDATE PRODUCT ATTRIBUTE
        public async Task<ProductAttributeValueDto> UpdateProductAttributeAsync(Guid id, AddUpdateProductAttributeDto input)
        {
            var product = await Repository.GetAsync(input.ProductId);
            if (product == null)
                throw new BusinessException(MyEcommerceDomainErrorCodes.ProductIsNotExists);

            var attribute = await _productAttributeRepository.GetAsync(x => x.Id == input.AttributeId);
            if (attribute == null)
                throw new BusinessException(MyEcommerceDomainErrorCodes.ProductAttributeIsNotExists);

            switch (attribute.DataType)
            {
                case AttributeType.Date:
                    if (input.DateTimeValue == null)
                    {
                        throw new BusinessException(MyEcommerceDomainErrorCodes.ProductAttributeValueIsNotValid);
                    }
                    var productAttributeDateTime = await _productAttributeDateTimeRepository.GetAsync(x => x.Id == id);
                    if (productAttributeDateTime == null)
                    {
                        throw new BusinessException(MyEcommerceDomainErrorCodes.ProductAttributeDateTimeIsNotExists);
                    }
                    productAttributeDateTime.Value = input.DateTimeValue.Value;
                    await _productAttributeDateTimeRepository.UpdateAsync(productAttributeDateTime);
                    break;
                case AttributeType.Int:
                    if (input.IntValue == null)
                    {
                        throw new BusinessException(MyEcommerceDomainErrorCodes.ProductAttributeValueIsNotValid);
                    }
                    var productAttributeInt = await _productAttributeIntRepository.GetAsync(x => x.Id == id);
                    if (productAttributeInt == null)
                    {
                        throw new BusinessException(MyEcommerceDomainErrorCodes.ProductAttributeIntIsNotExists);
                    }
                    productAttributeInt.Value = input.IntValue.Value;
                    await _productAttributeIntRepository.UpdateAsync(productAttributeInt);
                    break;
                case AttributeType.Decimal:
                    if (input.DecimalValue == null)
                    {
                        throw new BusinessException(MyEcommerceDomainErrorCodes.ProductAttributeValueIsNotValid);
                    }
                    var productAttributeDecimal = await _productAttributeDecimalRepository.GetAsync(x => x.Id == id);
                    if (productAttributeDecimal == null)
                    {
                        throw new BusinessException(MyEcommerceDomainErrorCodes.ProductAttributeDecimalIsNotExists);
                    }
                    productAttributeDecimal.Value = input.DecimalValue.Value;
                    await _productAttributeDecimalRepository.UpdateAsync(productAttributeDecimal);
                    break;
                case AttributeType.Varchar:
                    if (input.VarcharValue == null)
                    {
                        throw new BusinessException(MyEcommerceDomainErrorCodes.ProductAttributeValueIsNotValid);
                    }
                    var productAttributeVarchar = await _productAttributeVarcharRepository.GetAsync(x => x.Id == id);
                    if (productAttributeVarchar == null)
                    {
                        throw new BusinessException(MyEcommerceDomainErrorCodes.ProductAttributeVarcharIsNotExists);
                    }
                    productAttributeVarchar.Value = input.VarcharValue;
                    await _productAttributeVarcharRepository.UpdateAsync(productAttributeVarchar);
                    break;
                case AttributeType.Text:
                    if (input.TextValue == null)
                    {
                        throw new BusinessException(MyEcommerceDomainErrorCodes.ProductAttributeValueIsNotValid);
                    }
                    var productAttributeText = await _productAttributeTextRepository.GetAsync(x => x.Id == id);
                    if (productAttributeText == null)
                    {
                        throw new BusinessException(MyEcommerceDomainErrorCodes.ProductAttributeTextIsNotExists);
                    }
                    productAttributeText.Value = input.TextValue;
                    await _productAttributeTextRepository.UpdateAsync(productAttributeText);
                    break;
            }
            return new ProductAttributeValueDto()
            {
                AttributeId = input.AttributeId,
                Code = attribute.Code,
                DataType = attribute.DataType,
                DateTimeValue = input.DateTimeValue,
                DecimalValue = input.DecimalValue,
                Id = id,
                IntValue = input.IntValue,
                Label = attribute.Label,
                ProductId = input.ProductId,
                TextValue = input.TextValue
            };
        }
        #endregion

        #region REMOVE PRODUCT ATTRIBUTE
        public async Task RemoveProductAttributeAsync(Guid attributeId, Guid id)
        {
            var attribute = await _productAttributeRepository.GetAsync(x => x.Id == attributeId);
            if (attribute == null)
                throw new BusinessException(MyEcommerceDomainErrorCodes.ProductAttributeIsNotExists);
            switch (attribute.DataType)
            {
                case AttributeType.Date:
                    var productAttributeDateTime = await _productAttributeDateTimeRepository.GetAsync(x => x.Id == id);
                    if (productAttributeDateTime == null)
                    {
                        throw new BusinessException(MyEcommerceDomainErrorCodes.ProductAttributeDateTimeIsNotExists);
                    }
                    await _productAttributeDateTimeRepository.DeleteAsync(productAttributeDateTime);
                    break;

                case AttributeType.Int:
                    var productAttributeInt = await _productAttributeIntRepository.GetAsync(x => x.Id == id);
                    if (productAttributeInt == null)
                    {
                        throw new BusinessException(MyEcommerceDomainErrorCodes.ProductAttributeIntIsNotExists);
                    }
                    await _productAttributeIntRepository.DeleteAsync(productAttributeInt);
                    break;

                case AttributeType.Decimal:
                    var productAttributeDecimal = await _productAttributeDecimalRepository.GetAsync(x => x.Id == id);
                    if (productAttributeDecimal == null)
                    {
                        throw new BusinessException(MyEcommerceDomainErrorCodes.ProductAttributeDecimalIsNotExists);
                    }
                    await _productAttributeDecimalRepository.DeleteAsync(productAttributeDecimal);
                    break;

                case AttributeType.Varchar:
                    var productAttributeVarchar = await _productAttributeVarcharRepository.GetAsync(x => x.Id == id);
                    if (productAttributeVarchar == null)
                    {
                        throw new BusinessException(MyEcommerceDomainErrorCodes.ProductAttributeVarcharIsNotExists);
                    }
                    await _productAttributeVarcharRepository.DeleteAsync(productAttributeVarchar);
                    break;

                case AttributeType.Text:
                    var productAttributeText = await _productAttributeTextRepository.GetAsync(x => x.Id == id);
                    if (productAttributeText == null)
                    {
                        throw new BusinessException(MyEcommerceDomainErrorCodes.ProductAttributeTextIsNotExists);
                    }
                    await _productAttributeTextRepository.DeleteAsync(productAttributeText);
                    break;
            }
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