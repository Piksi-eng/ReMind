using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using ReMIND.Client.Business;
using ReMIND.Client.Business.Models;

namespace ReMIND.Client.TasksTab
{
    /// <summary>
    /// Interaction logic for GroupItem.xaml
    /// </summary>
    public partial class GroupItem : UserControl
    {
        public bool _TestMode { get; set; } = false;
        private JobGroup _group;
        public JobGroup AttachedGroup
        {
            get { return _group; }
            set
            {
                _group = value;
                RefreshStyle();
            }
        }

        public string SearchString => _group.Name.ToLower() + _group.Description.ToLower() + _group.Team.Name.ToLower();
        public string UpperText { get; set; }
        public string LowerText { get; set; }
        public GroupItem()
        {
            InitializeComponent();
            DataContext = this;
            AttachedGroup = new JobGroup();
            loadTestData();
        }
        public GroupItem(JobGroup group)
        {
            InitializeComponent();
            DataContext = this;
            AttachedGroup = group;
        }
        private void RefreshStyle()
        {
            UpperText = AttachedGroup.Team.Name;
            LowerText = AttachedGroup.Name;

            if (Utility.employeeRestriction)
            {
                editButton.Visibility = Visibility.Collapsed;
                deleteButton.Visibility = Visibility.Collapsed;
            }
            if (Utility.leaderRestriction)
            {
                bool hide = true;
                foreach (Team t in Utility.TeamsILead)
                {
                    if (AttachedGroup.teamID == t.ID)
                        hide = false;
                }
                if (hide)
                {
                    editButton.Visibility = Visibility.Collapsed;
                    deleteButton.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            ButtonsPanel.Visibility = Visibility.Visible;
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            ButtonsPanel.Visibility = Visibility.Hidden;
        }
        private void UserControl_PreviewDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (_TestMode)
                return;

            Utility.OpenViewGroup(AttachedGroup);
        }

        #region Buttons
        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (_TestMode)
                return;
            bool answer = ReMINDMessage.Show("Are you sure you want to delete" + AttachedGroup.Name, true, "Confirm");

            if(answer)
            {
                bool choise = ReMINDMessage.Show("Do you want to delete all jobs under this group?", true, "Confirm");
                await Utility.DeleteJobGroup(AttachedGroup, choise);
            }
            Utility.ReloadTasksData();
        }
        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (_TestMode)
                return;

            Utility.OpenEditGroup(AttachedGroup);
        }
        #endregion

        private void loadTestData()
        {
            Random rand = new Random();
            int i = rand.Next(0, 100);
            if (i < 50)
            {
                AttachedGroup.Team.Name = "Tim Raketa";
                if (i < 25)
                    AttachedGroup.Name = "Zavrsi UI rakete";
                else
                    AttachedGroup.Name = "Napravi pametne usisivace";
            }
            else
            {
                AttachedGroup.Team.Name = "Samo Ventilatori";
                if (i < 75)
                    AttachedGroup.Name = "Povezi Client i Server";
                else
                    AttachedGroup.Name = "Zavrsi TasksPage";
            }
            RefreshStyle();
        }

    }
}