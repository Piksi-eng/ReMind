using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReMIND.Server.Data;
using System;
using System.Threading.Tasks;
using ReMIND.Server.Helpers;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ReMIND.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController: ControllerBase
    {
        ReMindContext _context;
        Utility _util;        

        public AuthController(ReMindContext context, Utility util) {
            _context = context;
            _util = util;
        }
        
        [Route("Login")]
        [HttpPost]
        public async Task<ActionResult<Person>> Login() {
            try {                
                //"email:password" -> "pera@peric.com:pass12345"
                string authHeader = HttpContext.Request.Headers["Authentication"];
                if (authHeader != null) {

                    (string email, string password) = _util.authDecode(authHeader);

                    Person obj = await _util.EmailExists(email);
                    _util.verifyPassword(password, obj.Password);
                    
                    string newSessionID = BCrypt.Net.BCrypt.GenerateSalt()+email;
                    obj.SessionID = newSessionID;

                    _context.Persons.Update(obj);
                    await _context.SaveChangesAsync();

                    return Ok(obj);
                } else 
                    throw new ReMindException(Info.AuthWrong);

            } catch (ReMindException re) {
                return BadRequest(re.message);
            
            } catch (Exception e) {
                _util.useException(e);
                return BadRequest(Info.Unknown);
            } 
        }

        [Route("CheckSession/{sID}")]
        [HttpPost]
        public async Task<ActionResult> CheckSession(string sID) {
            Person obj = await _context.Persons.Where(p => p.SessionID == sID).FirstOrDefaultAsync();
            _util.IsItNull(obj);

            if ((DateTime.Now).Subtract(obj.LastActive).Minutes < 15)
                return BadRequest(Info.NoSession);
            return Ok(Info.LoggedIn);
        }
    }
}