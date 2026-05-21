using System.Threading.Tasks;

namespace MyEcommerce.Data;

public interface IMyEcommerceDbSchemaMigrator
{
    Task MigrateAsync();
}
