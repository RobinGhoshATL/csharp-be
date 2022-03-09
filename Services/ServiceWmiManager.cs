using System.Management;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Service.Control.Properties;
using Custom;

namespace ProSuite.Support.WebAPI.Services
{
    public class ServiceWmiManager : IDisposable
    {
        #region "Variables"
        private IConfiguration _configuration;
        #endregion "Variables"

        #region "Enums"
        /// <summary>
        /// Encapsulates Cluster Resource States
        /// </summary>
        /// 
        public enum ServiceStatus
        {
            Success = 0,
            NotSupported = 1,
            AccessDenied = 2,
            DependentServicesRunning = 3,
            InvalidServiceControl = 4,
            ServiceCannotAcceptControl = 5,
            ServiceNotActive = 6,
            ServiceRequestTimeout = 7,
            UnknownFailure = 8,
            PathNotFound = 9,
            ServiceAlreadyRunning = 10,
            ServiceDatabaseLocked = 11,
            ServiceDependencyDeleted = 12,
            ServiceDependencyFailure = 13,
            ServiceDisabled = 14,
            ServiceLogonFailure = 15,
            ServiceMarkedForDeletion = 16,
            ServiceNoThread = 17,
            StatusCircularDependency = 18,
            StatusDuplicateName = 19,
            StatusInvalidName = 20,
            StatusInvalidParameter = 21,
            StatusInvalidServiceAccount = 22,
            StatusServiceExists = 23,
            ServiceAlreadyPaused = 24,
            ServiceNotFound = 25
        }


        public enum ServiceState
        {
            Running,
            Stopped,
            Paused,
            StartPending,
            StopPending,
            PausePending,
            ContinuePending
        }

        public enum ServiceResourceState
        {
            Unknown,
            Running,
            Stopped,
            Paused,
            StartPending,
            StopPending,
            PausePending,
            ContinuePending
        }
        #endregion "Enums"

        #region "Properties"
        /// <summary>
        /// If true ClusterManager is connected in scope Cluster WMI. 
        /// </summary>
        public bool IsConnected { get; internal set; }


        /// <summary>
        /// Scope WMI
        /// </summary>
        /// <remarks>
        /// Default vale :  "root\\MSCluster"
        /// </remarks>
        public string Scope { get; internal set; }

        /// <summary>
        /// Domain where cluster is running
        /// </summary>
        /// <remarks>
        /// Default value : Environment.UserDomainName
        /// </remarks>
        public string Domain { get; set; }


        private string _machine;
        /// <summary>
        /// Machine Name or Cluster name
        /// </summary>
        /// <remarks>
        /// Default value :  Environment.MachineName
        /// </remarks>
        public string Machine
        {
            get { return _machine; }
            set
            {
                _machine = value;


                if (_machine == Environment.MachineName)
                {
                    Scope = ConfigurationManager.AppSetting["SystemServiceConfiguration:resource:LocalServiceScope"];
                }
                else
                {
                    Scope = string.Format(ConfigurationManager.AppSetting["SystemServiceConfiguration:resource:FormatServiceManagementScope"], Machine);
                    ///Scope = string.Format(_configuration.GetSection("SystemServiceConfiguration:resource:FormatServiceManagementScope").ToString(), Machine);
                }
            }
        }

        /// <summary>
        /// Username to connect
        /// </summary>
        /// <remarks>
        /// Default value : Environment.UserName
        /// </remarks>
        public string Username { get; set; }

        /// <summary>
        /// Password to connect
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Authentication mode
        /// </summary>
        /// <remarks>
        /// Default value : AuthenticationLevel.PacketPrivacy
        /// </remarks>
        public System.Management.AuthenticationLevel Authentication { get; set; }
        /// <summary>
        /// WMI Management Scope
        /// </summary>
        ManagementScope scopeCluster;
        #endregion "Properties"

