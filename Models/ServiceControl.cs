using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProSuite.Support.WebAPI.Models
{
    public class ServiceControl
    {
        public string serviceName { get; set; }
        public string machineNames { get; set; }
        public string status { get; set; }
        public string action { get; set; }
        public int timeout { get; set; }
        public string caseNumber { get; set; }
        public string clientId { get; set; }
        public string user { get; set; }



    }
}
