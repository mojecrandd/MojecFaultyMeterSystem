using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MojecFaultyMeter.Models
{
    public class Discos
    {
        public int DiscoID { get; set; }

        [Display(Name = "Disco")]
        public string DiscoName { get; set; }

        [Display(Name = "AB")]
        public string DiscoAB { get; set; } 


    }
}