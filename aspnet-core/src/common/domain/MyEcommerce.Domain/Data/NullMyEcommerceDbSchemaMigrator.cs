using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace MyEcommerce.Data;

/* This is used if database provider does't define
 * IMyEcommerceDbSchemaMigrator implementation.
 */
public class NullMyEcommerceDbSchemaMigrator : IMyEcommerceDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
