using Volo.Abp.Modularity;

namespace MyEcommerce.Public;

public abstract class MyEcommercePublicApplicationTestBase<TStartupModule> : PublicTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
