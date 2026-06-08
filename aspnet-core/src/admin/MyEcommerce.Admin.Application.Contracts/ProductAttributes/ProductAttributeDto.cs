using System;
using System.Collections.Generic;
using System.Text;
using MyEcommerce.ProductAttributes;
using Volo.Abp.Application.Dtos;

namespace MyEcommerce.Admin.ProductAttributes
{
    public class ProductAttributeDto : IEntityDto<Guid>
    {
        public string Code { get; set; }
        public AttributeType DataType { get; set; }
        public string Label { get; set; }
        public int SortOrder { get; set; }
        public bool Visibility { get; set; }
        public bool IsActive { get; set; }
        public bool IsRequired { get; set; }
        public bool IsUnique { get; set; }
        public string Note { get; set; }
        public Guid Id { get; set; }

        public string? GetObjectKey()
        {
            return Id.ToString();
        }
    }
}
