using ReMIND.Client.Business.Models.Types;

namespace ReMIND.Client.Business.Models
{
    public class TeamLink
    {
        #region Data
        public int ID { get; set; }
        public TitleType Title { get; set; }
        public Person Person { get; set; }
        public int personID { get; set; }
        public int teamID { get; set; }
        public Team Team { get; set; }

        #endregion
        #region Constructors

        public TeamLink()
        {

            this.Person = new Person();
            this.Team = new Team();

        }
        #endregion

    }
}
