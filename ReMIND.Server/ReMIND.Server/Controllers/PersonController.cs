using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReMIND.Server.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ReMIND.Server.Helpers;
using ReMIND.Server.Helpers.Email;

namespace ReMIND.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController: ControllerBase
    {
        ReMindContext _context;
        Utility _util;        

        public PersonController(ReMindContext context, Utility util) {
            _context = context;
            _util = util;
            _util.checkCaller();
        }
        
        [Route("Create")]
        [HttpPost]
        public async Task<ActionResult<Person>> Create([FromBody] Person newPerson) {
            try {                
                if (await _util.isCallerAdmin()) {

                    await _util.isEmailUsed(newPerson.Email);
                    await _util.isPhoneUsed(newPerson.Phone);

                    //string savePassword = BCrypt.Net.BCrypt.GenerateSalt().Substring(6, 7); Dzoni popravi
                    string savePassword = "remind";
                    newPerson.Password = _util.hashPassword(savePassword);
                    newPerson.SessionID = "";
                    newPerson.IsEmployed = true;

                    _context.Persons.Add(newPerson);
                    await _context.SaveChangesAsync();

                    //await _util.sendEmail(newPerson.Email, "Account creation", EmailInfo.setBody("Account", "Account created", _context.caller.Email, DateTime.Now.ToShortDateString(), $"<p>Your account has been created<br>Email: {newPerson.Email}<br>Password: {savePassword}<br>Please change the password once you log into the application.</p>"));
                    return Ok(newPerson);
                } else
                    return BadRequest(Info.NoPermission);

            } catch (ReMindException re) {
                return BadRequest(re.message);
            
            } catch (Exception e) {
                _util.useException(e);
                return BadRequest(Info.Unknown);
            }            
        }

        [Route("Update")]
        [HttpPut]
        public async Task<ActionResult<Person>> Update([FromBody] Person newPerson) {
            try {
                if (await _util.isCallerAdmin()) {

                    Person obj = await _context.Persons.FindAsync(newPerson.ID);
                    _util.IsItNull(obj);

                    if (obj.Email != newPerson.Email) 
                        await _util.isEmailUsed(newPerson.Email);
                    if (obj.Phone != newPerson.Phone)
                        await _util.isPhoneUsed(newPerson.Phone);

                    obj.Name = newPerson.Name;
                    obj.Phone = newPerson.Phone;                    
                    obj.Email = newPerson.Email;

                    if (obj.IsEmployed != newPerson.IsEmployed) {
                        if(newPerson.IsEmployed == false) {
                            List<TeamLink> arr = await _context.TeamLinks.Where(tl => tl.PersonID == obj.ID).ToListAsync();
                            foreach(TeamLink tl in arr)
                                _context.TeamLinks.Remove(tl);
                            obj.IsEmployed = newPerson.IsEmployed;
                            obj.SessionID = "";
                        } else 
                            obj.IsEmployed = true;
                    }

                    if (HttpContext.Request.Headers.ContainsKey("OldPassword") && HttpContext.Request.Headers.ContainsKey("NewPassword")) {
                        string OldPassword = HttpContext.Request.Headers["OldPassword"];
                        string NewPassword = HttpContext.Request.Headers["NewPassword"]; 

                        if(_util.verifyPassword(OldPassword, obj.Password)) {
                            newPerson.Password = _util.hashPassword(NewPassword);
                            obj.Password = newPerson.Password;

                            _context.Persons.Update(obj);
                            await _context.SaveChangesAsync();
                        }
                    }
                    
                    _context.Persons.Update(obj);
                    await _context.SaveChangesAsync();

                    return Ok(obj);
                }

                if (_context.caller.ID == newPerson.ID) {
                    
                    Person obj = await _context.Persons.FindAsync(newPerson.ID); 
                    _util.IsItNull(obj);

                    string OldPassword = HttpContext.Request.Headers["OldPassword"];
                    string NewPassword = HttpContext.Request.Headers["NewPassword"]; 

                    if(_util.verifyPassword(OldPassword, obj.Password)) {
                        newPerson.Password = _util.hashPassword(NewPassword);
                        obj.Password = newPerson.Password;

                        _context.Persons.Update(obj);
                        await _context.SaveChangesAsync();
                    }

                    return Ok(obj);
                }

                return BadRequest(Info.NoPermission);

            } catch (ReMindException re) {
                return BadRequest(re.message);

            } catch (Exception e) {
                _util.useException(e);
                return BadRequest(Info.Unknown);
            }            
        }

        [Route("Get/{pID}")]
        [HttpGet]
        public async Task<ActionResult<Person>> Get(int pID) {
            try {                
                if (await _util.isCallerAdmin() || _context.caller.ID == pID) {

                    Person obj = await _context.Persons.FindAsync(pID);
                    return Ok(obj);
                } 
                if (await _util.isCallerLeader()) {
                    Person obj = await _context.Persons.FindAsync(pID);
                    return Ok(obj);
                }
                else
                    return BadRequest(Info.NoPermission);

            } catch (ReMindException re) {
                return BadRequest(re.message);
            
            } catch (Exception e) {
                _util.useException(e);
                return BadRequest(Info.Unknown);
            }            
        }

        [Route("GetAll")]
        [HttpGet]
        public async Task<ActionResult<List<Person>>> GetAll() {
            try {                
                if (await _util.isCallerAdmin()) {

                    List<Person> arr = await _context.Persons.ToListAsync();
                    _util.IsItNull(arr);
                    
                    return Ok(arr);
                } else
                    return BadRequest(Info.NoPermission);

            } catch (ReMindException re) {
                return BadRequest(re.message);
            
            } catch (Exception e) {
                _util.useException(e);
                return BadRequest(Info.Unknown);
            }            
        }

        [Route("GetByTeam/{tID}")]
        [HttpGet]
        public async Task<ActionResult<List<Person>>> GetByTeam(int tID) {
            try {                
                if (await _util.isCallerAdmin()) {    
                    List<TeamLink> arrTL = await _context.TeamLinks.Where(tl => tl.TeamID == tID).ToListAsync();
                    List<Person> arr = new List<Person>();

                    foreach (TeamLink tl in arrTL) {
                        Person objPerson = await _context.Persons.FindAsync(tl.PersonID);
                        arr.Add(objPerson);
                    }
                    return Ok(arr);
                }

                if (await _util.isTeamLeader(tID)) {
                    List<TeamLink> arrTL = await _context.TeamLinks.Where(tl => tl.TeamID == tID).ToListAsync();
                    List<Person> arr = new List<Person>();

                    foreach (TeamLink tl in arrTL) {
                        Person objPerson = await _context.Persons.FindAsync(tl.PersonID);
                        arr.Add(objPerson);
                    }
                    return Ok(arr);
                }

                return Ok(_context.caller);

            } catch (ReMindException re) {
                return BadRequest(re.message);
            
            } catch (Exception e) {
                _util.useException(e);
                return BadRequest(Info.Unknown);
            }            
        }          

        [Route("Logout/{pID}")]
        [HttpPost]
        public async Task<ActionResult<Person>> Logout(int pID) {
            try {                
                if (await _util.isCallerAdmin() || _context.caller.ID == pID) {

                    Person obj = await _context.Persons.Where(p => p.ID == pID).FirstOrDefaultAsync();
                    obj.SessionID = "";

                    _context.Persons.Update(obj);
                    await _context.SaveChangesAsync();

                    return Ok(Info.LoggedOut);
                } else
                    return BadRequest(Info.NoPermission);

            } catch (ReMindException re) {
                return BadRequest(re.message);
            
            } catch (Exception e) {
                _util.useException(e);
                return BadRequest(Info.Unknown);
            } 
        }

         [Route("ResetPassword/{pID}")]
        [HttpPost]
        public async Task<ActionResult<Person>> ResetPassword(int pID) {
            try {                
                if (await _util.isCallerAdmin()) {

                    Person obj = await _context.Persons.FindAsync(pID);
                    _util.IsItNull(obj);

                    //string savePassword = BCrypt.Net.BCrypt.GenerateSalt().Substring(6, 7);
                    string savePassword = "remind"; //Dzoni popravi
                    obj.Password = _util.hashPassword(savePassword);
                    obj.SessionID = "";
                    
                    _context.Persons.Update(obj);
                    await _context.SaveChangesAsync();

                    //await _util.sendEmail(obj.Email, "Account creation", EmailInfo.setBody("Account", "Account created", _context.caller.Email, DateTime.Now.ToShortDateString(), $"<p>Your password has been reset.<br>Email: {obj.Email}<br>Password: {savePassword}<br>Please change the password once you log-in in the application.</p>"));

                    return Ok(Info.ResetPassword);
                } else
                    return BadRequest(Info.NoPermission);

            } catch (ReMindException re) {
                return BadRequest(re.message);
            
            } catch (Exception e) {
                _util.useException(e);
                return BadRequest(Info.Unknown);
            } 
        }
    }
}