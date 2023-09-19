using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Windows;
using System.Windows.Controls;

using ReMIND.Client.Business;
using ReMIND.Client.Business.Models;
using ReMIND.Client.Business.Models.Types;

namespace ReMIND.Client.Management
{
    /// <summary>
    /// Interaction logic for AddEditAccount.xaml
    /// </summary>
    public partial class AddEditAccount : UserControl
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

        Person Account { get; set; }

        public AddEditAccount(Person p = null)
        {
            InitializeComponent();
            DataContext = this;

            if (p == null)
                Account = new(); 
            else
            {
                Account = p;
                function = Function.Update;
            }
        }

        private void StatusPicker_SelectedItemChanged(object sender, object e)
        {
            //makes sure the status picker isn't empty
            if (StatusPicker.getSelectedString() == "")
                StatusPicker.selectFirst();
        }

        #region Buttons
        private async void SaveButton_ButtonClicked(object sender, RoutedEventArgs e)
        {
            if (!CheckFields())
                return;

            if (_function == Function.Create)
            {
                Person ToCreate = new();
                ToCreate.Name = NameTextBox.TextValue;
                ToCreate.Email = EmailTextBox.TextValue;
                try
                {
                    MailAddress meil = new(ToCreate.Email);
                }
                catch (Exception ex)
                {
                    ReMINDMessage.Show(ex.Message, false, "Failure");
                    return;
                }
                ToCreate.Phone = PhoneTextBox.TextValue;
                ToCreate.IsEmployed = StatusPicker.SelectedItem.ToString() == "Employed";

                bool Answer = ReMINDMessage.Show("Are you sure you want to create a new user?\n  Name: " + ToCreate.Name +
                                                                                            "\n  Email: " + ToCreate.Email +
                                                                                            "\n  Phone: " + ToCreate.Phone, true, "Confirm");
                if (Answer)
                {
                    Person CreatedPerson = await Utility.CreatePerson(ToCreate);
                    if (CreatedPerson != null)
                    {
                        if(AdminCheckbox.IsChecked)
                        {
                            Team adminTeam = await Utility.GetTeamById(1);
                            await Utility.AddPersonToTeam(CreatedPerson, adminTeam, TitleType.Admin);
                        }
                        List<Team> teamsToAdd = ObjectConverter.ToTypedList<Team>(TeamsDualListPicker.GetAdded());

                        List<Team> teamsToAddLeader = ObjectConverter.ToTypedList<Team>(TeamsDualListPicker.GetNewChecked());

                        teamsToAdd = teamsToAdd.Except(teamsToAddLeader).ToList();

                        foreach (Team tim in teamsToAdd)
                        {
                            await Utility.AddPersonToTeam(CreatedPerson, tim, TitleType.Employee);
                        }
                        foreach (Team tim in teamsToAddLeader)
                        {
                            await Utility.AddPersonToTeam(CreatedPerson, tim, TitleType.Leader);
                        }
                        ReMINDMessage.Show("User created", false, "Success");
                        Utility.ReloadManagementData();
                        Utility.ReloadTasksData();
                    }
                }
            }
            else //update part
            {
                Person toUpdate = new();
                toUpdate.ID = Account.ID;
                toUpdate.Name = NameTextBox.TextValue;
                toUpdate.Email = EmailTextBox.TextValue;
                toUpdate.Phone = PhoneTextBox.TextValue;
                toUpdate.IsEmployed = StatusPicker.SelectedItem.ToString() == "Employed";
                bool Answer = ReMINDMessage.Show("Are you sure you want to update User: " + Account.Name, true, "Confirm");
                if (Answer)
                {
                    if (AdminCheckbox.IsChecked)
                    {
                        if (TitleType.Admin != await Utility.GetTittle(Account.ID))
                        {
                            Team adminTeam = await Utility.GetTeamById(1);
                            await Utility.AddPersonToTeam(toUpdate, adminTeam, TitleType.Admin);
                        }
                    }
                    else
                    {
                        if (TitleType.Admin == await Utility.GetTittle(Account.ID))
                        {
                            Team adminTeam = await Utility.GetTeamById(1);
                            await Utility.RemovePersonFromTeam(toUpdate, adminTeam);
                        }
                    }

                    bool res = await Utility.UpdatePerson(toUpdate);
                    if(res)
                    {
                        List<Team> teamsToAdd = ObjectConverter.ToTypedList<Team>(TeamsDualListPicker.GetAdded());
                        List<Team> teamsToDelete = ObjectConverter.ToTypedList<Team>(TeamsDualListPicker.GetRemoved());
                        List<Team> teamsToAddLeader = ObjectConverter.ToTypedList<Team>(TeamsDualListPicker.GetNewChecked());
                        List<Team> teamsToUpdated = ObjectConverter.ToTypedList<Team>(TeamsDualListPicker.GetUpdated());
                        teamsToAdd = teamsToAdd.Except(teamsToAddLeader).ToList();
                        foreach (Team tim in teamsToAdd)
                        {
                            await Utility.AddPersonToTeam(toUpdate, tim, TitleType.Employee);
                        }
                        foreach (Team tim in teamsToAddLeader)
                        {
                            await Utility.AddPersonToTeam(toUpdate, tim, TitleType.Leader);
                        }
                        foreach (Team tim in teamsToDelete)
                        {
                            await Utility.RemovePersonFromTeam(toUpdate, tim);
                        }
                        foreach (Team tim in teamsToUpdated)
                        {
                            TeamLink teamLink = await Utility.GetTeamLinksByTeamAndPerson(tim, Account);
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

                        ReMINDMessage.Show("User updated", false, "Success");
                        Utility.ReloadManagementData();
                        Utility.ReloadTasksData();
                    }
                    
                }
            }
        }

        private void ClearButton_ButtonClicked(object sender, RoutedEventArgs e)
        {
            Account = new();

            NameTextBox.TextValue = ""; //name string
            EmailTextBox.TextValue = ""; //email string
            PhoneTextBox.TextValue = ""; //phone string
            StatusPicker.selectFirst();
            AdminCheckbox.IsChecked = false; //admin rights

            TeamsDualListPicker.ResetControlKeepData();

            function = Function.Create;
        }

        private async void DeleteButton_ButtonClicked(object sender, RoutedEventArgs e)
        {
            if(Account == null)
            {
                ReMINDMessage.Show("No account is seleceted", false, "Select account");
            }
            else
            {
                bool Answer = ReMINDMessage.Show("Are you sure you want to delete user: " + Account.Name, true, "Confirm");
                if (Answer)
                {
                    Account.IsEmployed = false;
                    bool res = await Utility.UpdatePerson(Account);
                    if (res)
                    {
                        ReMINDMessage.Show("User removed", false, "Success");
                        Utility.ReloadManagementData();
                    }
                }

            }
            //dodati ovaj kod ako je uspelo brisanje:
            ClearButton_ButtonClicked(ClearButton, e);
        }
        #endregion

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            string[] s = { "Employed", "Unemployed" };
            StatusPicker.AttachData(s);
            StatusPicker.ResetButtonVisible = false;
            StatusPicker.selectFirst();
            StatusPicker.DropdownButton.IsChecked = false;

            AdminCheckbox.IsChecked = false;

            if(function == Function.Create)
            {
                List<Team> allTeams = new();
                allTeams = await Utility.GetAllTeams();

                LastActiveLabel.Text = "";
                TeamsDualListPicker.AttachData(allTeams.ToArray());
                ResetPassButton.IsEnabled = false;
                ResetPassButton.Opacity = 0.3;
                PasswordLabel.Opacity = 0.3;
            }
            else
            {
                LastActiveLabel.Text = DateTime.Now.ToString("dd.MM.yyyy hh:mm");
                NameTextBox.TextValue = Account.Name;
                EmailTextBox.TextValue = Account.Email;
                PhoneTextBox.TextValue = Account.Phone;

                if (!Account.IsEmployed)
                {
                    StatusPicker.InputText.Text = "Unemployed";
                    StatusPicker.DropdownButton.IsChecked = false;
                }


                if(TitleType.Admin == await Utility.GetTittle(Account.ID))
                    AdminCheckbox.IsChecked = true;
                else
                    AdminCheckbox.IsChecked = false;

                List<Team> allTeams = await Utility.GetAllTeams();
                
                TeamsDualListPicker.AttachData(allTeams.ToArray());

                List<TeamLink> AllPersonTeamLinks = await Utility.GetTeamLinksByPerson(Account);
                List<Team> isLeader = new();
                List<Team> isMember = new();

                foreach (TeamLink tl in AllPersonTeamLinks)
                {
                    isMember.Add(await Utility.GetTeamById(tl.teamID));

                    if (tl.Title == TitleType.Leader)
                        isLeader.Add(await Utility.GetTeamById(tl.teamID));
                }

                TeamsDualListPicker.SelectData(isMember.ToArray());

                TeamsDualListPicker.CheckButtons(isLeader.ToArray());
            }
        }

        private async void ResetPassButton_ButtonClicked(object sender, RoutedEventArgs e)
        {
            await Utility.ResetPassword(Account);
            //piksi test this
        }

        private bool CheckFields()
        {
            bool passed = true;
            if (NameTextBox.TextValue == "")
            {
                NameTextBox.flickerWithRed();
                passed = false;
            }
            if (EmailTextBox.TextValue == "")
            {
                EmailTextBox.flickerWithRed();
                passed = false;
            }
            if(PhoneTextBox.TextValue == "")
            {
                PhoneTextBox.flickerWithRed();
                passed = false;
            }
            if(StatusPicker.getSelectedString() == "")
            {
                StatusPicker.flickerWithRed();
                passed = false;
            }
            return passed;
        }
    }
    enum Function
    {
        Create,
        Update
    }
}
