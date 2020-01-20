using System;
using System.Collections.Generic;
using System.Text;

namespace Kanapson.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public double Credit { get; set; }
    }
}
