using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MojecFaultyMeter.Models
{
    public class FaultyMeters
    {
        public int MeterID { get; set; }
        public string MeterNo { get; set; }
        public string CustomerName { get; set; }
        public string AccountNo { get; set; }
        public int DiscoID { get; set; }
        public string Tarriff { get; set; }
        public string MeterType { get; set; }
        public string Model { get; set; }
        public string Fault { get; set; }
        public string Status { get; set;}
        public string Replacementstat { get; set; }
        public string MeterReplacementNo { get; set; }
        public DateTime ReturnDate { get; set; }
        public int StoreUserID { get; set; }
        public int FactoryUserID { get; set; }
        public int DiscoUserID { get; set; }
        public DateTime DispatchedDate { get; set; }
        public string WorkOrderID { get; set; }
        public string AcceptedBy { get; set; }
        public string TreatedBy { get; set; }
        public string DiscoUser { get; set; }
        public string Disco { get; set; }

        public string UserID { get; set; }  

        Random random = new Random();

        public FaultyMeters()
        {
            WorkOrderID = Convert.ToString((long)Math.Floor(random.NextDouble() * 9_000_000_000L + 1_000_000_000L));
            //we did this to generate randome 9  digit number
           
        }




    }
}