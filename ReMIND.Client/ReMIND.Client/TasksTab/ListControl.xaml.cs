using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

using ReMIND.Client.Business.Models;

namespace ReMIND.Client.TasksTab
{
    /// <summary>
    /// Interaction logic for ListControl.xaml
    /// </summary>
    public partial class ListControl : UserControl
    {
        public ListControl()
        {
            InitializeComponent();
            Jobs = new();
            Groups = new();
            SelectedJobs = new();
            DataContext = this;
        }

        #region Data & Properties
        public List<Job> Jobs { get; set; }
        public List<JobGroup> Groups { get; set; }
        public List<Job> SelectedJobs { get; set; }
        public MainWindow mainWindow { get; set; }

        int _selectCounter = 0;
        int SelectCounter
        {
            get => _selectCounter;
            set
            {
                if (_selectCounter == 0 && value > 0) //kada povecavamo sa nule 
                    foreach (UIElement child in ItemList.Children)
                        child.Visibility = Visibility.Collapsed;

                else if (value == 0)
                    foreach (UIElement child in ItemList.Children)
                        child.Visibility = Visibility.Visible;

                else if (value < 0)
                    value = 0;

                _selectCounter = value;
            }
        }

        #endregion

        #region Load
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            TasksToggleButton.IsChecked = true;
            if (Utility.employeeRestriction)
            {
                AddButton.Visibility = Visibility.Collapsed;
                GroupsToggleButton.Visibility = Visibility.Collapsed;
            }
        }
        #endregion

        #region Input
        public void AttachData(List<Job> jobs, List<JobGroup> groups)
        {
            ItemList.Children.Clear();
            Jobs = jobs;
            Groups = groups;

            if (GroupsToggleButton.IsChecked == true)//create new groups
            {
                foreach (var group in Groups)
                {
                    GroupItem g = new(group);
                    ItemList.Children.Add(g);
                }
            }
            else //create new jobs
            {
                foreach (var job in Jobs)
                {
                    JobItem j = new(job);
                    if (job.IsDone)//na vrh ako je zavrsen
                        ItemList.Children.Insert(0, j);
                    else
                        ItemList.Children.Add(j);
                }
            }

            SearchBar.TextValue = "";
            SelectCounter = 0;
            SelectedJobs = new();
        }
        #endregion

        #region Buttons
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (GroupsToggleButton.IsChecked == true)
            {
                Utility.OpenAddGroup();
            }
            else
            {
                Utility.OpenAddJob();
            }
        }

        private void GroupsToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            TasksToggleButton.IsChecked = false;
            GroupsToggleButton.IsEnabled = false;
            TasksToggleButton.IsEnabled = true;

            //loading groups into list
            ItemList.Children.Clear();
            SelectCounter = 0;
            foreach (JobGroup group in Groups)
            {
                GroupItem g = new(group);
                ItemList.Children.Add(g);
            }

            SearchBar.TextValue = "";
            Select(SelectedJobs);
        }

        private void TasksToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            GroupsToggleButton.IsChecked = false;
            TasksToggleButton.IsEnabled = false;
            GroupsToggleButton.IsEnabled = true;

            //loading jobs into list
            ItemList.Children.Clear();
            SelectCounter = 0;
            foreach (var job in Jobs)
            {
                JobItem j = new(job);
                ItemList.Children.Add(j);
            }

            SearchBar.TextValue = "";
            Select(SelectedJobs);
        }
        #endregion

        #region Select, Deselect & Search

        public void Select(List<Job> jobs)
        {
            if (GroupsToggleButton.IsChecked == true)
            {//select groups
                foreach (var child in ItemList.Children)
                {
                    GroupItem group = (GroupItem)child;
                    foreach(var job in jobs)
                    {
                        if (group.AttachedGroup.Jobs.Contains(job))
                        {
                            SelectCounter++;
                            SelectedJobs.Add(job);
                            group.Visibility = Visibility.Visible;
                        }
                    }
                }
            }
            else
            {//select jobs
                foreach (JobItem ji in ItemList.Children)
                {
                    foreach (var job in jobs)
                    {
                        if (ji.AttachedJob.Equals(job))
                        {
                            SelectCounter++;
                            SelectedJobs.Add(job);
                            ji.Visibility = Visibility.Visible;
                        }
                    }
                }
            }
            SearchBar.TextValue = "";
        }
        public void Deselect(List<Job> jobs)
        {
            if (GroupsToggleButton.IsChecked == true)
            {//deselect groups
                foreach (var child in ItemList.Children)
                {
                    GroupItem group = (GroupItem)child;
                    foreach (var job in jobs)
                    {
                        if (group.AttachedGroup.Jobs.Contains(job))
                        {
                            group.Visibility = Visibility.Collapsed;
                            SelectCounter--;
                            SelectedJobs.Remove(job);
                        }
                    }
                }
            }
            else
            {//deselect jobs
                foreach (JobItem ji in ItemList.Children)
                {
                    foreach (var job in jobs)
                    {
                        if (ji.AttachedJob.Equals(job))
                        {
                            ji.Visibility = Visibility.Collapsed;
                            SelectCounter--;
                            SelectedJobs.Remove(job);
                        }
                    }
                }
            }
        }

        #endregion

        #region Search
        /// <summary>
        /// Searches through the contents of the list
        /// </summary>
        private void SearchBar_InputTextChanged(object sender, string e)
        {
            string searchText = e.ToLower();//e==text iz polja
            if (e == "")
            {
                foreach (var child in ItemList.Children)
                {
                    UIElement elem = (UIElement)child;
                    elem.Visibility = Visibility.Visible;
                }
                return;
            }

            if (GroupsToggleButton.IsChecked == true)
            {//search groups
                foreach (var child in ItemList.Children)
                {
                    GroupItem group = (GroupItem)child;
                    group.Visibility = group.SearchString.Contains(searchText) ?
                        Visibility.Visible : Visibility.Collapsed;
                }
            }
            else
            {//search tasks
                foreach (var child in ItemList.Children)
                {
                    JobItem job = (JobItem)child;
                    job.Visibility = job.SearchString.Contains(searchText) ?
                        Visibility.Visible : Visibility.Collapsed;
                }
            }
        }
        #endregion

        #region Scroll
        private void Scroller_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            double marginTop = Scroller.VerticalOffset * Scroller.ViewportHeight / Scroller.ExtentHeight;
            if (double.IsNaN(marginTop)) //ako delimo nulom onda je bljak pa moramo da mu stavimo value na 0
                marginTop = 0;
            ScrollBorder.Margin = new Thickness(0, marginTop, 0, 0);

            double scrollbarHeight = Scroller.ViewportHeight / Scroller.ExtentHeight * Scroller.ViewportHeight;
            if (double.IsInfinity(scrollbarHeight)) //similar thing here
                scrollbarHeight = Scroller.ViewportHeight;
            ScrollBorder.Height = scrollbarHeight;
        }

        #endregion

    }
}