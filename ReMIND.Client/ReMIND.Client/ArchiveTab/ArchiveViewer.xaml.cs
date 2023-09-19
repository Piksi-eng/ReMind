using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

using ReMIND.Client.Business.Models;
using ReMIND.Client.Business.Models.Types;

namespace ReMIND.Client.ArchiveTab
{
    /// <summary>
    /// Interaction logic for ArchiveViewer.xaml
    /// </summary>
    public partial class ArchiveViewer : UserControl
    {

        public ArchiveViewer()
        {
            Utility.archiveViewer = this;
            InitializeComponent();
        }

        #region Data & Properties
        int CurrentPage = 0; //pages start from 0
        int ItemsPerPage = 50;
        string NameFilter = "";
        string TeamFilter = "";
        string ContactFilter = "";
        string GroupFilter = "";
        string TagFilter = "";
        Person EmployeeFilter = null;
        bool? FinishedFilter = null;
        int? WeightFilter = null;
        FromToDate IntervalFilter = new();

        SortEnum _sort = SortEnum.None; //please access this through the property, at any time.
        public SortEnum CurrentSortStyle
        {
            get => _sort;
            set
            {
                _sort = value;
                //SortItems();
            }
        }
        bool EndOfList = false;
        #endregion

        #region Load
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ReloadPage();
        }
        private async void loadDropdowns()
        {
            #region Names
            List <string> names = new();
            names = await Utility.GetNames();
            NameDropdown.AttachData(names.ToArray());
            #endregion

            #region Teams
            List<string> teams = new();
            teams = await Utility.GetTeams();
            TeamDropdown.AttachData(teams.ToArray());
            #endregion

            #region Contacts
            List<string> contacts = new();
            contacts = await Utility.GetContacts();
            ContactDropdown.AttachData(contacts.ToArray());
            #endregion

            #region Groups
            List<string> groupNames = new();
            groupNames = await Utility.GetGroupNames();
            GroupDropdown.AttachData(groupNames.ToArray());
            #endregion

            #region Types
            TagDropdown.AttachData(Utility.JobTags.ToArray());
            #endregion

            #region Employees
            List<Person> people = new();
            if (Utility.UserTittle == TitleType.Leader)
            {
                foreach (Team t in Utility.TeamsILead)
                {
                    List<Person> peopleToAdd = await Utility.GetAllPeopleByTeam(t);
                    people = people.Union(peopleToAdd).Distinct().ToList();
                }
            }
            else if (Utility.UserTittle == TitleType.Admin)
            {
                foreach (Team t in Utility.MyTeams)
                {
                    List<Person> peopleToAdd = await Utility.GetAllPeopleByTeam(t);
                    people = people.Union(peopleToAdd).Distinct().ToList();
                }
            }
            EmployeeDropdown.AttachData(people.ToArray());
            #endregion

            #region Finished
            string[] finishedOptions = { "yes", "no" };
            FinishedDropdown.AttachData(finishedOptions);
            #endregion

            #region Weight
            object[] weights = { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
            WeightDropdown.AttachData(weights);
            #endregion
        }
        public void ReloadPage()
        {
         
            loadDropdowns();
            reloadArchive();
        }
        #endregion

        #region Filters
        private void Name_SelectedItemChanged(object sender, object e)
        {
            NameFilter = e.ToString();
            reloadArchive();
        }
        private void Team_SelectedItemChanged(object sender, object e)
        {
            TeamFilter = e.ToString();
            reloadArchive();
        }
        private void Contact_SelectedItemChanged(object sender, object e)
        {
            ContactFilter = e.ToString();
            reloadArchive();
        }
        private void Group_SelectedItemChanged(object sender, object e)
        {
            GroupFilter = e.ToString();
            reloadArchive();
        }
        private void Tag_SelectedItemChanged(object sender, object e)
        {
            TagFilter = e.ToString();
            reloadArchive();
        }
        private void Employee_SelectedItemChanged(object sender, object e)
        {
            if (e.ToString() != "")
                EmployeeFilter = ObjectConverter.ToTypedInstance<Person>(e);
            else
                EmployeeFilter = null;
            reloadArchive();
        }
        private void Finished_SelectedItemChanged(object sender, object e)
        {
            FinishedFilter = e.ToString() == "yes" ?
                             true : e.ToString() != "" ?
                             false : null;
            reloadArchive();
        }
        private void Weight_SelectedItemChanged(object sender, object e)
        {
            WeightFilter = e.ToString() == "" ?
                           null : Convert.ToInt32(e.ToString());
            reloadArchive();
        }
        private void FromDate_SelectedDateChanged(object sender, DateTime? e)
        {
            IntervalFilter.DateFrom = e;
            reloadArchive();
        }
        private void ToDate_SelectedDateChanged(object sender, DateTime? e)
        {
            IntervalFilter.DateTo = e;
            reloadArchive();
        }
        #endregion 

        #region Sorters
        private void uncheckOtherToggles(object tb)
        {
            ToggleButton sender = (ToggleButton)tb;

            for(int i = 1; i < SortersStackPanel.Children.Count; i++)
            {
                Border b = (Border)SortersStackPanel.Children[i];
                ToggleButton toggleToCheck = (ToggleButton)b.Child;
                if (toggleToCheck.Equals(sender))
                    continue;

                toggleToCheck.IsChecked = null;
            }

            SortItems();
        }
        private void SortItems()
        {
            for (int i = 0; i < ArchiveItemsStackPanel.Children.Count; i++)
            {
                ArchiveItem ai1 = (ArchiveItem)ArchiveItemsStackPanel.Children[i];
                for (int j = i + 1; j < ArchiveItemsStackPanel.Children.Count; j++)
                {
                    ArchiveItem ai2 = (ArchiveItem)ArchiveItemsStackPanel.Children[j];
                    if (ai1 < ai2)
                    {
                        JobArchive temp = ai1.ArchivedJob;
                        ai1.AttachData(ai2.ArchivedJob);
                        ai2.AttachData(temp);
                    }
                }
            }

        }
        #endregion

        #region Clear selection
        private void ClearButton_ButtonClicked(object sender, RoutedEventArgs e)
        {
            NameDropdown.ClearSelectionWithoutEvents();
            TeamDropdown.ClearSelectionWithoutEvents();
            ContactDropdown.ClearSelectionWithoutEvents();
            GroupDropdown.ClearSelectionWithoutEvents();
            TagDropdown.ClearSelectionWithoutEvents();
            EmployeeDropdown.ClearSelectionWithoutEvents();
            FinishedDropdown.ClearSelectionWithoutEvents();
            WeightDropdown.ClearSelectionWithoutEvents();
            FromDatePicker.IsDateSelected = false;
            ToDatePicker.IsDateSelected = false;
            ResetVariables();
            CurrentPage = 0;
            reloadArchive();
        }
        private void ResetVariables()
        {
            NameFilter = "";
            TeamFilter = "";
            ContactFilter = "";
            GroupFilter = "";
            TagFilter = "";
            EmployeeFilter = null;
            FinishedFilter = null;
            WeightFilter = null;
        }
        #endregion

        #region God forbid
        private void NameSortToggle_Checked(object sender, RoutedEventArgs e) { CurrentSortStyle = SortEnum.NameAsc; uncheckOtherToggles(sender); }

        private void NameSortToggle_Unchecked(object sender, RoutedEventArgs e) { CurrentSortStyle = SortEnum.NameDesc; uncheckOtherToggles(sender); }

        private void TeamSortToggle_Checked(object sender, RoutedEventArgs e) { CurrentSortStyle = SortEnum.TeamAsc; uncheckOtherToggles(sender); }

        private void TeamSortToggle_Unchecked(object sender, RoutedEventArgs e) { CurrentSortStyle = SortEnum.TeamDesc; uncheckOtherToggles(sender); }

        private void ContactSortToggle_Checked(object sender, RoutedEventArgs e) { CurrentSortStyle = SortEnum.ContactAsc; uncheckOtherToggles(sender); }

        private void ContactSortToggle_Unchecked(object sender, RoutedEventArgs e) { CurrentSortStyle = SortEnum.ContactDesc; uncheckOtherToggles(sender); }

        private void GroupSortToggle_Checked(object sender, RoutedEventArgs e) { CurrentSortStyle = SortEnum.GroupAsc; uncheckOtherToggles(sender); }

        private void GroupSortToggle_Unchecked(object sender, RoutedEventArgs e) { CurrentSortStyle = SortEnum.GroupDesc; uncheckOtherToggles(sender); }

        private void TypeSortToggle_Checked(object sender, RoutedEventArgs e) { CurrentSortStyle = SortEnum.TagAsc; uncheckOtherToggles(sender); }

        private void TypeSortToggle_Unchecked(object sender, RoutedEventArgs e) { CurrentSortStyle = SortEnum.TagDesc; uncheckOtherToggles(sender); }

        private void EmployeeSortToggle_Checked(object sender, RoutedEventArgs e) { CurrentSortStyle = SortEnum.EmployeeAsc; uncheckOtherToggles(sender); }

        private void EmployeeSortToggle_Unchecked(object sender, RoutedEventArgs e) { CurrentSortStyle = SortEnum.EmployeeDesc; uncheckOtherToggles(sender); }

        private void FinishedSortToggle_Checked(object sender, RoutedEventArgs e) { CurrentSortStyle = SortEnum.FinishedAsc; uncheckOtherToggles(sender); }

        private void FinishedSortToggle_Unchecked(object sender, RoutedEventArgs e) { CurrentSortStyle = SortEnum.FinishedDesc; uncheckOtherToggles(sender); }

        private void WeightSortToggle_Checked(object sender, RoutedEventArgs e) { CurrentSortStyle = SortEnum.WeightAsc; uncheckOtherToggles(sender); }

        private void WeightSortToggle_Unchecked(object sender, RoutedEventArgs e) { CurrentSortStyle = SortEnum.WeightDesc; uncheckOtherToggles(sender); }

        private void DateSortToggle_Checked(object sender, RoutedEventArgs e) { CurrentSortStyle = SortEnum.DateAsc; uncheckOtherToggles(sender); }

        private void DateSortToggle_Unchecked(object sender, RoutedEventArgs e) { CurrentSortStyle = SortEnum.DateDesc; uncheckOtherToggles(sender); }

        #endregion
        
        #region Scroll
        private void Scroller_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            double marginTop = Scroller.VerticalOffset * Scroller.ViewportHeight / Scroller.ExtentHeight;
            if (double.IsNaN(marginTop)) //ako delimo nulom onda je bljak pa moramo da mu stavimo value na 0
                marginTop = 0;
            ScrollBorder.Margin = new Thickness(3, 1 + marginTop, 3, 1);

            double scrollbarHeight = Scroller.ViewportHeight * Scroller.ViewportHeight / Scroller.ExtentHeight;
            if (double.IsInfinity(scrollbarHeight)) //similar thing here
                scrollbarHeight = Scroller.ViewportHeight;

            ScrollBorder.Height = Scroller.ExtentHeight >= Scroller.ViewportHeight ? //ne pitaj zasto
                scrollbarHeight : Scroller.ExtentHeight;


            if (ArchiveItemsStackPanel.Children.Count == 0 || EndOfList)
                return;

            if (Scroller.VerticalOffset + Scroller.ViewportHeight >= Scroller.ExtentHeight)
                nextPage();
        }
        #endregion

