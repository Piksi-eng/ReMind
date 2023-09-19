using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.SKCharts;
using Microsoft.Win32;
using ReMIND.Client.Business;
using ReMIND.Client.Business.Models;
using ReMIND.Client.Business.Models.Types;

namespace ReMIND.Client.AnalyticsTab
{
    /// <summary>
    /// Interaction logic for AnalyticsViewer.xaml
    /// </summary>
    public partial class AnalyticsViewer : UserControl
    {

        #region Properties & Data
        public FromToDate IntervalFilter { get; set; } = new();
        public string TagFilter { get; set; }

        public bool includeTeams
        {
            get => TeamsDualList.IsEnabled;
            set => TeamsDualList.IsEnabled = value;
        }
        public bool includeEmployees
        {
            get => AccountsDualList.IsEnabled;
            set => AccountsDualList.IsEnabled = value;
        }
        #endregion

        public AnalyticsViewer()
        {
            Utility.analyticsViewer = this;
            InitializeComponent();
            Tools.GraphColorManager.loadColors();
            IntervalFilter.DateFrom = DateTime.Now.Date.AddDays(-7);
            IntervalFilter.DateTo = DateTime.Now.Date;
        }

        #region Load
        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            GraphPresenter.resetGraphData();
            IntervalFilter = new();
            WeekPeriodButton.IsChecked = false;
            WeekPeriodButton.IsChecked = true;

            List<string> tagNames = await Utility.GetNamesTagNames();
            TagDropdown.AttachData(tagNames.ToArray());

            TeamsToggle.IsChecked = true;
            EmployeesToggle.IsChecked = true;

            List<Person> people = new();
            List<string> peopleNames = await Utility.GetEmployeNames();
            foreach(Team t in Utility.TeamsILead)
            {
                people = people.Union(await Utility.GetAllPeopleByTeam(t)).ToList();
            }
            List<Person> displayPeople = new();
            foreach(Person p in people)
            {
                if (peopleNames.Contains(p.Name)) ;
                displayPeople.Add(p);
            }
            //List<string> peopleNames = await Utility.Get
            AccountsDualList.AttachData(displayPeople.ToArray());

            List<Team> teams = new();
            teams = Utility.TeamsILead;
            List<string> teamNames = new();

            teamNames = teamNames.Union(await Utility.GetTeams()).ToList();

            //ucitamo samo teamove koje trenutno leadujemo i koje postoje u arhivi oba moraju da budu tacno
            List<string> teamsIleadString = new();
            foreach(Team t in teams)
            {
                if(teamNames.Contains(t.Name))
                    teamsIleadString.Add(t.Name);
            }

            TeamsDualList.AttachData(teamsIleadString.ToArray());

            //ReloadGraph();
        }
        #endregion

        #region Date Filters
        private void FromDate_SelectedDateChanged(object sender, DateTime? e)
        {
            if (e == null)
                return;

            if (e.Value > DateTime.Now.Date)
            {
                FromDatePicker.SelectDate(DateTime.Now.Date);
                return;
            }
            
            WeekPeriodButton.IsChecked = false;
            MonthPeriodButton.IsChecked = false;
            Month3PeriodButton.IsChecked = false;
            Month6PeriodButton.IsChecked = false;
            YearPeriodButton.IsChecked = false;
            IntervalFilter.DateFrom = e;

            if (!ToDatePicker.IsDateSelected)
                IntervalFilter.DateTo = DateTime.Now.Date;
            ReloadGraph();
            
        }

        private void ToDate_SelectedDateChanged(object sender, DateTime? e)
        {
            if (e == null)
                return;

            if (e.Value > DateTime.Now.Date)
            {
                ToDatePicker.SelectDate(DateTime.Now.Date);
                return;
            }

            if (!FromDatePicker.IsDateSelected)
                    return;

            WeekPeriodButton.IsChecked = false;
            MonthPeriodButton.IsChecked = false;
            Month3PeriodButton.IsChecked = false;
            Month6PeriodButton.IsChecked = false;
            YearPeriodButton.IsChecked = false;
            IntervalFilter.DateTo = e;
            ReloadGraph();
        }
        #endregion

        #region Period Buttons
        private void Week_Checked(object sender, RoutedEventArgs e)
        {
            IntervalFilter.DateFrom = DateTime.Now.Date.AddDays(-7);
            IntervalFilter.DateTo = DateTime.Now.Date;
            ReloadGraph();
        }

        private void Month_Checked(object sender, RoutedEventArgs e)
        {
            IntervalFilter.DateFrom = DateTime.Now.Date.AddMonths(-1);
            IntervalFilter.DateTo = DateTime.Now.Date;
            ReloadGraph();

        }

        private void Month3_Checked(object sender, RoutedEventArgs e)
        {
            IntervalFilter.DateFrom = DateTime.Now.Date.AddMonths(-3);
            IntervalFilter.DateTo = DateTime.Now.Date;
            ReloadGraph();
        }

