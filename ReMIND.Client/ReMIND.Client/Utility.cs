using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

using Newtonsoft.Json;

using ReMIND.Client.AnalyticsTab;
using ReMIND.Client.ArchiveTab;
using ReMIND.Client.Business;
using ReMIND.Client.Business.Models;
using ReMIND.Client.Business.Models.Types;
using ReMIND.Client.Management;
using ReMIND.Client.TasksTab;

namespace ReMIND.Client
{
    public static class Utility
    {
        public static bool TestEnv = false;

        #region Data & Properties
        public static bool employeeRestriction = false; 
        public static bool leaderRestriction = false;
        public static bool adminRestriction = false;
        public static Person User;
        public static List<Team> Teams;
        //ovo ispod su svi relevantni timovi u kojoj se osoba nalazi;
        public static List<Team> MyTeams = new();
        public static List<Team> TeamsILead = new();
        public static List<Person> People;
        public static List<Person> MyPeople;
        public static List<Job> MyJobs = new();
        public static TitleType UserTittle;
        public static List<JobTag> JobTags = new();
        public static MainWindow mainWindow { get; set; }
        //public static string API_URI = Properties.Settings.Default.API_URL;
        public static string API_URI = "https://localhost:5001";
        #endregion

        #region Auth
        public static async Task<Person> LoginUser(string auth)
        {
            if (!TestEnv)
            {

                //var values = new Dictionary<string, string>
                //{

                //};
                HttpResponseMessage response;
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (HttpClient client = new HttpClient(clientHandler))
                {

                    string uriplaceholder = API_URI + "/api/Auth/Login";
                    client.DefaultRequestHeaders.Add("Authentication", auth);
                    //var cont = new FormUrlEncodedContent(values);
                    response = await client.PostAsync(uriplaceholder, null);

                }

                if (!response.IsSuccessStatusCode)
                {
                    //ReMINDMessage.Show(await response.Content.ReadAsStringAsync(), false, "Failed Login");
                    throw new Exception(await response.Content.ReadAsStringAsync());
                }


                var content = await response.Content.ReadAsStringAsync();
                Person getUser = JsonConvert.DeserializeObject<Person>(content);

                response.Dispose();

                User = new() //testing purposes
                {
                    ID = getUser.ID,
                    Name = getUser.Name,
                    Email = getUser.Email,
                    Phone = getUser.Phone,
                    IsEmployed = true,
                    SessionID = getUser.SessionID
                };
                if (getUser.SessionID != null)
                    return User;
                else
                    return null;
            }
            else
            {
                return User = new Person
                {
                    ID = 3,
                    Name = "TestEnv",
                    Email = "TestMail@example.com",
                    Phone = "4206900",
                    IsEmployed = true,
                    SessionID = "admin"
                };
            }
        }

        public static async Task<bool> CheckSession(string sID)
        {
            if (!TestEnv)
            {

                var values = new Dictionary<string, string>
                {

                };
                HttpResponseMessage response;
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (HttpClient client = new HttpClient(clientHandler))
                {

                    string uriplaceholder = API_URI + "/api/Auth/CheckSession/" + sID + "";
                    //client.DefaultRequestHeaders.Add("Authentication", sID);
                    var cont = new FormUrlEncodedContent(values);
                    response = await client.PostAsync(uriplaceholder, cont);

                }

                if (!response.IsSuccessStatusCode)
                {
                    ReMINDMessage.Show(await response.Content.ReadAsStringAsync(), false, "Failed Login");

                    return false;
                }

                var content = await response.Content.ReadAsStringAsync();
                //string getUser = JsonConvert.DeserializeObject<string>(content);

                response.Dispose();


                if (content != null)
                    return true;
                else
                    return false;
            }
            else
            {
                return true;
            }
        }
        #endregion

        #region Team

        //not tested
        public static async Task<Team> CreateTeam(Team teamToCreate)
        {
            if (!TestEnv)
            {
                string NewTeamJson = JsonConvert.SerializeObject(teamToCreate);
                HttpResponseMessage response;
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (HttpClient client = new HttpClient(clientHandler))
                {

                    string uriplaceholder = API_URI + "/api/Team/Create";
                    client.DefaultRequestHeaders.Add("SessionID", User.SessionID);
                    var cont = new StringContent(NewTeamJson, Encoding.UTF8, "application/json");
                    response = await client.PostAsync(uriplaceholder, cont);

                }
                Team getTeam = new();
                if (!response.IsSuccessStatusCode)
                {
                    //throw new Exception(await response.Content.ReadAsStringAsync());
                    ReMINDMessage.Show(await response.Content.ReadAsStringAsync(), false, "Failuer");
                    return null;
                }
                    

                

                var content = await response.Content.ReadAsStringAsync();
                getTeam = JsonConvert.DeserializeObject<Team>(content);

                response.Dispose();

                return getTeam;


            }
            else //in test environment
            {
                Team getTeam = new Team();
                return getTeam;
            }
        }

        //not tested
        public static async Task<Team> UpdateTeam(Team teamToEdit)
        {
            if (!TestEnv)
            {
                string NewTeamJson = JsonConvert.SerializeObject(teamToEdit);
                HttpResponseMessage response;
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (HttpClient client = new HttpClient(clientHandler))
                {

                    string uriplaceholder = API_URI + "/api/Team/Update";
                    client.DefaultRequestHeaders.Add("SessionID", User.SessionID);
                    var cont = new StringContent(NewTeamJson, Encoding.UTF8, "application/json");
                    response = await client.PutAsync(uriplaceholder, cont);

                }
                if (!response.IsSuccessStatusCode)
                    throw new Exception(await response.Content.ReadAsStringAsync());

                Team getTeam;

                var content = await response.Content.ReadAsStringAsync();
                getTeam = JsonConvert.DeserializeObject<Team>(content);

                response.Dispose();


                return getTeam;

            }
            else //in test environment
            {
                Team getTeam = new Team();
                return getTeam;
            }
        }

