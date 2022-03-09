using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ProSuite.Support.WebAPI.Middleware
{
    public class CorsMiddleware
    {
        #region "Variables"
        private readonly RequestDelegate _next;
        #endregion "Variables"

        #region "Constructor"
        public CorsMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        #endregion "Constructor"

        #region "Public Methods"
        public async Task Invoke(HttpContext context)
        {
            context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            context.Response.Headers.Add("Access-Control-Allow-Credentials", "true");
            // Added "Accept-Encoding" to this list
            context.Response.Headers.Add("Access-Control-Allow-Headers", "Overwrite,Expires, authorization, Destination, Pragma, Content-Type, Depth, User-Agent, Translate, Range, Content-Range, Timeout, X-File-Size, X-Requested-With, If-Modified - Since, X-File-Name, Cache-Control, Location, Lock-Token, If");
            context.Response.Headers.Add("Access-Control-Allow-Methods", "ACL, CANCELUPLOAD, CHECKIN, CHECKOUT, COPY, DELETE, GET, HEAD, LOCK, MKCALENDAR, MKCOL, MOVE, OPTIONS, POST, PROPFIND, PROPPATCH, PUT, REPORT, SEARCH, UNCHECKOUT, UNLOCK, UPDATE, VERSION - CONTROL");
            context.Response.Headers.Add("Access-Control-Expose-Headers", "DAV, content - length, Allow, Authorization");
            context.Response.Headers.Add("Cache-Control", "no-cache");
            context.Response.Headers.Add("Pragma", "no-cache");

            // New Code Starts here
            if (context.Request.Method == "OPTIONS")
            {
                context.Response.StatusCode = (int)HttpStatusCode.OK;
                await context.Response.WriteAsync(string.Empty);
            }
            // New Code Ends here

            await _next(context);
        }
        #endregion "Public Methods"
    }
}
