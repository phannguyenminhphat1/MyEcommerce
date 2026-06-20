using Microsoft.Extensions.Localization;
using MyEcommerce.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace MyEcommerce.Public.Web;

[Dependency(ReplaceServices = true)]
public class MyEcommercePublicBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<MyEcommerceResource> _localizer;

    public MyEcommercePublicBrandingProvider(IStringLocalizer<MyEcommerceResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
