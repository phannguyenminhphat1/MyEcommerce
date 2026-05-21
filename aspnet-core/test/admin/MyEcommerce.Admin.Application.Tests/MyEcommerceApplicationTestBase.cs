using Volo.Abp.Modularity;

namespace MyEcommerce.Admin;

public abstract class MyEcommerceApplicationTestBase<TStartupModule> : MyEcommerceTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