        #region "Constructor"
        //public ServiceWmiManager(IConfiguration configuration)
        //{
        //    _configuration = configuration;
        //}
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="clustername">Cluster to manage</param>
        public ServiceWmiManager(string clustername)
        {
            Machine = clustername;
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="clustername"></param>
        /// 
        public ServiceWmiManager(string clustername,IConfiguration _config)
        {
            _configuration = _config;
            Machine = clustername;
           
        }
        public ServiceWmiManager()
        {
            Machine = ConfigurationManager.AppSetting["SystemServiceConfiguration:resource:LocalCluster"];
        }

        #endregion "Constructor"

        #region "Public Methods"
        /// <summary>
        /// Connects to the WMI Management scope specified by the clustername
        /// </summary>
        public void Connect()
        {
            var connection = new ConnectionOptions
            {
                Authentication = Authentication
            };
            if (Username != null)
            {
                connection.Username = Username;
            }
            if (Password != null)
            {
                connection.Password = Password;
            }
            if (Domain != null)
            {
                connection.Authority = string.Format(ConfigurationManager.AppSetting["SystemServiceConfiguration:resource:FormatAuthority"] , Domain);
            }
            scopeCluster = new ManagementScope(Scope, connection);
            scopeCluster.Connect();
            IsConnected = scopeCluster.IsConnected;
        }

        public void Disconnect()
        {
            scopeCluster = null;
            IsConnected = false;
        }

        /// <summary>
        /// Public method to online a resource
        /// </summary>
        /// <param name="clusterResource">Resource to bring online</param>
        /// <returns>Resource state after operation occurs</returns>
        /// 
        public void RestartService(string svcName) {

            StopService(svcName);
            StartService(svcName);
           

        }





        public  ServiceStatus StartService(string svcName)
        {
            string objPath = string.Format("Win32_Service.Name='{0}'", svcName);
            using (ManagementObject service = new ManagementObject(new ManagementPath(objPath)))
            {
                try
                {
                    ManagementBaseObject outParams = service.InvokeMethod("StartService", null, null);

                    return (ServiceStatus)Enum.Parse(typeof(ServiceStatus), outParams["ReturnValue"].ToString());
                }
                catch (Exception ex)
                {
                    if (ex.Message.ToLower().Trim() == "not found" || ex.GetHashCode() == 41149443)
                        return ServiceStatus.ServiceNotFound;
                    else
                        throw ex;
                }
            }
        }

        public  ServiceStatus StopService(string svcName)
        {
            string objPath = string.Format("Win32_Service.Name='{0}'", svcName);
            using (ManagementObject service = new ManagementObject(new ManagementPath(objPath)))
            {
                try
                {
                    ManagementBaseObject outParams = service.InvokeMethod("StopService", null, null);

                    return (ServiceStatus)Enum.Parse(typeof(ServiceStatus), outParams["ReturnValue"].ToString());
                }
                catch (Exception ex)
                {
                    if (ex.Message.ToLower().Trim() == "not found" || ex.GetHashCode() == 41149443)
                        return ServiceStatus.ServiceNotFound;
                    else
                        throw ex;
                }
            }
        }


        public void GetAllServices()
        {
            ManagementObjectSearcher windowsServicesSearcher = new ManagementObjectSearcher("root\\cimv2", "select * from Win32_Service");
            ManagementObjectCollection objectCollection = windowsServicesSearcher.Get();

            Console.WriteLine("There are {0} Windows services: ", objectCollection.Count);

            foreach (ManagementObject windowsService in objectCollection)
            {
                PropertyDataCollection serviceProperties = windowsService.Properties;
                foreach (PropertyData serviceProperty in serviceProperties)
                {
                    if (serviceProperty.Value != null)
                    {
                        Console.WriteLine("Windows service property name: {0}", serviceProperty.Name);
                        Console.WriteLine("Windows service property value: {0}", serviceProperty.Value);
                    }
                }
                Console.WriteLine("---------------------------------------");
            }
        }





        public ServiceResourceState GetResourceState(string svcResource)
        {
            ServiceResourceState result = ServiceResourceState.Unknown;

            if (!scopeCluster.IsConnected)
            {
                scopeCluster.Connect();
            }

            string path = string.Format(ConfigurationManager.AppSetting["SystemServiceConfiguration:resource:WmiServiceResourceQuery"], svcResource);

            try
            {
                using (ManagementObject service = new ManagementObject(new ManagementPath(path)))
                {
                    result = GetState(service[ConfigurationManager.AppSetting["SystemServiceConfiguration:resource:StateProperty"]].ToString());
                }
            }

            catch 
            {
                result = ServiceResourceState.Unknown;
            }

            return result;
        }
        /// <summary>
        /// Cleans up the object implementing IDispose
        /// </summary>
        public void Dispose()
        {
            if (IsConnected)
            {
                Disconnect();
            }

        }
        #endregion "Public Methods"

        #region "Private Methods"
        private static ServiceResourceState GetState(String state)
        {
            ServiceResourceState result = ServiceResourceState.Unknown;
            try
            {
                result = (ServiceResourceState)Enum.Parse(typeof(ServiceResourceState), state);
            }
            catch
            {
                result = ServiceResourceState.Unknown;
            }

            return result;
        }
       
     
        #endregion "Private Methods"

    }

}
