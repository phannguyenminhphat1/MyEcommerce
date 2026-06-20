using MyEcommerce.EntityFrameworkCore;
using MyEcommerce.Public;
using Volo.Abp.Autofac;
using Volo.Abp.Caching;
using Volo.Abp.Caching.StackExchangeRedis;
using Volo.Abp.Modularity;

namespace MyEcommerce.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(AbpCachingStackExchangeRedisModule),
    typeof(MyEcommerceEntityFrameworkCoreModule),
    typeof(MyEcommercePublicApplicationContractsModule)
    )]
public class MyEcommerceDbMigratorModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpDistributedCacheOptions>(options => { options.KeyPrefix = "MyEcommerce:"; });
    }
}
