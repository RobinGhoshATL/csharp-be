using Microsoft.AspNetCore.SignalR;
using ProSuite.Support.WebAPI.Migration.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProSuite.Support.WebAPI.SignalR.Data;
using ProSuite.Support.WebAPI.SignalR.Hubs;
using ProSuite.Support.WebAPI.SignalR.Models;
using Syncfusion.EJ2.Charts;

namespace ProSuite.Support.WebAPI.Hubs
{
    public class PowerShellHub : Hub
    {
        public static IHubContext<PowerShellHub> Current { get; set; }
        private readonly static ConnectionMapping<string> _connections = new ConnectionMapping<string>();

        public string GetConnectionId() => Context.ConnectionId;

        public async void SendMessage(PSData _message)
        {
           
            List<string> ReceiverConnectionids = _connections.GetConnections(_message.Receiverid).ToList<string>();

            if (ReceiverConnectionids.Count() > 0)
            {
                //Save-Receive-Message
                try
                {
                    await Current.Clients.Clients(ReceiverConnectionids).SendAsync("PSScriptResponse", _message);
                }
                catch (Exception) { }
            }
        }

        public override Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            if (httpContext != null)
            {
                try
                {
                    //Add Logged User
                    var userName = httpContext.Request.Query["user"].ToString();
                    var connId = Context.ConnectionId.ToString();
                    _connections.Add(userName, connId);                    
                }
                catch (Exception) { }
            }
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            var httpContext = Context.GetHttpContext();
            if (httpContext != null)
            {
                //Remove Logged User
                var username = httpContext.Request.Query["user"];
                _connections.Remove(username, Context.ConnectionId);
            }
            return base.OnDisconnectedAsync(exception);
        }
    }
}
