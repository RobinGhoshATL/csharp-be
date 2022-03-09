using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NLog;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ProSuite.Support.WebAPI.Middleware
{
    public class RequestHandlerMiddleware
    {
        #region "Variables"
        private readonly RequestDelegate _next;
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private IConfiguration _configuration;
        #endregion "Variables"

        #region "Constructor"
        public RequestHandlerMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            this._next = next;
            this._configuration = configuration;
        }
        #endregion "Constructor"

        #region "Public Methods"
        public async Task Invoke(HttpContext context)
        {
            context.Request.EnableBuffering();
            
            string user = context.Request.Headers["User"];
            string role = context.Request.Headers["Role"];

            string defaultPath =string.Empty;


            if (!string.IsNullOrEmpty(defaultPath))
            {
                setDefaultPath(defaultPath, user, role);
            }

            setDefaultPath(_configuration.GetSection("Logging:Location:Default").Value, string.Empty, "baseDir");

            var body = await new StreamReader(context.Request.Body).ReadToEndAsync();
            //  logger.Info($"Body: {body}");
            context.Request.Body.Position = 0;

            //  logger.Info($"Host: {context.Request.Host.Host}");
            _logger.Info($"Client IP: {context.Connection.RemoteIpAddress}, Timestamp: {DateTime.Now.ToString("yyyy/MM/dd")}");
            await _next(context);
        }

        private void setDefaultPath(string path,string user, string role)
        {
            var updatePath = path;
            if (!System.IO.Directory.Exists(path))
            {
                try
                {
                    System.IO.Directory.CreateDirectory(path);
                }
                catch (Exception)
                {
                    // set default directory 
                    updatePath = "C:/temp";
                }
               
            }

            if (!string.IsNullOrEmpty(user))
            {
                GlobalDiagnosticsContext.Set("User", user);

                bool exists = System.IO.Directory.Exists(String.Format("{0}/{1}", updatePath, user));

                if (!exists)
                {
                    System.IO.Directory.CreateDirectory(String.Format("{0}/{1}", updatePath, user));
                }
                updatePath = String.Format("{0}/{1}", updatePath, user);
            }
            LogManager.Configuration.Variables[role] = updatePath;
        }
        #endregion "Public Methods"
    }
}
