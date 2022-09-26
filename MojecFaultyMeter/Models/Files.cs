using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MojecFaultyMeter.Models
{
    public class Files
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ContentType { get; set; }
        public byte[] Data { get; set; }
    }
}