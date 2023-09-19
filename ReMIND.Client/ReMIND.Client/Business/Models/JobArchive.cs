using System;

namespace ReMIND.Client.Business.Models
{
    public class JobArchive
    {
        #region Data
        public int ID { get; set; }
        public DateTime Creation { get; set; }
        public DateTime Deadline { get; set; }
        public DateTime LastModified { get; set; }
        public DateTime? Finished { get; set; }
        public string Name { get; set; }
        public string Contact { get; set; }
        public int JobWeight { get; set; }
        public string JobGroupName { get; set; }
        public string JobTagName { get; set; }
        public Person Person { get; set; }

        public int PersonID { get; set; }

        public int tdiD;
        public string TeamName { get; set; }
        #endregion

        #region Constructors
        public JobArchive()
        {
            this.Person = new Person();
        }
        public JobArchive(int id, string name, string contact, int jobWeight, Person person, string teamName,
                          DateTime creation, DateTime deadLine, DateTime finished)
        {
            this.ID = id;
            this.Name = name;
            this.Contact = contact;
            this.JobWeight = jobWeight;
            this.Person = person;
            this.TeamName = teamName;
            this.Creation = creation;
            this.Deadline = deadLine;
            this.Finished = finished;

        }
        public JobArchive(int id, string name, string contact, int jobWeight, string taskGroupName, string jobTagName, Person person, string teamName,
                  DateTime creation, DateTime deadLine)
        {
            this.ID = id;
            this.Name = name;
            this.Contact = contact;
            this.JobWeight = jobWeight;
            this.Person = person;
            this.TeamName = teamName;
            this.Creation = creation;
            this.Deadline = deadLine;
            this.Finished = null;
            JobTagName = jobTagName;
            JobGroupName = taskGroupName;

        }

        #endregion

        #region Methods

        #endregion
    }
}
