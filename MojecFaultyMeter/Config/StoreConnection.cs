using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace MojecFaultyMeter.Config
{
    public class StoreConnection
    {
       
            public static string GetConnection()
            {
                return ConfigurationManager.ConnectionStrings["MojecFaultyMeter"].ConnectionString;

            }
        
    }
}