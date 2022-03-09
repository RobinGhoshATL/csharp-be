using ProSuite.Support.WebAPI.Provisions.Models;
using ProSuite.Support.WebAPI.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace System.Service.Control.PowershellScripting.Data
{
    public class PowerShellScriptingRepository
    {
        private readonly PSManager _psManager;


        public PowerShellScriptingRepository(PSManager psManager)
        {
            this._psManager = psManager;
        }



        public async Task<ProvisionsResponse> StepExecution(string path,string user)
        {
            var provisionMesssage = new List<PowerShellScriptingMessage>();
            var provisionResponse = new ProvisionsResponse();


            if (path.Substring(0, 5).Equals("https"))
            {
                var fileName = Path.GetRandomFileName().ToString() + ".ps1";
                var filePath = "Temp/" + fileName;
                try
                {
                    WebClient webClient = new WebClient();
                    webClient.DownloadFile(path, filePath);
                    path = filePath;
                }

                catch (Exception ex)
                {
                    provisionMesssage.Add(new PowerShellScriptingMessage(null,ex.ToString(), false));
                    provisionResponse.Response = provisionMesssage;
                    return provisionResponse;
                }

            }





            provisionMesssage = await TestScripts(path,user);
            provisionResponse.Response = provisionMesssage;
            provisionResponse.Success = provisionMesssage.All(kvp => kvp.Success == true);
            return provisionResponse;
        }


        public async Task<List<PowerShellScriptingMessage>> TestScripts(string path,string user)
        {
            string scriptName = string.Empty;
            string scriptPath = string.Empty;
            string scriptLabel = string.Empty;
            string scriptContents = string.Empty;
            var provisionMesssage = new List<PowerShellScriptingMessage>();
            Dictionary<string, string> scriptParamsOutput = new Dictionary<string, string>();
            Dictionary<string, string> scriptParameters = new Dictionary<string, string>();
            scriptParamsOutput.Add("MESSAGE", string.Empty);
            path = path.Replace(@"\", @"////");


            bool exists = false;

            if (exists = File.Exists(path))
            {
                PSScript psScript = PSScript.GetPSScriptDetails("TestPSScript", "Scripts");

              
                var userName = scriptParameters.Where(kvp => kvp.Key == "userName")
                 .Select(kvp => kvp.Value)
                 .FirstOrDefault();
                scriptParameters.Add("userName",user);
                scriptParameters.Add("step", "TestPSScript");
                scriptName = psScript.Name;
                scriptLabel = psScript.Label;
                scriptPath = path;
                scriptContents = GetFileContents(scriptPath);

                provisionMesssage.Add(await PowerShellExecute(scriptPath, scriptContents, scriptParameters, false));
                return provisionMesssage;
            }
            else
            {
                provisionMesssage.Add(new PowerShellScriptingMessage(null, "ERROR: Given script path not found", false));
                return provisionMesssage;

            }


            // End Test Steps
        }






        public async Task<PowerShellScriptingMessage> PowerShellExecute(string scriptPath, string scriptContents, Dictionary<string, string> scriptParameters, bool isDebug)
        {
            _psManager.InitializeRunspaces(2, 10);
            return await PowerShellExecuteScript(scriptPath, scriptContents, scriptParameters, isDebug);
        }

        public async Task<PowerShellScriptingMessage> PowerShellExecuteScript(string scriptPath, string scriptContents, Dictionary<string, string> scriptParameters, bool isDebug)
        {
            var msg = new PowerShellScriptingMessage();
            var psResult = await _psManager.RunScript(scriptContents, scriptParameters, isDebug);

            if (!string.IsNullOrEmpty(scriptPath))
            {
                psResult.FilePath = scriptPath;
                psResult.FileName = Path.GetFileName(scriptPath);

            }
            msg.Response = psResult;
            msg.Success = (psResult.ErrorMessage.Count > 0) ? false : true;
            return msg;
        }



        public static string GetFileContents(string path)
        {
            string contents = string.Empty;

            if (!string.IsNullOrEmpty(path))
            {
                // This text is added only once to the file.
                if (System.IO.File.Exists(path))
                    contents = System.IO.File.ReadAllText(path);
            }
            return contents;
        }
    }
}