        #region Data Interaction Methods
        private async void reloadArchive()
        {
            Utility.StartLoading();

            CurrentPage = 0;
            ArchiveItemsStackPanel.Children.Clear();
            List<JobArchive> archive = await getArchiveItems();

            int i = 1;
            foreach (JobArchive job in archive)
            {
                ArchiveItem ai = new(i++, job);
                ai.ArchiveParent = this;
                ArchiveItemsStackPanel.Children.Add(ai);
            }
            Utility.StopLoading();
        }
        private async void nextPage()
        {
            CurrentPage++;
            List<JobArchive> nextArchivePage = await getArchiveItems();
            if (nextArchivePage == null)
                EndOfList = true;

            int i = CurrentPage * ItemsPerPage;
            foreach (JobArchive job in nextArchivePage)
            {
                ArchiveItem ai = new(i++, job);
                ai.ArchiveParent = this;
                ArchiveItemsStackPanel.Children.Add(ai);
            }
        }
        private async Task<List<JobArchive>> getArchiveItems()
        {
            List<JobArchive> archive = new();
            int startIndex = CurrentPage * ItemsPerPage;
            int endIndex = (CurrentPage * ItemsPerPage) + ItemsPerPage;
            //place holder
            string filter = createQuery();

            archive = await Utility.GetAllJobArhiveQuerry(IntervalFilter, filter, startIndex, endIndex);
            return archive;
        }

        //q?jobName=Meeting&weight=2
        public string createQuery()
        {
            string query = "q?";
            if (NameFilter != "")
                query = query + "jobName=" + NameFilter + "&";
            if ( TeamFilter != "")
                query = query + "teamName=" + TeamFilter + "&";
            if (ContactFilter != "")
                query = query + "contact=" + ContactFilter + "&";
            if (GroupFilter != "")
                query = query + "groupName=" + GroupFilter + "&";
            if (TagFilter != "")
                query = query + "tagName=" + TagFilter + "&"; 
            if (EmployeeFilter != null)
                query = query + "employeeID=" + EmployeeFilter.ID + "&";
            if (FinishedFilter != null)
                query = query + "finished=" + FinishedFilter + "&";
            if (WeightFilter != null)
                query = query + "weight=" + WeightFilter + "&";


            query = query.Remove(query.Length - 1);
            return query;
        }
        #endregion

    }
}
