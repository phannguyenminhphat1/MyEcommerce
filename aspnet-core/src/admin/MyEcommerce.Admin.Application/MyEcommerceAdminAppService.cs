using System;
using System.Collections.Generic;
using System.Text;
using MyEcommerce.Localization;
using Volo.Abp.Application.Services;

namespace MyEcommerce.Admin;

/* Inherit your application services from this class.
 */
public abstract class MyEcommerceAdminAppService : ApplicationService
{
    protected MyEcommerceAdminAppService()
    {
        LocalizationResource = typeof(MyEcommerceResource);
    }
}
