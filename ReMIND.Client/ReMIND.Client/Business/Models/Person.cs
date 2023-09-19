using System;
using System.Collections.Generic;

namespace ReMIND.Client.Business.Models
{
    public class Person
    {

        #region Data
        public int ID { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        
        public string Password { get; set; }

        //prolly won't be used
        //public DateTime LastActive { get; set; }

        public bool IsEmployed { get; set; }

        public string SessionID { get; set; }

        //public TeamLink TeamLink { get; set; }

        public List<Job> Jobs { get; set; }

        #endregion

        #region Constructors
        public Person()
        {
            this.Jobs = new List<Job>();
        }
        public Person(int id, string name, string email, string phone,bool isEmployed, string sessionID, List<Job> jobs)
        {
            this.ID = id;
            this.Name = name;
            this.Email = email;
            this.Phone = phone;
            this.IsEmployed = isEmployed;
            this.SessionID = sessionID;
            this.Jobs = jobs;

        }
        #endregion

        #region Methods
        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object obj)
        {
            try
            {
                Person toCompare = (Person)obj;
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
        public void AddJob(Job job)
        {
            Jobs.Add(job);
        }

        #endregion

    }
}
