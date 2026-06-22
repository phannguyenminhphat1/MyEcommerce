using Volo.Abp.Application.Dtos;

namespace MyEcommerce.Public
{
    public class BaseListFilterDto : PagedResultRequestDto
    {
        public string? Keyword { get; set; }
    }
}