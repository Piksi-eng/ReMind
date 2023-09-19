using ReMIND.Client.Business.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using ReMIND.Client.Business;
using ReMIND.Client.Business.Models;
using ReMIND.Client.Business.Models.Types;

namespace ReMIND.Client.TasksTab
{
    /// <summary>
    /// Interaction logic for AddEditJob.xaml
    /// </summary>
    public partial class AddEditJob : UserControl
    {
        public event EventHandler<bool> RequestWindowClose;
        public Job Job { get; set; }
        public AddEditJob()
        {
            InitializeComponent();
            DataContext = this;
            Job = null;
        }
        public AddEditJob(Job job)
            : this()
        {
            Job = job;
        }

        #region Load
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            loadDefaults();
            //nikola ovde smo zvali populate feilds ali samo dovodi do bugova da evaluiramo da li stvarno treba ovo da postoj ovde

            populateFields();
        }
        public async void populateFields()
        {
            if (Job == null)
            {
                ArchiveButton.Visibility = Visibility.Hidden;
                return;
            }

            JobNameInput.TextValue = Job.Name;
            WeightDropdown.selectObject(Job.JobWeight);

            //ovo dole su objekti vezani za posao
            #region Team
            Team jobTeam = new();
            jobTeam = Job.Team;
            object[] teams = { jobTeam };

            TeamsDropdown.AttachData(teams);

            TeamsDropdown.selectObjectWithoutEvents(teams[0]);
            TeamsDropdown.IsEnabled = false; //team posla je uneditable
            #endregion

            List<Person> people = await Utility.GetAllPeopleByTeam(jobTeam);
            AccountsDualList.AttachData(people.ToArray());

            JobGroup jobGroup = new();
            if (Job.JobGroup != null && Job.JobGroup.Name != null)
            {
                jobGroup = Job.JobGroup;
                object[] groups = { jobGroup };

                GroupsDropdown.AttachData(groups);

                GroupsDropdown.selectObjectWithoutEvents(groups[0]);
            }

            GroupsDropdown.IsEnabled = false;

            #region Tag
            JobTag jobTag = new();
            jobTag = Job.JobTag;

            TagsDropdown.selectObject(jobTag);
            #endregion

            #region Assigned ppl
            //Utility.StartLoading();
            List<Job> sameJobs = await Utility.GetAllSameJobs(Job);
            Thread.Sleep(50);
            //Utility.StopLoading();
           

            List<Person> assignedPeople = new(); 
            List<Person> finishedPeople = new(); 

            foreach(Job j in sameJobs)
            {
                if (j.IsDone)
                {
                    finishedPeople.Add(j.Person);
                    assignedPeople.Add(j.Person);
                }     
                else
                {
                    assignedPeople.Add(j.Person);
                }
            }

          
            AccountsDualList.SelectData(assignedPeople.ToArray());
            AccountsDualList.CheckButtons(finishedPeople.ToArray());
            #endregion

            #region Deadline & Repeat
            JobDatePicker.SelectDate(Job.Deadline);
            RepeatCheckbox.IsChecked = Job.RecurringType != RecurringType.NonRecurring;
            JobMultiplierTextBox.Text = Job.Multiplier.ToString();
            WeekRecurring.IsChecked = Job.RecurringType == RecurringType.Weekly;
            MonthRecurring.IsChecked = Job.RecurringType == RecurringType.Monthly;
            #endregion

            #region Description
            DescriptionBox.Text = Job.Description;
            #endregion

            SaveButton.DisplayText = "UPDATE";
        }
        public void loadDefaults()
        {
            List<JobTag> tags = new();
            List<Team> teams = new();
            object[] weights = { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
            tags = Utility.JobTags;
            teams = Utility.MyTeams;

            TeamsDropdown.AttachData(teams.ToArray());
            TagsDropdown.AttachData(tags.ToArray());
            WeightDropdown.AttachData(weights);
        }

        #endregion

        #region Buttons
        private void ArchiveButton_ButtonClicked(object sender, RoutedEventArgs e)
        {
            archiveJob(getJobFromFields());
            RequestWindowClose?.Invoke(this, true);
        }
        private void SaveButton_ButtonClicked(object sender, RoutedEventArgs e)
        {
            if (!CheckFields())
                return;

            if (Job == null)//save
                saveJob(getJobFromFields());
            else//update
                updateJob(getJobFromFields());

            RequestWindowClose?.Invoke(this, true);
        }
        private void CancelButton_ButtonClicked(object sender, RoutedEventArgs e)
        {
            if (Job == null || isJobChanged())
                if (!ReMINDMessage.Show("There are some unsaved changes. Close without saving the job?", true, "Confirm"))
                    return;

            RequestWindowClose?.Invoke(this, false);
        }
        #endregion

        #region UI Events
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {//proveravanje multiplier inputa, osiguravamo se da su samo brojke izm 0 i 99
            try
            {
                TextBox tb = (TextBox)sender;
                int months = Convert.ToInt32(tb.Text);
                if (months < 0 || months > 99)
                {
                    e.Handled = true;
                    return;
                }
            }
            catch
            {
                e.Handled = true;
            }
        }

        private void CleanCheckbox_CheckedChanged(object sender, bool e)
        {
            if (e)
            {
                DateSelectionStackPanel.IsEnabled = true;
                TextBlockThatSaysEVERY.Opacity = 1;
            }
            else
            {
                DateSelectionStackPanel.IsEnabled = false;
                TextBlockThatSaysEVERY.Opacity = 0.5;
            }
        }
        #endregion

        #region Data Events
        private async void TeamsDropdown_SelectedItemChanged(object sender, object e)
        {
            if (e.ToString() == "") //event salje "" kada je selekcija prazna
            {
                GroupsDropdown.ResetControl();
                AccountsDualList.ResetControl();
                return;
            }

            Team selectedTeam = ObjectConverter.ToTypedInstance<Team>(e);
            List<JobGroup> groups = new();
            List<Person> people = new();
            people = await Utility.GetAllPeopleByTeam(selectedTeam);
            groups = await Utility.GetAllJobGroupsByTeam(selectedTeam);

            GroupsDropdown.AttachData(groups.ToArray());
            AccountsDualList.AttachData(people.ToArray());
        }
        #endregion

        #region Helper Methods
        private Job getJobFromFields()
        {
            Job newJob = new();
            //newJob.ID = Job.ID;

            newJob.Name = JobNameInput.TextValue;
            newJob.Contact = Utility.User.Email;
            newJob.Deadline = JobDatePicker.SelectedDate;
            newJob.Description = DescriptionBox.Text;

            newJob.RecurringType = RecurringType.NonRecurring;
            newJob.Multiplier = 0;
            //nikola daj mi requring tipe
            //izvini piksi, evo
            if (RepeatCheckbox.IsChecked)
            {
                if (MonthRecurring.IsChecked == true)
                    newJob.RecurringType = RecurringType.Monthly;
                else if (WeekRecurring.IsChecked == true)
                    newJob.RecurringType = RecurringType.Weekly;
                else//trebalo bi da je daily ako ova dva nisu, posto je on default checked;
                    newJob.RecurringType = RecurringType.Daily;
                newJob.Multiplier = Convert.ToInt32(JobMultiplierTextBox.Text);
            }

            newJob.LastModified = DateTime.Now;
            newJob.JobWeight = Convert.ToInt32(WeightDropdown.getSelectedString());
            
            newJob.Team = ObjectConverter.ToTypedInstance<Team>(TeamsDropdown.getSelectedObject());
            newJob.JobGroup = ObjectConverter.ToTypedInstance<JobGroup>(GroupsDropdown.getSelectedObject());
            if (newJob.JobGroup != null)
                newJob.jobGroupID = newJob.JobGroup.ID;
            newJob.JobTag = ObjectConverter.ToTypedInstance<JobTag>(TagsDropdown.getSelectedObject());
            newJob.JobTagID = newJob.JobTag.ID;
            newJob.teamID = newJob.Team.ID;

            return newJob;
        }
        private async void updateJob(Job job)
        {
            job.ID = Job.ID;
            List<Person> newPeople = ObjectConverter.ToTypedList<Person>(AccountsDualList.GetAdded());
            List<Person> deletedPeople = ObjectConverter.ToTypedList<Person>(AccountsDualList.GetRemoved());
            List<Person> finishPeople = ObjectConverter.ToTypedList<Person>(AccountsDualList.GetNewChecked());
            List<Person> updatePeople = ObjectConverter.ToTypedList<Person>(AccountsDualList.GetUpdated());

            List<Job> sameJobs = await Utility.GetAllSameJobs(Job);

            await Utility.UpdateAllJob(job);

            job.Team = Job.Team;
            job.JobGroup = Job.JobGroup;
            job.ID = 0;
            foreach(Person p in newPeople)
            {
                if (job.JobGroup.Name == null)
                    job.JobGroup = null;
                job.Person = p;
                job.personID = p.ID;
                await Utility.CreateJob(job);
            }
            foreach (Person p in deletedPeople)
            {
                foreach(Job j in sameJobs)
                {
                    if (j.personID == p.ID)
                        await Utility.DeleteJob(j);
                }
            }
            foreach (Person p in finishPeople)
            {
                foreach (Job j in sameJobs)
                {
                    if (j.personID == p.ID)
                    {
                        j.IsDone = true;
                        await Utility.UpdateJob(j);
                    }
                }
            }
            foreach (Person p in updatePeople)
            {
                Job j = sameJobs.Find(x => x.personID == p.ID);

                    if (j.personID == p.ID)
                    {
                        if (j.IsDone == false)
                            j.IsDone = true;
                        else
                            j.IsDone = false;

                        await Utility.UpdateJob(j);
                    }
            }
            Utility.ReloadTasksData();
        }
        private async void saveJob(Job job)
        {
            List<Person> assignedPeople = ObjectConverter.ToTypedList<Person>(AccountsDualList.GetAdded());
            foreach(Person p in assignedPeople)
            {
                job.Person = p;
                job.personID = p.ID;
                await Utility.CreateJob(job);
            }
            Utility.ReloadTasksData();
        }
        private async void archiveJob(Job job)
        {
            job.ID = Job.ID;
            job.personID = Job.personID;
            job.teamID = Job.teamID;
            job.JobTagID = Job.JobTagID;
            await Utility.Archive(job);
            Utility.ReloadTasksData();
        }
        private bool isJobChanged()
        {
            //note: fali lista osoba, doduse ne znam koliko je pametno jer bi bilo skupo po performance za proveru

            if (JobNameInput.TextValue != Job.Name)
                return true;
            if (ObjectConverter.ToTypedInstance<Team>(TeamsDropdown.getSelectedObject()) != Job.Team)
                return true;

            if (Job.JobGroup.Name == null
                && GroupsDropdown.getSelectedString() != "")
                return true;
            else if (Job.JobGroup.Name != null
                && ObjectConverter.ToTypedInstance<JobGroup>(GroupsDropdown.getSelectedObject()) != Job.JobGroup)
                return true;

            if (ObjectConverter.ToTypedInstance<JobTag>(TagsDropdown.getSelectedObject()) != Job.JobTag)
                return true;
            if (JobDatePicker.SelectedDate != Job.Deadline)
                return true;
            if (DescriptionBox.Text != Job.Description)
                return true;
            if (JobMultiplierTextBox.Text != Job.Multiplier.ToString())
                return true;

            RecurringType tp = RecurringType.NonRecurring;
            int mp = 0;
            if (RepeatCheckbox.IsChecked)
            {
                if (MonthRecurring.IsChecked == true)
                    tp = RecurringType.Monthly;
                else if (WeekRecurring.IsChecked == true)
                    tp = RecurringType.Weekly;
                else//trebalo bi da je daily ako ova dva nisu, posto je on default checked;
                    tp = RecurringType.Daily;
                mp = Convert.ToInt32(JobMultiplierTextBox.Text);
            }
            if (tp != Job.RecurringType)
                return true;
            if (mp != Job.Multiplier)
                return true;

            return false;
        }
        #endregion

        #region UI Red thingie
        public bool CheckFields()
        {
            bool passed = true;
            if(JobNameInput.TextValue == "")
            {
                JobNameInput.flickerWithRed();
                passed = false;
            }
            if(!JobDatePicker.IsDateSelected)
            {
                JobDatePicker.flickerWithRed();
                passed = false;
            }
            if (TeamsDropdown.getSelectedString() == "")
            {
                TeamsDropdown.flickerWithRed();
                passed = false;
            }
            if (WeightDropdown.getSelectedString() == "")
            {
                WeightDropdown.flickerWithRed();
                passed = false;
            }
            if (AccountsDualList.RightListStackPanel.Children.Count == 0)
            {
                AccountsDualList.flickerWithRed();
                passed = false;
            }
            if(DescriptionBox.Text.Length <= 10)
            {
                Utility.flickerPropertyWithRed(DescriptionBox, BackgroundProperty, ((SolidColorBrush)DescriptionBox.Background).Color);
                passed = false;
            }

            return passed;
        }
        #endregion

    }
}
