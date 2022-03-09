using Cassia;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProSuite.Support.WebAPI.Data;
using ProSuite.Support.WebAPI.DB;
using ProSuite.Support.WebAPI.Middleware;

using ProSuite.Support.WebAPI.Services;
using Microsoft.Identity.Web;
using ProSuite.Support.WebAPI.Hubs;
using System.Service.Control.PowershellScripting.Data;  
using System;
using System.Service.Control.DataBaseConnection.Data;

namespace ProSuite.Support.WebAPI
{

    public class Startup
    {
        private string _file = "appsettings.Development.json";
        public Startup(IHostEnvironment environment)
        {
            try
            {
                var env = environment.EnvironmentName;
                if (env == "Development")
                {
                    _file = $"appsettings.{env}.json";
                }
                var builder = new ConfigurationBuilder()
                .SetBasePath(environment.ContentRootPath)
                .AddJsonFile(_file, optional: false, reloadOnChange: true)

                .AddEnvironmentVariables();

                Configuration = builder.Build();
                StaticConfig = builder.Build();
            }

            catch (Exception ex) {
                throw ex;
            }
           
        }

        public IConfiguration Configuration { get; }
        public static IConfiguration StaticConfig { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
           

         

            services.AddTransient<Database>();
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder => builder
                    .WithOrigins("http://localhost:4200","https://angui-2-csharp.azurewebsites.net")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
              .AddMicrosoftIdentityWebApi(Configuration);

            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdministratorRole",
                     policy => policy.RequireRole("admin", "editor", "user"));
            });


           
            services.AddControllers();
           
            services.AddScoped<ServiceControlRepository>();
            services.AddScoped<DatabaseConnectRepository>();
            services.AddScoped<PSManager>();
            services.AddScoped<PowerShellScriptingRepository>();
            
            services.AddSignalR();
            services.AddScoped<PowerShellHub>();


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // production build error
                app.UseExceptionHandler(appBuilder =>
                {
                    appBuilder.Run(async c =>
                    {
                        c.Response.StatusCode = 500;
                        await c.Response.WriteAsync("Server error");
                    });
                });
            }
            app.UseCors("CorsPolicy");

            app.UseMiddleware<RequestHandlerMiddleware>();
            app.UseMiddleware(typeof(CorsMiddleware));
            app.UseMiddleware(typeof(ResponseTimeMiddleware));

            app.UseAuthentication();

            app.UseRouting();

           app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
               
                endpoints.MapControllers();
                endpoints.MapHub<PowerShellHub>("/pshub");


            });
             PowerShellHub.Current = app.ApplicationServices.GetService<IHubContext<PowerShellHub>>();


        }
    }
}
