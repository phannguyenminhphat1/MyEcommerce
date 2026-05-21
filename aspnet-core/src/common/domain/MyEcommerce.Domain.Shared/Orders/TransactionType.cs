using System;
using System.Collections.Generic;
using System.Text;

namespace MyEcommerce.Orders
{
    public enum TransactionType
    {
        ConfirmOrder,
        StartProcessing,
        FinishOrder,
        CancelOrder
    }
}
