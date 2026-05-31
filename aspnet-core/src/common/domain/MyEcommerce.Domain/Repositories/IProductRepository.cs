using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyEcommerce.Products;
using Volo.Abp.Domain.Repositories;

namespace MyEcommerce.Repositories
{
    public interface IProductRepository : IRepository<Product, Guid>
    {
        Task<List<Product>> GetListFilterAsync(
            string? keyword,
            Guid? categoryId,
            int skipCount,
            int maxResultCount
        );
        Task<long> GetCountAsync(
            string? keyword,
            Guid? categoryId
        );
    }
}