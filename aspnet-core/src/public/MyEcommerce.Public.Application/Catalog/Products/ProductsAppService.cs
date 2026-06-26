using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyEcommerce.Manufacturers;
using MyEcommerce.Orders;
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
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<OrderItem> _orderItemRepository;
        private readonly IRepository<Manufacturer> _manufacturerRepository;

        public ProductsAppService(
            IProductRepository productRepository,
            ProductManager productManager,
            IBlobContainer<ProductThumbnailPictureContainer> blobContainer,
            IRepository<ProductAttribute> productAttributeRepository,
            IRepository<ProductAttributeDateTime> productAttributeDateTimeRepository,
            IRepository<ProductAttributeInt> productAttributeIntRepository,
            IRepository<ProductAttributeDecimal> productAttributeDecimalRepository,
            IRepository<ProductAttributeVarchar> productAttributeVarcharRepository,
            IRepository<ProductAttributeText> productAttributeTextRepository,
            IRepository<Order> orderRepository,
            IRepository<OrderItem> orderItemRepository,
            IRepository<Manufacturer> manufacturerRepository) : base(productRepository)
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
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _manufacturerRepository = manufacturerRepository;
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
        public async Task<PagedResult<ProductInListDto>> GetListFilterAsync(ProductListFilterDto input)
        {
            var totalCount = await _productRepository.GetCountAsync(input.Keyword, input.CategoryId);
            var data = await _productRepository.GetListFilterAsync(
                input.Keyword,
                input.CategoryId,
                (input.CurrentPage - 1) * input.PageSize,
                input.PageSize);
            return new PagedResult<ProductInListDto>(
                ObjectMapper.Map<List<Product>, List<ProductInListDto>>(data),
                totalCount,
                input.CurrentPage,
                input.PageSize
            );
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
        public async Task<PagedResult<ProductAttributeValueDto>> GetListProductAttributesAsync(ProductAttributeListFilterDto input)
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
            var data = await AsyncExecuter
                .ToListAsync(query.OrderByDescending(x => x.Label).Skip((input.CurrentPage - 1) * input.PageSize)
                .Take(input.PageSize));
            return new PagedResult<ProductAttributeValueDto>(data,
                totalCount,
                input.CurrentPage,
                input.PageSize
            );
        }
        #endregion

        #region GET LIST TOP SELLER
        public async Task<List<ProductInListDto>> GetListTopSellerAsync(int numberOfRecords)
        {
            var productQuery = await Repository.GetQueryableAsync();
            var orderQuery = await _orderRepository.GetQueryableAsync();
            var orderItemQuery = await _orderItemRepository.GetQueryableAsync();
            var manufacturerQuery = await _manufacturerRepository.GetQueryableAsync();

            var query =
                from oi in orderItemQuery
                join o in orderQuery on oi.OrderId equals o.Id
                join p in productQuery on oi.ProductId equals p.Id
                join m in manufacturerQuery on p.ManufacturerId equals m.Id
                where p.IsActive && o.Status == OrderStatus.Finished
                group new { oi, p, m } by p into g
                orderby g.Sum(x => x.oi.Quantity) descending
                select new ProductInListDto
                {
                    Id = g.Key.Id,
                    ManufacturerId = g.Key.ManufacturerId,
                    ManufacturerName = g.Select(x => x.m.Name).FirstOrDefault() ?? string.Empty,
                    Name = g.Key.Name,
                    Code = g.Key.Code,
                    Slug = g.Key.Slug,
                    ProductType = g.Key.ProductType,
                    SKU = g.Key.SKU,
                    SortOrder = g.Key.SortOrder,
                    Visibility = g.Key.Visibility,
                    IsActive = g.Key.IsActive,
                    CategoryId = g.Key.CategoryId,
                    ThumbnailPicture = g.Key.ThumbnailPicture,
                    SellPrice = g.Key.SellPrice
                };

            var products = await AsyncExecuter.ToListAsync(
                query.Take(numberOfRecords)
            );
            if (products.Count < numberOfRecords)
            {
                var remaining = numberOfRecords - products.Count;
                var excludeIds = products.Select(p => p.Id).ToList();
                var additionalQuery =
                    from p in productQuery
                    join m in manufacturerQuery on p.ManufacturerId equals m.Id
                    where p.IsActive && !excludeIds.Contains(p.Id)
                    orderby p.CreationTime descending
                    select new ProductInListDto
                    {
                        Id = p.Id,
                        ManufacturerId = p.ManufacturerId,
                        ManufacturerName = m.Name,
                        Name = p.Name,
                        Code = p.Code,
                        Slug = p.Slug,
                        ProductType = p.ProductType,
                        SKU = p.SKU,
                        SortOrder = p.SortOrder,
                        Visibility = p.Visibility,
                        IsActive = p.IsActive,
                        CategoryId = p.CategoryId,
                        ThumbnailPicture = p.ThumbnailPicture,
                        SellPrice = p.SellPrice
                    };
                var additionalProducts = await AsyncExecuter.ToListAsync(additionalQuery.Take(remaining));
                products.AddRange(additionalProducts);
            }
            return products;
        }
        #endregion

        #region GET PRODUCT BY SLUG
        public async Task<ProductDto> GetBySlugAsync(string slug)
        {
            var product = await _productRepository.GetAsync(x => x.Slug == slug);
            return ObjectMapper.Map<Product, ProductDto>(product);
        }
        #endregion
    }
}