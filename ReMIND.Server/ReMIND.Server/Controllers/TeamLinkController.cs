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
    public class TeamLinkController: ControllerBase
    {
        ReMindContext _context;
        Utility _util;        

        public TeamLinkController(ReMindContext context, Utility util) {
            _context = context;
            _util = util;
        }

        [Route("Create")]
        [HttpPost]
        public async Task<ActionResult<TeamLink>> Create(TeamLink newTeamLink) {
            try {                
                if (await _util.isCallerAdmin()) {
                    
                    await _util.isTeamLinkTaken(newTeamLink.Title, newTeamLink.PersonID, newTeamLink.TeamID);
                    _context.TeamLinks.Add(newTeamLink);

                    await _context.SaveChangesAsync();

                    return Ok(newTeamLink);
                }
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
        public async Task<ActionResult<TeamLink>> Update(TeamLink newTeamLink) {
            try {                
                if (await _util.isCallerAdmin()) {
                    TeamLink tlObj = await _context.TeamLinks.FindAsync(newTeamLink.ID);
                    _util.IsItNull(tlObj);

                    tlObj.Title = newTeamLink.Title;
                    tlObj.PersonID = newTeamLink.PersonID;
                    tlObj.TeamID = newTeamLink.TeamID;

                    _context.TeamLinks.Update(tlObj);
                    await _context.SaveChangesAsync();

                    return Ok(newTeamLink);
                }
                return BadRequest(Info.NoPermission);

            } catch (ReMindException re) {
                return BadRequest(re.message);
            
            } catch (Exception e) {
                _util.useException(e);
                return BadRequest(Info.Unknown);
            }            
        }

        [Route("Delete/{tlID}")]
        [HttpDelete]
        public async Task<ActionResult<TeamLink>> Delete(int tlID) {
            try {                
                if (await _util.isCallerAdmin()) {

                    TeamLink obj = await _context.TeamLinks.FindAsync(tlID);

                    _context.TeamLinks.Remove(obj);
                    await _context.SaveChangesAsync();

                    return Ok("Link" + Info.Deleted);
                }
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
        public async Task<ActionResult<List<TeamLink>>> GetByPerson(int pID) {
            try {                
                if (await _util.isCallerEmployee() || await _util.isCallerLeader() || await _util.isCallerAdmin()) {

                    List<TeamLink> arr = await _context.TeamLinks.Where(t => t.PersonID == pID).Include(tl => tl.Person).Include(tl => tl.Team).ToListAsync();
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
        public async Task<ActionResult<List<TeamLink>>> GetByTeam(int tID) {
            try {                
                if (await _util.isCallerAdmin()) {

                    List<TeamLink> arr = await _context.TeamLinks.Where(t => t.TeamID == tID).Include(tl => tl.Person).Include(tl => tl.Team).ToListAsync();
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