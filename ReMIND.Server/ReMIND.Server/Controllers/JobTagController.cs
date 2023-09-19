using Microsoft.AspNetCore.Mvc;
using ReMIND.Server.Data;
using System;
using System.Threading.Tasks;
using ReMIND.Server.Helpers;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ReMIND.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobTagController: ControllerBase
    {
        ReMindContext _context;
        Utility _util;        

        public JobTagController(ReMindContext context, Utility util) {
            _context = context;
            _util = util;
            _util.checkCaller();
        }

        [Route("Create")]
        [HttpPost]
        public async Task<ActionResult<JobTag>> Create([FromBody] JobTag newJobTag) {
            try {                
                if (await _util.isCallerAdmin()) {

                    await _util.isJobTagTaken(newJobTag.Name, newJobTag.Color);

                    _context.JobTags.Add(newJobTag);
                    await _context.SaveChangesAsync();

                    return Ok(newJobTag);
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
        public async Task<ActionResult<JobTag>> Update([FromBody] JobTag newJobTag) {
            try {                
                if (await _util.isCallerAdmin()) {
                    
                    JobTag obj = await _context.JobTags.FindAsync(newJobTag.ID);

                    if (obj.Name != newJobTag.Name)
                        await _util.isJobTagNameTaken(newJobTag.Name);
                    if (obj.Color != newJobTag.Color)
                        await _util.isJobTagColorTaken(newJobTag.Color);
                    
                    obj.Name = newJobTag.Name;
                    obj.Color = newJobTag.Color;

                    _context.JobTags.Update(obj);
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

        [Route("Delete/{jtID}")]
        [HttpDelete]
        public async Task<ActionResult<JobTag>> Delete(int jtID) {
            try {                
                if (await _util.isCallerAdmin()) {
                    
                    JobTag obj = await _context.JobTags.FindAsync(jtID);
                    _util.IsItNull(obj);

                    List<Job> arr = await _context.Jobs.Where(j => j.JobTagID == jtID).ToListAsync();
                    foreach (Job j in arr) {
                        j.JobTagID = 1;
                        _context.Jobs.Update(j);
                    }

                    _context.JobTags.Remove(obj);
                    await _context.SaveChangesAsync();

                    return Ok(obj.Name + Info.Deleted);
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
        public async Task<ActionResult<List<JobTag>>> GetAll() {
            try {
                List<JobTag> arr = await _context.JobTags.ToListAsync();
                _util.IsItNull(arr);
                
                return Ok(arr);

            } catch (ReMindException re) {
                return BadRequest(re.message);
            
            } catch (Exception e) {
                _util.useException(e);
                return BadRequest(Info.Unknown);
            }            
        }

        [Route("GetNumberOfJobs/{jtID}")]
        [HttpGet]
        public async Task<ActionResult<List<JobTag>>> GetNumberOfJobs(int jtID) {
            try {
                if (await _util.isCallerAdmin()) {
                    int num = await _context.Jobs.Where(j => j.JobTagID == jtID).AsQueryable().CountAsync();
                    return Ok(num);
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