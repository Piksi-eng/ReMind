using Microsoft.AspNetCore.Mvc;
using ReMIND.Server.Data;
using System.Threading.Tasks;
using ReMIND.Server.Helpers;
using ReMIND.Server.Helpers.Email;
using ReMIND.Server.Hubs;
using System;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ReMIND.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        ReMindContext _context;
        Utility _util;   
        IHubContext<NotificationHub> _nh;   

        public AdminController(ReMindContext context, Utility util, IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _util = util;
            _nh = hubContext;

        }

        //[Route("CreateJob")]
        //[HttpPost]
        //public async Task<ActionResult> CreateJob([FromBody] Job obj) {
        //    _context.Jobs.Add(obj);
        //    await _context.SaveChangesAsync();
        //    return Ok(obj);
        //}

        //[Route("CreateJobArchive")]
        //[HttpPost]
        //public async Task<ActionResult> CreateJobArchive([FromBody] JobArchive obj) {
        //    _context.JobArchives.Add(obj);
        //    await _context.SaveChangesAsync();
        //    return Ok(obj);
        //}

        //[Route("CreateJobTag")]
        //[HttpPost]
        //public async Task<ActionResult> CreateJobTag([FromBody] JobTag obj) {
        //    _context.JobTags.Add(obj);
        //    await _context.SaveChangesAsync();
        //    return Ok(obj);
        //}

        //[Route("CreatePerson")]
        //[HttpPost]
        //public async Task<ActionResult> CreatePerson([FromBody] Person obj)
        //{
        //    obj.Password = _util.hashPassword(obj.Password);
        //    _context.Persons.Add(obj);
        //    await _context.SaveChangesAsync();
        //    return Ok(obj);
        //}

        //[Route("CreateaJobGroup")]
        //[HttpPost]
        //public async Task<ActionResult> CreateJobGroup([FromBody] JobGroup obj) {
        //    _context.JobGroups.Add(obj);
        //    await _context.SaveChangesAsync();
        //    return Ok(obj);
        //}

        //[Route("CreateTeam")]
        //[HttpPost]
        //public async Task<ActionResult> CreateTeam([FromBody] Team obj) {
        //    _context.Teams.Add(obj);
        //    await _context.SaveChangesAsync();
        //    return Ok(obj);
        //}

        //[Route("CreateTeamLink")]
        //[HttpPost]
        //public async Task<ActionResult> CreateTeamLink([FromBody] TeamLink obj)
        //{
        //    _context.TeamLinks.Add(obj);
        //    await _context.SaveChangesAsync();
        //    return Ok(obj);
        //}

        //[Route("GetSalt")]
        //[HttpGet]
        //public async Task<ActionResult> GetSalt() {
        //    return await Task.FromResult(Ok(BCrypt.Net.BCrypt.GenerateSalt().Substring(6,7)));
        //}

        //[Route("SendMsg")]
        //[HttpGet]
        //public async Task<ActionResult> SendMsg() {
        //    await _nh.Clients.User("admin").SendAsync("Reload");
        //    return Ok("Empty");
        //}

        // [Route("TestEmail/{recipientEmail}/{subject}")]
        // [HttpGet]
        // public async Task<ActionResult> TestEmail(string recipientEmail, string subject) {
        //     await _util.sendEmail(recipientEmail, subject, EmailInfo.setJob("Naslov","test@example.com", (DateTime.Now).ToShortDateString(), "Lorem ipsum brt moj"));
        //     return Ok("Email has been sent");
        // }

        [Route("Test")]
        [HttpGet]
        public async Task<ActionResult> Test()
        {
            return Ok("ReMIND Server is currently running.");
        }
    }
}
