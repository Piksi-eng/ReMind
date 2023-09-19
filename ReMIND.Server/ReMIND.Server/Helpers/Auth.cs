
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading.Tasks;
using ReMIND.Server.Data;

namespace ReMIND.Server.Helpers
{
    public class Auth
    {
        readonly RequestDelegate _next;

        public Auth(RequestDelegate next) {
            _next = next;
        }

        public async Task Invoke(HttpContext context, ReMindContext _context) {
            string sID = context.Request.Headers["SessionID"];
            if (!string.IsNullOrEmpty(sID))
                _context.caller = _context.Persons.Where(p => p.SessionID == sID).FirstOrDefault();
            else 
                _context.caller = null;
            await _next(context);
        }    
    }
}