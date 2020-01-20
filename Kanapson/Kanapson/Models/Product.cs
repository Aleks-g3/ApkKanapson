using System;
using System.Collections.Generic;
using System.Text;

namespace Kanapson.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public ushort Amount { get; set; }
    }
}
