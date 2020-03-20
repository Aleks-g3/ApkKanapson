using System;
using System.Collections.Generic;
using System.Text;

namespace Kanapson.Models
{
    public class Product_Order
    {
        public int Id { get; set; }

        public virtual Product product { get; set; }

        public ushort count { get; set; }

        public double PriceEach { get; set; }
    }
}
