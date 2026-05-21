using Volo.Abp.Modularity;

namespace MyEcommerce;

public abstract class MyEcommerceApplicationTestBase<TStartupModule> : MyEcommerceTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
