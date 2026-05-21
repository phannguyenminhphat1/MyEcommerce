using Volo.Abp.Modularity;

namespace MyEcommerce;

[DependsOn(
    typeof(MyEcommerceApplicationModule),
    typeof(MyEcommerceDomainTestModule)
)]
public class MyEcommerceApplicationTestModule : AbpModule
{

}
