using Volo.Abp.Modularity;

namespace MyEcommerce;

[DependsOn(
    typeof(MyEcommerceDomainModule),
    typeof(MyEcommerceTestBaseModule)
)]
public class MyEcommerceDomainTestModule : AbpModule
{

}
