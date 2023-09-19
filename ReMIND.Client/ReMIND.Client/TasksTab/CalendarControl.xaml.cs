using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using ReMIND.Client.Business.Models;
using ReMIND.Client.Business.Models.Types;
using ReMIND.Client.Elements;

namespace ReMIND.Client.TasksTab
{
    /// <summary>
    /// Interaction logic for CalendarControl.xaml
    /// </summary>
    public partial class CalendarControl : UserControl
    {

        #region Data & Properties
        public List<Job> Jobs { get; set; }
        public List<Job> DisplayedJobs { get; set; }
        public List<JobGroup> JobGroups { get; set; }
        public List<Person> Employees { get; set; }
        public List<TeamLink> Links { get; set; }

        public DateTime ActiveMonth;
        public CalendarCell[,] Cells { get; set; }

        private ListControl _listControl;
        public ListControl ListControl
        {
            get => _listControl;
            set { _listControl = value; ReloadData(); }
        }
        #endregion

        public CalendarControl()
        {
            InitializeComponent();
            Utility.calendarControl = this;
            ActiveMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            initializeCells();
            RefreshStyle();
        }

        //ovo radi sve pls samo ovo zovite ako hocete da refresh kalendar
        public async void ReloadData()
        {
            initializeCells();
            RefreshStyle();

            #region Data
            Jobs = new();
            JobGroups = new();
            Employees = new();
            Links = new();
            #endregion

            #region Interval
            FromToDate ActiveDate = new FromToDate();
            ActiveDate.DateFrom = ActiveMonth;
            ActiveDate.DateTo = ActiveMonth.AddMonths(1).AddDays(-1);//last day of month
            #endregion

            #region loading data based on selected interval
            Jobs = await Utility.GetAllRelavantJobsByDate(ActiveDate);
            Jobs = Jobs.Distinct().ToList();

            //piksi ovo ispod je sus
            //nikola deluje da radi?
            List<Team> teamsIlead = Utility.TeamsILead;
            foreach(Team t in teamsIlead)
            {
                JobGroups = JobGroups.Union(await Utility.GetAllJobGroupsByTeam(t)).ToList();
            }
            #endregion

            #region Applying filters
            List<Job> jobsToDisplay = Jobs.ToList();
            if (TeamSelector.getSelectedString() != "")
            {
                Team teamFilter = ObjectConverter.ToTypedInstance<Team>(TeamSelector.getSelectedObject());
                foreach (Job job in Jobs)
                    if (!job.Team.Equals(teamFilter))
                        jobsToDisplay.Remove(job);
            }

            if (EmployeeSelector.getSelectedString() != "")
            {
                Person employeeFilter = ObjectConverter.ToTypedInstance<Person>(EmployeeSelector.getSelectedObject());
                foreach (Job job in Jobs)
                    if (!job.Person.Equals(employeeFilter))
                        jobsToDisplay.Remove(job);
            }

            if (TagSelector.getSelectedString() != "")
            {
                JobTag tagFilter = ObjectConverter.ToTypedInstance<JobTag>(TagSelector.getSelectedObject());
                foreach (Job job in Jobs)
                    if (!job.JobTag.Equals(tagFilter))
                        jobsToDisplay.Remove(job);
            }
            Jobs = jobsToDisplay;
            #endregion

            #region Crtanje
            // this is the position of the "first day in the month" cell
            int startJ = ((int)ActiveMonth.DayOfWeek + 6) % 7; //sunday=0 so i do this to make monday=0
            int startI = Convert.ToInt32(!Convert.ToBoolean(startJ));

            foreach (Job job in Jobs) //populate cells with their respectful jobs
            { // jobI & jobJ is its position in the calendar matrix
                int temp = startJ + job.Deadline.Day - 1; //ne pitaj me zasto, ali ovo ovako radi.
                int jobJ = temp % 7;
                int jobI = startI + temp / 7;
                Cells[jobI, jobJ].AddJob(job);
            }

            //Joca: daj week numbers u niz
            //ActiveMonth
            //startI
            //startJ
            int[] weekNumbersArray = new int[6];
            
            #endregion

            #region Filter Dropdowns stuff
            List<Team> teams = new();
            List<Person> people = new();
            List<JobTag> tags = new();
            tags = Utility.JobTags;
            teams = Utility.MyTeams;
            if (Utility.UserTittle == TitleType.Leader)
            {
                foreach (Team t in teamsIlead)
                {
                    List<Person> peopleToAdd = await Utility.GetAllPeopleByTeam(t);
                    people = people.Union(peopleToAdd).Distinct().ToList();
                }
            }
            else if (Utility.UserTittle == TitleType.Admin)
            {
                foreach (Team t in teams)
                {
                    List<Person> peopleToAdd = await Utility.GetAllPeopleByTeam(t);
                    people = people.Union(peopleToAdd).Distinct().ToList();
                }
            }
            else
                people.Add(Utility.User);


            TeamSelector.AttachData(teams.ToArray());
            EmployeeSelector.AttachData(people.ToArray());
            TagSelector.AttachData(tags.ToArray());
            #endregion

            ListControl.AttachData(Jobs, JobGroups);

            if (Utility.employeeRestriction)
            {
                EmployeeSelector.Visibility = Visibility.Collapsed;
            }
        }

