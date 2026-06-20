using Volo.Abp.Modularity;

namespace MyEcommerce.Public;

[DependsOn(
    typeof(MyEcommercePublicApplicationModule),
    typeof(MyEcommerceDomainTestModule)
)]
public class MyEcommercePublicApplicationTestModule : AbpModule
{

}
