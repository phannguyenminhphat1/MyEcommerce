using MyEcommerce.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace MyEcommerce.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class MyEcommerceController : AbpControllerBase
{
    protected MyEcommerceController()
    {
        LocalizationResource = typeof(MyEcommerceResource);
    }
}