        #region Cell Style & Initialization
        /// <summary>
        /// Generates CalendarCell objects on first load and populates the <see cref="Cells"/> matrix. call after reloaddata
        /// </summary>
        public void initializeCells()
        {
            CalendarGrid.Children.RemoveRange(7, 42); //god forbid
            Cells = new CalendarCell[6, 7];
            for (int i = 1; i < 7; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    CalendarCell cell = new CalendarCell();
                    cell.DateText = $"{j + ((i - 1) * 7) + 1}";
                    cell.CellActiveChanged += Cell_ActiveChanged;
                    Cells[i - 1, j] = cell;

                    Viewbox vb = new();
                    Grid.SetRow(vb, i);
                    Grid.SetColumn(vb, j);
                    vb.Child = cell;
                    CalendarGrid.Children.Add(vb);
                }
            }
        }


        /// <summary>
        /// Refreshes the style of cells based on the <see cref="ActiveMonth"/> property.
        /// This includes the date label as well as cell's background. call after initializeCells
        /// </summary>
        public void RefreshStyle()
        {
            //initializeCells();
            MonthLabel.Text = ActiveMonth.ToString("MMMM yyyy");

            // this is the position of the "first day in the month" cell
            int startJ = ((int)ActiveMonth.DayOfWeek + 6) % 7; //sunday=0 so i do this to make monday=0
            int startI = Convert.ToInt32(!Convert.ToBoolean(startJ));

            #region Last Month

            int startI2 = startI;
            int startJ2 = startJ - 1;
            if (startJ2 < 0) { startJ2 = 6; startI2--; }

            DateTime lastMonth = ActiveMonth.AddMonths(-1);

            int daysInLastMonth = DateTime.DaysInMonth(lastMonth.Year, lastMonth.Month);
            int counter = daysInLastMonth;

            for (int i = startI2; i >= 0; i--)
            {
                if (i < startI2)
                    startJ2 = 6;
                for (int j = startJ2; j >= 0; j--)
                {
                    Cells[i, j].IsEnabled = false;
                    Cells[i, j].DateText = $"{counter--}";
                }
            }
            #endregion

            #region Current & Next Month
            counter = 1;
            int daysInMonth = DateTime.DaysInMonth(ActiveMonth.Year, ActiveMonth.Month);
            bool enabler = true;

            for (int i = startI; i < 6; i++)
            {
                if (i > startI)
                    startJ = 0;

                for (int j = startJ; j < 7; j++)
                {
                    Cells[i, j].Enabled = enabler;
                    Cells[i, j].DateText = $"{counter}";

                    //today
                    if (DateTime.Now.Date == new DateTime(ActiveMonth.Year, ActiveMonth.Month, Cells[i, j].DateNumber))
                        Cells[i, j].Today = true;

                    if (counter++ == daysInMonth)
                    {
                        counter = 1;
                        enabler = false; //enabler = disabler haha 
                    }
                }
            }

            #endregion

        }
        #endregion

        #region Select/Deselect

        public void SelectJobsForDisplay()
        {
            Team teamFilter = new();
            Person employeeFilter = new();
            JobTag tagFilter = new();

            DisplayedJobs = Jobs;

            if(TeamSelector.getSelectedString() != "")
                teamFilter = ObjectConverter.ToTypedInstance<Team>(TeamSelector.getSelectedObject());

            if (EmployeeSelector.getSelectedString() != "")
                employeeFilter = ObjectConverter.ToTypedInstance<Person>(EmployeeSelector.getSelectedObject());

            if (TagSelector.getSelectedString() != "")
                tagFilter = ObjectConverter.ToTypedInstance<JobTag>(TagSelector.getSelectedObject());
        }

        private void Cell_ActiveChanged(object sender, bool e)
        {
            CalendarCell cell = (CalendarCell)sender;
            List<Job> jobsToSend = new();
            foreach (JobTagEllipse jte in cell.Jobs)
                jobsToSend.Add(jte.Job);

            if (e) //Activated
                ListControl.Select(jobsToSend);
            else //Deactivated
                ListControl.Deselect(jobsToSend);
        }

        #endregion

        #region UI Events
        private void PrevMonth_Click(object sender, RoutedEventArgs e)
        {
            ActiveMonth = ActiveMonth.AddMonths(-1);
            ReloadData();
            initializeCells(); //god forbid
            RefreshStyle();
        }

        private void NextMonth_Click(object sender, RoutedEventArgs e)
        {
            ActiveMonth = ActiveMonth.AddMonths(1);
            ReloadData();
            initializeCells(); //god forbid
            RefreshStyle();
        }
        #endregion

        #region Filters
        private void ClearFilters_Click(object sender, RoutedEventArgs e)
        {
            foreach (SearchDropdown child in SearchDropdownsList.Children)
            {
                child.ClearSelectionWithoutEvents();
            }
            ReloadData();
        }
        private async void TeamSelector_SelectedItemChanged(object sender, object e)
        {
            if (e.ToString() != "")
            {
                object oldSelectedObject = EmployeeSelector.getSelectedObject();

                Team selectedTeam = ObjectConverter.ToTypedInstance<Team>(e);
                List<Person> teamMembers = new();
                //teamMembers = await Utility.GetAllPeopleByTeam(selectedTeam);

                EmployeeSelector.AttachData(teamMembers.ToArray());
                EmployeeSelector.selectObject(oldSelectedObject); //trying to select again osobu koja je bila vec selected
            }

            ReloadData();
        }

        private void EmployeeSelector_SelectedItemChanged(object sender, object e)
        {
            //nikola: filter data na kalendaru(update teamSelector)
            ReloadData();
        }

        private void TagSelector_SelectedItemChanged(object sender, object e)
        {
            ReloadData();
        }
        #endregion

    }
}
