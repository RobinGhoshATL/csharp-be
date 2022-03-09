using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Management;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using ProSuite.Support.WebAPI.DB;
using ProSuite.Support.WebAPI.Services;
using System.Management.Automation;
using Newtonsoft.Json.Linq;
using ProSuite.Support.WebAPI.Models;

namespace ProSuite.Support.WebAPI.Data
{
    public class ServiceControlRepository
    {
        #region "Variables"

        private IConfiguration _configuration;
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

      

        private string _systemName = null;
     
        #endregion "Variables"

        #region "Constructor"

        public ServiceControlRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #endregion "Constructor"

        #region "Public Methods"
    

    public async Task<List<SystemServiceControl>> GetServiceStatus(string clusterName,string resourceNames)
        {
            _systemName = clusterName;
           


            try
            {
                var response = new List<SystemServiceControl>();

                using (ServiceWmiManager svc = new ServiceWmiManager(_systemName, _configuration))
                {

                    ServiceConnection(svc);
                  

                    //Get Service Names List
                    String[] resources = resourceNames.Split(",");

                    foreach (string resource in resources)
                    {
                        string resourcename = string.Empty;
                        string content = resource.ToString();

                        // get resource by content   
                      

                        _logger.Info($"Service: Get resource state...ResourceName={resource}");
                        try
                        {
                            //Get States to the service
                            await Task.Run(() => response.Add(UpdateServiceColor(svc.GetResourceState(resource), resource.ToString())));
                        }
                        catch
                        {
                            response.Add(UpdateServiceColor(ServiceWmiManager.ServiceResourceState.Unknown, resource.ToString()));
                        }
                    }

                }
                _logger.Info($"Service: Get resource state complete...Response={response}");
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error("Service: Get resource state exception: ", ex.Message.ToString());
                throw;
            }

        }



        public   async Task<List<SystemServiceControl>> GetAllServices(string clusterName)
        {
            _systemName = clusterName;
           


            try
            {
                var response = new List<SystemServiceControl>();

                using (ServiceWmiManager svc = new ServiceWmiManager(_systemName, _configuration))
                {

                    ServiceConnection(svc);

                    svc.GetAllServices();
                   
                        ServiceController[] scServices;
                        scServices = ServiceController.GetServices();

                        foreach (ServiceController scTemp in scServices)
                        {
                        try
                        {
                            await Task.Run(() => response.Add(UpdateServiceColor(svc.GetResourceState(scTemp.ServiceName.ToString()), scTemp.ServiceName.ToString())));
                        }
                        catch
                        {
                            response.Add(UpdateServiceColor(ServiceWmiManager.ServiceResourceState.Unknown, scTemp.ServiceName.ToString()));
                        }
                    }



                   
                }
                _logger.Info($"Service: Get resource state complete...Response={response}");
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error("Service: Get resource state exception: ", ex.Message.ToString());
                throw;
            }

        }
      
        public async Task<List<SystemServiceControl>> RestartSystemService(string svcName,string resourceNames)
        {
            _systemName = svcName;
           

           

            var response = new List<SystemServiceControl>();

            using (ServiceWmiManager svc = new ServiceWmiManager(svcName))
            {
                ServiceConnection(svc);

                string resourcename = string.Empty;

                String[] resources = resourceNames.Split(",");

                foreach (string resource in resources)
                {
                    var state = svc.GetResourceState(resource.ToString());
                    if (state.ToString().Equals("Running"))
                    {
                        await Task.Run(() => svc.RestartService(resource));
                        response.Add(new SystemServiceControl() { ServiceName = resource.ToString(), status = state.ToString(), message = "Restarted Successfully", icon = "cancel" });
                    }
                    else if (state.ToString().Equals("Unknown"))
                    {
                        response.Add(new SystemServiceControl() { ServiceName = resource.ToString(), status = state.ToString(), message = "Service Not Found", icon = "cancel" });
                    }
                    else
                    {
                        response.Add(new SystemServiceControl() { ServiceName = resource.ToString(), status = state.ToString(), message = "Service needs to be in running state", icon = "cancel" });
                    }
                }

            }



            return response;

        }


