using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyEcommerce.Public.Web.Pages.Auth
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnGet()
        {
            HttpContext.Session.Remove(MyEcommerceConsts.Cart);
            return SignOut(CookieAuthenticationDefaults.AuthenticationScheme,
                OpenIdConnectDefaults.AuthenticationScheme);
        }
    }
}