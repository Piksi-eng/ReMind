using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using ReMIND.Client.Business.Models;
using ReMIND.Client.Business.Models.Types;

namespace ReMIND.Client.Management
{
    /// <summary>
    /// Interaction logic for TeamItem.xaml
    /// </summary>
    public partial class TeamItem : UserControl
    {
        public event EventHandler<Team> EditClicked;
        public Team Team { get; set; }
        public TeamItem(Team team)
        {
            InitializeComponent();
            DataContext = this;
            Team = team;

            SearchString = team.Name.ToLower();
            foreach(JobGroup group in Team.TaskGroup)
                SearchString += group.Name.ToLower();
        }
        public string SearchString { get; }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            EditButton.Visibility = Visibility.Visible;
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            EditButton.Visibility = Visibility.Hidden;
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            EditClicked?.Invoke(this, Team);
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            int membersNum = 0;
            int leadersNum = 0;
            List<TeamLink> AllPersonTeamLinks = await Utility.GetTeamLinksByTeam(Team);
            membersNum = AllPersonTeamLinks.Count;
            foreach(TeamLink tl in AllPersonTeamLinks)
            {
                if (tl.Title == TitleType.Leader)
                    leadersNum++;
            }



            MembersTextblock.Text = $"{membersNum} member";
            if (membersNum != 1) MembersTextblock.Text += "s";

            LeadersTextblock.Text = $"{leadersNum} leader";
            if (leadersNum != 1) LeadersTextblock.Text += "s";
        }
    }
}
