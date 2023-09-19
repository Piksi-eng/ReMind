using Microsoft.AspNetCore.Mvc;
using ReMIND.Server.Data;
using System;
using System.Threading.Tasks;
using ReMIND.Server.Helpers;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using ReMIND.Server.Helpers.Email;

namespace ReMIND.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobGroupController: ControllerBase
    {
        ReMindContext _context;
        Utility _util;        

        public JobGroupController(ReMindContext context, Utility util) {
            _context = context;
            _util = util;
            _util.checkCaller();
        }

        [Route("Create")]
        [HttpPost]
        public async Task<ActionResult<JobGroup>> Create([FromBody] JobGroup newJobGroup) {
            try {   
                newJobGroup.isRead = false;
                newJobGroup.Counter = 0;
                newJobGroup.CreatorID = _context.caller.ID;  

                if (await _util.isCallerAdmin() || await _util.isTeamLeader(newJobGroup.TeamID)) {

                    _context.JobGroups.Add(newJobGroup);
                    await _context.SaveChangesAsync();
                    
                    List<TeamLink> arrTL = await _context.TeamLinks.Where(tl => tl.TeamID == newJobGroup.TeamID && tl.Title == TitleType.Leader).ToListAsync();
                    foreach(TeamLink tl in arrTL) {
                        Person p = await _context.Persons.FindAsync(tl.PersonID);
                        if (p != null) {
                            if (_context.caller.ID != p.ID) {
                                await _util.SendReloadJobs(p.SessionID);
                                await _util.SendReloadNotifications(p.SessionID);
                                // await _util.sendEmail(p.Email,"New group:" + newJobGroup.Name, EmailInfo.setBody("GROUP", newJobGroup.Name, "", "", newJobGroup.Description));
                            }
                        }
                    }                    

                    return Ok(newJobGroup);
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
        public async Task<ActionResult<JobGroup>> Update([FromBody] JobGroup newJobGroup) {
            try {
                if (await _util.isCallerAdmin() || await _util.isTeamLeader(newJobGroup.TeamID)) {

                    JobGroup obj = await _context.JobGroups.FindAsync(newJobGroup.ID);
                    _util.IsItNull(obj);

                    obj.Name = newJobGroup.Name;
                    obj.Description = newJobGroup.Description;

                    obj.isRead = newJobGroup.isRead;

                    if (obj.TeamID != newJobGroup.TeamID) {

                        List<Job> arrJ = await _context.Jobs.Where(j => j.JobGroupID == obj.ID).ToListAsync();
                        if (arrJ != null)
                            foreach(Job j in arrJ)
                                _context.Jobs.Remove(j);

                        obj.TeamID = newJobGroup.TeamID;
                        obj.isRead = false;
                        obj.Counter = 0;
                    }

                    _context.JobGroups.Update(obj);
                    await _context.SaveChangesAsync();

                    List<TeamLink> arrTL = await _context.TeamLinks.Where(tl => tl.TeamID == obj.TeamID && tl.Title == TitleType.Leader).ToListAsync();
                    foreach(TeamLink tl in arrTL) {
                        Person p = await _context.Persons.FindAsync(tl.PersonID);
                        if (p != null) {
                            if (_context.caller.ID != p.ID) {
                                await _util.SendReloadJobs(p.SessionID);
                                await _util.SendReloadNotifications(p.SessionID);
                                // await _util.sendEmail(p.Email,"Group updated:" + obj.Name, EmailInfo.setBody("GROUP", obj.Name, "", "", obj.Description));
                            }
                        }
                    }

                    return Ok(obj);
                } else 
                    return BadRequest(Info.NoPermission);

            } catch (ReMindException re) {
                return BadRequest(re.message);
            
            } catch (Exception e) {
                _util.useException(e);
                return BadRequest(Info.Unknown);
            } 
        }

        [Route("Delete/{jgID}/{choice}")]
        [HttpDelete]
        public async Task<ActionResult<JobGroup>> Delete(int jgID, bool choice) {
            try {
                JobGroup jg = await _context.JobGroups.FindAsync(jgID);
                _util.IsItNull(jg);

                if (await _util.isCallerAdmin() || await _util.isTeamLeader(jg.TeamID)) {
                    
                    List<Job> arrJ = await _context.Jobs.Where(j => j.JobGroupID == jg.ID).ToListAsync();
                    
                    if (arrJ != null) {
                        if (choice) {                                
                            foreach(Job j in arrJ)
                                _context.Jobs.Remove(j);
                        } else {
                            foreach(Job j in arrJ) {
                                j.JobGroupID = 0;
                                _context.Jobs.Update(j);
                            }
                        }
                    }

                    _context.JobGroups.Remove(jg);
                    await _context.SaveChangesAsync();

                    List<TeamLink> arrTL = await _context.TeamLinks.Where(tl => tl.TeamID == jg.TeamID && tl.Title == TitleType.Leader).ToListAsync();
                    foreach(TeamLink tl in arrTL) {
                        Person p = await _context.Persons.FindAsync(tl.PersonID);
                        if (p != null) {
                            if (_context.caller.ID != p.ID) {
                                await _util.SendReloadJobs(p.SessionID);
                                await _util.SendReloadNotifications(p.SessionID);
                                // await _util.sendEmail(p.Email,"Group updated:" + obj.Name, EmailInfo.setBody("GROUP", obj.Name, "", "", obj.Description));
                            }
                        }
                    }

                    return Ok(jg.Name + Info.Deleted);                    
                } else 
                    return BadRequest(Info.NoPermission);

            } catch (ReMindException re) {
                return BadRequest(re.message);
            
            } catch (Exception e) {
                _util.useException(e);
                return BadRequest(Info.Unknown);
            } 
        }
    
        [Route("GetByID/{jgID}")]
        [HttpGet]
        public async Task<ActionResult<JobGroup>> GetByID(int jgID) {
            try {   
                JobGroup jg = await _context.JobGroups.FindAsync(jgID);
                _util.IsItNull(jg);

                if (await _util.isCallerAdmin() || await _util.isInTeam(jg.TeamID)) {
                    return Ok(jg);
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
        public async Task<ActionResult<List<JobGroup>>> GetByTeam(int tID) {
            try {
                if (await _util.isCallerAdmin() || await _util.isTeamLeader(tID)) {

                    List<JobGroup> arr = await _context.JobGroups.Where(jg => jg.TeamID == tID).ToListAsync();
                    _util.IsItNull(arr);

                    return arr;
                } else 
                    return BadRequest(Info.NoPermission);

            } catch (ReMindException re) {
                return BadRequest(re.message);
            
            } catch (Exception e) {
                _util.useException(e);
                return BadRequest(Info.Unknown);
            } 
        }

        [Route("GetByCounter/{counter}")]
        [HttpGet]
        public async Task<ActionResult<List<JobGroup>>> GetByCounter(int counter) {
            try {
                if (await _util.isCallerAdmin()) {

                    List<JobGroup> arr;
                    if (counter > 0)
                        arr = await _context.JobGroups.Where(jg => jg.Counter > 0).ToListAsync();                        
                    else
                        arr = await _context.JobGroups.Where(jg => jg.Counter == 0).ToListAsync();                        
                    
                    _util.IsItNull(arr);
                    return Ok(arr);
                }

                if (await _util.isCallerLeader() || await _util.isCallerEmployee()) {
                    
                    List<TeamLink> arrTL = await _context.TeamLinks.Where(tl => tl.PersonID == _context.caller.ID).ToListAsync();
                    _util.IsItNull(arrTL);
                    List<JobGroup> arrJG = new List<JobGroup>();

                    if (counter > 0) {
                        foreach(TeamLink tl in arrTL) {
                            JobGroup obj = await _context.JobGroups.Where(jg => jg.TeamID == tl.TeamID && jg.Counter > 0).FirstOrDefaultAsync();
                            arrJG.Add(obj);
                        }
                    } else {
                        foreach(TeamLink tl in arrTL) {
                        JobGroup obj = await _context.JobGroups.Where(jg => jg.TeamID == tl.TeamID && jg.Counter == 0).FirstOrDefaultAsync();
                        arrJG.Add(obj);
                        }
                    _util.IsItNull(arrJG);
                    return Ok(arrJG);
                    }
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