using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace MyEcommerce.EntityFrameworkCore;

/* This class is needed for EF Core console commands
 * (like Add-Migration and Update-Database commands) */
public class MyEcommerceDbContextFactory : IDesignTimeDbContextFactory<MyEcommerceDbContext>
{
    public MyEcommerceDbContext CreateDbContext(string[] args)
    {
        MyEcommerceEfCoreEntityExtensionMappings.Configure();

        var configuration = BuildConfiguration();

        var builder = new DbContextOptionsBuilder<MyEcommerceDbContext>()
            .UseSqlServer(configuration.GetConnectionString("Default"));

        return new MyEcommerceDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../MyEcommerce.DbMigrator/"))
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}
