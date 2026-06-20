using MyEcommerce.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace MyEcommerce.Public.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class MyEcommercePublicController : AbpControllerBase
{
    protected MyEcommercePublicController()
    {
        LocalizationResource = typeof(MyEcommerceResource);
    }
}
