using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MyEcommerce.Public.Web.ViewComponents
{
    public class HeaderViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View();
        }
    }
}
