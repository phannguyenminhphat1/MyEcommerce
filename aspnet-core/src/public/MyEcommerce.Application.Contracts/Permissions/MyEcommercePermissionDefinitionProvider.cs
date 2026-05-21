using MyEcommerce.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace MyEcommerce.Permissions;

public class MyEcommercePermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(MyEcommercePermissions.GroupName);
        //Define your own permissions here. Example:
        //myGroup.AddPermission(MyEcommercePermissions.MyPermission1, L("Permission:MyPermission1"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<MyEcommerceResource>(name);
    }
}
