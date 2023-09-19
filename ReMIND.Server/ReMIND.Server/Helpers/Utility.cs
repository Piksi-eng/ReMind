using System;
using ReMIND.Server.Data;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Text;
using System.Net.Mail;
using ReMIND.Server.Helpers.Email;
using System.Net;
using Microsoft.AspNetCore.SignalR;
using ReMIND.Server.Hubs;

namespace ReMIND.Server.Helpers
{
    public class Utility
    {
        Data.ReMindContext _context;
        IHubContext<NotificationHub> _nh; 

        public Utility(Data.ReMindContext context, IHubContext<NotificationHub> hubContext) {
            _context = context;
            _nh = hubContext;
        }

        #region Caller
        public void checkCaller() {
            if (_context.caller == null)            
                throw new ReMindException(Info.NoSession);
            if (_context.caller.IsEmployed == false)
                throw new ReMindException(Info.Unemployed);
            _context.caller.LastActive = DateTime.Now;
        }

        public async Task<TitleType> getCallerTitle() {
            List<TeamLink> tl = await _context.TeamLinks.Where(obj => obj.Person.ID == _context.caller.ID).ToListAsync();
            if (tl == null || tl.Count() == 0) {
                throw new ReMindException(Info.NoPrivileges);
            }
            TitleType maxRank = tl.Max(obj => obj.Title);
            return maxRank;           
        }

        public async Task<bool> isCallerAdmin() {
            TitleType isAdmin = await this.getCallerTitle();
            return isAdmin >= TitleType.Admin ? true : false;
        }

        public async Task<bool> isCallerLeader() {
            TitleType isLeader = await this.getCallerTitle();
            return isLeader >= TitleType.Leader ? true : false;
        }

        public async Task<bool> isCallerEmployee() {
            TitleType isEmployee = await this.getCallerTitle();
            return isEmployee >= TitleType.Employee ? true : false;
        }

        public async Task<bool> isTeamLeader(int tID) {
            TeamLink tl = await _context.TeamLinks.Where(obj => obj.Person.ID == _context.caller.ID && obj.TeamID == tID && obj.Title == TitleType.Leader).FirstOrDefaultAsync();
            return (tl != null) ? true : false;
        }

        public async Task<bool> isInTeam(int tID) {
            TeamLink tl = await _context.TeamLinks.Where(obj => obj.Person.ID == _context.caller.ID && obj.TeamID == tID).FirstOrDefaultAsync();
            return (tl != null) ? true : false;
        }
        #endregion

        #region Person Check 
        public async Task<Person> EmailExists(string email) {
            Person obj = await _context.Persons.Where(p => p.Email == email).FirstOrDefaultAsync();
            if (obj == null)
                throw new ReMindException(Info.WrongEmail);
            return  obj;
        }

        public async Task isEmailUsed(string email) {
            if (await Task.FromResult(_context.Persons.Any(p => p.Email == email))) {
                throw new ReMindException(Info.EmailUsed); 
            }
        }

        public async Task isPhoneUsed(string phone) {
            if (await Task.FromResult(_context.Persons.Any(p => p.Phone == phone)))
                throw new ReMindException(Info.PhoneUsed);
        }
        #endregion

        #region Taken
        public async Task isTeamNameTaken(string name) {
            if (await Task.FromResult(_context.Teams.Any(t => t.Name.ToLower() == name.ToLower()))) {
                throw new ReMindException(Info.TeamNameUsed); 
            }
        }

        public async Task isTeamLinkTaken(TitleType t, int pID, int tID) {
            if (await Task.FromResult(_context.TeamLinks.Any(tl => tl.Title == t && tl.PersonID == pID && tl.TeamID == tID))) {
                throw new ReMindException(Info.TeamLinkUsed); 
            }
        }

        public async Task isJobTagTaken(string name, string color) {
            if (await Task.FromResult(_context.JobTags.Any(jt => jt.Name.ToLower() == name.ToLower() || jt.Color == color))) {
                throw new ReMindException(Info.JobTagNameColorUsed); 
            }
        }

        public async Task isJobTagNameTaken(string name) {
            if (await Task.FromResult(_context.JobTags.Any(jt => jt.Name.ToLower() == name.ToLower()))) {
                throw new ReMindException(Info.JobTagNameUsed); 
            }
        }

        public async Task isJobTagColorTaken(string color) {
            if (await Task.FromResult(_context.JobTags.Any(jt => jt.Color == color))) {
                throw new ReMindException(Info.JobTagColorUsed); 
            }
        }
        #endregion
        
        #region Password
        public string hashPassword(string pass) {
            string password = BCrypt.Net.BCrypt.HashPassword(pass);
            return password;
        }

        public bool verifyPassword(string passedPassword, string databasePassword) {
            bool verified = BCrypt.Net.BCrypt.Verify(passedPassword, databasePassword);
            if (verified == false)
                throw new ReMindException(Info.BadPassword);
            return verified;
        }
        #endregion

        #region Helper Functions
        public void useException(Exception e) {
            Console.WriteLine(e.StackTrace);
            //add logger or something else later
        }

        public void IsItNull(Object obj) {
            if (obj == null)
                throw new ReMindException(Info.NoObject);
        }

        public async void updateJobGroupCounter(Job oldJob, Job newJob) {
            if (oldJob.isDone != newJob.isDone) {
                if (newJob.JobGroupID != null) {
                    JobGroup jg = await _context.JobGroups.FindAsync(newJob.JobGroupID);
                    IsItNull(jg);
                    
                    if (newJob.isDone == true) {
                    jg.Counter--;
                    _context.JobGroups.Update(jg);
                    }                  

                    if (newJob.isDone == false) {
                        jg.Counter++;
                        _context.JobGroups.Update(jg);
                    }  
                }
            }
        }

        public async Task<List<Job>> getSameJobs(Job objJ) {
            List<Job> arr = await _context.Jobs.Where(j => j.Deadline == objJ.Deadline && j.Name == objJ.Name && j.TeamID == objJ.TeamID)
                                                                .Include(j => j.Person)
                                                                 .Include(j => j.Team)
                                                              .Include(j => j.JobGroup)
                                                               .ToListAsync();
            return arr;
        }
        #endregion
    
        #region login 
        public (string email, string password) authDecode(string authHeader) {
            string[] arr = authHeader.Split(':', 2);
            string email = arr[0];
            string password = arr[1];

            return (email, password);
        }
        #endregion

        #region sendEmail
        public async Task sendEmail(string recipientEmail, string subject, string body, string displayName = "ReMIND Application") {
            try
            {
                using (SmtpClient client = new SmtpClient(EmailInfo.SmtpClient))
                {
                    client.Port = EmailInfo.Port;
                    client.Credentials = new NetworkCredential(EmailInfo.HostEmail, EmailInfo.HostPassword);
                    client.EnableSsl = true;

                    using (MailMessage msg = new MailMessage(EmailInfo.HostEmail, recipientEmail, subject, body))
                    {
                        msg.From = new MailAddress(EmailInfo.HostEmail, displayName);
                        msg.IsBodyHtml = true;
                        await client.SendMailAsync(msg);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Email send failure", ex);
            }
        }
        #endregion

        #region hubs
        public async Task SendReloadJobs(string sID) {
            await _nh.Clients.Group(sID).SendAsync("ReloadJobs");
        }

        public async Task SendReloadNotifications(string sID) {
            await _nh.Clients.Group(sID).SendAsync("ReloadNotifications");
        }
        #endregion
    }
}