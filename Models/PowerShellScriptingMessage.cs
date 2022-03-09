using ProSuite.Support.WebAPI.Migration.Models;
using System.Collections.Generic;

namespace ProSuite.Support.WebAPI.Provisions.Models
{
    public class PowerShellScriptingMessage
    {
        public PowerShellScriptingMessage() { }
        public PSResult Response { get; set; }
        public string Message { get; set; }
        public string ClientId { get; set; }
        public string ClientName { get; set; }
        public bool Success { get; set; }
        public PowerShellScriptingMessage(PSResult response, string message, bool success) { Response = response; Message =message; Success = success; }
    }

    public class ProvisionsResponse
    {
        public List<PowerShellScriptingMessage> Response { get; set; }

        public bool Success { get; set; }

    }
}
