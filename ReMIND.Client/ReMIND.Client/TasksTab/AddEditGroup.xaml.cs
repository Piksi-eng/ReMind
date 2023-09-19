using ReMIND.Client.Business.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using ReMIND.Client.Business.Models;

namespace ReMIND.Client.TasksTab
{
    /// <summary>
    /// Interaction logic for AddEditGroup.xaml
    /// </summary>
    public partial class AddEditGroup : UserControl
    {
        public event EventHandler<bool> RequestWindowClose;
        public JobGroup JobGroup { get; set; }

        public AddEditGroup()
        {
            InitializeComponent();
            DataContext = this;
            JobGroup = null;
        }
        public AddEditGroup(JobGroup group)
            : this()
        {
            JobGroup = group;
        }

        #region Load
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            loadDefaults();
            populateFields();
            TeamsDropdown.ResetButtonVisible = false;
        }
        public void loadDefaults()
        {
            List<Team> teams = new();
            teams = Utility.MyTeams;

            TeamsDropdown.AttachData(teams.ToArray());
        }
        public void populateFields()
        {
            if (JobGroup == null)
            {
                return;
            }

            JobNameInput.TextValue = JobGroup.Name;
            DescriptionBox.AppendText(JobGroup.Description);
            SaveButton.DisplayText = "UPDATE";

            Team groupTeam = new();
            groupTeam = JobGroup.Team;
            TeamsDropdown.selectObject(groupTeam);
        }
        #endregion

        #region Buttons
        private void SaveButton_ButtonClicked(object sender, RoutedEventArgs e)
        {
            if (!CheckFields())
                return;

            if (JobGroup != null)
            {//update
                updateGroup(getGroupFromFields());
            }
            else
            {//create
                saveGroup(getGroupFromFields());
            }
            RequestWindowClose?.Invoke(this, true);
        }

        private void CancelButton_ButtonClicked(object sender, RoutedEventArgs e)
        {
            RequestWindowClose?.Invoke(this, false);
        }
        #endregion

        #region Helper Methods
        private JobGroup getGroupFromFields()
        {
            JobGroup group = new();
            //group.ID = JobGroup.ID;
            group.Name = JobNameInput.TextValue;
            group.Team = ObjectConverter.ToTypedInstance<Team>(TeamsDropdown.getSelectedObject());
            group.Description = DescriptionBox.Text;

            return group;
        }
        private async void updateGroup(JobGroup group)
        {
            group.ID = JobGroup.ID;
            await Utility.UpdateJobGroup(group);
            Utility.ReloadTasksData();

        }
        private async void saveGroup(JobGroup group)
        {
            await Utility.CreateJobGroup(group);
            Utility.ReloadTasksData();

        }
        #endregion

        public bool CheckFields()
        {
            bool passed = true;
            if(JobNameInput.TextValue == "")
            {
                JobNameInput.flickerWithRed();
                passed = false;
            }
            if(TeamsDropdown.getSelectedString() == "")
            {
                TeamsDropdown.flickerWithRed();
                passed = false;
            }
            if (DescriptionBox.Text == "")
            {
                Utility.flickerPropertyWithRed(DescriptionBox, BackgroundProperty, ((SolidColorBrush)DescriptionBox.Background).Color);
                passed = false;
            }

            return passed;
        }
    }
}
