using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ProSuite.Support.WebAPI.Data;
using ProSuite.Support.WebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.SignalR;

namespace System.Service.Control.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class WindowsServiceControlController : ControllerBase
    {
        #region "Variables"
        private readonly ServiceControlRepository _repository;
        #endregion "Variables"

        #region "Constructor"
        public WindowsServiceControlController(ServiceControlRepository repository)
        {
            this._repository = repository ?? throw new ArgumentNullException(nameof(repository));
         
        }
        #endregion "Constructor"

        #region "Public Methods"
        [HttpGet]
        
        public async Task<List<SystemServiceControl>> GetServiceStatus(string cluster, string resourceNames)
        {
            var response = _repository.GetServiceStatus(cluster,resourceNames);
            return await response;
        }

       

     
        [Route("stopSystem")]
       
        public async Task<List<SystemServiceControl>> StopSystemService(string cluster, string resourceNames)
        {
            var response = _repository.StopSystemService(cluster, resourceNames);
            return await response;
        }

        [Route("restartSystem")]
      
        public async Task<List<SystemServiceControl>> RestartSystemService(string cluster,string resourceNames)
        {
            var response = _repository.RestartSystemService(cluster,resourceNames);
            return await response;
        }




        [Route("startSystem")]
       
        public async Task<List<SystemServiceControl>> StartSystemService(string cluster, string resourceNames)
        {
            var response = _repository.StartSystemService(cluster,resourceNames);
            return await response;
        }




        [Route("getAllServices")]
      
        public async Task<List<SystemServiceControl>> GetAllServices(string cluster)

        {
            
            var response = _repository.GetAllServices(cluster);        
            return await response;
        }

        

        [Route("getSystem")]
      
        public List<String>  GetMachineName()
        {
            List<string> response = new List<string>();
            response.Add(_repository.getMachineName());
            return response;
        }

        #endregion "Public Methods"
    }
}
