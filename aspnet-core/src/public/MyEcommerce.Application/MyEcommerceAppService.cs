using System;
using System.Collections.Generic;
using System.Text;
using MyEcommerce.Localization;
using Volo.Abp.Application.Services;

namespace MyEcommerce;

/* Inherit your application services from this class.
 */
public abstract class MyEcommerceAppService : ApplicationService
{
    protected MyEcommerceAppService()
    {
        LocalizationResource = typeof(MyEcommerceResource);
    }
}