        //nottested
        public static async Task<bool> DeleteTeam(Team teamToDelete)
        {
            if (!TestEnv)
            {
                HttpResponseMessage response;
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (HttpClient client = new HttpClient(clientHandler))
                {

                    string uriplaceholder = API_URI + "/api/Team/Delete/" + teamToDelete.ID;
                    client.DefaultRequestHeaders.Add("SessionID", User.SessionID);
                    response = await client.DeleteAsync(uriplaceholder);

                }
                if (!response.IsSuccessStatusCode)
                    throw new Exception(await response.Content.ReadAsStringAsync());

                //string getString;

                var content = await response.Content.ReadAsStringAsync();
                //getString = JsonConvert.DeserializeObject<string>(content);

                response.Dispose();

                if (content != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else //in test environment
            {
                return true;
            }
        }

        //Returns all teams for current person
        public static async Task<List<Team>> GetAllTeamsByPerson(Person Account = null)
        {
            if (!TestEnv)
            {
                if (Account == null)
                {
                    Account = User;
                }
                HttpResponseMessage response;
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (HttpClient client = new HttpClient(clientHandler))
                {

                    string uriplaceholder = API_URI + "/api/Team/GetByPerson/" + Account.ID;
                    client.DefaultRequestHeaders.Add("SessionID", User.SessionID);

                    response = await client.GetAsync(uriplaceholder);

                }

                List<Team> RetTeams = new List<Team>();

                if (!response.IsSuccessStatusCode)
                    throw new Exception(await response.Content.ReadAsStringAsync());

                var content = await response.Content.ReadAsStringAsync();
                RetTeams = JsonConvert.DeserializeObject<List<Team>>(content);

                response.Dispose();

                if (RetTeams.Any(x=> x.Name == "Welcome"))
                {
                    Team t = RetTeams.Single(r => r.Name == "Welcome");
                    if (t != null)
                        RetTeams.Remove(t);
                }


                return RetTeams;
            }
            else
            {
                Team TestTeam;
                Teams = new List<Team>();
                Teams.Add(TestTeam = new Team
                {
                    ID = 1,
                    Name = "Test tim 1"
                });
                Teams.Add(TestTeam = new Team
                {
                    ID = 2,
                    Name = "Test tim 2"
                });
                Teams.Add(TestTeam = new Team
                {
                    ID = 3,
                    Name = "Test tim 3"
                });


                return Teams;
            }
        }

        public static async Task<Team> GetTeamById(int id)
        {
            if (!TestEnv)
            {
                HttpResponseMessage response;
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (HttpClient client = new HttpClient(clientHandler))
                {

                    string uriplaceholder = API_URI + "/api/Team/GetByID/" + id;
                    client.DefaultRequestHeaders.Add("SessionID", User.SessionID);

                    response = await client.GetAsync(uriplaceholder);

                }

                Team RetTeams = new Team();

                if (!response.IsSuccessStatusCode)
                    throw new Exception(await response.Content.ReadAsStringAsync());

                var content = await response.Content.ReadAsStringAsync();
                RetTeams = JsonConvert.DeserializeObject<Team>(content);

                response.Dispose();



                return RetTeams;
            }
            else
            {
                Team RetTeams = new Team();
                return RetTeams;
            }
        }

        public static Team GetJobTeamByidLocal(int id)
        {
            Team tim = MyTeams.Find(x => x.ID == id);
            return tim;
        }

        //Returns all teams **only for admins** used in manage tab
        public static async Task<List<Team>> GetAllTeams()
        {
            if (!TestEnv)
            {
                HttpResponseMessage response;
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (HttpClient client = new HttpClient(clientHandler))
                {

                    string uriplaceholder = API_URI + "/api/Team/GetAll";
                    client.DefaultRequestHeaders.Add("SessionID", User.SessionID);

                    response = await client.GetAsync(uriplaceholder);
                }
                if (!response.IsSuccessStatusCode)
                    throw new Exception(await response.Content.ReadAsStringAsync());


                List<Team> AllTeams = new List<Team>();

                var content = await response.Content.ReadAsStringAsync();
                AllTeams = JsonConvert.DeserializeObject<List<Team>>(content);

                response.Dispose();
                AllTeams.RemoveAt(0);
                if (AllTeams.Any(x => x.Name == "Welcome"))
                {
                    Team t = AllTeams.Single(r => r.Name == "Welcome");
                    if (t != null)
                        AllTeams.Remove(t);
                }
                return AllTeams;

            }
            else
            {
                List<Team> AllTeams = new List<Team>();
                AllTeams.Add(new Team()
                {
                    Name = "Samo Ventilatori",
                    Jobs = new(),
                    TeamLink = new(),
                    TaskGroup = new()
                });
                AllTeams.Add(new Team()
                {
                    Name = "Tim Raketa",
                    Jobs = new(),
                    TeamLink = new(),
                    TaskGroup = new()
                });
                AllTeams.Add(new Team()
                {
                    Name = "Hogwartz Express",
                    Jobs = new(),
                    TeamLink = new(),
                    TaskGroup = new()
                });
                return AllTeams;
            }
        }

        public static async Task<bool> GetAllMyTeams()
        {
            if (!TestEnv)
            {
                if (UserTittle == TitleType.Admin)
                {
                    MyTeams = await GetAllTeams();
                }
                else //leaders and empoleyes
                {
                    MyTeams = await GetAllTeamsByPerson(User);
                }
                return true;


            }
            else
            {


                return true;
            }
        }

        public static async Task<List<Team>> GetTeamsByLeader(Person p)
        {
            List<Team> retTeams = new();
            if (UserTittle == TitleType.Admin)
            {
                TeamsILead = await GetAllTeams();
                retTeams = TeamsILead;
                return retTeams;
            }

            List <TeamLink> teamLinks = await GetTeamLinksByPerson(p);

            foreach (TeamLink tl in teamLinks)
            {
                if (tl.personID == p.ID && tl.Title == TitleType.Leader)
                    retTeams.Add(await GetTeamById(tl.teamID));
            }
            TeamsILead = retTeams;

            return retTeams;
        }
        #endregion

        #region Person

        //Returns True if success
        public static async Task<Person> CreatePerson(Person newPerson)
        {
            if (!TestEnv)
            {
                //za sada svakome se ostavlja ovaj password
                newPerson.Password = "admin123";
                string NewPersonJson = JsonConvert.SerializeObject(newPerson);
                HttpResponseMessage response;
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (HttpClient client = new HttpClient(clientHandler))
                {

                    string uriplaceholder = API_URI + "/api/Person/Create";
                    client.DefaultRequestHeaders.Add("SessionID", User.SessionID);
                    var cont = new StringContent(NewPersonJson, Encoding.UTF8, "application/json");
                    response = await client.PostAsync(uriplaceholder, cont);

                }
                if (!response.IsSuccessStatusCode)
                {
                    ReMINDMessage.Show(await response.Content.ReadAsStringAsync(), false, "Failure");
                    Person NullUser = null;
                    return NullUser;
                }
                //throw new Exception(await response.Content.ReadAsStringAsync());

                Person GetUser;

                var content = await response.Content.ReadAsStringAsync();
                GetUser = JsonConvert.DeserializeObject<Person>(content);

                await CreateWelcomeJob(GetUser.ID);

                response.Dispose();

                return GetUser;

            }
            else //in test environment
            {
                Person GetUser = new Person
                {
                    Name = "TestPerson2",
                    Email = "TestMail2@example.com",
                    Phone = "42069002",
                    Password = "123",
                    IsEmployed = true,
                    SessionID = "admin"
                };
                return GetUser;
            }
        }

        //Returns all persons **admin only* used in manage page
        public static async Task<List<Person>> GetAllPeople()
        {
            if (!TestEnv)
            {
                HttpResponseMessage response;
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (HttpClient client = new HttpClient(clientHandler))
                {

                    string uriplaceholder = API_URI + "/api/Person/GetAll";
                    client.DefaultRequestHeaders.Add("SessionID", User.SessionID);

                    response = await client.GetAsync(uriplaceholder);
                }
                if (!response.IsSuccessStatusCode)
                    throw new Exception(await response.Content.ReadAsStringAsync());

                var content = await response.Content.ReadAsStringAsync();
                People = JsonConvert.DeserializeObject<List<Person>>(content);

                response.Dispose();

                return People;

            }
            else
            {
                People = new List<Person>();
                People.Add(new Person(1, "stojko", "mail.com", "06412345", true, "123", new()));
                People.Add(new Person(1, "milojko", "nomail.com", "06412345", true, "123", new()));
                People.Add(new Person(1, "radasin", "letters.com", "06412345", true, "123", new()));
                return People;
            }
        }

        public static async Task GetAllMyPeople()
        {
            if(UserTittle == TitleType.Admin)
            {
                MyPeople = await GetAllPeople();
            }
            else
            {
                foreach(Team t in MyTeams)
                {
                    MyPeople = MyPeople.Union(await GetAllPeopleByTeam(t)).ToList();
                }
            }

        }

        public static async Task<bool> UpdatePerson(Person newUser)
        {
            if (!TestEnv)
            {

                //nije bitno sta pise ovde samo da ne puca kontroler ne pitaj
                //ne pitaj
                newUser.Password = "123";
                //ovo ne menja password

                string NewUserJson = JsonConvert.SerializeObject(newUser);
                HttpResponseMessage response;
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (HttpClient client = new HttpClient(clientHandler))
                {

                    string uriplaceholder = API_URI + "/api/Person/Update";
                    client.DefaultRequestHeaders.Add("SessionID", User.SessionID);
                    var cont = new StringContent(NewUserJson, Encoding.UTF8, "application/json");
                    response = await client.PutAsync(uriplaceholder, cont);

                }
                if (!response.IsSuccessStatusCode)
                    throw new Exception(await response.Content.ReadAsStringAsync());


                Person GetUser;

                var content = await response.Content.ReadAsStringAsync();
                GetUser = JsonConvert.DeserializeObject<Person>(content);

                response.Dispose();


                return true;
            }
            else
            {
                return true;
            }
        }
        //not tested
        public static async Task<bool> UpdatePasword(string Oldpassword, string NewPassword)
        {
            if (!TestEnv)
            {
                Person senUser = User;
                senUser.Password = Oldpassword;
                string NewUserJson = JsonConvert.SerializeObject(senUser);
                HttpResponseMessage response;
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (HttpClient client = new HttpClient(clientHandler))
                {

                    string uriplaceholder = API_URI + "/api/Person/Update";
                    client.DefaultRequestHeaders.Add("SessionID", User.SessionID);
                    client.DefaultRequestHeaders.Add("OldPassword", Oldpassword);
                    client.DefaultRequestHeaders.Add("NewPassword", NewPassword);
                    var cont = new StringContent(NewUserJson, Encoding.UTF8, "application/json");
                    response = await client.PutAsync(uriplaceholder, cont);

                }
                if (!response.IsSuccessStatusCode)
                    throw new Exception(await response.Content.ReadAsStringAsync());


                Person GetUser;
                var content = await response.Content.ReadAsStringAsync();
                GetUser = JsonConvert.DeserializeObject<Person>(content);

                response.Dispose();

                return true;
            }
            else
            {
                return true;
            }
        }

        public static async Task<Person> GetPersonByID(int id)
        {
            if (!TestEnv)
            {
                HttpResponseMessage response;
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (HttpClient client = new HttpClient(clientHandler))
                {

                    string uriplaceholder = API_URI + "/api/Person/Get/" + id;
                    client.DefaultRequestHeaders.Add("SessionID", User.SessionID);

                    response = await client.GetAsync(uriplaceholder);
                }
                if (!response.IsSuccessStatusCode)
                    throw new Exception(await response.Content.ReadAsStringAsync());

                var content = await response.Content.ReadAsStringAsync();
                Person getPerson = JsonConvert.DeserializeObject<Person>(content);

                response.Dispose();

                return getPerson;

            }
            else
            {
                Person person = new Person(1, "stojko", "mail.com", "06412345", true, "123", new());
                return person;
            }
        }

        public static async Task<List<Person>> GetAllPeopleByTeam(Team team)
        {
            if (!TestEnv)
            {
                HttpResponseMessage response;
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (HttpClient client = new HttpClient(clientHandler))
                {

                    string uriplaceholder = API_URI + "/api/Person/GetByTeam/" + team.ID;
                    client.DefaultRequestHeaders.Add("SessionID", User.SessionID);

                    response = await client.GetAsync(uriplaceholder);
                }
                if (!response.IsSuccessStatusCode)
                    throw new Exception(await response.Content.ReadAsStringAsync());

                var content = await response.Content.ReadAsStringAsync();
                List<Person> getPeople = JsonConvert.DeserializeObject<List<Person>>(content);

                response.Dispose();

                return getPeople;

            }
            else
            {
                List<Person> ReturnPeople = new List<Person>();
                ReturnPeople.Add(new Person(1, "stojko", "mail.com", "06412345", true, "123", new()));
                ReturnPeople.Add(new Person(1, "milojko", "nomail.com", "06412345", true, "123", new()));
                ReturnPeople.Add(new Person(1, "radasin", "letters.com", "06412345", true, "123", new()));
                return ReturnPeople;
            }
        }

        public static async Task<bool> ResetPassword(Person P)
        {
            if (!TestEnv)
            {
                var values = new Dictionary<string, string>
                {

                };
                HttpResponseMessage response;
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (HttpClient client = new HttpClient(clientHandler))
                {

                    string uriplaceholder = API_URI + "/api/Person/ResetPassword/" + P.ID;
                    client.DefaultRequestHeaders.Add("SessionID", User.SessionID);
                    var cont = new FormUrlEncodedContent(values);
                    response = await client.PostAsync(uriplaceholder, cont);
                }
                if (!response.IsSuccessStatusCode)
                    throw new Exception(await response.Content.ReadAsStringAsync());

                var content = await response.Content.ReadAsStringAsync();
                //List<Person> getPeople = JsonConvert.DeserializeObject<List<Person>>(content);

                response.Dispose();

                return true;

            }
            else
            {

                return true;
            }
        }
        #endregion

        #region Job
        //not tested
        public static async Task<bool> CreateJob(Job job)
        {
            if (!TestEnv)
            {
                job.personID = job.Person.ID;
                job.teamID = job.Team.ID;
                job.JobTagID = job.JobTag.ID;
                if (job.JobGroup == null || job.JobGroup.Name == null)
                {
                    job.jobGroupID = null;
                }
                else
                    job.jobGroupID = job.JobGroup.ID;



                //Job newJob = job;
                //newJob.Team = null;
                //newJob.Person = null;

                string NewJobJson = JsonConvert.SerializeObject(job);
                HttpResponseMessage response;
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (HttpClient client = new HttpClient(clientHandler))
                {

                    string uriplaceholder = API_URI + "/api/Job/Create";
                    client.DefaultRequestHeaders.Add("SessionID", User.SessionID);
                    var cont = new StringContent(NewJobJson, Encoding.UTF8, "application/json");
                    response = await client.PostAsync(uriplaceholder, cont);

                }
                if (!response.IsSuccessStatusCode)
                    throw new Exception(await response.Content.ReadAsStringAsync());

                Job getJob;

                var content = await response.Content.ReadAsStringAsync();
                getJob = JsonConvert.DeserializeObject<Job>(content);

                response.Dispose();

                if (getJob != null)
                {
                    return true;
                }
                else
                    return false;
            }
            else //in test environment
            {
                return true;
            }
        }

        public static async Task<bool> CreateWelcomeJob(int id)
        {
            if (!TestEnv)
            {

                HttpResponseMessage response;
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (HttpClient client = new HttpClient(clientHandler))
                {

                    string uriplaceholder = API_URI + "/api/Job/WelcomeNewPerson/" + id;
                    client.DefaultRequestHeaders.Add("SessionID", User.SessionID);

                    response = await client.GetAsync(uriplaceholder);

                }
                if (!response.IsSuccessStatusCode)
                    throw new Exception(await response.Content.ReadAsStringAsync());


                var content = await response.Content.ReadAsStringAsync();
                //getJob = JsonConvert.DeserializeObject<Job>(content);

                response.Dispose();

                if (content != null)
                {
                    return true;
                }
                else
                    return false;
            }
            else //in test environment
            {
                return true;
            }
        }

        //not tested
        public static async Task<bool> UpdateJob(Job job)
        {
            if (!TestEnv)
            {
                job.personID = job.Person.ID;
                //job.teamID = job.Team.ID;
                job.JobTagID = job.JobTag.ID;
                if (job.jobGroupID == null)
                    job.JobGroup = null;


                string NewJobJson = JsonConvert.SerializeObject(job);
                HttpResponseMessage response;
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (HttpClient client = new HttpClient(clientHandler))
                {

                    string uriplaceholder = API_URI + "/api/Job/Update";
                    client.DefaultRequestHeaders.Add("SessionID", User.SessionID);
                    var cont = new StringContent(NewJobJson, Encoding.UTF8, "application/json");
                    response = await client.PutAsync(uriplaceholder, cont);

                }
                if (!response.IsSuccessStatusCode)
                {
                    ReMINDMessage.Show(await response.Content.ReadAsStringAsync(), false, "Test");
                    return false;
                    //throw new Exception(await response.Content.ReadAsStringAsync());
                }
                    

                Job getJob;

                var content = await response.Content.ReadAsStringAsync();
                getJob = JsonConvert.DeserializeObject<Job>(content);

                response.Dispose();

                if (getJob != null)
                {
                    return true;
                }
                else
                    return false;
            }
            else //in test environment
            {
                return true;
            }
        }

        //not tested
        //ovo updatuje sve poslove i sve iste poslove
        public static async Task<bool> UpdateAllJob(Job job)
        {
            if (!TestEnv)
            {
                job.personID = job.Person.ID;
                job.teamID = job.Team.ID;
                job.JobTagID = job.JobTag.ID;
                if (job.jobGroupID == null)
                    job.JobGroup = null;


                job.Person = null;
                job.Team = null;
                job.JobGroup = null;

                string NewJobJson = JsonConvert.SerializeObject(job);
                HttpResponseMessage response;
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (HttpClient client = new HttpClient(clientHandler))
                {

                    string uriplaceholder = API_URI + "/api/Job/UpdateAll";
                    client.DefaultRequestHeaders.Add("SessionID", User.SessionID);
                    var cont = new StringContent(NewJobJson, Encoding.UTF8, "application/json");
                    response = await client.PutAsync(uriplaceholder, cont);

                }
                if (!response.IsSuccessStatusCode)
                    throw new Exception(await response.Content.ReadAsStringAsync());

                Job getJob;

                var content = await response.Content.ReadAsStringAsync();
                getJob = JsonConvert.DeserializeObject<Job>(content);

                response.Dispose();

                if (getJob != null)
                {
                    return true;
                }
                else
                    return false;
            }
            else //in test environment
            {
                return true;
            }
        }

        //not tested
        public static async Task<bool> DeleteJob(Job job)
        {
            if (!TestEnv)
            {
                job.personID = job.Person.ID;
                job.teamID = job.Team.ID;
                job.JobTagID = job.JobTag.ID;
                job.jobGroupID = job.JobGroup.ID;
                HttpResponseMessage response;
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (HttpClient client = new HttpClient(clientHandler))
                {

                    string uriplaceholder = API_URI + "/api/Job/Delete/" + job.ID;
                    client.DefaultRequestHeaders.Add("SessionID", User.SessionID);
                    response = await client.DeleteAsync(uriplaceholder);

                }

                if (!response.IsSuccessStatusCode)
                    throw new Exception(await response.Content.ReadAsStringAsync());



                var content = await response.Content.ReadAsStringAsync();
                //string getstring = JsonConvert.DeserializeObject<string>(content);

                response.Dispose();

                if (content != null)
                {
                    return true;
                }
                else
                    return false;
            }
            else //in test environment
            {
                return true;
            }

        }

        //not tested
        //ovo obrise sve iste poslove
        public static async Task<bool> DeleteAllJob(Job job)
        {
            if (!TestEnv)
            {
                job.personID = job.Person.ID;
                job.teamID = job.Team.ID;
                job.JobTagID = job.JobTag.ID;
                if (job.JobGroup != null)
                    job.jobGroupID = job.JobGroup.ID;
                else
                    job.jobGroupID = null;
                HttpResponseMessage response;
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (HttpClient client = new HttpClient(clientHandler))
                {

                    string uriplaceholder = API_URI + "/api/Job/DeleteAll/" + job.ID;
                    client.DefaultRequestHeaders.Add("SessionID", User.SessionID);
                    response = await client.DeleteAsync(uriplaceholder);

                }

                if (!response.IsSuccessStatusCode)
                    throw new Exception(await response.Content.ReadAsStringAsync());



                var content = await response.Content.ReadAsStringAsync();
                //string getstring = JsonConvert.DeserializeObject<string>(content);

                response.Dispose();

                if (content != null)
                {
                    return true;
                }
                else
                    return false;
            }
            else //in test environment
            {
                return true;
            }

        }

        //vraca Sve poslove vezano za trenutnog ili nekog drugog korisnika
        public static async Task<List<Job>> GetAllJobsByPerson(FromToDate date, Person Account = null)
        {
            if (!TestEnv)
            {
                if (Account == null)
                {
                    Account = User;
                }

                string NewDateJson = JsonConvert.SerializeObject(date);
                HttpResponseMessage response;
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (HttpClient client = new HttpClient(clientHandler))
                {

                    string uriplaceholder = API_URI + "/api/Job/GetByDatePerson/" + Account.ID;
                    client.DefaultRequestHeaders.Add("SessionID", User.SessionID);
                    var cont = new StringContent(NewDateJson, Encoding.UTF8, "application/json");
                    response = await client.PostAsync(uriplaceholder, cont);
                }

                if (!response.IsSuccessStatusCode)
                    throw new Exception(await response.Content.ReadAsStringAsync());


                var content = await response.Content.ReadAsStringAsync();
                List<Job> JobList = new List<Job>();
                JobList = JsonConvert.DeserializeObject<List<Job>>(content);

                response.Dispose();
                foreach (Job j in JobList)
                {
                    //j.Team = await GetTeamById(j.teamID);
                    //j.Person = await GetPersonByID(j.personID);
                    //if (j.jobGroupID != null)
                    //    j.JobGroup = await GetJobGroupByID((int)j.jobGroupID);
                    //j.JobTag = GetJobTagByid(j.JobTagID);
                    j.JobTag = JobTags.Find(x => x.ID == j.JobTagID);
                }

                return JobList;

            }
            else
            {
                List<Job> ReturnJob = new List<Job>();
                for (int i = 1; i < 11; i++)
                {
                    ReturnJob.Add(loadTestJob(i));
                }
                return ReturnJob;
            }
        }

        public static async Task<List<Job>> GetAllJobsByGroup(FromToDate date, JobGroup jobGroup)
        {
            if (!TestEnv)
            {

                string NewDateJson = JsonConvert.SerializeObject(date);
                HttpResponseMessage response;
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (HttpClient client = new HttpClient(clientHandler))
                {

                    string uriplaceholder = API_URI + "/api/Job/GetByDateGroup/" + jobGroup.ID;
                    client.DefaultRequestHeaders.Add("SessionID", User.SessionID);
                    var cont = new StringContent(NewDateJson, Encoding.UTF8, "application/json");
                    response = await client.PostAsync(uriplaceholder, cont);
                }

                if (!response.IsSuccessStatusCode)
                    throw new Exception(await response.Content.ReadAsStringAsync());


                var content = await response.Content.ReadAsStringAsync();
                List<Job> JobList = new List<Job>();
                JobList = JsonConvert.DeserializeObject<List<Job>>(content);

                response.Dispose();
                foreach (Job j in JobList)
                {
                    //j.Team = await GetTeamById(j.teamID);
                    //j.Person = await GetPersonByID(j.personID);
                    //if (j.jobGroupID != null)
                    //    j.JobGroup = await GetJobGroupByID((int)j.jobGroupID);
                    //j.JobTag = GetJobTagByid(j.JobTagID);
                    j.JobTag = JobTags.Find(x => x.ID == j.JobTagID);
                }

                return JobList;

            }
            else
            {
                List<Job> ReturnJob = new List<Job>();
                for (int i = 1; i < 11; i++)
                {
                    ReturnJob.Add(loadTestJob(i));
                }
                return ReturnJob;
            }
        }

        //Vraca sve poslove vezane za listu timova
        public static async Task<List<Job>> GetAllJobsByTeam(FromToDate date, List<Team> LTeams = null)
        {
            if (!TestEnv)
            {
                List<Job> ReturnJobs = new List<Job>();
                if (LTeams == null)
                {
                    LTeams = MyTeams;
                }
                foreach (Team t in LTeams)
                {
                    string NewDateJson = JsonConvert.SerializeObject(date);
                    HttpResponseMessage response;
                    HttpClientHandler clientHandler = new HttpClientHandler();
                    clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                    using (HttpClient client = new HttpClient(clientHandler))
                    {

                        string uriplaceholder = API_URI + "/api/Job/GetByDateTeam/" + t.ID;
                        client.DefaultRequestHeaders.Add("SessionID", User.SessionID);
                        var cont = new StringContent(NewDateJson, Encoding.UTF8, "application/json");
                        response = await client.PostAsync(uriplaceholder, cont);
                    }

                    if (!response.IsSuccessStatusCode)
                        throw new Exception(await response.Content.ReadAsStringAsync());


                    var content = await response.Content.ReadAsStringAsync();
                    List<Job> JobList = new List<Job>();
                    JobList = JsonConvert.DeserializeObject<List<Job>>(content);

                    response.Dispose();
                    foreach (Job j in JobList)
                    {
                    //    j.Team = await GetTeamById(j.teamID);
                    //    j.Person = await GetPersonByID(j.personID);
                    //    if (j.jobGroupID != null)
                    //        j.JobGroup = await GetJobGroupByID((int)j.jobGroupID);
                    //    j.JobTag = GetJobTagByid(j.JobTagID);
                    //    ReturnJobs.Add(j);
                    j.JobTag = JobTags.Find(x => x.ID == j.JobTagID);
                    }
                    ReturnJobs = ReturnJobs.Union(JobList).ToList();
                }

                return ReturnJobs;

            }
            else
            {
                List<Job> ReturnJob = new List<Job>();
                for (int i = 1; i < 11; i++)
                {
                    ReturnJob.Add(loadTestJob(i));
                }
                return ReturnJob;
            }
        }

        //vraca sve poslove **Admin only** 
        public static async Task<List<Job>> GetAllJobsByDate(FromToDate date)

        {
            if (!TestEnv)
            {
                string NewDateJson = JsonConvert.SerializeObject(date);
                HttpResponseMessage response;
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (HttpClient client = new HttpClient(clientHandler))
                {

                    string uriplaceholder = API_URI + "/api/Job/GetByDateAll";
                    client.DefaultRequestHeaders.Add("SessionID", User.SessionID);
                    var cont = new StringContent(NewDateJson, Encoding.UTF8, "application/json");
                    response = await client.PostAsync(uriplaceholder, cont);
                }

                if (!response.IsSuccessStatusCode)
                    throw new Exception(await response.Content.ReadAsStringAsync());


                var content = await response.Content.ReadAsStringAsync();
                List<Job> JobList = new List<Job>();
                JobList = JsonConvert.DeserializeObject<List<Job>>(content);

                foreach (Job j in JobList)
                {
                    //j.Team = MyTeams.Find(x => x.ID == j.teamID);
                    //j.Person = MyPeople.Find(x => x.ID == j.personID);
                    //if (j.jobGroupID != null)
                        //j.JobGroup = await GetJobGroupByID((int)j.jobGroupID);
                    j.JobTag = JobTags.Find(x => x.ID == j.JobTagID);
                }

                response.Dispose();

                return JobList;

            }
            else
            {
                List<Job> ReturnJob = new List<Job>();
                for (int i = 1; i < 31; i++)
                {
                    ReturnJob.Add(loadTestJob(i));
                }
                return ReturnJob;
            }
        }


        //za kalendar se koristi
        public static async Task<List<Job>> GetAllRelavantJobsByDate(FromToDate ActiveDate)
        {
            List<Job> ReturnJobs = new List<Job>();

            if (Utility.UserTittle == TitleType.Leader)
            {
                ReturnJobs = await Utility.GetAllJobsByTeam(ActiveDate, TeamsILead);
                ReturnJobs = ReturnJobs.Union(await Utility.GetAllJobsByPerson(ActiveDate)).ToList();
            }
            else if (Utility.UserTittle == TitleType.Employee)
            {
                ReturnJobs = await Utility.GetAllJobsByPerson(ActiveDate);
            }
            else if (Utility.UserTittle == TitleType.Admin)
            {
                ReturnJobs = await Utility.GetAllJobsByDate(ActiveDate);
            }

            return ReturnJobs;
        }

        //za log in se koristi
        public static async Task<bool> GetAllMyJobs()
        {
            List<Job> setJobs = new List<Job>();
            FromToDate ActiveDate = new FromToDate();
            ActiveDate.DateFrom = null;
            ActiveDate.DateTo = null;


            setJobs = await Utility.GetAllJobsByPerson(ActiveDate);


            MyJobs = setJobs;

            return true;
        }

        public static async Task<List<Job>> GetAllUnreadJobsNotifications()
        {
            if (TestEnv)
                return new List<Job>();

            List<Job> ReturnJobs = new List<Job>();
            FromToDate ActiveDate = new FromToDate();
            ActiveDate.DateFrom = null;
            ActiveDate.DateTo = null;
            List<Job> JobList = new List<Job>();
            JobList = await GetAllJobsByPerson(ActiveDate);

            foreach (Job j in JobList)
            {
                if (!j.IsRead)
                {
                    ReturnJobs.Add(j);
                }
            }

            return ReturnJobs;
        }
        public static async Task<List<Job>> GetAllSameJobs(Job job)

        {
            if (!TestEnv)
            {
                HttpResponseMessage response;
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (HttpClient client = new HttpClient(clientHandler))
                {

                    string uriplaceholder = API_URI + "/api/Job/GetByID/" + job.ID;
                    client.DefaultRequestHeaders.Add("SessionID", User.SessionID);
                    response = await client.GetAsync(uriplaceholder);
                }

                if (!response.IsSuccessStatusCode)
                    throw new Exception(await response.Content.ReadAsStringAsync());


                var content = await response.Content.ReadAsStringAsync();
                List<Job> JobList = new List<Job>();
                JobList = JsonConvert.DeserializeObject<List<Job>>(content);

                foreach (Job j in JobList)
                {
                    //j.Team = await GetTeamById(j.teamID);
                    //j.Person = await GetPersonByID(j.personID);
                    //if (j.jobGroupID != null)
                    //    j.JobGroup = await GetJobGroupByID((int)j.jobGroupID);
                    j.JobTag = JobTags.Find(x => x.ID == j.JobTagID);
                }

                response.Dispose();

                return JobList;

            }
            else
            {
                List<Job> ReturnJob = new List<Job>();
                for (int i = 1; i < 31; i++)
                {
                    ReturnJob.Add(loadTestJob(i));
                }
                return ReturnJob;
            }
        }

        public static List<Job> RemoveDuplicates(List<Job> jobs)
        {
            List<Job> returnJobs = new();

            foreach (Job j in jobs)
            {

            }

            return returnJobs;
        }



        //admin and leader only
        //not tested
        public static async Task<bool> Archive(Job JobToArhive)
        {
            if (!TestEnv)
            {
                if (JobToArhive.JobGroup.Name == null)
                {
                    JobToArhive.jobGroupID = null;
                    JobToArhive.JobGroup = null;
                }
                else
                    JobToArhive.jobGroupID = JobToArhive.JobGroup.ID;
                string NewJobJson = JsonConvert.SerializeObject(JobToArhive);
                HttpResponseMessage response;
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (HttpClient client = new HttpClient(clientHandler))
                {

                    string uriplaceholder = API_URI + "/api/Job/Archive";
                    client.DefaultRequestHeaders.Add("SessionID", User.SessionID);
                    var cont = new StringContent(NewJobJson, Encoding.UTF8, "application/json");
                    response = await client.PostAsync(uriplaceholder, cont);

                }
                if (!response.IsSuccessStatusCode)
                    throw new Exception(await response.Content.ReadAsStringAsync());



                var content = await response.Content.ReadAsStringAsync();
                //string getString = JsonConvert.DeserializeObject<string>(content);

                response.Dispose();

                if (content != null)
                {
                    await DeleteAllJob(JobToArhive);
                    return true;
                    
                }
                else
                    return false;
            }
            else //in test environment
            {
                return true;
            }
        }
        #endregion

        #region JobGroup

        //not tested
        public static async Task<bool> CreateJobGroup(JobGroup jobGroup)
        {
            if (!TestEnv)
            {
                jobGroup.teamID = jobGroup.Team.ID;
                string NewJobJson = JsonConvert.SerializeObject(jobGroup);
                HttpResponseMessage response;
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (HttpClient client = new HttpClient(clientHandler))
                {
                    string uriplaceholder = API_URI + "/api/JobGroup/Create";
                    client.DefaultRequestHeaders.Add("SessionID", User.SessionID);
                    var cont = new StringContent(NewJobJson, Encoding.UTF8, "application/json");
                    response = await client.PostAsync(uriplaceholder, cont);
                }
                if (!response.IsSuccessStatusCode)
                    throw new Exception(await response.Content.ReadAsStringAsync());

                Job getJob;

                var content = await response.Content.ReadAsStringAsync();
                getJob = JsonConvert.DeserializeObject<Job>(content);

                response.Dispose();

                if (getJob != null)
                {
                    return true;
                }
                else
                    return false;
            }
            else //in test environment
            {
                return true;
            }
        }

        //not tested
        public static async Task<bool> UpdateJobGroup(JobGroup jobGroup)
        {
            if (!TestEnv)
            {
                jobGroup.teamID = jobGroup.Team.ID;
                string NewJobJson = JsonConvert.SerializeObject(jobGroup);
                HttpResponseMessage response;
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (HttpClient client = new HttpClient(clientHandler))
                {

                    string uriplaceholder = API_URI + "/api/JobGroup/Update";
                    client.DefaultRequestHeaders.Add("SessionID", User.SessionID);
                    var cont = new StringContent(NewJobJson, Encoding.UTF8, "application/json");
                    response = await client.PutAsync(uriplaceholder, cont);

                }
                if (!response.IsSuccessStatusCode)
                    throw new Exception(await response.Content.ReadAsStringAsync());

                Job getJob;

                var content = await response.Content.ReadAsStringAsync();
                getJob = JsonConvert.DeserializeObject<Job>(content);

                response.Dispose();

                if (getJob != null)
                {
                    return true;
                }
                else
                    return false;
            }
            else //in test environment
            {
                return true;
            }
        }

        //not tested
        public static async Task<bool> DeleteJobGroup(JobGroup jobGroup, bool choise)
        {
            if (!TestEnv)
            {
                jobGroup.teamID = jobGroup.Team.ID;
                HttpResponseMessage response;
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (HttpClient client = new HttpClient(clientHandler))
                {

                    string uriplaceholder = API_URI + "/api/JobGroup/Delete/" + jobGroup.ID + "/" + choise;
                    client.DefaultRequestHeaders.Add("SessionID", User.SessionID);
                    response = await client.DeleteAsync(uriplaceholder);

                }
                if (!response.IsSuccessStatusCode)
                    throw new Exception(await response.Content.ReadAsStringAsync());



                var content = await response.Content.ReadAsStringAsync();
                //string getstring = JsonConvert.DeserializeObject<string>(content);

                response.Dispose();

                if (content != null)
                {
                    return true;
                }
                else
                    return false;
            }
            else //in test environment
            {
                return true;
            }

        }

        public static async Task<JobGroup> GetJobGroupByID(int id)
        {
            if (!TestEnv)
            {

                HttpResponseMessage response;
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (HttpClient client = new HttpClient(clientHandler))
                {

                    string uriplaceholder = API_URI + "/api/JobGroup/GetByID/" + id;
                    client.DefaultRequestHeaders.Add("SessionID", User.SessionID);
                    response = await client.GetAsync(uriplaceholder);

                }
                if (!response.IsSuccessStatusCode)
                    throw new Exception(await response.Content.ReadAsStringAsync());

                JobGroup getJobGroup;

                var content = await response.Content.ReadAsStringAsync();
                getJobGroup = JsonConvert.DeserializeObject<JobGroup>(content);

                getJobGroup.Team = await GetTeamById(getJobGroup.teamID);

                response.Dispose();

                return getJobGroup;
            }
            else //in test environment
            {
                JobGroup getJobGroup = new JobGroup();
                return getJobGroup;
            }
        }

        public static async Task<List<JobGroup>> GetAllJobGroupsByTeam(Team team)
        {
            if (!TestEnv)
            {

                HttpResponseMessage response;
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (HttpClient client = new HttpClient(clientHandler))
                {

                    string uriplaceholder = API_URI + "/api/JobGroup/GetByTeam/" + team.ID;
                    client.DefaultRequestHeaders.Add("SessionID", User.SessionID);
                    response = await client.GetAsync(uriplaceholder);

                }
                if (!response.IsSuccessStatusCode)
                    throw new Exception(await response.Content.ReadAsStringAsync());

                List<JobGroup> getJobGroups;

                var content = await response.Content.ReadAsStringAsync();
                getJobGroups = JsonConvert.DeserializeObject<List<JobGroup>>(content);

                foreach(JobGroup jg in getJobGroups)
                {
                    jg.Team = await GetTeamById(jg.teamID);
                }
                response.Dispose();

                return getJobGroups;
            }
            else //in test environment
            {
                List<JobGroup> getJobGroups = new List<JobGroup>();
                return getJobGroups;
            }
        }

        public static async Task<List<JobGroup>> GetAllJobGroupsByCounter(int Counter)
        {
            if (!TestEnv)
            {

                HttpResponseMessage response;
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (HttpClient client = new HttpClient(clientHandler))
                {

                    string uriplaceholder = API_URI + "/api/JobGroup/GetByCounter" + Counter;
                    client.DefaultRequestHeaders.Add("SessionID", User.SessionID);
                    response = await client.GetAsync(uriplaceholder);

                }
                if (!response.IsSuccessStatusCode)
                    throw new Exception(await response.Content.ReadAsStringAsync());

                List<JobGroup> getJobGroups;

                var content = await response.Content.ReadAsStringAsync();
                getJobGroups = JsonConvert.DeserializeObject<List<JobGroup>>(content);

                foreach (JobGroup jg in getJobGroups)
                {
                    jg.Team = await GetTeamById(jg.teamID);
                }

                response.Dispose();

                return getJobGroups;
            }
            else //in test environment
            {
                List<JobGroup> getJobGroups = new List<JobGroup>();
                return getJobGroups;
            }
        }

        public static async Task<List<JobGroup>> GetAllJobsGroupByPerson(Person person)
        {
            if (!TestEnv)
            {

                HttpResponseMessage response;
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (HttpClient client = new HttpClient(clientHandler))
                {

                    string uriplaceholder = API_URI + "/api/JobGroup/GetByTeam" + person.ID;
                    client.DefaultRequestHeaders.Add("SessionID", User.SessionID);
                    response = await client.GetAsync(uriplaceholder);

                }
                if (!response.IsSuccessStatusCode)
                    throw new Exception(await response.Content.ReadAsStringAsync());

                List<JobGroup> getJobGroups;

                var content = await response.Content.ReadAsStringAsync();
                getJobGroups = JsonConvert.DeserializeObject<List<JobGroup>>(content);

                foreach (JobGroup jg in getJobGroups)
                {
                    jg.Team = await GetTeamById(jg.teamID);
                }
                response.Dispose();

                return getJobGroups;
            }
            else //in test environment
            {
                List<JobGroup> getJobGroups = new List<JobGroup>();
                return getJobGroups;
            }
        }

        public static async Task<List<JobGroup>> GetAllUnreadJobGroupsNotifications()
        {
            if (TestEnv)
                return new List<JobGroup>();

            List<JobGroup> ReturnJobGroup = new List<JobGroup>();
            FromToDate ActiveDate = new FromToDate();
            ActiveDate.DateFrom = null;
            ActiveDate.DateTo = null;
            List<JobGroup> JobGroupList = new List<JobGroup>();
            foreach(Team t in TeamsILead)
            {
                JobGroupList = JobGroupList.Union(await GetAllJobGroupsByTeam(t)).ToList();
            }
            

            foreach (JobGroup j in JobGroupList)
            {
                if (!j.IsRead)
                {
                    ReturnJobGroup.Add(j);
                }
            }

            return ReturnJobGroup;
        }

        #endregion

        #region JobTag

        public static async Task<bool> CreateJobTag(JobTag jobTag)
        {
            if (!TestEnv)
            {
                string NewJobTagJson = JsonConvert.SerializeObject(jobTag);
                HttpResponseMessage response;
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (HttpClient client = new HttpClient(clientHandler))
                {

                    string uriplaceholder = API_URI + "/api/JobTag/Create";
                    client.DefaultRequestHeaders.Add("SessionID", User.SessionID);
                    var cont = new StringContent(NewJobTagJson, Encoding.UTF8, "application/json");
                    response = await client.PostAsync(uriplaceholder, cont);

                }
                if (!response.IsSuccessStatusCode)
                {
                    ReMINDMessage.Show(await response.Content.ReadAsStringAsync(), false, "Failed Login");

                    return false;
                }

                JobTag GetJobTag;

                var content = await response.Content.ReadAsStringAsync();
                GetJobTag = JsonConvert.DeserializeObject<JobTag>(content);

                response.Dispose();

                return true;

            }
            else //in test environment
            {
                return true;
            }
        }

        public static async Task<bool> UpdatedJobTag(JobTag jobTag)
        {
            if (!TestEnv)
            {
                string NewJobTagJson = JsonConvert.SerializeObject(jobTag);
                HttpResponseMessage response;
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (HttpClient client = new HttpClient(clientHandler))
                {

                    string uriplaceholder = API_URI + "/api/JobTag/Update";
                    client.DefaultRequestHeaders.Add("SessionID", User.SessionID);
                    var cont = new StringContent(NewJobTagJson, Encoding.UTF8, "application/json");
                    response = await client.PutAsync(uriplaceholder, cont);

                }
                if (!response.IsSuccessStatusCode)
                    throw new Exception(await response.Content.ReadAsStringAsync());

                JobTag GetJobTag;

                var content = await response.Content.ReadAsStringAsync();
                GetJobTag = JsonConvert.DeserializeObject<JobTag>(content);

                response.Dispose();

                return true;

            }
            else //in test environment
            {
                return true;
            }
        }

        //not tested
        public static async Task<bool> DeleteJobTag(JobTag jobTag)
        {
            if (!TestEnv)
            {

                HttpResponseMessage response;
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (HttpClient client = new HttpClient(clientHandler))
                {

                    string uriplaceholder = API_URI + "/api/JobTag/Delete/" + jobTag.ID;
                    client.DefaultRequestHeaders.Add("SessionID", User.SessionID);
                    response = await client.DeleteAsync(uriplaceholder);
                }
                if (!response.IsSuccessStatusCode)
                    throw new Exception(await response.Content.ReadAsStringAsync());


                var content = await response.Content.ReadAsStringAsync();
                //string get = JsonConvert.DeserializeObject<string>(content);


                response.Dispose();

                return true;

            }
            else //in test environment
            {
                return true;
            }
        }

        //returns all jobs tags used in manage page
        public static async Task<List<JobTag>> GetAllTags()
        {
            if (!TestEnv)
            {
                HttpResponseMessage response;
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (HttpClient client = new HttpClient(clientHandler))
                {

                    string uriplaceholder = API_URI + "/api/JobTag/GetAll";
                    client.DefaultRequestHeaders.Add("SessionID", User.SessionID);

                    response = await client.GetAsync(uriplaceholder);
                }
                if (!response.IsSuccessStatusCode)
                    throw new Exception(await response.Content.ReadAsStringAsync());

                //List<JobTag> JobTags;

                var content = await response.Content.ReadAsStringAsync();
                JobTags = JsonConvert.DeserializeObject<List<JobTag>>(content);

                response.Dispose();

                return JobTags;

            }
            else
            {
                List<JobTag> JobTags = new();
                JobTags.Add(new(1, "Meeting", "#166986"));
                JobTags.Add(new(2, "QA Task", "#33EEFF"));
                JobTags.Add(new(3, "Devs lmao", "#444"));

                return JobTags;
            }
        }

        public static JobTag GetJobTagByid(int id)
        {
            JobTag ReturnJobTag = new JobTag();
            foreach(JobTag jt in JobTags)
            {
                if (jt.ID == id)
                    ReturnJobTag = jt;
            }
            return ReturnJobTag;
        }

        public static async Task<int> GetNumberOfJobs(int id)
        {
            if (!TestEnv)
            {
                HttpResponseMessage response;
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (HttpClient client = new HttpClient(clientHandler))
                {

                    string uriplaceholder = API_URI + "/api/JobTag/GetNumberOfJobs/" + id;
                    client.DefaultRequestHeaders.Add("SessionID", User.SessionID);

                    response = await client.GetAsync(uriplaceholder);
                }
                if (!response.IsSuccessStatusCode)
                    throw new Exception(await response.Content.ReadAsStringAsync());

                //List<JobTag> JobTags;

                var content = await response.Content.ReadAsStringAsync();
                int count = JsonConvert.DeserializeObject<int>(content);

                response.Dispose();

                return count;

            }
            else
            {
                return 2;
            }
        }
        #endregion

        #region JobArchive

        //not tested
        //Ovo je tu samo u slucaju da ona filter funkcija ne radi
        public static async Task<List<JobArchive>> GetAllJobArhive()
        {
            if (!TestEnv)
            {
                HttpResponseMessage response;
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (HttpClient client = new HttpClient(clientHandler))
                {
                    string uriplaceholder = API_URI + "/api/JobArchive/GetAll";
                    client.DefaultRequestHeaders.Add("SessionID", User.SessionID);
                    response = await client.GetAsync(uriplaceholder);
                }

                if (!response.IsSuccessStatusCode)
                    throw new Exception(await response.Content.ReadAsStringAsync());


                var content = await response.Content.ReadAsStringAsync();
                List<JobArchive> JobArhiveList = new List<JobArchive>();
                JobArhiveList = JsonConvert.DeserializeObject<List<JobArchive>>(content);

                response.Dispose();

                foreach (JobArchive ja in JobArhiveList)
                {
                    ja.Person = await GetPersonByID(ja.PersonID);
                }
                return JobArhiveList;

            }
            else
            {
                List<JobArchive> JobArhiveList = new List<JobArchive>();
                return JobArhiveList;
            }
        }
        //q?jobName=Meeting&weight=2
        //mora da pocne sa q? i izmedju svakog mora da postoj &
        public static async Task<List<JobArchive>> GetAllJobArhiveQuerry(FromToDate ActiveDate, string querry, int star, int num)
        {
            if (!TestEnv)
            {
                string NewDateJson = JsonConvert.SerializeObject(ActiveDate);
                HttpResponseMessage response;
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (HttpClient client = new HttpClient(clientHandler))
                {
                    string uriplaceholder = API_URI + "/api/JobArchive/Get/"+ star + "/" + num + "/" + querry;
                    client.DefaultRequestHeaders.Add("SessionID", User.SessionID);
                    var cont = new StringContent(NewDateJson, Encoding.UTF8, "application/json");
                    response = await client.PostAsync(uriplaceholder, cont);
                }

                if (!response.IsSuccessStatusCode)
                {
                    List<JobArchive> JobArhiveListEmpty = new List<JobArchive>();
                    return JobArhiveListEmpty;
                }
                    

                    //throw new Exception(await response.Content.ReadAsStringAsync());


                var content = await response.Content.ReadAsStringAsync();
                List<JobArchive> JobArhiveList = new List<JobArchive>();
                JobArhiveList = JsonConvert.DeserializeObject<List<JobArchive>>(content);

                response.Dispose();

                foreach (JobArchive ja in JobArhiveList)
                {
                    ja.Person = await GetPersonByID(ja.PersonID);
                }

                return JobArhiveList;

            }
            else
            {
                List<JobArchive> JobArhiveList = new List<JobArchive>();
                return JobArhiveList;
            }
        }
        public static async Task<List<string>> GetNames()
        {
            if (!TestEnv)
            {
                HttpResponseMessage response;
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (HttpClient client = new HttpClient(clientHandler))
                {
                    string uriplaceholder = API_URI + "/api/JobArchive/GetNames";
                    client.DefaultRequestHeaders.Add("SessionID", User.SessionID);
                    response = await client.GetAsync(uriplaceholder);
                }
                if (!response.IsSuccessStatusCode)
                    throw new Exception(await response.Content.ReadAsStringAsync());

                var content = await response.Content.ReadAsStringAsync();
                List<string> retString = new List<string>();

                retString = JsonConvert.DeserializeObject<List<string>>(content);

                response.Dispose();
                return retString;

            }
            else
            {
                List<string> retString = new List<string>();
                retString.Add("Nikola");
                retString.Add("Ivan");
                retString.Add("Milos");
                retString.Add("Nikolija");
                retString.Add("Ivana");
                retString.Add("Milosina");
                retString.Add("Ne znam vise imena");

                return retString;
;
            }
        }
        public static async Task<List<string>> GetTeams()
        {
            if (!TestEnv)
            {
                HttpResponseMessage response;
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (HttpClient client = new HttpClient(clientHandler))
                {
                    string uriplaceholder = API_URI + "/api/JobArchive/GetTeams";
                    client.DefaultRequestHeaders.Add("SessionID", User.SessionID);
                    response = await client.GetAsync(uriplaceholder);
                }
                if (!response.IsSuccessStatusCode)
                    throw new Exception(await response.Content.ReadAsStringAsync());

                var content = await response.Content.ReadAsStringAsync();
                List<string> retString = new List<string>();

                retString = JsonConvert.DeserializeObject<List<string>>(content);

                response.Dispose();
                return retString;

            }
            else
            {
                List<string> retString = new List<string>();
                retString.Add("Nikola");
                retString.Add("Ivan");
                retString.Add("Milos");
                retString.Add("Nikolija");
                retString.Add("Ivana");
                retString.Add("Milosina");
                retString.Add("Ne znam vise imena");

                return retString;
                ;
            }
        }
        public static async Task<List<string>> GetContacts()
        {
            if (!TestEnv)
            {
                HttpResponseMessage response;
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (HttpClient client = new HttpClient(clientHandler))
                {
                    string uriplaceholder = API_URI + "/api/JobArchive/GetContact";
                    client.DefaultRequestHeaders.Add("SessionID", User.SessionID);
                    response = await client.GetAsync(uriplaceholder);
                }
                if (!response.IsSuccessStatusCode)
                    throw new Exception(await response.Content.ReadAsStringAsync());

                var content = await response.Content.ReadAsStringAsync();
                List<string> retString = new List<string>();

                retString = JsonConvert.DeserializeObject<List<string>>(content);

                response.Dispose();
                return retString;

            }
            else
            {
                List<string> retString = new List<string>();
                retString.Add("Nikola");
                retString.Add("Ivan");
                retString.Add("Milos");
                retString.Add("Nikolija");
                retString.Add("Ivana");
                retString.Add("Milosina");
                retString.Add("Ne znam vise imena");

                return retString;
                ;
            }
        }
        public static async Task<List<string>> GetGroupNames()
        {
            if (!TestEnv)
            {
                HttpResponseMessage response;
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (HttpClient client = new HttpClient(clientHandler))
                {
                    string uriplaceholder = API_URI + "/api/JobArchive/GetGroupNames";
                    client.DefaultRequestHeaders.Add("SessionID", User.SessionID);
                    response = await client.GetAsync(uriplaceholder);
                }
                if (!response.IsSuccessStatusCode)
                    throw new Exception(await response.Content.ReadAsStringAsync());

                var content = await response.Content.ReadAsStringAsync();
                List<string> retString = new List<string>();

                retString = JsonConvert.DeserializeObject<List<string>>(content);

                response.Dispose();
                retString.Remove(null);
                return retString;

            }
            else
            {
                List<string> retString = new List<string>();
                retString.Add("Nikola");
                retString.Add("Ivan");
                retString.Add("Milos");
                retString.Add("Nikolija");
                retString.Add("Ivana");
                retString.Add("Milosina");
                retString.Add("Ne znam vise imena");

                return retString;
                ;
            }
        }
        public static async Task<List<string>> GetNamesTagNames()
        {
            if (!TestEnv)
            {
                HttpResponseMessage response;
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (HttpClient client = new HttpClient(clientHandler))
                {
                    string uriplaceholder = API_URI + "/api/JobArchive/GetTagNames";
                    client.DefaultRequestHeaders.Add("SessionID", User.SessionID);
                    response = await client.GetAsync(uriplaceholder);
                }
                if (!response.IsSuccessStatusCode)
                    throw new Exception(await response.Content.ReadAsStringAsync());

                var content = await response.Content.ReadAsStringAsync();
                List<string> retString = new List<string>();

                retString = JsonConvert.DeserializeObject<List<string>>(content);

                response.Dispose();
                return retString;

            }
            else
            {
                List<string> retString = new List<string>();
                retString.Add("Nikola");
                retString.Add("Ivan");
                retString.Add("Milos");
                retString.Add("Nikolija");
                retString.Add("Ivana");
                retString.Add("Milosina");
                retString.Add("Ne znam vise imena");

                return retString;
                ;
            }
        }

        public static async Task<List<string>> GetEmployeNames()
        {
            if (!TestEnv)
            {
                HttpResponseMessage response;
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (HttpClient client = new HttpClient(clientHandler))
                {
                    string uriplaceholder = API_URI + "/api/JobArchive/GetEmployeNames";
                    client.DefaultRequestHeaders.Add("SessionID", User.SessionID);
                    response = await client.GetAsync(uriplaceholder);
                }
                if (!response.IsSuccessStatusCode)
                    throw new Exception(await response.Content.ReadAsStringAsync());

                var content = await response.Content.ReadAsStringAsync();
                List<string> retString = new List<string>();

                retString = JsonConvert.DeserializeObject<List<string>>(content);

                response.Dispose();
                return retString;

            }
            else
            {
                List<string> retString = new List<string>();
                retString.Add("Nikola");
                retString.Add("Ivan");
                retString.Add("Milos");
                retString.Add("Nikolija");
                retString.Add("Ivana");
                retString.Add("Milosina");
                retString.Add("Ne znam vise imena");

                return retString;
                ;
            }
        }

        #endregion

        #region TeamLink
        public static async Task<bool> SetMyTittle(int PersonId)
        {
            if (!TestEnv)
            {
                HttpResponseMessage response;
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (HttpClient client = new HttpClient(clientHandler))
                {

                    string uriplaceholder = API_URI + "/api/TeamLink/GetByPerson/" + PersonId;
                    client.DefaultRequestHeaders.Add("SessionID", User.SessionID);
                    response = await client.GetAsync(uriplaceholder);
                }
                if (!response.IsSuccessStatusCode)
                    throw new Exception(await response.Content.ReadAsStringAsync());

                var content = await response.Content.ReadAsStringAsync();
                List<TeamLink> Teamlinks;
                Teamlinks = JsonConvert.DeserializeObject<List<TeamLink>>(content);
                if(Teamlinks.Count == 0)
                {
                    ReMINDMessage.Show("You are currently in no active teams and therefore can't use the application. Conact your Administrator to activade your account"
                                        , false, "Can't log in");
                    //ugasi app?
                    return false;

                }
                

                response.Dispose();

                TeamLink Max = new TeamLink();
                Max.Title = TitleType.Employee;
                foreach (TeamLink t in Teamlinks)
                {
                    if (t.Title > Max.Title)
                    {
                        Max = t;
                    }
                }

                UserTittle = Max.Title;
                if(UserTittle == TitleType.Employee)
                {
                    employeeRestriction = true;
                }
                if (UserTittle == TitleType.Leader)
                {
                    leaderRestriction = true;

                }
                if (UserTittle == TitleType.Admin)
                {
                    adminRestriction = true;
                }
                return true;

            }
            else
            {
                UserTittle = TitleType.Admin;
                return true;
            }
        }

        public static async Task<TitleType> GetTittle(int PersonId)
        {
            if (!TestEnv)
            {
                HttpResponseMessage response;
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (HttpClient client = new HttpClient(clientHandler))
                {

                    string uriplaceholder = API_URI + "/api/TeamLink/GetByPerson/" + PersonId;
                    client.DefaultRequestHeaders.Add("SessionID", User.SessionID);
                    response = await client.GetAsync(uriplaceholder);
                }
                if (!response.IsSuccessStatusCode)
                    throw new Exception(await response.Content.ReadAsStringAsync());

                var content = await response.Content.ReadAsStringAsync();
                List<TeamLink> Teamlinks;
                Teamlinks = JsonConvert.DeserializeObject<List<TeamLink>>(content);


                response.Dispose();

                TeamLink Max = new TeamLink();
                Max.Title = TitleType.Employee;
                foreach (TeamLink t in Teamlinks)
                {
                    t.Team = await GetTeamById(t.teamID);
                    t.Person = await GetPersonByID(t.personID);
                    if (t.Title > Max.Title)
                    {
                        Max = t;
                    }
                }
                TitleType ReturnTittle;
                ReturnTittle = Max.Title;
                return ReturnTittle;

            }
            else
            {
                UserTittle = TitleType.Admin;
                return UserTittle;
            }
        }

        //not tested
        public static async Task<bool> CreateTeamLink(TeamLink teamLinkToCreate)
        {
            if (!TestEnv)
            {
                string NewTeamJson = JsonConvert.SerializeObject(teamLinkToCreate);
                HttpResponseMessage response;
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (HttpClient client = new HttpClient(clientHandler))
                {

                    string uriplaceholder = API_URI + "/api/TeamLink/Create";
                    client.DefaultRequestHeaders.Add("SessionID", User.SessionID);
                    var cont = new StringContent(NewTeamJson, Encoding.UTF8, "application/json");
                    response = await client.PostAsync(uriplaceholder, cont);

                }
                if (!response.IsSuccessStatusCode)
                    throw new Exception(await response.Content.ReadAsStringAsync());

                TeamLink getTeamLink;

                var content = await response.Content.ReadAsStringAsync();
                getTeamLink = JsonConvert.DeserializeObject<TeamLink>(content);

                response.Dispose();

                if (getTeamLink != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else //in test environment
            {
                return true;
            }
        }
        //not tested
        public static async Task<bool> UpdateTeamLink(TeamLink teamLinkToEdit)
        {
            if (!TestEnv)
            {
                string NewTeamJson = JsonConvert.SerializeObject(teamLinkToEdit);
                HttpResponseMessage response;
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (HttpClient client = new HttpClient(clientHandler))
                {

                    string uriplaceholder = API_URI + "/api/TeamLink/Update";
                    client.DefaultRequestHeaders.Add("SessionID", User.SessionID);
                    var cont = new StringContent(NewTeamJson, Encoding.UTF8, "application/json");
                    response = await client.PutAsync(uriplaceholder, cont);

                }
                if (!response.IsSuccessStatusCode)
                    throw new Exception(await response.Content.ReadAsStringAsync());

                TeamLink getTeamLink;

                var content = await response.Content.ReadAsStringAsync();
                getTeamLink = JsonConvert.DeserializeObject<TeamLink>(content);

                response.Dispose();

                if (getTeamLink != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else //in test environment
            {
                return true;
            }
        }
        //nottested
        public static async Task<bool> DeleteTeamLink(TeamLink teamLinkToDelete)
        {
            if (!TestEnv)
            {
                HttpResponseMessage response;
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (HttpClient client = new HttpClient(clientHandler))
                {

                    string uriplaceholder = API_URI + "/api/TeamLink/Delete/" + teamLinkToDelete.ID;
                    client.DefaultRequestHeaders.Add("SessionID", User.SessionID);
                    response = await client.DeleteAsync(uriplaceholder);

                }
                if (!response.IsSuccessStatusCode)
                    throw new Exception(await response.Content.ReadAsStringAsync());

                TeamLink getTeamLink;

                var content = await response.Content.ReadAsStringAsync();
                //getTeamLink = JsonConvert.DeserializeObject<TeamLink>(content);

                response.Dispose();

                if (content != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else //in test environment
            {
                return true;
            }
        }

        //not tested
        public static async Task<bool> AddPersonToTeam(Person p, Team t, TitleType type)
        {

            TeamLink teamLink = new TeamLink();
            teamLink.Person = p;
            teamLink.personID = p.ID;
            teamLink.Team = t;
            teamLink.teamID = t.ID;
            teamLink.Title = type;
            return await CreateTeamLink(teamLink);

        }

        //not tested
        public static async Task<bool> RemovePersonFromTeam(Person p, Team t)
        {
            List<TeamLink> teamLinks = new List<TeamLink>();
            teamLinks = await GetTeamLinksByPerson(p);
            List<TeamLink> teamLinksToDelete = new List<TeamLink>();
            foreach (TeamLink tl in teamLinks)
            {
                if (tl.teamID == t.ID)
                {
                    teamLinksToDelete.Add(tl);
                }
            }          
            foreach(TeamLink tl in teamLinksToDelete)
            {
                await DeleteTeamLink(tl);
            }
            return true;
        }

        //not tested
        public static async Task<List<TeamLink>> GetTeamLinksByPerson(Person p)
        {
            if (!TestEnv)
            {
                HttpResponseMessage response;
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (HttpClient client = new HttpClient(clientHandler))
                {

                    string uriplaceholder = API_URI + "/api/TeamLink/GetByPerson/" + p.ID;
                    client.DefaultRequestHeaders.Add("SessionID", User.SessionID);
                    response = await client.GetAsync(uriplaceholder);
                }
                if (!response.IsSuccessStatusCode)
                    throw new Exception(await response.Content.ReadAsStringAsync());

                var content = await response.Content.ReadAsStringAsync();
                List<TeamLink> Teamlinks;
                Teamlinks = JsonConvert.DeserializeObject<List<TeamLink>>(content);

                return Teamlinks;

            }
            else
            {
                List<TeamLink> Teamlinks = new List<TeamLink>();
                return Teamlinks;
            }
        }

        //not tested
        public static async Task<List<TeamLink>> GetTeamLinksByTeam(Team t)
        {
            if (!TestEnv)
            {
                HttpResponseMessage response;
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (HttpClient client = new HttpClient(clientHandler))
                {

                    string uriplaceholder = API_URI + "/api/TeamLink/GetByTeam/" + t.ID;
                    client.DefaultRequestHeaders.Add("SessionID", User.SessionID);
                    response = await client.GetAsync(uriplaceholder);
                }
                if (!response.IsSuccessStatusCode)
                    throw new Exception(await response.Content.ReadAsStringAsync());

                var content = await response.Content.ReadAsStringAsync();
                List<TeamLink> Teamlinks;
                Teamlinks = JsonConvert.DeserializeObject<List<TeamLink>>(content);

                return Teamlinks;

            }
            else
            {
                List<TeamLink> Teamlinks = new List<TeamLink>();
                return Teamlinks;
            }
        }

        public static async Task<TeamLink> GetTeamLinksByTeamAndPerson(Team t, Person p)
        {
            if (!TestEnv)
            {
                List<TeamLink> teamLinkByTeam = new();
                teamLinkByTeam = await GetTeamLinksByTeam(t);
                TeamLink returnLink = new();
                returnLink = null;
                foreach (TeamLink tl in teamLinkByTeam)
                {
                    if (tl.personID == p.ID)
                        returnLink = tl;
                }
                return returnLink;

            }
            else
            {
                TeamLink Teamlinks = new TeamLink();
                return Teamlinks;
            }
        }

        #endregion

        #region Misc

        #region Loading & Restart
        public static void RestartApp()
        {
            System.Diagnostics.Process.Start(System.Reflection.Assembly.GetEntryAssembly().Location.Replace(".dll", ".exe"));
            Application.Current.Shutdown();
        }
        public static void StartLoading()
        {
            mainWindow.LoadingScreen.Visibility = Visibility.Visible;
        }
        public static void StopLoading()
        {
            mainWindow.LoadingScreen.Visibility = Visibility.Collapsed;
        }
        #endregion

        #region Tabs, Reload
        public static CalendarControl calendarControl;
        public static void ReloadTasksData()
        {
            StartLoading();
            calendarControl.ReloadData();
            StopLoading();
        }

        public static AnalyticsViewer analyticsViewer;
        public static void ReloadAnalyticsData()
        {
            analyticsViewer.ReloadGraph();
        }

        public static ArchiveViewer archiveViewer;
        public static void ReloadArchiveData()
        {    
            archiveViewer.ReloadPage();
        }

        public static Manager managerControl;
        public static ManagementViewer managerViewer;
        public static void ReloadManagementData()
        {
            managerControl.ReloadData();
            managerViewer.Refresh();
        }
        #endregion

        #region Notifs
        public static void hideNotification()
        {
            mainWindow.hideNotificationIndicator();
        }
        public static void showNotification()
        {
            mainWindow.showNotificationIndicator();
        }
        #endregion

        #endregion

        #region Helper methods
        /// <summary>
        /// Compares s1 with s2.
        /// </summary>
        /// <returns>true if s1 < s2</returns>
        public static bool areStringsSorted(string s1, string s2)
        {
            if (s1 == null)
                return false;
            if (s2 == null)
                return true;

            s1 = s1.ToLower();
            s2 = s2.ToLower();

            int lenght = s1.Length < s2.Length ?
                         s1.Length : s2.Length;

            for (int i = 0; i < lenght; i++)
            {
                if (s1[i] < s2[i])
                    return true;
                else if (s1[i] > s2[i])
                    return false;
            }
            return s1.Length <= s2.Length;
        }

        public static void flickerPropertyWithRed(UIElement element, DependencyProperty property, object oldValue)
        {
            Color redColor = (Color)ColorConverter.ConvertFromString("#D0480E");
            Brush redBrush = new SolidColorBrush(redColor);
            var animation = new ColorAnimation(redColor, TimeSpan.FromMilliseconds(150));
            animation.AutoReverse = true;
            animation.RepeatBehavior = new RepeatBehavior(3);

            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(animation);
            Storyboard.SetTarget(animation, element);
            PropertyPath colorTargetPath = new PropertyPath("(0).(1)", property, SolidColorBrush.ColorProperty);
            Storyboard.SetTargetProperty(animation, colorTargetPath);

            //storyboard.Completed += (sender, args) =>
            //{
            //    element.SetValue(property, new SolidColorBrush((Color)oldValue));
            //};//nikola

            storyboard.Begin();
        }
        public static Job loadTestJob(int id) //testing purposes, don't delete please
        {
            Random rand = new Random();
            int i = rand.Next(0, 100);
            Job AttachedJob = new Job();
            AttachedJob.ID = id;
            AttachedJob.Deadline = new DateTime(2022, 6, id);
            AttachedJob.Description = "";
            if (i < 25)
            {
                AttachedJob.JobTag.Color = "#318FB7";
                AttachedJob.JobTag.Name = "Meeting";
                AttachedJob.JobGroup.Name = "TASK1";
                if (i < 10)
                {
                    AttachedJob.Name = "Call about 1";
                    AttachedJob.Contact = "Dzoni";
                    AttachedJob.Person.Name = "Dzoni";
                }
                else
                {
                    AttachedJob.Name = "Call about 2";
                    AttachedJob.Contact = "Piksi";
                    AttachedJob.Person.Name = "Piksi";
                }
            }
            else if (i < 50)
            {
                AttachedJob.JobTag.Color = "#D0480E";
                AttachedJob.JobTag.Name = "Problem";
                AttachedJob.JobGroup.Name = "TASK2";
                if (i < 10)
                {
                    AttachedJob.Name = "Figure out something";
                    AttachedJob.Contact = "Piksi";
                    AttachedJob.Person.Name = "Piksi";
                }
                else
                {
                    AttachedJob.Name = "Ask others for help";
                    AttachedJob.Contact = "Lajron";
                    AttachedJob.Person.Name = "Lajron";
                }
            }
            else if (i < 75)
            {
                AttachedJob.JobTag.Color = "#698626";
                AttachedJob.JobTag.Name = "Development";
                AttachedJob.JobGroup.Name = "TASK3";
                if (i < 10)
                {
                    AttachedJob.Name = "Implement Server";
                    AttachedJob.Contact = "Lajron";
                    AttachedJob.Person.Name = "Lajron";
                }
                else
                {
                    AttachedJob.Name = "Implement Client";
                    AttachedJob.Contact = "Dzoni";
                    AttachedJob.Person.Name = "Dzoni";
                }
            }
            else
            {
                AttachedJob.JobTag.Color = "#D0A90E";
                AttachedJob.JobTag.Name = "Marketing";
                AttachedJob.JobGroup.Name = "TASK4";
                if (i < 10)
                {
                    AttachedJob.Name = "Instagram Marketing";
                    AttachedJob.Contact = "Lajron";
                    AttachedJob.Person.Name = "Lajron";
                }
                else
                {
                    AttachedJob.Name = "Youtube ads lmaoo";
                    AttachedJob.Contact = "Dzoni";
                    AttachedJob.Person.Name = "Dzoni";
                }
            }
            return AttachedJob;
        }
        #endregion

        #region Job/Group Add/View/Edit
        public static void OpenAddJob()
        {
            mainWindow.Blur();
            JobAndGroupViewerContainer container = new();
            container.TaskButton.IsChecked = true;
            container.ShowDialog();
            mainWindow.Unblur();
        }
        public static void OpenEditJob(Job job)
        {
            mainWindow.Blur();
            JobAndGroupViewerContainer container = new(job);
            container.ShowDialog();
            mainWindow.Unblur();
        }
        public static void OpenViewJob(Job job)
        {
            mainWindow.Blur();
            JobViewWindow window = new(mainWindow, job);
            window.ShowDialog();
            mainWindow.Unblur();
        }
        public static void OpenAddGroup()
        {
            mainWindow.Blur();
            JobAndGroupViewerContainer container = new();
            container.GroupButton.IsChecked = true;
            container.ShowDialog();
            mainWindow.Unblur();
        }
        public static void OpenEditGroup(JobGroup group)
        {
            mainWindow.Blur();
            JobAndGroupViewerContainer container = new(null, group);
            container.ShowDialog();
            mainWindow.Unblur();
        }
        public static void OpenViewGroup(JobGroup group)
        {
            mainWindow.Blur();
            GroupViewWindow window = new(mainWindow, group);
            window.ShowDialog();
            mainWindow.Unblur();
        }
        #endregion

    }
}