        public async Task<List<SystemServiceControl>> StopSystemService(string svcName,string resourceNames)
        {
            _systemName = svcName;
           

         
            var response = new List<SystemServiceControl>();

            using (ServiceWmiManager svc = new ServiceWmiManager(svcName))
            {
                ServiceConnection(svc);

                string resourcename = string.Empty;

                String[] resources = resourceNames.Split(",");

                foreach (string resource in resources)
                {
                    var state = svc.GetResourceState(resource.ToString());
                    if (state.ToString().Equals("Running"))
                    {
                        await Task.Run(() => svc.StopService(resource));
                        state = svc.GetResourceState(resource.ToString());
                        response.Add(new SystemServiceControl() { ServiceName = resource.ToString(), status = state.ToString(), message = "Stopped Successfully", icon = "cancel" });
                    }
                    else if (state.ToString().Equals("Unknown"))
                    {
                        response.Add(new SystemServiceControl() { ServiceName = resource.ToString(), status = state.ToString(), message = "Service Not Found", icon = "cancel" });
                    }
                    else
                    {
                        response.Add(new SystemServiceControl() { ServiceName = resource.ToString(), status = state.ToString(), message = "Service needs to be in running state", icon = "cancel" });
                    }
                }

            }



            return response;

        }

        public string getMachineName() {

            try
            {
                return Environment.MachineName.ToString();
            }

            catch {
                return "Some Error Occured. Probably check permissions";
            }

        }

        public async Task<List<SystemServiceControl>> StartSystemService(string svcName, string resourceNames)
        {
            _systemName = svcName;
           
            var response = new List<SystemServiceControl>();

            using (ServiceWmiManager svc = new ServiceWmiManager(svcName))
            {
                ServiceConnection(svc);

                string resourcename = string.Empty;

                String[] resources = resourceNames.Split(",");

                foreach (string resource in resources)
                {
                    var state = svc.GetResourceState(resource.ToString());
                    if (state.ToString().Equals("Stopped"))
                    {
                        await Task.Run(() => svc.StartService(resource));
                        state = svc.GetResourceState(resource.ToString());
                        response.Add(new SystemServiceControl() { ServiceName = resource.ToString(), status = state.ToString(), message = "Started Successfully", icon = "cancel" });
                    }
                    else if (state.ToString().Equals("Unknown"))
                    {
                        response.Add(new SystemServiceControl() { ServiceName = resource.ToString(), status = state.ToString(), message = "Service Not Found", icon = "cancel" });
                    }
                    else
                    {
                        response.Add(new SystemServiceControl() { ServiceName = resource.ToString(), status = state.ToString(), message = "Service needs to be in stopped state", icon = "cancel" });
                    }
                }

            }



            return response;

        }




        #endregion "Public Methods"

        #region "Private Methods"


        private SystemServiceControl UpdateServiceColor(ServiceWmiManager.ServiceResourceState state, string serviceName)
        {
            switch (state)
            {
                case ServiceWmiManager.ServiceResourceState.Stopped:
                    return new SystemServiceControl() { ServiceName = serviceName.ToString(), color = "Brown", status = "Stopped", icon = "cancel" };
                case ServiceWmiManager.ServiceResourceState.ContinuePending:
                    return new SystemServiceControl() { ServiceName = serviceName.ToString(), color = "Brown", status = "Failed", icon = "cancel" };
                case ServiceWmiManager.ServiceResourceState.StopPending:
                    return new SystemServiceControl() { ServiceName = serviceName.ToString(), color = "Orange", status = "Stopping", icon = "autorenew" };
                case ServiceWmiManager.ServiceResourceState.Running:
                    return new SystemServiceControl() { ServiceName = serviceName.ToString(), color = "Green", status = "Running", icon = "check_circle" };
                case ServiceWmiManager.ServiceResourceState.StartPending:
                    return new SystemServiceControl() { ServiceName = serviceName.ToString(), color = "blue", status = "Starting", icon = "autorenew" };
                case ServiceWmiManager.ServiceResourceState.Unknown:
                    return new SystemServiceControl() { ServiceName = serviceName.ToString(), color = "Gray", status = "UnKnown", icon = "help_outline" };
            }
            return new SystemServiceControl() { ServiceName = serviceName.ToString(), color = "Brown", status = "Stopped", icon = "cancel" };
        }




        private void ServiceConnection(ServiceWmiManager service)
        {

            if (service.Machine == Environment.MachineName || service.Machine == "localhost")
            {
                service.Authentication = AuthenticationLevel.None;
                service.Connect();
            }
            else
            {
                service.Authentication = AuthenticationLevel.PacketPrivacy;
                service.Username = _configuration.GetSection("SystemServiceConfiguration:UserName").Value;
                service.Password = _configuration.GetSection("SystemServiceConfiguration:Password").Value;
                service.Domain = _configuration.GetSection("SystemServiceConfiguration:IP").Value;
                service.Connect();
            }
        }




        #endregion "Private Methods"
    }
}