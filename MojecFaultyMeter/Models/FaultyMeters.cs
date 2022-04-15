using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MojecFaultyMeter.Models
{
    public class FaultyMeters
    {
        public int MeterID { get; set; }
        [Display(Name = "Meter No")]
        public string MeterNo { get; set; }
        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }
        [Display(Name = "Account No")]
        public string AccountNo { get; set; }
        public int DiscoID { get; set; }
        [Display(Name = "Tariff")]
        public string Tarriff { get; set; }
        [Display(Name = "Meter Type")]
        public string MeterType { get; set; }
        [Display(Name = "Model")]
        public string Model { get; set; }
        [Display(Name = "Fault")]
        public string Fault { get; set; }
        [Display(Name = "Status")]
        public string Status { get; set;}
        [Display(Name = "IsReplaced")]
        public string Replacementstat { get; set; }
        [Display(Name = "Replacement NO")]
        public string MeterReplacementNo { get; set; }
        [Display(Name = "Returned Date")]
        [DataType(DataType.Date)]
        public string ReturnDate { get; set; }
        public int StoreUserID { get; set; }
        public int FactoryUserID { get; set; }
        public int DiscoUserID { get; set; }
        public string DispatchedDate { get; set; }
        [Display(Name = "WorkOrder")]
        public string WorkOrderID { get; set; }

        [Display(Name = "Store User")]
        public string AcceptedBy { get; set; }
        [Display(Name = "Factory User")]
        public string TreatedBy { get; set; }
        [Display(Name = "Disco User")]
        public string DiscoUser { get; set; }

        [Display(Name = "Disco")]
        public string Disco { get; set; }

        [Display(Name = "Faulty Comment")]
        public string Faultcomment { get; set; }    

        public string UserID { get; set; }
        [Display(Name = "Rejection Comment")]
        public string Rejectcomment { get; set; }
        Random random = new Random();

        public FaultyMeters()
        {
            WorkOrderID = Convert.ToString((long)Math.Floor(random.NextDouble() * 9_000_000_000L + 1_000_000_000L));
            //we did this to generate randome 9  digit number
           
        }




    }
}