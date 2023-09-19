using Microsoft.AspNetCore.Mvc;
using ReMIND.Server.Data;
using System;
using System.Threading.Tasks;
using ReMIND.Server.Helpers;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace ReMIND.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobArchiveController: ControllerBase
    {
        ReMindContext _context;
        Utility _util;

        public class jobArchiveQuery {            
            public string jobName {get; set;}
            public string teamName {get; set;}
            public string contact {get; set;}
            public string groupName {get; set;} 
            public string tagName {get; set;}
            public int? employeeID {get; set;}
            public bool? finished {get; set;}
            public int? weight {get; set;}
        }

        public JobArchiveController(ReMindContext context, Utility util) {
            _context = context;
            _util = util;
            _util.checkCaller();
        }
        
        [Route("GetAll")]
        [HttpGet]
        public async Task<ActionResult<List<JobArchive>>> GetAll() {
            try {      
                if (await _util.isCallerAdmin()) {
                    return Ok(await _context.JobArchives.ToListAsync());
                } 

                if (await _util.isCallerLeader()) {
                    List<TeamLink> arrTL = await _context.TeamLinks.Where(tl => tl.PersonID == _context.caller.ID && tl.Title == TitleType.Leader).ToListAsync();
                    _util.IsItNull(arrTL);

                    List<JobArchive> arrJA = new List<JobArchive>();
                    foreach(TeamLink tl in arrTL) {
                        arrJA.AddRange(_context.JobArchives.Where(ja => ja.tdID == tl.TeamID));
                    }
                    return Ok(arrJA);
                }     

                return BadRequest(Info.NoPermission);
            } catch (ReMindException re) {
                return BadRequest(re.message);
            
            } catch (Exception e) {
                _util.useException(e);
                return BadRequest(Info.Unknown);
            } 
        }

        [Route("Get/{start}/{num}/q")]
        [HttpPost]
        public async Task<ActionResult<List<JobArchive>>> Get([FromQuery] jobArchiveQuery objJAQ, [FromBody] FromToDate ftdObj, int start, int num) {
            try {      
                if (await _util.isCallerAdmin() || await _util.isCallerLeader()) {

                    List<JobArchive> arrJA = new List<JobArchive>();

                    if (await _util.isCallerAdmin())
                        arrJA = await _context.JobArchives.ToListAsync();
                    else {
                        IQueryable<TeamLink> arrTL = _context.TeamLinks.Where(tl => tl.PersonID == _context.caller.ID && tl.Title == TitleType.Leader).AsQueryable();
                        _util.IsItNull(arrTL);

                        List<JobArchive> arrTMP = new List<JobArchive>();
                        foreach(TeamLink tl in arrTL)
                            arrJA.AddRange(_context.JobArchives.Where(ja => ja.tdID == tl.TeamID));
                    }
                    
                    //Filter 
                    if (!string.IsNullOrEmpty(objJAQ.jobName))
                        arrJA = arrJA.Where(ja => ja.Name == objJAQ.jobName).ToList();

                    if (!string.IsNullOrEmpty(objJAQ.teamName))
                        arrJA = arrJA.Where(ja => ja.TeamName == objJAQ.teamName).ToList();

                    if (!string.IsNullOrEmpty(objJAQ.contact))
                        arrJA = arrJA.Where(ja => ja.Contact == objJAQ.contact).ToList();

                    if (!string.IsNullOrEmpty(objJAQ.groupName))
                        arrJA = arrJA.Where(ja => ja.JobGroupName == objJAQ.groupName).ToList();

                    if (!string.IsNullOrEmpty(objJAQ.tagName))
                        arrJA = arrJA.Where(ja => ja.JobTagName == objJAQ.tagName).ToList();

                    if (objJAQ.employeeID.HasValue)
                        arrJA = arrJA.Where(ja => ja.PersonID == objJAQ.employeeID).ToList();

                    if (objJAQ.weight.HasValue)
                        arrJA = arrJA.Where(ja => ja.JobWeight == objJAQ.weight).ToList();

                    if (objJAQ.finished.HasValue)
                        if (objJAQ.finished == true)
                            arrJA = arrJA.Where(ja => ja.Finished != null).ToList();
                        else
                            arrJA = arrJA.Where(ja => ja.Finished == null).ToList();

                    if (ftdObj.DateFrom.HasValue)
                        arrJA = arrJA.Where(ja => ftdObj.DateFrom <= ja.Deadline).ToList();

                    if (ftdObj.DateTo.HasValue)
                        arrJA = arrJA.Where(ja => ja.Deadline <= ftdObj.DateTo).ToList();

                    //Get certain number
                    if (start >= arrJA.Count())
                        return BadRequest(Info.IndexOutOfRange);
                    if (start + num >= arrJA.Count())
                        num = arrJA.Count() - start;

                    arrJA = arrJA.GetRange(start, num);

                    return Ok(arrJA);
                } else 
                    return BadRequest(Info.NoPermission);
            } catch (ReMindException re) {
                return BadRequest(re.message);
            
            } catch (Exception e) {
                _util.useException(e);
                return BadRequest(Info.Unknown);
            } 
        }

        [Route("GetNames")]
        [HttpGet]
        public async Task<ActionResult<List<string>>> Getnames() {
            try {      
                if (await _util.isCallerAdmin()) {
                    List<string> res = await _context.JobArchives.Select(ja => ja.Name).ToListAsync();
                    return Ok(res.Distinct().ToList());
                } 

                if (await _util.isCallerLeader()) {
                    List<TeamLink> arrTL = await _context.TeamLinks.Where(tl => tl.PersonID == _context.caller.ID && tl.Title == TitleType.Leader).ToListAsync();
                    _util.IsItNull(arrTL);

                    List<JobArchive> arrJA = new List<JobArchive>();
                    foreach(TeamLink tl in arrTL) {
                        arrJA.AddRange(_context.JobArchives.Where(ja => ja.tdID == tl.TeamID));
                    }
                    List<string> res = arrJA.Select(ja => ja.Name).ToList();
                    return Ok(res.Distinct().ToList());
                }     

                return BadRequest(Info.NoPermission);
            } catch (ReMindException re) {
                return BadRequest(re.message);
            
            } catch (Exception e) {
                _util.useException(e);
                return BadRequest(Info.Unknown);
            } 
        }

        [Route("GetEmployeNames")]
        [HttpGet]
        public async Task<ActionResult<List<string>>> GetEmployeNames() {
            try {      
                if (await _util.isCallerAdmin()) {
                    List<string> res = await _context.Persons.Select(p => p.Name).ToListAsync();
                    return Ok(res.Distinct().ToList());
                } 

                if (await _util.isCallerLeader()) {
                    List<TeamLink> arrTL = await _context.TeamLinks.Where(tl => tl.PersonID == _context.caller.ID && tl.Title == TitleType.Leader).ToListAsync();
                    _util.IsItNull(arrTL);

                    List<JobArchive> arrJA = new List<JobArchive>();
                    foreach(TeamLink tl in arrTL) {
                        arrJA.AddRange(_context.JobArchives.Where(ja => ja.tdID == tl.TeamID).Include(ja => ja.Person));
                    }

                    List<string> res = arrJA.Select(ptl => ptl.Person.Name).ToList();
                    return Ok(res.Distinct().ToList());
                }     

                return BadRequest(Info.NoPermission);
            } catch (ReMindException re) {
                return BadRequest(re.message);
            
            } catch (Exception e) {
                _util.useException(e);
                return BadRequest(Info.Unknown);
            } 
        }

        [Route("GetTeams")]
        [HttpGet]
        public async Task<ActionResult<List<string>>> GetTeams() {
            try {      
                if (await _util.isCallerAdmin()) {
                    List<string> res = await _context.JobArchives.Select(ja => ja.TeamName).ToListAsync();
                    return Ok(res.Distinct().ToList());
                } 

                if (await _util.isCallerLeader()) {
                    List<TeamLink> arrTL = await _context.TeamLinks.Where(tl => tl.PersonID == _context.caller.ID && tl.Title == TitleType.Leader).ToListAsync();
                    _util.IsItNull(arrTL);

                    List<JobArchive> arrJA = new List<JobArchive>();
                    foreach(TeamLink tl in arrTL) {
                        arrJA.AddRange(_context.JobArchives.Where(ja => ja.tdID == tl.TeamID));
                    }
                    List<string> res = arrJA.Select(ja => ja.TeamName).ToList();
                    return Ok(res.Distinct().ToList());
                }     

                return BadRequest(Info.NoPermission);
            } catch (ReMindException re) {
                return BadRequest(re.message);
            
            } catch (Exception e) {
                _util.useException(e);
                return BadRequest(Info.Unknown);
            } 
        }

        [Route("GetContact")]
        [HttpGet]
        public async Task<ActionResult<List<string>>> GetContact() {
            try {      
                if (await _util.isCallerAdmin()) {
                    List<string> res = await _context.JobArchives.Select(ja => ja.Contact).ToListAsync();
                    return Ok(res.Distinct().ToList());
                } 

                if (await _util.isCallerLeader()) {
                    List<TeamLink> arrTL = await _context.TeamLinks.Where(tl => tl.PersonID == _context.caller.ID && tl.Title == TitleType.Leader).ToListAsync();
                    _util.IsItNull(arrTL);

                    List<JobArchive> arrJA = new List<JobArchive>();
                    foreach(TeamLink tl in arrTL) {
                        arrJA.AddRange(_context.JobArchives.Where(ja => ja.tdID == tl.TeamID));
                    }
                    List<string> res = arrJA.Select(ja => ja.Contact).ToList();
                    return Ok(res.Distinct().ToList());
                }     

                return BadRequest(Info.NoPermission);
            } catch (ReMindException re) {
                return BadRequest(re.message);
            
            } catch (Exception e) {
                _util.useException(e);
                return BadRequest(Info.Unknown);
            } 
        }

        [Route("GetGroupNames")]
        [HttpGet]
        public async Task<ActionResult<List<string>>> GetGroupNames() {
            try {      
                if (await _util.isCallerAdmin()) {
                    List<string> res = await _context.JobArchives.Select(ja => ja.JobGroupName).ToListAsync();
                    return Ok(res.Distinct().ToList());
                } 

                if (await _util.isCallerLeader()) {
                    List<TeamLink> arrTL = await _context.TeamLinks.Where(tl => tl.PersonID == _context.caller.ID && tl.Title == TitleType.Leader).ToListAsync();
                    _util.IsItNull(arrTL);

                    List<JobArchive> arrJA = new List<JobArchive>();
                    foreach(TeamLink tl in arrTL) {
                        arrJA.AddRange(_context.JobArchives.Where(ja => ja.tdID == tl.TeamID));
                    }
                    List<string> res = arrJA.Select(ja => ja.JobGroupName).ToList();
                    return Ok(res.Distinct().ToList());
                }     

                return BadRequest(Info.NoPermission);
            } catch (ReMindException re) {
                return BadRequest(re.message);
            
            } catch (Exception e) {
                _util.useException(e);
                return BadRequest(Info.Unknown);
            } 
        }

        [Route("GetTagNames")]
        [HttpGet]
        public async Task<ActionResult<List<string>>> GetTagNames() {
            try {      
                if (await _util.isCallerAdmin()) {
                    List<string> res = await _context.JobArchives.Select(ja => ja.JobTagName).ToListAsync();
                    return Ok(res.Distinct().ToList());
                } 

                if (await _util.isCallerLeader()) {
                    List<TeamLink> arrTL = await _context.TeamLinks.Where(tl => tl.PersonID == _context.caller.ID && tl.Title == TitleType.Leader).ToListAsync();
                    _util.IsItNull(arrTL);

                    List<JobArchive> arrJA = new List<JobArchive>();
                    foreach(TeamLink tl in arrTL) {
                        arrJA.AddRange(_context.JobArchives.Where(ja => ja.tdID == tl.TeamID));
                    }
                    List<string> res = arrJA.Select(ja => ja.JobTagName).ToList();
                    return Ok(res.Distinct().ToList());

                }     

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