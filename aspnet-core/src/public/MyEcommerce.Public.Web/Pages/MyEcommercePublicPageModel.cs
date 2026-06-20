using MyEcommerce.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace MyEcommerce.Public.Web.Pages;

/* Inherit your PageModel classes from this class.
 */
public abstract class MyEcommercePublicPageModel : AbpPageModel
{
    protected MyEcommercePublicPageModel()
    {
        LocalizationResourceType = typeof(MyEcommerceResource);
    }
}
