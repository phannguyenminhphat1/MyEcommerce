using Xunit;

namespace MyEcommerce.EntityFrameworkCore;

[CollectionDefinition(MyEcommerceTestConsts.CollectionDefinitionName)]
public class MyEcommerceEntityFrameworkCoreCollection : ICollectionFixture<MyEcommerceEntityFrameworkCoreFixture>
{

}
