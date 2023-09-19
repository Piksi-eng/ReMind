using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using ReMIND.Client.Business;
using ReMIND.Client.Business.Models;

namespace ReMIND.Client.TasksTab
{
    /// <summary>
    /// Interaction logic for JobItem.xaml
    /// </summary>
    public partial class JobItem : UserControl
    {
        public bool _TestMode { get; set; } = false;
        private Job _job;
        public Job AttachedJob
        {
            get { return _job; }
            set
            {
                _job = value;
                RefreshStyle();
            }
        }
        public string SearchString { get; set; }

        public string UpperText { get; set; }
        public string LowerText { get; set; }
        public string JobTag { get; set; }
        public JobItem()
        {
            InitializeComponent();
            DataContext = this;
            AttachedJob = new Job();
        }
        public JobItem(Job job)
        {
            InitializeComponent();
            AttachedJob = job;

            SearchString = _job.Name.ToLower() + _job.Description.ToLower() + _job.Person.Name.ToLower() + _job.Contact.ToLower() + _job.Deadline.ToString("dd.MM.yyyy") + _job.JobTag.Name.ToLower();
            if (AttachedJob.JobGroup != null)
                SearchString = SearchString + AttachedJob.JobGroup.Name;
            
            DataContext = this;

            RefreshStyle();
        }
        public void newData(Job job)
        {
            AttachedJob = job;
            RefreshStyle();
        }

        public void RefreshStyle()
        {
            UpperTextLabel.Text = $"{AttachedJob.Deadline:dd.MM.yyyy.} - {AttachedJob.JobGroup.Name}";
            if (!AttachedJob.IsDone && AttachedJob.Deadline.Date == DateTime.Now.Date)
                UpperTextLabel.Foreground = Brushes.Red;

            LowerTextLabel.Text = $"{AttachedJob.JobTag.Name}: {AttachedJob.Name}";

            if(AttachedJob.JobTag.Color != "" && AttachedJob.JobTag.Color != null)
                TagBorder.Background = new BrushConverter().ConvertFromString($"{AttachedJob.JobTag.Color}") as SolidColorBrush;

            if (AttachedJob.IsDone)
            {
                var brush = new LinearGradientBrush();
                brush.StartPoint = new Point(0, 0);
                brush.EndPoint = new Point(1, 0);
                brush.GradientStops.Add(new((Color)ColorConverter.ConvertFromString(AttachedJob.JobTag.Color), 0));
                brush.GradientStops.Add(new(Colors.Transparent, 3.3));

                IsDoneBorder.Background = brush;
                Foreground = Brushes.White;

                checkButton.Source = new BitmapImage(new Uri("../Resources/check-white.png", UriKind.Relative));
                archiveButton.Source = new BitmapImage(new Uri("../Resources/archive-white.png", UriKind.Relative));
                editButton.Source = new BitmapImage(new Uri("../Resources/edit-white.png", UriKind.Relative));
                deleteButton.Source = new BitmapImage(new Uri("../Resources/delete-white.png", UriKind.Relative));
            }

            if (Utility.employeeRestriction)
            {
                archiveButton.Visibility = Visibility.Hidden;
                editButton.Visibility = Visibility.Collapsed;
                deleteButton.Visibility = Visibility.Collapsed;
            }
            if (Utility.leaderRestriction)
            {
                bool hide = true;
                foreach (Team t in Utility.TeamsILead)
                {
                    if (AttachedJob.teamID == t.ID)
                        hide = false;
                }
                if (hide)
                {
                    archiveButton.Visibility = Visibility.Hidden;
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

            Utility.OpenViewJob(AttachedJob);
        }

        #region Buttons
        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (_TestMode)
                return;

            bool answer = ReMINDMessage.Show("Are you sure you want to remove the Job" + AttachedJob.Name, true, "Confirm");

            if (answer)
            {
                bool res = await Utility.DeleteAllJob(AttachedJob);

                if (res)
                    ReMINDMessage.Show(AttachedJob.Name + "has been removed", false, "Succsus");


                Utility.ReloadTasksData();
            }
            
        }
        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (_TestMode)
                return;
            Utility.OpenEditJob(AttachedJob);
        }
        private async void Archive_Click(object sender, RoutedEventArgs e)
        {
            if (_TestMode)
                return;

            Utility.StartLoading();
            await Utility.Archive(AttachedJob);
            Utility.ReloadTasksData();
            Utility.StopLoading();
        }
        private async void Finish_Click(object sender, RoutedEventArgs e)
        {
            if (_TestMode)
                return;

            bool answer = ReMINDMessage.Show("Are you sure you want " + AttachedJob.Name + "to mark as fiished?", true, "Confirm");

            if (answer)
            {
                if(Utility.UserTittle != Business.Models.Types.TitleType.Employee)
                {
                    AttachedJob.IsDone = true;
                    List<Job> sameJobs = await Utility.GetAllSameJobs(AttachedJob);
                    foreach (Job j in sameJobs)
                    {
                        j.IsDone = true;
                        bool ress = await Utility.UpdateJob(j);
                    }
                }
                else
                {
                    AttachedJob.IsDone = true;
                    bool res = await Utility.UpdateAllJob(AttachedJob);
                }




                //if (res)
                //ReMINDMessage.Show(AttachedJob.Name + "has been marked as finished", false, "Succsus");


                Utility.ReloadTasksData();
            }
        }
        #endregion

    }
}