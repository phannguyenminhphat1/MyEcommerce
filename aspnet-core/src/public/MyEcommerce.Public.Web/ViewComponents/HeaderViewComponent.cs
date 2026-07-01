using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyEcommerce.Public.ProductCategories;
using MyEcommerce.Public.Web.Models;
using Volo.Abp.Caching;

namespace MyEcommerce.Public.Web.ViewComponents
{
    public class HeaderViewComponent : ViewComponent
    {
        private readonly IProductCategoriesAppService _productCategoriesAppService;
        private readonly IDistributedCache<HeaderCacheItem> _distributedCache;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HeaderViewComponent(
            IProductCategoriesAppService productCategoriesAppService,
            IDistributedCache<HeaderCacheItem> distributedCache,
            IHttpContextAccessor httpContextAccessor)
        {
            _productCategoriesAppService = productCategoriesAppService;
            _distributedCache = distributedCache;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var cacheItem = await _distributedCache.GetOrAddAsync(MyEcommercePublicConsts.CacheKeys.HeaderData, async () =>
            {
                var model = await _productCategoriesAppService.GetListAllAsync();
                return new HeaderCacheItem()
                {
                    Categories = model
                };
            },
            () => new Microsoft.Extensions.Caching.Distributed.DistributedCacheEntryOptions
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddHours(12)
            });

            var cartItems = new List<CartItem>();
            var session = _httpContextAccessor.HttpContext?.Session;
            var cart = session?.GetString(MyEcommerceConsts.Cart);
            if (!string.IsNullOrWhiteSpace(cart))
            {
                var productCarts = JsonSerializer.Deserialize<Dictionary<string, CartItem>>(cart);
                cartItems = productCarts?.Values.ToList() ?? new List<CartItem>();
            }

            return View(new HeaderCacheItem
            {
                Categories = cacheItem?.Categories,
                CartItems = cartItems
            });
        }
    }
}
