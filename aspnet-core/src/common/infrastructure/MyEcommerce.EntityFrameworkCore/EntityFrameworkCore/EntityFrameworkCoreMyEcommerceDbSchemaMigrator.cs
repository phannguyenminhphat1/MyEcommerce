using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyEcommerce.Data;
using Volo.Abp.DependencyInjection;

namespace MyEcommerce.EntityFrameworkCore;

public class EntityFrameworkCoreMyEcommerceDbSchemaMigrator
    : IMyEcommerceDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreMyEcommerceDbSchemaMigrator(
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolve the MyEcommerceDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<MyEcommerceDbContext>()
            .Database
            .MigrateAsync();
    }
}
