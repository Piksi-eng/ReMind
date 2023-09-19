using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using ReMIND.Client.Business.Models;

namespace ReMIND.Client.Management
{
    /// <summary>
    /// Interaction logic for TagItem.xaml
    /// </summary>
    public partial class TagItem : UserControl
    {
        public event EventHandler<JobTag> EditClicked;
        public JobTag JobTag { get; set; }
        public TagItem(JobTag tag)
        {
            InitializeComponent();
            JobTag = tag;
            DataContext = JobTag;
        }

        public string SearchString => $"{JobTag.Name.ToLower()}{JobTag.Color.ToLower()}";

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
            EditClicked?.Invoke(this, JobTag);
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            int jobsNum = 0;
            jobsNum = await Utility.GetNumberOfJobs(JobTag.ID);


            JobsTextblock.Text = $"{jobsNum} related job";
            if (jobsNum != 1) JobsTextblock.Text += "s";
        }
    }
}
