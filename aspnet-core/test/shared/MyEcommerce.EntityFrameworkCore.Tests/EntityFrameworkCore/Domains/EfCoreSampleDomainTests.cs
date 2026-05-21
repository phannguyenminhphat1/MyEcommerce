using MyEcommerce.Samples;
using Xunit;

namespace MyEcommerce.EntityFrameworkCore.Domains;

[Collection(MyEcommerceTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<MyEcommerceEntityFrameworkCoreTestModule>
{

}
