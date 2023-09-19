using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using ReMIND.Client.Business;
using ReMIND.Client.Business.Models;
using ReMIND.Client.Business.Models.Types;

namespace ReMIND.Client.Management
{
    /// <summary>
    /// Interaction logic for AddEditTeam.xaml
    /// </summary>
    public partial class AddEditTeam : UserControl
    {
        Function _function = Function.Create;
        Function function
        {
            get => _function;
            set
            {
                _function = value;
                SaveButton.DisplayText = value == Function.Create ?
                    "SAVE" : "UPDATE";
            }
        }

        public Team Team = new();
        public AddEditTeam(Team t = null)
        {
            InitializeComponent();
            DataContext = this;

            if (t == null)
                t = new();
            else
            {
                Team = t;
                function = Function.Update;
            }
        }
        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //loading here for testing purpuses

            List<Person> accounts = new();
            accounts = await Utility.GetAllPeople();
            accounts.RemoveAll(person => person.IsEmployed == false);
            AccountDualListPicker.AttachData(accounts.ToArray());

            if(function == Function.Update)
            {
                TeamNameInput.TextValue = Team.Name;

                List<Person> members = new();
                members = await Utility.GetAllPeopleByTeam(Team);
                AccountDualListPicker.SelectData(members.ToArray());

                List<TeamLink> AllPersonTeamLinks = await Utility.GetTeamLinksByTeam(Team);
                List<Person> leaders = new();
                foreach (TeamLink tl in AllPersonTeamLinks)
                {
                    if (tl.Title == TitleType.Leader)
                        leaders.Add(await Utility.GetPersonByID(tl.personID));
                }
                AccountDualListPicker.CheckButtons(leaders.ToArray());
            }
        }

        private async void SaveButton_ButtonClicked(object sender, RoutedEventArgs e)
        {
            if (!CheckFields())
                return;

            Team.Name = TeamNameInput.TextValue;

            if(function == Function.Create)
            {
                bool Answer = ReMINDMessage.Show("Are you sure you want to create a new team named '" + Team.Name + "'?", true, "Confirm");
                if(Answer)
                {
                    Team toCreate = Team;
                    Team createdTeam = await Utility.CreateTeam(toCreate);
                    if(createdTeam !=null)
                    {
                        List<Person> membersToAdd = ObjectConverter.ToTypedList<Person>(AccountDualListPicker.GetAdded());
                        List<Person> lidersToAdd = ObjectConverter.ToTypedList<Person>(AccountDualListPicker.GetNewChecked());
                        membersToAdd = membersToAdd.Except(lidersToAdd).ToList();
                        foreach (Person p in membersToAdd)
                        {
                            await Utility.AddPersonToTeam(p, createdTeam, TitleType.Employee);
                        }
                        foreach (Person p in lidersToAdd)
                        {
                            await Utility.AddPersonToTeam(p, createdTeam, TitleType.Leader);
                        }
                        ReMINDMessage.Show("Team Created", false, "Success");
                        Utility.ReloadManagementData();
                        Utility.ReloadTasksData();
                        await Utility.GetAllMyTeams();
                    }
                }

            }
            else//update
            {
                bool Answer = ReMINDMessage.Show("Are you sure you want to update team '" + Team.Name + "'?", true, "Confirm");
                if (Answer)
                {
                    Team toUpdate = Team;
                    Team updatedTeam = await Utility.UpdateTeam(toUpdate);

                    if(updatedTeam != null)
                    {
                        List<Person> membersToAdd = ObjectConverter.ToTypedList<Person>(AccountDualListPicker.GetAdded());
                        List<Person> lidersToAdd = ObjectConverter.ToTypedList<Person>(AccountDualListPicker.GetNewChecked());
                        List<Person> membersToRemove = ObjectConverter.ToTypedList<Person>(AccountDualListPicker.GetRemoved());
                        List<Person> membersToUpdate = ObjectConverter.ToTypedList<Person>(AccountDualListPicker.GetUpdated());
                        membersToAdd = membersToAdd.Except(lidersToAdd).ToList();
                        foreach (Person p in membersToAdd)
                        {
                            await Utility.AddPersonToTeam(p, updatedTeam, TitleType.Employee);
                        }
                        foreach (Person p in lidersToAdd)
                        {
                            await Utility.AddPersonToTeam(p, updatedTeam, TitleType.Leader);
                        }
                        foreach (Person p in membersToRemove)
                        {
                            await Utility.RemovePersonFromTeam(p, Team);
                        }
                        foreach (Person p in membersToUpdate)
                        {
                            TeamLink teamLink = await Utility.GetTeamLinksByTeamAndPerson(Team, p);
                            if (teamLink.Title == TitleType.Employee)
                            {
                                teamLink.Title = TitleType.Leader;
                                await Utility.UpdateTeamLink(teamLink);
                            }
                            else if (teamLink.Title == TitleType.Leader)
                            {
                                teamLink.Title = TitleType.Employee;
                                await Utility.UpdateTeamLink(teamLink);
                            }
                        }
                        ReMINDMessage.Show("Team Updated", false, "Success");
                        Utility.ReloadManagementData();
                        Utility.ReloadTasksData();
                        await Utility.GetAllMyTeams();
                    }
                }
            }
            clearAll();
        }

        private void ClearButton_ButtonClicked(object sender, RoutedEventArgs e)
        {
            clearAll();
        }
        private void clearAll()
        {
            Team = new();
            function = Function.Create;
            TeamNameInput.TextValue = "";
            TeamNameInput.PlaceholderLabel.Visibility = Visibility.Visible;

            AccountDualListPicker.ResetControlKeepData();
        }

        private async void DeleteButton_ButtonClicked(object sender, RoutedEventArgs e)
        {
            bool res = await Utility.DeleteTeam(Team);
            if(res)
            {
                ReMINDMessage.Show("Team deleted", false, "Success");
                Utility.ReloadManagementData();
            }
            clearAll();
        }

        private bool CheckFields()
        {
            if(TeamNameInput.TextValue == "")
            {
                TeamNameInput.flickerWithRed();
                return false;
            }
            return true;
        }
    }
}