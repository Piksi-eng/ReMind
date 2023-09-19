using Microsoft.AspNetCore.Mvc;
using ReMIND.Server.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ReMIND.Server.Helpers;

namespace ReMIND.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamController: ControllerBase
    {
        ReMindContext _context;
        Utility _util;        

        public TeamController(ReMindContext context, Utility util) {
            _context = context;
            _util = util;
        }

        [Route("Create")]
        [HttpPost]
        public async Task<ActionResult<Team>> Create([FromBody] Team newTeam) {
            try {                
                if (await _util.isCallerAdmin()) {

                    await _util.isTeamNameTaken(newTeam.Name);

                    _context.Teams.Add(newTeam);
                    await _context.SaveChangesAsync();
                    
                    return Ok(newTeam);
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
        public async Task<ActionResult<Team>> Update([FromBody] Team newTeam) {
            try {                
                if (await _util.isCallerAdmin()) {

                    Team obj = await _context.Teams.FindAsync(newTeam.ID);
                    _util.IsItNull(obj);

                    if (obj.Name != newTeam.Name)
                        await _util.isTeamNameTaken(newTeam.Name);

                    List<JobArchive> listJA = await _context.JobArchives.Where(ja => ja.TeamName == obj.Name).ToListAsync();
                    obj.Name = newTeam.Name;

                    listJA.ForEach(ja => {
                        ja.TeamName =  newTeam.Name;
                        _context.JobArchives.Update(ja);
                    });

                    _context.Teams.Update(obj);
                    await _context.SaveChangesAsync();
                    
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

        [Route("Delete/{tID}")]
        [HttpDelete]
        public async Task<ActionResult> Delete(int tID) {
            try {                
                if (await _util.isCallerAdmin()) {
                    
                    List<TeamLink> arrTL = await _context.TeamLinks.Where(tl => tl.TeamID == tID).ToListAsync();
                    _util.IsItNull(arrTL);

                    foreach(TeamLink tl in arrTL)
                        _context.TeamLinks.Remove(tl);

                    Team obj = await _context.Teams.FindAsync(tID);

                    _context.Teams.Remove(obj);
                    await _context.SaveChangesAsync();
                    
                    return Ok(obj.Name + Info.Deleted);
                } else
                    return BadRequest(Info.NoPermission);

            } catch (ReMindException re) {
                return BadRequest(re.message);
            
            } catch (Exception e) {
                _util.useException(e);
                return BadRequest(Info.Unknown);
            }            
        }

        [Route("GetByID/{tID}")]
        [HttpGet]
        public async Task<ActionResult<Team>> GetByID(int tID) {
            try {   
                if (await _util.isCallerAdmin() || await _util.isTeamLeader(tID)) {

                    Team obj = await _context.Teams.FindAsync(tID);
                    _util.IsItNull(obj);

                    return Ok(obj);
                } 
                
                if (await _util.isCallerEmployee()) {
                    TeamLink objTL = await _context.TeamLinks.Where(tl => tl.PersonID == _context.caller.ID && tl.TeamID == tID).FirstOrDefaultAsync();
                    _util.IsItNull(objTL);

                    Team obj = await _context.Teams.FindAsync(tID);
                    _util.IsItNull(obj);

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

        [Route("GetByPerson/{pID}")]
        [HttpGet]
        public async Task<ActionResult<List<Team>>> GetByPerson(int pID) {
            try {                
                if (await _util.isCallerAdmin() || _context.caller.ID == pID) {

                    List<TeamLink> arrTL = await _context.TeamLinks.Where(tl => tl.PersonID == pID).ToListAsync();
                    _util.IsItNull(arrTL);

                    List<Team> arr = new List<Team>();
                    foreach(TeamLink tl in arrTL) {
                        Team objTM = await _context.Teams.FindAsync(tl.TeamID);
                        arr.Add(objTM);
                    }

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

        //Testirati kad imamo JOB GROUP
        [Route("GetByGroup/{gID}")]
        [HttpGet]
        public async Task<ActionResult<Team>> GetByGroup(int gID) {
            try {
                JobGroup objJG = await _context.JobGroups.Where(jg => jg.ID == gID).FirstOrDefaultAsync();
                _util.IsItNull(objJG);

                if (await _util.isCallerAdmin() || await _util.isInTeam(objJG.TeamID)) {

                    Team obj = await _context.Teams.FindAsync(objJG.TeamID);

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

        [Route("GetAll")]
        [HttpGet]
        public async Task<ActionResult<List<Team>>> GetAll() {
            try {                
                if (await _util.isCallerAdmin()) {

                    List<Team> arr = await _context.Teams.ToListAsync();
                    _util.IsItNull(arr);

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
    }
}