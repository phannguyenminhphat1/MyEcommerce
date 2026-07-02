using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyEcommerce.Emailing;
using MyEcommerce.Public.Orders;
using MyEcommerce.Public.Web.Extensions;
using MyEcommerce.Public.Web.Models;
using Volo.Abp.Emailing;
using Volo.Abp.TextTemplating;

namespace MyEcommerce.Public.Web.Pages.Cart
{
    public class CheckoutModel : PageModel
    {
        private readonly IOrdersAppService _ordersAppService;
        private readonly IEmailSender _emailSender;
        private readonly ITemplateRenderer _templateRenderer;
        public CheckoutModel(IOrdersAppService ordersAppService, IEmailSender emailSender, ITemplateRenderer templateRenderer)
        {
            _ordersAppService = ordersAppService;
            _emailSender = emailSender;
            _templateRenderer = templateRenderer;
        }
        public List<CartItem> CartItems { get; set; }
        public bool? CreateStatus { set; get; }
        public bool ShowLoginPopup { get; set; }

        [BindProperty]
        public OrderDto Order { set; get; }

        public void OnGet()
        {
            CartItems = GetCartItems().Where(x => x.Selected).ToList();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            CartItems = GetCartItems().Where(x => x.Selected).ToList();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (!User.Identity.IsAuthenticated)
            {
                ShowLoginPopup = true;
                return Page();
            }

            if (!CartItems.Any())
            {
                ModelState.AddModelError(string.Empty, "Please select at least one item before placing an order.");
                return Page();
            }

            var cartItems = new List<OrderItemDto>();
            foreach (var item in CartItems)
            {
                cartItems.Add(new OrderItemDto()
                {
                    Price = item.Product.SellPrice,
                    ProductId = item.Product.Id,
                    Quantity = item.Quantity
                });
            }
            var currentUserId = User.GetUserId();
            var order = await _ordersAppService.CreateAsync(new CreateOrderDto()
            {
                CustomerName = Order.CustomerName,
                CustomerAddress = Order.CustomerAddress,
                CustomerPhoneNumber = Order.CustomerPhoneNumber,
                Items = cartItems,
                CustomerUserId = currentUserId
            });
            CartItems = GetCartItems().Where(x => x.Selected).ToList();

            if (order != null)
            {
                var email = User.GetSpecificClaim(ClaimTypes.Email);
                var emailBody = await _templateRenderer.RenderAsync(
                    EmailTemplates.CreateOrderEmail,
                    new
                    {
                        message = "Create order successfully"
                    });
                await _emailSender.SendAsync(email, "Created order successfully", emailBody);
                CreateStatus = true;
            }
            else
            {
                CreateStatus = false;
            }
            return Page();
        }

        private List<CartItem> GetCartItems()
        {
            var cart = HttpContext.Session.GetString(MyEcommerceConsts.Cart);
            var productCarts = new Dictionary<string, CartItem>();
            if (cart != null)
            {
                productCarts = JsonSerializer.Deserialize<Dictionary<string, CartItem>>(cart);
            }
            return productCarts.Values.ToList();
        }

    }
}
