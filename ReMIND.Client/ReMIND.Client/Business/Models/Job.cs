using System;

using ReMIND.Client.Business.Models.Types;

namespace ReMIND.Client.Business.Models
{
    public class Job
    {
        #region Data
        public int ID { get; set; }
        public DateTime Deadline { get; set; }
        public DateTime LastModified { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public RecurringType RecurringType { get; set; }
        public int Multiplier { get; set; }
        public string Contact { get; set; }
        public int JobWeight { get; set; }
        public bool IsDone { get; set; }

        public bool IsRead { get; set; }
        public int JobTagID { get; set; }
        public JobTag JobTag { get; set; }
        public int personID { get; set; }
        public Person Person { get; set; }
        public int teamID { get; set; }
        public Team Team { get; set; }
        public int? jobGroupID { get; set; }
        public JobGroup JobGroup { get; set; }
        #endregion

        #region Constructors
        public Job()
        {

            this.Person = new Person();
            this.Team = new Team();
            this.JobGroup = new JobGroup();
            JobTag = new(); //dodao sam ovo jer je falilo -dzt

        }
        public Job(int id, string name, string description, string contact, 
                   int jobWeight, JobTag jobTag, Person person, Team team, JobGroup taskGroup, DateTime deadLine, RecurringType recurringType = 0, int multiplier = 0)
        {
            this.ID = id;
            this.Name = name;
            this.Description = description;
            this.RecurringType = recurringType;
            this.Multiplier = multiplier;
            this.Contact = contact;
            this.JobWeight = jobWeight;
            if (jobWeight < 0)
                this.JobWeight = 0;
            if (jobWeight > 8)
                this.JobWeight = 8;
            //On creation job is not done
            this.IsDone = false;
            this.IsRead = false;
            this.JobTag = jobTag;
            this.Person = person;
            this.Team = team;
            this.JobGroup = taskGroup;
            this.Deadline = deadLine;
        }

        #endregion

        #region Methods
        public void Finish()
        {
            this.IsDone = true;
        }
        public override string ToString()
        {
            return Name;
        }
        public override bool Equals(object obj)
        {
            try
            {
                Job toCompare = (Job)obj;
                bool ret = (Name == toCompare.Name && Deadline == toCompare.Deadline && teamID == toCompare.teamID);
                return ret;
            }
            catch
            {
                return false;
            }
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Deadline, teamID);
        }
        #endregion

    }
}
