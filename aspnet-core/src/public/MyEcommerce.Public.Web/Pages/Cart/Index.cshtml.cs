using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyEcommerce.Public.Products;
using MyEcommerce.Public.Web.Models;

namespace MyEcommerce.Public.Web.Pages.Cart
{
    public class IndexModel : PageModel
    {
        private readonly IProductsAppService _productsAppService;
        public IndexModel(IProductsAppService productsAppService)
        {
            _productsAppService = productsAppService;
        }

        [BindProperty]
        public List<CartItem> CartItems { get; set; }
        public async Task OnGetAsync(string action, string id)
        {
            var cart = HttpContext.Session.GetString(MyEcommerceConsts.Cart);
            var productCarts = new Dictionary<string, CartItem>();
            if (cart != null)
            {
                productCarts = JsonSerializer.Deserialize<Dictionary<string, CartItem>>(cart);
            }
            if (!string.IsNullOrEmpty(action))
            {
                if (action == "add")
                {
                    var product = await _productsAppService.GetAsync(Guid.Parse(id));
                    if (cart == null)
                    {
                        productCarts.Add(id, new CartItem()
                        {
                            Product = product,
                            Quantity = 1
                        });
                        HttpContext.Session.SetString(MyEcommerceConsts.Cart, JsonSerializer.Serialize(productCarts));
                    }
                    else
                    {
                        productCarts = JsonSerializer.Deserialize<Dictionary<string, CartItem>>(cart);
                        if (productCarts.ContainsKey(id))
                        {
                            productCarts[id].Quantity += 1;
                        }
                        else
                        {
                            productCarts.Add(id, new CartItem()
                            {
                                Product = product,
                                Quantity = 1
                            });
                        }
                        HttpContext.Session.SetString(MyEcommerceConsts.Cart, JsonSerializer.Serialize(productCarts));
                    }
                }
                else if (action == "remove")
                {
                    productCarts = JsonSerializer.Deserialize<Dictionary<string, CartItem>>(cart);
                    if (productCarts.ContainsKey(id))
                    {
                        productCarts.Remove(id);
                    }

                    HttpContext.Session.SetString(MyEcommerceConsts.Cart, JsonSerializer.Serialize(productCarts));
                }
            }
            CartItems = productCarts.Values.ToList();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var cart = HttpContext.Session.GetString(MyEcommerceConsts.Cart);
            var productCarts = JsonSerializer.Deserialize<Dictionary<string, CartItem>>(cart) ?? new Dictionary<string, CartItem>();

            if (CartItems != null)
            {
                foreach (var item in productCarts.Values)
                {
                    var cartItem = CartItems.FirstOrDefault(x => x.Product.Id == item.Product.Id);
                    if (cartItem != null)
                    {
                        item.Quantity = cartItem.Quantity;
                        item.Selected = cartItem.Selected;
                        item.Product = await _productsAppService.GetAsync(cartItem.Product.Id);
                    }
                }
            }

            HttpContext.Session.SetString(MyEcommerceConsts.Cart, JsonSerializer.Serialize(productCarts));
            return Redirect("/shop-cart.html");
        }

        public async Task<IActionResult> OnPostCheckoutAsync()
        {
            var cart = HttpContext.Session.GetString(MyEcommerceConsts.Cart);
            var productCarts = JsonSerializer.Deserialize<Dictionary<string, CartItem>>(cart) ?? new Dictionary<string, CartItem>();

            if (CartItems != null)
            {
                foreach (var item in productCarts.Values)
                {
                    var cartItem = CartItems.FirstOrDefault(x => x.Product.Id == item.Product.Id);
                    if (cartItem != null)
                    {
                        item.Quantity = cartItem.Quantity;
                        item.Selected = cartItem.Selected;
                        item.Product = await _productsAppService.GetAsync(cartItem.Product.Id);
                    }
                }
            }

            HttpContext.Session.SetString(MyEcommerceConsts.Cart, JsonSerializer.Serialize(productCarts));

            if (!productCarts.Values.Any(x => x.Selected))
            {
                ModelState.AddModelError(string.Empty, "Please select at least one item to checkout.");
                CartItems = productCarts.Values.ToList();
                return Page();
            }

            return Redirect("/checkout.html");
        }
    }
}
