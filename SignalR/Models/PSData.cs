using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProSuite.Support.WebAPI.SignalR.Models
{
    public class PSData
    {
        public string Connectionid { get; set; }
        public string Senderid { get; set; }
        public string Receiverid { get; set; }
        public string scriptName { get; set; }
        public string Message { get; set; }
        public int Percent { get; set; }
    }
}
