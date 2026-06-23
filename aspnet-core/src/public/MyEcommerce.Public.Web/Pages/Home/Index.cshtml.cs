using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using MyEcommerce.Public.ProductCategories;
using MyEcommerce.Public.Products;
using MyEcommerce.Public.Web.Models;
using Volo.Abp.Caching;

namespace MyEcommerce.Public.Web.Pages;

public class IndexModel : MyEcommercePublicPageModel
{
    private readonly IDistributedCache<HomeCacheItem> _distributedCache;
    private readonly IProductCategoriesAppService _productCategoriesAppService;
    private readonly IProductsAppService _productsAppService;
    public List<ProductCategoryInListDto>? Categories { get; set; }
    public List<ProductInListDto>? TopSellerProducts { get; set; }
    public IndexModel(IProductCategoriesAppService productCategoriesAppService,
       IProductsAppService productsAppService, IDistributedCache<HomeCacheItem> distributedCache)
    {
        _productCategoriesAppService = productCategoriesAppService;
        _productsAppService = productsAppService;
        _distributedCache = distributedCache;
    }
    public async Task OnGet()
    {
        var cacheItem = await _distributedCache.GetOrAddAsync(MyEcommercePublicConsts.CacheKeys.HomeData, async () =>
        {
            var allCategories = await _productCategoriesAppService.GetListAllAsync();
            var rootCategories = allCategories.Where(x => x.ParentId == null).ToList();
            foreach (var category in rootCategories)
            {
                category.Children = rootCategories.Where(x => x.ParentId == category.Id).ToList();
            }
            var topSellerProducts = await _productsAppService.GetListTopSellerAsync(10);
            return new HomeCacheItem()
            {
                TopSellerProducts = topSellerProducts,
                Categories = rootCategories
            };
        }, () => new DistributedCacheEntryOptions
        {
            AbsoluteExpiration = DateTimeOffset.Now.AddHours(12)
        });

        TopSellerProducts = cacheItem?.TopSellerProducts;
        Categories = cacheItem?.Categories;
    }
}
