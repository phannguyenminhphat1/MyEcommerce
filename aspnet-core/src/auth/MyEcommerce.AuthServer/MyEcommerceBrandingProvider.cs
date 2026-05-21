using Microsoft.Extensions.Localization;
using MyEcommerce.Localization;
using Volo.Abp.Ui.Branding;
using Volo.Abp.DependencyInjection;

namespace MyEcommerce;

[Dependency(ReplaceServices = true)]
public class MyEcommerceBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<MyEcommerceResource> _localizer;

    public MyEcommerceBrandingProvider(IStringLocalizer<MyEcommerceResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
