using Volo.Abp.Modularity;

namespace MyEcommerce.Admin;

[DependsOn(
    typeof(MyEcommerceAdminApplicationModule),
    typeof(MyEcommerceDomainTestModule)
)]
public class MyEcommerceApplicationTestModule : AbpModule
{

}
