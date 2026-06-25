namespace MyEcommerce.Public
{
    public class BaseListFilterDto : PagedResultRequestBase
    {
        public string? Keyword { get; set; }
    }
}