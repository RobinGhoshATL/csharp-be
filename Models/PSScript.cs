using Microsoft.Extensions.Configuration;
using System;
using System.Linq;

namespace ProSuite.Support.WebAPI.Provisions.Models
{
    public class PSScript
    {
        #region "Constructor"
        private PSScript(string name, string label, string path) { Name = name; Label = label; Path = path; }
        #endregion "Constructor"

        public string Name { get; set; }
        public string Path { get; set; }
        public string Label { get; set; }

        #region "Public Methods"
        public static PSScript GetPSScriptDetails(string stepName, string sectionName)
        {

            var name = string.Empty;
            var label = string.Empty;
            var script = string.Empty;

          

            try
            {
               

                if (sectionName == "Scripts")
                {
                    name = "TestPSScript";
                    label = "Test PS Script";
                    script = "PowershellScripting////Script////get-api.ps1";
                }

                return new PSScript(name, label, script);
            }
            catch (Exception)
            {
                throw new Exception(string.Format("PS: Script does not exist, Step Name: {0}, Section Name: {1} ", stepName, sectionName));
            }


        }
        #endregion " #endregion "Public Methods"
    }
}
