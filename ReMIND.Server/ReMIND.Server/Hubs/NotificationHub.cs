using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Http;
using System;
using ReMIND.Server.Data;
using System.Collections.Generic;

namespace ReMIND.Server.Hubs
{
    public class NotificationHub: Hub
    {
        ReMindContext _context;

        public NotificationHub(ReMindContext context) {
            _context = context;
        }

        public override async Task OnConnectedAsync() {
            
            string sID = Context.GetHttpContext().Request.Headers["SessionID"];           
            
            await Groups.AddToGroupAsync(Context.ConnectionId, sID);
            await base.OnConnectedAsync();
        }
    }
}