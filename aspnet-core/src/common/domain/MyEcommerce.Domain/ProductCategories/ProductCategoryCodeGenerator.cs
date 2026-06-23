using System.Threading.Tasks;
using MyEcommerce.IdentitySettings;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace MyEcommerce.ProductCategories
{
    public class ProductCategoryCodeGenerator : ITransientDependency
    {
        private readonly IRepository<IdentitySetting, string> _identitySettingRepository;
        public ProductCategoryCodeGenerator(IRepository<IdentitySetting, string> identitySettingRepository)
        {
            _identitySettingRepository = identitySettingRepository;
        }
        public async Task<string> GenerateAsync()
        {
            string newCode;
            var identitySetting = await _identitySettingRepository.FindAsync(MyEcommerceConsts.ProductCategoryIdentitySettingId);
            if (identitySetting == null)
            {
                identitySetting = await _identitySettingRepository.InsertAsync(new IdentitySetting(MyEcommerceConsts.ProductCategoryIdentitySettingId, "ProductCategory", MyEcommerceConsts.ProductCategoryIdentitySettingPrefix, 1, 1));
                newCode = identitySetting.Prefix + identitySetting.CurrentNumber;
            }
            else
            {
                identitySetting.CurrentNumber += identitySetting.StepNumber;
                newCode = identitySetting.Prefix + identitySetting.CurrentNumber;
                await _identitySettingRepository.UpdateAsync(identitySetting);
            }
            return newCode;
        }
    }
}