using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MojecFaultyMeter.Models
{
    public class DispatchOrders
    {
        public int DispatchID { get; set; }
        public string DispatchWorkOrder { get; set; }
        public DateTime DispatchDate { get; set; }
        public int MeterID { get; set; }
        public int DiscoID { get; set; }

        public string Disco { get; set; }



    }
}