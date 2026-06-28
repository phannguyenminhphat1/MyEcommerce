using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyEcommerce.Public.ProductCategories;
using MyEcommerce.Public.Products;
using MyEcommerce.Public.Products.Attributes;

namespace MyEcommerce.Public.Web.Pages.Products
{
    public class DetailsModel : PageModel
    {
        private readonly IProductsAppService _productsAppService;
        private readonly IProductCategoriesAppService _productCategoriesAppService;
        public DetailsModel(IProductsAppService productsAppService,
            IProductCategoriesAppService productCategoriesAppService)
        {
            _productsAppService = productsAppService;
            _productCategoriesAppService = productCategoriesAppService;
        }
        public ProductCategoryDto Category { get; set; } = new ProductCategoryDto();
        public ProductDto Product { get; set; } = new ProductDto();
        public List<ProductAttributeValueDto> ProductAttributes { get; set; } = new();

        public async Task OnGetAsync(string categorySlug, string slug)
        {
            Category = await _productCategoriesAppService.GetBySlugAsync(categorySlug);
            Product = await _productsAppService.GetBySlugAsync(slug);

            if (Product.Id != System.Guid.Empty)
            {
                ProductAttributes = await _productsAppService.GetListProductAttributeAllAsync(Product.Id);
            }
        }
    }
}
