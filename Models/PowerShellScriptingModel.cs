using System.Collections.Generic;
using System.Data;
using System.Xml;

namespace ProSuite.Support.WebAPI.Provisions.Models
{
    public class PowerShellScriptingModel
    {
        //public string[] Clients { get; set; }
        public string Source { get; set; }
        ////public string Destination { get; set; }
        ////public XmlDocument ClientXml { get; set; }
    }
   
    public class ProvisionConfiguration
    {
        public Dictionary<string, string> Defaults { get; set; }
        public Dictionary<string, string> ConnectionStrings { get; set; }
        public Dictionary<string, string> ScriptsPath { get; set; }
        public string ErrorResponse { get; set; }

    }

    public class ProvisionSteps
    {
        public List<Scripts> Scripts { get; set; }
    }

    public class Scripts
    {
        public string Name { get; set; }
        public string Label { get; set; }
        public bool IsDisabled { get; set; }
        public string Script { get; set; }
        public string CommandType { get; set; }
    }

   

}
