using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Kanapson.Models
{
    public class Order
    {
        public int Id { get; set; }

        public virtual List<Product_Order> Products_order { get; set; }

        public DateTime orderTimes { get; set; }

        public virtual User User { get; set; }

        public double Sum { get; set; }

        public string Status { get; set; }

        public string Payment { get; set; }
    }
}
