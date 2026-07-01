using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyEcommerce.Products;
using MyEcommerce.Repositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace MyEcommerce.EntityFrameworkCore.Repositories
{
  public class ProductRepository : EfCoreRepository<MyEcommerceDbContext, Product, Guid>, IProductRepository
  {
    public ProductRepository(IDbContextProvider<MyEcommerceDbContext> dbContextProvider) : base(dbContextProvider) { }

    public async Task<long> GetCountAsync(string? keyword, Guid? categoryId)
    {
      var dbSet = await GetDbSetAsync();
      return await dbSet.WhereIf(!string.IsNullOrWhiteSpace(keyword), x => x.Name.Contains(keyword!)).WhereIf(categoryId.HasValue, x => x.CategoryId == categoryId).LongCountAsync();
    }

    public async Task<List<Product>> GetListFilterAsync(string? keyword, Guid? categoryId, int skipCount, int maxResultCount)
    {
      var dbSet = await GetDbSetAsync();
      return await dbSet.Include(x => x.Category)
          .WhereIf(!string.IsNullOrWhiteSpace(keyword), x => x.Name.Contains(keyword!))
          .WhereIf(categoryId.HasValue, x => x.CategoryId == categoryId)
          .Skip(skipCount)
          .Take(maxResultCount)
          .OrderByDescending(x => x.CreationTime)
          .ToListAsync();
    }

    public async Task<Product?> GetBySlugWithCategoryAsync(string slug)
    {
      var dbSet = await GetDbSetAsync();
      return await dbSet.Include(x => x.Category)
          .FirstOrDefaultAsync(x => x.Slug == slug);
    }

    public async Task<Product?> GetByIdWithCategoryAsync(Guid id)
    {
      var dbSet = await GetDbSetAsync();
      return await dbSet.Include(x => x.Category)
          .FirstOrDefaultAsync(x => x.Id == id);
    }
  }
}
