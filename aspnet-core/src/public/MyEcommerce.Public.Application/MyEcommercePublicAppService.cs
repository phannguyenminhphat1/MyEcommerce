using System;
using System.Collections.Generic;
using System.Text;
using MyEcommerce.Localization;
using Volo.Abp.Application.Services;

namespace MyEcommerce.Public;

/* Inherit your application services from this class.
 */
public abstract class MyEcommercePublicAppService : ApplicationService
{
    protected MyEcommercePublicAppService()
    {
        LocalizationResource = typeof(MyEcommerceResource);
    }
}
