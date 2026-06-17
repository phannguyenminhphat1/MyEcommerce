namespace MyEcommerce;

public static class MyEcommerceDomainErrorCodes
{
    // Product (1000x)
    public const string ProductNameAlreadyExists = "MyEcommerce:10001";
    public const string ProductCodeAlreadyExists = "MyEcommerce:10002";
    public const string ProductSkuAlreadyExists = "MyEcommerce:10003";
    public const string ProductNotFound = "MyEcommerce:10004";

    // Product Category (1010x)
    public const string ProductCategoryNotFound = "MyEcommerce:10101";
    public const string ProductCategoryInactive = "MyEcommerce:10102";

    // Product Attribute (1020x)
    public const string ProductAttributeNotFound = "MyEcommerce:10201";
    public const string ProductAttributeValueNotFound = "MyEcommerce:10202";
    public const string ProductAttributeValueInvalid = "MyEcommerce:10203";

    public const string ProductAttributeDateTimeNotFound = "MyEcommerce:10204";
    public const string ProductAttributeIntNotFound = "MyEcommerce:10205";
    public const string ProductAttributeDecimalNotFound = "MyEcommerce:10206";
    public const string ProductAttributeVarcharNotFound = "MyEcommerce:10207";
    public const string ProductAttributeTextNotFound = "MyEcommerce:10208";

    // Identity Role (2000x)
    public const string RoleNameAlreadyExists = "MyEcommerce:20001";

    // Identity User (2010x)
    public const string UserNotFound = "MyEcommerce:20101";
    public const string UserNameAlreadyExists = "MyEcommerce:20102";
    public const string UserEmailAlreadyExists = "MyEcommerce:20103";
    public const string UserPhoneNumberAlreadyExists = "MyEcommerce:20104";
    public const string UserIsInactive = "MyEcommerce:20105";
}