using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEcommerce.Orders.Events
{
    public class NewOrderCreatedEvent
    {
        public string CustomerEmail { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}