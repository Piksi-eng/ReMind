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
    public class JobController: ControllerBase
    {
        ReMindContext _context;
        Utility _util;        

        public JobController(ReMindContext context, Utility util) {
            _context = context;
            _util = util;
            _util.checkCaller();
        }

        [Route("Create")]
        [HttpPost]
        public async Task<ActionResult<Job>> Create([FromBody] Job newJob) {
            try {

                if (await _util.isCallerAdmin() || await _util.isTeamLeader(newJob.TeamID)) {

                    newJob.Team = await _context.Teams.FindAsync(newJob.TeamID);
                    newJob.Person = await _context.Persons.FindAsync(newJob.PersonID);
                    newJob.JobGroup = await _context.JobGroups.FindAsync(newJob.JobGroupID);
                    newJob.JobTag = await _context.JobTags.FindAsync(newJob.JobTagID);

                    newJob.isRead = false;
                    newJob.LastModified = DateTime.Now;
                    newJob.isDone = false;
                    newJob.Contact = _context.caller.Email;
                    

                    if (newJob.JobGroupID != null) {
                        JobGroup jg = await _context.JobGroups.FindAsync(newJob.JobGroupID);
                        _util.IsItNull(jg);

                        jg.Counter++;
                        _context.JobGroups.Update(jg);
                    }

                    if (newJob.JobTagID == null)   
                        newJob.JobTagID = 1;



                    _context.Jobs.Add(newJob);
                    await _context.SaveChangesAsync();

                    Person p = await _context.Persons.FindAsync(newJob.PersonID);
                    if (_context.caller.ID != p.ID) {
                        await _util.SendReloadJobs(p.SessionID);
                        await _util.SendReloadNotifications(p.SessionID);
                    //await _util.sendEmail(p.Email,"New job:" + newJob.Name, EmailInfo.setBody("JOB", newJob.Name, _context.caller.Email, "Date:" + newJob.Deadline.ToShortDateString(), newJob.Description));
                    }

                    return Ok(newJob);
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
        public async Task<ActionResult<Job>> Update([FromBody] Job newJob) {
            try {
                if (await _util.isCallerAdmin() || await _util.isTeamLeader(newJob.TeamID)) {
                    Job obj = await _context.Jobs.FindAsync(newJob.ID);
                    _util.IsItNull(obj);


                    obj.Deadline = newJob.Deadline;
                    obj.LastModified = DateTime.Now;
                    obj.Name = newJob.Name;
                    obj.Description = newJob.Description;
                    obj.RecurringType = newJob.RecurringType;
                    obj.Multiplier = newJob.Multiplier;
                    obj.JobWeight = newJob.JobWeight;

                    obj.isRead = newJob.isRead;
                    _util.updateJobGroupCounter(obj, newJob);
                    obj.isDone = newJob.isDone;
                    

                    if (newJob.JobTagID == null)   
                        obj.JobTagID = 1; 
                    else
                        obj.JobTagID = newJob.JobTagID;


                    obj.PersonID = newJob.PersonID;
                    obj.TeamID = newJob.TeamID;
                    obj.JobGroupID = newJob.JobGroupID;

                    obj.Team = await _context.Teams.FindAsync(obj.TeamID);
                    obj.Person = await _context.Persons.FindAsync(obj.PersonID);
                    obj.JobGroup = await _context.JobGroups.FindAsync(obj.JobGroupID);
                    obj.JobTag = await _context.JobTags.FindAsync(obj.JobTagID);

                    
                    _context.Jobs.Update(obj);
                    await _context.SaveChangesAsync();

                    Person p = await _context.Persons.FindAsync(obj.PersonID);
                    if (_context.caller.ID != p.ID) {
                        await _util.SendReloadJobs(p.SessionID);
                        await _util.SendReloadNotifications(p.SessionID);
                        // await _util.sendEmail(p.Email,"Job updated: " + obj.Name, EmailInfo.setBody("JOB", obj.Name, _context.caller.Email, "Date:" + obj.Deadline.ToShortDateString(), obj.Description));
                    }

                    return Ok(obj);
                }

                if (await _util.isCallerEmployee()) {
                    Job obj = await _context.Jobs.FindAsync(newJob.ID);
                    _util.IsItNull(obj);

                    if (obj.isDone != newJob.isDone) {
                        Person p = await _context.Persons.Where(p => p.Email == newJob.Contact).FirstOrDefaultAsync();
                        if (_context.caller.ID != p.ID) {
                            await _util.SendReloadJobs(p.SessionID);
                            await _util.SendReloadNotifications(p.SessionID);
                            //await _util.sendEmail(obj.Contact,"Job updated: " + obj.Name, EmailInfo.setBody("JOB", obj.Name, _context.caller.Email, "Date:" + obj.Deadline.ToShortDateString(), "STATUS: " + obj.isDone));
                        }
                    }

                    obj.isRead = newJob.isRead;
                    _util.updateJobGroupCounter(obj, newJob);
                    obj.isDone = newJob.isDone;  

                    _context.Jobs.Update(obj);
                    await _context.SaveChangesAsync();
                    
                    
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

        [Route("Delete/{jID}")]
        [HttpDelete]
        public async Task<ActionResult<Job>> Delete(int jID) {
            try {
                Job obj = await _context.Jobs.FindAsync(jID);
                    _util.IsItNull(obj);

                if (await _util.isCallerAdmin() || await _util.isTeamLeader(obj.TeamID)) {
                    if (obj.JobGroupID != null) {
                        if (obj.isDone == false) {
                            JobGroup jg = await _context.JobGroups.FindAsync(obj.JobGroupID);
                            _util.IsItNull(jg);

                            jg.Counter--;
                            _context.JobGroups.Update(jg);
                        }
                    }
                    
                    _context.Jobs.Remove(obj);
                    await _context.SaveChangesAsync();

                    Person p = await _context.Persons.FindAsync(obj.PersonID);
                    if (_context.caller.ID != p.ID)
                        await _util.SendReloadJobs(p.SessionID);
                    
                    return Ok("Job" + Info.Deleted);
                } else 
                    return BadRequest(Info.NoPermission);

            } catch (ReMindException re) {
                return BadRequest(re.message);
            
            } catch (Exception e) {
                _util.useException(e);
                return BadRequest(Info.Unknown);
            } 
        }

        [Route("UpdateAll")]
        [HttpPut]
        public async Task<ActionResult<Job>> UpdateAll([FromBody] Job newJob) {
            try {
                if (await _util.isCallerAdmin() || await _util.isTeamLeader(newJob.TeamID)) {

                    Job objJ = await _context.Jobs.FindAsync(newJob.ID);
                    _util.IsItNull(objJ);

                    List<Job> arr = await _util.getSameJobs(objJ);
                    foreach(Job obj in arr) {
                        obj.Deadline = newJob.Deadline;
                        obj.LastModified = DateTime.Now;
                        obj.Name = newJob.Name;
                        obj.Description = newJob.Description;
                        obj.RecurringType = newJob.RecurringType;
                        obj.Multiplier = newJob.Multiplier;
                        obj.JobWeight = newJob.JobWeight;

                        obj.isRead = obj.isRead;
                        _util.updateJobGroupCounter(obj, newJob);
                        obj.isDone = obj.isDone;

                        obj.JobTagID = newJob.JobTagID;
                        obj.TeamID = newJob.TeamID;
                        obj.JobGroupID = newJob.JobGroupID;
                        
                        obj.Team = await _context.Teams.FindAsync(obj.TeamID);
                        obj.Person = await _context.Persons.FindAsync(obj.PersonID);
                        obj.JobGroup = await _context.JobGroups.FindAsync(obj.JobGroupID);
                        obj.JobTag = await _context.JobTags.FindAsync(obj.JobTagID);

                        Person p = await _context.Persons.FindAsync(obj.PersonID);
                        if (_context.caller.ID != p.ID) {
                            await _util.SendReloadJobs(p.SessionID);
                            await _util.SendReloadNotifications(p.SessionID);
                            // await _util.sendEmail(p.Email,"Job updated: " + obj.Name, EmailInfo.setBody("JOB", obj.Name, obj.Contact, "Date:" + obj.Deadline.ToShortDateString(), obj.Description));
                        }

                        _context.Jobs.Update(obj);
                    }                    
                    await _context.SaveChangesAsync();

                    return Ok(objJ);
                }

                if (await _util.isCallerEmployee()) {
                    Job obj = await _context.Jobs.FindAsync(newJob.ID);
                    _util.IsItNull(obj);

                    if (obj.isDone != newJob.isDone) {
                        Person p = await _context.Persons.Where(p => p.Email == newJob.Contact).FirstOrDefaultAsync();
                        if (_context.caller.ID != p.ID) {
                            await _util.SendReloadJobs(p.SessionID);
                            await _util.SendReloadNotifications(p.SessionID);
                            //await _util.sendEmail(obj.Contact,"Job updated: " + obj.Name, EmailInfo.setBody("JOB", obj.Name, _context.caller.Email, "Date:" + obj.Deadline.ToShortDateString(), "STATUS: " + obj.isDone));
                        }
                    }

                    obj.isRead = newJob.isRead;
                    _util.updateJobGroupCounter(obj, newJob);
                    obj.isDone = newJob.isDone;  

                    _context.Jobs.Update(obj);
                    await _context.SaveChangesAsync();                    
                    
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

        [Route("DeleteAll/{jID}")]
        [HttpDelete]
        public async Task<ActionResult<Job>> DeleteAll(int jID) {
            try {
                Job objJ = await _context.Jobs.FindAsync(jID);
                _util.IsItNull(objJ);

                if (await _util.isCallerAdmin() || await _util.isTeamLeader(objJ.TeamID)) {
                    List<Job> arr = await _util.getSameJobs(objJ);
                    foreach(Job obj in arr) {
                        if (obj.JobGroupID != null) {
                            if (obj.isDone == false) {
                                JobGroup jg = await _context.JobGroups.FindAsync(obj.JobGroupID);
                                _util.IsItNull(jg);

                                jg.Counter--;
                                _context.JobGroups.Update(jg);
                            }
                        }
                        _context.Jobs.Remove(obj);
                        Person p = await _context.Persons.FindAsync(obj.PersonID);
                        if (_context.caller.ID != p.ID)
                            await _util.SendReloadJobs(p.SessionID);
                    }             
                    
                    await _context.SaveChangesAsync();                    
                    return Ok("Job" + Info.Deleted);
                } else 
                    return BadRequest(Info.NoPermission);

            } catch (ReMindException re) {
                return BadRequest(re.message);
            
            } catch (Exception e) {
                _util.useException(e);
                return BadRequest(Info.Unknown);
            } 
        }

        [Route("GetByID/{jID}")]
        [HttpGet]
        public async Task<ActionResult<List<Job>>> GetByID(int jID) {
            try {
                Job objJ = await _context.Jobs.FindAsync(jID);
                _util.IsItNull(jID);

                if (await _util.isCallerAdmin() || await _util.isTeamLeader(objJ.TeamID)) {
                    List<Job> arr = await _util.getSameJobs(objJ);
                    return Ok(arr);
                }                
                return BadRequest(Info.NoPermission);

            } catch (ReMindException re) {
                return BadRequest(re.message);
            
            } catch (Exception e) {
                _util.useException(e);
                return BadRequest(Info.Unknown);
            } 
        }

        [Route("GetByDateAll")]
        [HttpPost]
        public async Task<ActionResult<List<Job>>> GetByDateAll([FromBody] FromToDate dObj) {
            try {
                if (await _util.isCallerAdmin()) {
                    List<Job> arr;
                    if (dObj.DateFrom != null && dObj.DateTo != null)
                        arr = await _context.Jobs.Where(j => dObj.DateFrom <= j.Deadline && j.Deadline <= dObj.DateTo)
                                                    .Include(j => j.Person)
                                                    .Include(j => j.Team)
                                                    .Include(j => j.JobGroup)
                                                    .ToListAsync();
                    else
                        arr = await _context.Jobs   .Include(j => j.Person)
                                                    .Include(j => j.Team)
                                                    .Include(j => j.JobGroup)
                                                    .ToListAsync();
                    return Ok(arr);
                }                
                return BadRequest(Info.NoPermission);

            } catch (ReMindException re) {
                return BadRequest(re.message);
            
            } catch (Exception e) {
                _util.useException(e);
                return BadRequest(Info.Unknown);
            } 
        }

        [Route("GetByDatePerson/{pID}")]
        [HttpPost]
        public async Task<ActionResult<List<Job>>> GetByDatePerson([FromBody] FromToDate dObj, int pID) {
            try {
                if (await _util.isCallerAdmin() || _context.caller.ID == pID) {
                    List<Job> arr;
                    if (dObj.DateFrom != null && dObj.DateTo != null)
                        arr = await _context.Jobs.Where(j => dObj.DateFrom <= j.Deadline && j.Deadline <= dObj.DateTo && j.PersonID == pID).Include(j => j.Person)
                                                    .Include(j => j.Team)
                                                    .Include(j => j.JobGroup)
                                                    .ToListAsync();
                    else
                        arr = await _context.Jobs.Where(j => j.PersonID == pID).Include(j => j.Person)
                                                    .Include(j => j.Team)
                                                    .Include(j => j.JobGroup)
                                                    .ToListAsync();
                    return Ok(arr);
                }                
                return BadRequest(Info.NoPermission);

            } catch (ReMindException re) {
                return BadRequest(re.message);
            
            } catch (Exception e) {
                _util.useException(e);
                return BadRequest(Info.Unknown);
            } 
        }

        [Route("GetByDateTeam/{tID}")]
        [HttpPost]
        public async Task<ActionResult<List<Job>>> GetByDateTeam([FromBody] FromToDate dObj, int tID) {
            try {
                if (await _util.isCallerAdmin() || await _util.isTeamLeader(tID)) {
                    List<Job> arr;
                    if (dObj.DateFrom != null && dObj.DateTo != null)
                        arr = await _context.Jobs.Where(j => dObj.DateFrom <= j.Deadline && j.Deadline <= dObj.DateTo && j.TeamID == tID).Include(j => j.Person)
                                                    .Include(j => j.Team)
                                                    .Include(j => j.JobGroup)
                                                    .ToListAsync();
                    else
                        arr = await _context.Jobs.Where(j => j.TeamID == tID).Include(j => j.Person)
                                                    .Include(j => j.Team)
                                                    .Include(j => j.JobGroup)
                                                    .ToListAsync();
                    return Ok(arr);
                }                
                return BadRequest(Info.NoPermission);

            } catch (ReMindException re) {
                return BadRequest(re.message);
            
            } catch (Exception e) {
                _util.useException(e);
                return BadRequest(Info.Unknown);
            } 
        }

        [Route("GetByDateGroup/{jgID}")]
        [HttpPost]
        public async Task<ActionResult<List<Job>>> GetByDateGroup([FromBody] FromToDate dObj, int jgID) {
            try {
                if (await _util.isCallerAdmin() || await _util.isCallerLeader()) {
                    List<Job> arr;
                    if (dObj.DateFrom != null && dObj.DateTo != null)
                        arr = await _context.Jobs.Where(j => dObj.DateFrom <= j.Deadline && j.Deadline <= dObj.DateTo && j.JobGroupID == jgID).Include(j => j.Person)
                                                    .Include(j => j.Team)
                                                    .Include(j => j.JobGroup)
                                                    .ToListAsync();
                    else
                        arr = await _context.Jobs.Where(j => j.JobGroupID == jgID).Include(j => j.Person)
                                                    .Include(j => j.Team)
                                                    .Include(j => j.JobGroup)
                                                    .ToListAsync();
                    return Ok(arr);
                }            
                return BadRequest(Info.NoPermission);

            } catch (ReMindException re) {
                return BadRequest(re.message);
            
            } catch (Exception e) {
                _util.useException(e);
                return BadRequest(Info.Unknown);
            } 
        }

        [Route("WelcomeNewPerson/{pID}")]
        [HttpGet]
        public async Task<ActionResult<List<Job>>> WelcomeNewPerson(int pID) {
            try {
                if (await _util.isCallerAdmin()) {
                    Job jobChangePassword = new Job();
                        jobChangePassword.Name = "Change Password!";
                        jobChangePassword.Description = "Important!\nChange your current password!";
                        jobChangePassword.RecurringType = RecurringType.NonRecurring;
                        jobChangePassword.Multiplier = 0;
                        jobChangePassword.JobTagID = 1;
                        jobChangePassword.Team = await _context.Teams.Where(t => t.Name == "Welcome").FirstOrDefaultAsync();
                        jobChangePassword.TeamID = jobChangePassword.Team.ID;
                        jobChangePassword.PersonID = pID;
                        jobChangePassword.JobGroupID = null;
                        jobChangePassword.JobWeight = 0;                        
                        jobChangePassword.Person = await _context.Persons.FindAsync(pID);
                        jobChangePassword.JobGroup = null;
                        jobChangePassword.JobTag = await _context.JobTags.Where(t => t.ID == 1).FirstOrDefaultAsync();
                        jobChangePassword.Deadline = DateTime.Now.Date.AddDays(7);
                        

                        jobChangePassword.isRead = false;
                        jobChangePassword.LastModified = DateTime.Now;
                        jobChangePassword.isDone = false;
                        jobChangePassword.Contact = _context.caller.Email;

                        Job welcomeToTheTeam = new Job();
                        welcomeToTheTeam.Name = "Welcome to the team!";
                        welcomeToTheTeam.RecurringType = RecurringType.NonRecurring;
                        welcomeToTheTeam.Multiplier = 0;
                        welcomeToTheTeam.JobTagID = 1;
                        welcomeToTheTeam.Team = await _context.Teams.Where(t => t.Name == "Welcome").FirstOrDefaultAsync();
                        welcomeToTheTeam.TeamID = jobChangePassword.Team.ID;
                        welcomeToTheTeam.PersonID = pID;
                        welcomeToTheTeam.JobGroupID = null;
                        welcomeToTheTeam.JobWeight = 0;
                        welcomeToTheTeam.Person = await _context.Persons.FindAsync(pID);
                        welcomeToTheTeam.JobGroup = null;
                        welcomeToTheTeam.JobTag = await _context.JobTags.Where(t => t.ID == 1).FirstOrDefaultAsync();
                        welcomeToTheTeam.Deadline = DateTime.Now.Date.AddDays(7);
                        welcomeToTheTeam.Description = $"Important!\nWelcome to the team, {jobChangePassword.Person.Name} ! You're currently viewing a job, press the finished box in the right " +
                        $"corner and after that the UPDATE button to save the changes.\nAfter doing that your leader will confirm the change and you are clear to go!";

                    welcomeToTheTeam.isRead = false;
                        welcomeToTheTeam.LastModified = DateTime.Now;
                        welcomeToTheTeam.isDone = false;
                        welcomeToTheTeam.Contact = _context.caller.Email;

                        _context.Jobs.Add(jobChangePassword);
                        _context.Jobs.Add(welcomeToTheTeam);
                        await _context.SaveChangesAsync();
                    return Ok(Info.SuccessWelcome);
                }            
                return BadRequest(Info.NoPermission);

            } catch (ReMindException re) {
                return BadRequest(re.message);
            
            } catch (Exception e) {
                _util.useException(e);
                return BadRequest(Info.Unknown);
            } 
        }

        [Route("Archive")]
        [HttpPost]
        public async Task<ActionResult<string>> Archive([FromBody] Job finJob) {
            try {
                if (await _util.isCallerAdmin() || await _util.isCallerLeader()) {

                    Job objJ = await _context.Jobs.FindAsync(finJob.ID);
                    _util.IsItNull(objJ);

                    List<Job> arr = await _util.getSameJobs(objJ);

                    foreach(Job obj in arr) {
                        JobArchive objA = new JobArchive();
                        
                        objA.Deadline = obj.Deadline;
                        objA.LastModified = obj.LastModified;
                        objA.Finished = (obj.isDone) ? obj.LastModified : null;
                        objA.Name = obj.Name;
                        objA.Contact = obj.Contact;
                        objA.JobWeight = obj.JobWeight;

                        if (obj.JobGroupID != null) {
                            JobGroup jgPH = await _context.JobGroups.FindAsync(obj.JobGroupID);
                            objA.JobGroupName = jgPH.Name;
                        }
                        else
                        {

                        }

                        JobTag jtPH = await _context.JobTags.FindAsync(obj.JobTagID);
                        Team tnPH = await _context.Teams.FindAsync(obj.TeamID);

                       
                        objA.JobTagName = jtPH.Name;
                        objA.tdID = tnPH.ID;
                        objA.TeamName = tnPH.Name;

                        objA.PersonID = obj.PersonID;

                        _context.JobArchives.Add(objA);

                        if (obj.RecurringType != RecurringType.NonRecurring) {
                            Job jobToArhive = new Job();
                            jobToArhive.Name = obj.Name;
                            jobToArhive.Description = obj.Description;
                            jobToArhive.RecurringType = obj.RecurringType;
                            jobToArhive.Multiplier = obj.Multiplier;
                            jobToArhive.JobTagID = obj.JobTagID;
                            jobToArhive.TeamID = obj.TeamID;
                            jobToArhive.PersonID = obj.PersonID;
                            jobToArhive.JobGroupID = obj.JobGroupID;
                            jobToArhive.JobWeight = obj.JobWeight;
                            jobToArhive.Team = await _context.Teams.FindAsync(obj.TeamID);
                            jobToArhive.Person = await _context.Persons.FindAsync(obj.PersonID);
                            jobToArhive.JobGroup = await _context.JobGroups.FindAsync(obj.JobGroupID);
                            jobToArhive.JobTag = await _context.JobTags.FindAsync(obj.JobTagID);

                            jobToArhive.isRead = false;
                            jobToArhive.LastModified = DateTime.Now;
                            jobToArhive.isDone = false;
                            jobToArhive.Contact = obj.Contact;

                            switch(obj.RecurringType) {
                                case RecurringType.Daily:
                                    jobToArhive.Deadline = obj.Deadline.AddDays(obj.Multiplier * 1);
                                    break;
                                case RecurringType.Weekly:
                                    jobToArhive.Deadline = obj.Deadline.AddDays(obj.Multiplier * 7);
                                    break;
                                case RecurringType.Monthly:
                                    jobToArhive.Deadline = obj.Deadline.AddMonths(obj.Multiplier * 1);
                                    break;
                                default:
                                    throw new ReMindException(Info.ReccuringTypeError);
                            }
                            _context.Jobs.Add(jobToArhive);

                        }
                        Person p = await _context.Persons.FindAsync(obj.PersonID);
                        if (_context.caller.ID != p.ID) {
                            await _util.SendReloadJobs(p.SessionID);
                            await _util.SendReloadNotifications(p.SessionID);                  
                        }
                    }
                    await _context.SaveChangesAsync();
                    return Ok(Info.SuccessArchive);
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