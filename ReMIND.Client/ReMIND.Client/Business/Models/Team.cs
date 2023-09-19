using System;
using System.Collections.Generic;

namespace ReMIND.Client.Business.Models
{
    public class Team
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public List<TeamLink> TeamLink { get; set; }
        public List<JobGroup> TaskGroup { get; set; }
        public List<Job> Jobs { get; set; }

        public Team()
        {

            this.TeamLink = new List<TeamLink>();
            this.TaskGroup = new List<JobGroup>();
            this.Jobs = new List<Job>();

        }

        public override string ToString()
        {
            return Name;
        }
        public override bool Equals(object obj)
        {
            try
            {
                Team toCompare = (Team)obj;
                return ID == toCompare.ID;
            }
            catch
            {
                return false;
            }
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(ID, Name);
        }
    }
}
