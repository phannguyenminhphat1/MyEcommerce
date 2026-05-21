using MyEcommerce.Samples;
using Xunit;

namespace MyEcommerce.EntityFrameworkCore.Applications;

[Collection(MyEcommerceTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<MyEcommerceEntityFrameworkCoreTestModule>
{

}
