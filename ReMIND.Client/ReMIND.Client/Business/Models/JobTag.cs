namespace ReMIND.Client.Business.Models
{
    public class JobTag
    {
        #region Data
        public int ID { get; set; }

        public string Name { get; set; }

        public string Color { get; set; }

        //public List<Job> Job { get; set; }

        //public List<JobArchive> JobArchive { get; set; }
        #endregion

        #region Constructors
        public JobTag()
        {

        }
        public JobTag(int id, string name, string color)
        {
            this.ID = id;
            this.Name = name;
            this.Color = color;
        }
        #endregion

        #region Methods
        public override string ToString()
        {
            return Name;
        }

        #endregion
    }
}