        private void Month6_Checked(object sender, RoutedEventArgs e)
        {
            IntervalFilter.DateFrom = DateTime.Now.Date.AddMonths(-6);
            IntervalFilter.DateTo = DateTime.Now.Date;
            ReloadGraph();
        }

        private void Year_Checked(object sender, RoutedEventArgs e)
        {
            IntervalFilter.DateFrom = DateTime.Now.Date.AddMonths(-12);
            IntervalFilter.DateTo = DateTime.Now.Date;
            ReloadGraph();
        }
        #endregion

        #region DualList Toggles (Show/Hide)
        private void TeamsToggle_Checked(object sender, RoutedEventArgs e)
        {
            includeTeams = true;
            ReloadGraph();
        }
        private void TeamsToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            includeTeams = false;
            ReloadGraph();
        }

        private void EmployeesToggle_Checked(object sender, RoutedEventArgs e)
        {
            includeEmployees = true;
            ReloadGraph();
        }
        private void EmployeesToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            includeEmployees = false;
            ReloadGraph();
        }
        #endregion

        #region DualList Events
        private void TeamsDualList_SelectedItemsChanged(object sender, bool e)
        {
            ReloadGraph();
        }

        private void AccountsDualList_SelectedItemsChanged(object sender, bool e)
        {
            ReloadGraph();
        }
        #endregion

        #region Tags & Clear
        private void TagDropdown_SelectedItemChanged(object sender, object e)
        {
            if (e.ToString() == "")
                TagFilter = null;
            else
                TagFilter = e.ToString();
            ReloadGraph();
        }

        private void CleanButton_ButtonClicked(object sender, RoutedEventArgs e)
        {
            TagDropdown.ClearSelectionWithoutEvents();
            FromDatePicker.IsDateSelected = false;
            ToDatePicker.IsDateSelected = false;
            AccountsDualList.ResetControlKeepData();
            TeamsDualList.ResetControlKeepData();

            //this one reloads the graph by itself, others shouldn't
            WeekPeriodButton.IsChecked = true;
        }
        #endregion

        public async void ReloadGraph()
        {
            GraphPresenter.resetGraphData();
            GraphPresenter.updateLabels(IntervalFilter);

            #region Employees
            if (includeEmployees)
            {
                //lista selected naloga iz duallist
                List<Person> people = new();
                var selectedDictionary = AccountsDualList.GetAllSelected<Person>();
                foreach(var pair in selectedDictionary)
                    people.Add(pair.Key);

                //kreiranje dictionary koji vezuje odredjenu osobu sa poslovima
                Dictionary<Person, List<JobArchive>> archivedPeopleJobs = new Dictionary<Person, List<JobArchive>>();

                //punimo za svaku osobu relevant poslove koje povucemo iz baze
                foreach(Person p in people)
                {
                    List<JobArchive> jobs = await Utility.GetAllJobArhiveQuerry(IntervalFilter, "q?employeeID=" + p.ID + "&tagName=" + TagFilter , 0, 10000);
                    //List<JobArchive> jobs = await Utility.GetAllJobArhiveQuerry(IntervalFilter, "q?employeeID=" + p.ID, 0, 10000);
                    archivedPeopleJobs.Add(p, jobs);
                }

                GraphPresenter.addEmployeeData(archivedPeopleJobs, selectedDictionary);
            }
            #endregion

            #region Teams
            if (includeTeams)
            {
                //piksi ovde povlacimo podatke za Teams
                //imas this.IntervalFilter
                //imas this.TagFilter

                //nikola:izvucicu ovde listu naloga
                List<string> teams = new();
                teams = await Utility.GetTeams();
                var selectedDictionary = TeamsDualList.GetAllSelected<string>();
                Dictionary<string, List<JobArchive>> archivedPeopleJobs = new Dictionary<string, List<JobArchive>>();
                List<JobArchive> archivedTeamJobs = new();
                foreach (var d in selectedDictionary)
                {
                    List<JobArchive> jobs = await Utility.GetAllJobArhiveQuerry(IntervalFilter, "q?teamName=" + d.Key.ToString() + "&tagName=" + TagFilter, 0, 10000);
                    //List<JobArchive> jobs = await Utility.GetAllJobArhiveQuerry(IntervalFilter, "q?teamName=" + d.Key.ToString(), 0, 10000);
                    archivedPeopleJobs.Add(d.Key, jobs);
                }
                GraphPresenter.addTeamsData(archivedPeopleJobs, selectedDictionary);

            }
            #endregion
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveDialog = new();

            saveDialog.Filter = "png files (*.png)|*.png";
            saveDialog.FilterIndex = 1;

            if (saveDialog.ShowDialog() == true)
            {
                var chart = new SKCartesianChart(GraphPresenter.chart);
                chart.SaveImage(saveDialog.FileName);


                ReMINDMessage.Show("Graph saved succesfully under the path \n" + saveDialog.FileName, false, "Image saved.");

                Process.Start(new ProcessStartInfo
                {
                    FileName = saveDialog.FileName,
                    UseShellExecute = true
                });
            }
        }
    }
}
