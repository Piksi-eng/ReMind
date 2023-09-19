using System.Collections.Generic;

namespace ReMIND.Client.Business.Models
{
    public class JobGroup
    {
        #region Data
        public int ID { get; set; }
        public string Name { get; set; }

        public bool IsRead { get; set; }

        public bool DidAdminCreate { get; set; }
        public string Description { get; set; }
        public int Counter { get; set; }
        public Team Team { get; set; }
        public int teamID { get; set; }
        public List<Job> Jobs { get; set; }

        #endregion

        #region Constructors
        public JobGroup()
        {
            this.Team = new Team();
            this.Jobs = new List<Job>();
        }

        public JobGroup(int id, string name, string description, int counter, Team team, List<Job> jobs)
        {
            this.ID = id;
            this.Name = name;
            this.Description = description;
            this.Counter = counter;
            this.Team = team;
            this.Jobs = jobs;
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            return Name;
        }
        public void AddJob(Job job)
        {
            Jobs.Add(job);
        }

        #endregion
    }
}
