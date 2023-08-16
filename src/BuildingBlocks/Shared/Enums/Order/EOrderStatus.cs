﻿namespace Shared.Enums.Order;

public enum EOrderStatus
{
    New = 1, //start with 1,0 is used for filter All = 0
    Pending, // order is pending, not activeties for a period time.
    Paid, // order is paid
    Shipping, // order is on the shipping
    Fulfulled, //order is on fulfilled
}
