using ProSuite.Support.WebAPI.Hubs;
using ProSuite.Support.WebAPI.Migration.Models;
using ProSuite.Support.WebAPI.SignalR.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProSuite.Support.WebAPI.Services
{
    public class PSManager
    {
        #region "Variables"
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly PowerShellHub _hubContext;
        private List<string> _errors = new List<string>();
        private List<string> _warnings = new List<string>();
        #endregion "Variables"

        public PSManager(PowerShellHub hubContext)
        {
            _hubContext = hubContext;
        }

        /// <summary>
        /// The PowerShell runspace pool.
        /// </summary>
        private RunspacePool RsPool { get; set; }

        /// <summary>
        /// Initialize the runspace pool.
        /// </summary>
        /// <param name="minRunspaces"></param>
        /// <param name="maxRunspaces"></param>
        public void InitializeRunspaces(int minRunspaces, int maxRunspaces)
        {
            InitializeRunspaces(minRunspaces, maxRunspaces, null, null);
        }

        /// <summary>
        /// Initialize the runspace pool.
        /// </summary>
        /// <param name="minRunspaces"></param>
        /// <param name="maxRunspaces"></param>
        /// <param name="modulesToLoad"></param>
        /// <param name="importModulesFromPath"></param>
        public void InitializeRunspaces(int minRunspaces, int maxRunspaces, string[] modulesToLoad, List<string> importModulesFromPath)
        {
            // create the default session state.
            // session state can be used to set things like execution policy, language constraints, etc.
            // optionally load any modules (by name) that were supplied.

            var defaultSessionState = InitialSessionState.CreateDefault();
            defaultSessionState.ExecutionPolicy = Microsoft.PowerShell.ExecutionPolicy.Unrestricted;

            if (importModulesFromPath != null)
            {
                foreach (string moduleFromPath in importModulesFromPath)
                    defaultSessionState.ImportPSModulesFromPath(moduleFromPath);
            }

            if (modulesToLoad != null)
            {
                foreach (var moduleName in modulesToLoad)
                    defaultSessionState.ImportPSModule(moduleName);
            }

            // use the runspace factory to create a pool of runspaces
            // with a minimum and maximum number of runspaces to maintain.

            RsPool = RunspaceFactory.CreateRunspacePool(defaultSessionState);
            RsPool.SetMinRunspaces(minRunspaces);
            RsPool.SetMaxRunspaces(maxRunspaces);

            // set the pool options for thread use.
            // we can throw away or re-use the threads depending on the usage scenario.

            RsPool.ThreadOptions = PSThreadOptions.UseNewThread;

            // open the pool. 
            // this will start by initializing the minimum number of runspaces.

            RsPool.Open();
        }

        /// <summary>
        /// Runs a powershell script
        /// </summary>
        /// <param name="scriptContents"></param>
        /// <param name="scriptParameters"></param>
        /// <returns></returns>
        public async Task<PSResult> RunScript(string scriptContents, Dictionary<string, string> scriptParameters, bool isDebug = true)
        {
            var psResponse = new PSResult();
            var response = new List<string>();
            var errors = new List<string>();
            var warnings = new List<string>();

            if (!isDebug)
            {

                PSDataCollection<PSObject> pipelineObjects;
                scriptParameters.Add("stringOutput", "Out-String");


                pipelineObjects = await RunScript(scriptContents, scriptParameters);

                if (pipelineObjects.Count != 0)
                {
                    StringBuilder stringBuilder = new StringBuilder();

                    foreach (PSObject item in pipelineObjects)
                    {
                        stringBuilder.AppendLine(item.BaseObject.ToString());
                    }
                    response.Add(stringBuilder.ToString());
                }

                if (_errors.Count > 0)
                {
                    response = _errors;
                    errors = _errors;
                }

                if (_warnings.Count != 0)
                {
                    response = _warnings;
                    warnings = _warnings;
                }

                psResponse.scriptContent = scriptContents;
                psResponse.Response = response;
                psResponse.ErrorMessage = errors;
                psResponse.WarningMessage = warnings;

                _logger.Debug("PS: Response: {0}", response[0].ToString());

                return psResponse;
            }
            else
            {
                _logger.Debug("PS: ---------------------------------------------");
                _logger.Debug("PS: Script: {0}", scriptContents);
                _logger.Debug(":- ---------------------------------------------");
                response.Add("PS: Running in debug mode \n");
                psResponse.scriptContent = scriptContents;
                psResponse.Response = response;
                psResponse.ErrorMessage = errors;
                psResponse.WarningMessage = warnings;
                return psResponse;
            }
        }

        public async Task<PSDataCollection<PSObject>> RunScript(string scriptContents, Dictionary<string, string> scriptParameters)
        {
            var user = string.Empty;
            var step = string.Empty;
            var outString = string.Empty;
            scriptParameters.TryGetValue("userName", out user);
            scriptParameters.TryGetValue("step", out step);
            scriptParameters.TryGetValue("stringOutput", out outString);



                if (RsPool == null)
                {
                    throw new ApplicationException("Runspace Pool must be initialized before calling RunScript().");
                }

            using (System.Management.Automation.PowerShell ps = PowerShell.Create())
            {
                ps.RunspacePool = RsPool;

                ps.AddScript(scriptContents);

                scriptParameters.Remove("userName");
                scriptParameters.Remove("step");
                scriptParameters.Remove("stringOutput");

                ps.AddParameters(scriptParameters);

                if (!string.IsNullOrEmpty(outString))
                {
                    ps.AddCommand(outString);
                   
                }

                ps.Streams.Error.DataAdded += (s, e) => Error_DataAdded(s, e, user, step); // Error_DataAdded;
                ps.Streams.Warning.DataAdded += (s, e) => Warning_DataAdded(s, e, user, step);
                ps.Streams.Information.DataAdded += (s, e) => Information_DataAdded(s, e, user, step);
                ps.Streams.Progress.DataAdded += (s, e) => Progress_DataAdded(s, e, user, step);
                ps.Streams.Verbose.DataAdded += (s, e) => Verbose_DataAdded(s, e, user, step);

                PSDataCollection<PSObject> pipelineObjects;

                pipelineObjects = await Task.Run(async () => await ps.InvokeAsync());

                if (ps.Streams.Error.Count != 0)
                {
                    foreach (var error in ps.Streams.Error)
                    {
                        _errors.Add(error.ToString());
                    }
                }

                if (ps.Streams.Warning.Count != 0)
                {
                    foreach (var error in ps.Streams.Warning)
                    {
                        _warnings.Add(error.ToString());
                    }
                }

                return pipelineObjects;
            }
        }

        /// <summary>
        /// Handles data-added events for the information stream.
        /// </summary>
        /// <remarks>
        /// Note: Write-Host and Write-Information messages will end up in the information stream.
        /// </remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Information_DataAdded(object sender, DataAddedEventArgs e, string userName, string step)
        {
            var streamObjectsReceived = sender as PSDataCollection<InformationRecord>;
            var currentStreamRecord = streamObjectsReceived[e.Index];

            _logger.Debug($"PS: InfoStreamEvent: {currentStreamRecord.MessageData}");
            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(step))
            {
                var psData = new PSData();
                psData.Message = String.Format("INFO: {0}", currentStreamRecord.MessageData);
                psData.Receiverid = userName;
                psData.scriptName = step;
                _hubContext.SendMessage(psData);
            }
        }

        /// <summary>
        /// Handles data-added events for the warning stream.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Warning_DataAdded(object sender, DataAddedEventArgs e, string userName, string step)
        {
            var streamObjectsReceived = sender as PSDataCollection<WarningRecord>;
            var currentStreamRecord = streamObjectsReceived[e.Index];

            _logger.Debug($"PS: WarningStreamEvent: {currentStreamRecord.Message}");
            var psData = new PSData();
            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(step))
            {
                psData.Message = String.Format("WARNING: {0}", currentStreamRecord.Message);
                psData.Receiverid = userName;
                psData.scriptName = step;
                _hubContext.SendMessage(psData);
            }

        }

        /// <summary>
        /// Handles data-added events for the error stream.
        /// </summary>
        /// <remarks>
        /// Note: Uncaught terminating errors will stop the pipeline completely.
        /// Non-terminating errors will be written to this stream and execution will continue.
        /// </remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Error_DataAdded(object sender, DataAddedEventArgs e, string userName, string step)
        {
            var streamObjectsReceived = sender as PSDataCollection<ErrorRecord>;
            var currentStreamRecord = streamObjectsReceived[e.Index];

            _logger.Debug($"PS: ErrorStreamEvent: {currentStreamRecord.Exception}");
            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(step))
            {
                var psData = new PSData();
                psData.Message = String.Format("ERROR: {0}", currentStreamRecord.Exception);
                psData.Receiverid = userName;
                psData.scriptName = step;
                _hubContext.SendMessage(psData);
            }
        }

        private void Progress_DataAdded(object sender, DataAddedEventArgs eventargs, string userName, string step)
        {
            ProgressRecord newRecord = ((PSDataCollection<ProgressRecord>)sender)[eventargs.Index];
            _logger.Debug($"PS: Progress: {newRecord.PercentComplete}");
            _logger.Debug($"PS: current status: {newRecord.StatusDescription}");
            _logger.Debug($"PS: current Activity: {newRecord.Activity}");
            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(step))
            {
                var psData = new PSData();
                psData.Message = String.Format("Progress: {0} Current Status: {1} Current Activity: {2}", newRecord.PercentComplete, newRecord.StatusDescription, newRecord.Activity);
                psData.Percent = newRecord.PercentComplete;
                psData.Receiverid = userName;
                psData.scriptName = step;
                _hubContext.SendMessage(psData);
            }
        }

        private void Verbose_DataAdded(object sender, DataAddedEventArgs eventargs, string userName, string step)
        {
            VerboseRecord newRecord = ((PSDataCollection<VerboseRecord>)sender)[eventargs.Index];
            _logger.Debug($"PS: Verbose: {newRecord.Message}");
            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(step))
            {
                var psData = new PSData();
                psData.Message = String.Format("Verbose: {0}", newRecord.Message);
                psData.Receiverid = userName;
                psData.scriptName = step;
                _hubContext.SendMessage(psData);
            }
        }
    };

}
