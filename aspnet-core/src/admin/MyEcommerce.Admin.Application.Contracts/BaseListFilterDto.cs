using Volo.Abp.Application.Dtos;

namespace MyEcommerce.Admin
{
    public class BaseListFilterDto : PagedResultRequestDto
    {
        public string? Keyword { get; set; }
    }
}