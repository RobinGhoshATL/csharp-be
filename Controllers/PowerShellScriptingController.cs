using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProSuite.Support.WebAPI.Provisions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Service.Control.PowershellScripting.Data;
using System.Threading;
using System.Threading.Tasks;

namespace System.Service.Control.Controller
{
    [Route("api/[controller]")]
    [Authorize] //(Policy = "RequireAdministratorRole")]
    [ApiController]

    public class PowerShellScriptingController : ControllerBase
    {

        private readonly PowerShellScriptingRepository _repository;

        public PowerShellScriptingController(PowerShellScriptingRepository repository)
        {

            this._repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        [Route("powershellscript")]
        [HttpGet]
        public async Task<ProvisionsResponse> RunPSScript(string path, [FromHeader] string User)
        {

            try
            {
                var response = await _repository.StepExecution(path,User);

                return response;
            }
            catch (Exception ex) when (ex is TaskCanceledException || ex is OperationCanceledException)
            {

                throw new Exception();

            }
        }
    }
}
