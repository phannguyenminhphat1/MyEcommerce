using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using MyEcommerce.Orders;
using Volo.Abp.Application.Dtos;

namespace MyEcommerce.Public.Orders
{
    public class OrderDto : EntityDto<Guid>
    {
        public string? Code { get; set; }
        public OrderStatus Status { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public double ShippingFee { get; set; }
        public double Tax { get; set; }
        public double Total { get; set; }
        public double Subtotal { get; set; }
        public double Discount { get; set; }
        public double GrandTotal { get; set; }
        [Required(ErrorMessage = "Customer name is required.")]
        public string CustomerName { get; set; }
        [Required(ErrorMessage = "Customer phone number is required.")]
        public string CustomerPhoneNumber { get; set; }
        [Required(ErrorMessage = "Customer address is required.")]
        public string CustomerAddress { get; set; }
        public Guid? CustomerUserId { get; set; }
    }
}
