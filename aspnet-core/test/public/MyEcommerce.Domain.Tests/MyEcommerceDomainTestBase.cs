using Volo.Abp.Modularity;

namespace MyEcommerce;

/* Inherit from this class for your domain layer tests. */
public abstract class MyEcommerceDomainTestBase<TStartupModule> : MyEcommerceTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
