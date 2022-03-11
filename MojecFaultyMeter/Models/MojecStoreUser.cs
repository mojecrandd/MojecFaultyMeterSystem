using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MojecFaultyMeter.Models
{
    public class MojecStoreUser
    {
        public int UserID { get; set; }
        public string Fullname { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
    }
}