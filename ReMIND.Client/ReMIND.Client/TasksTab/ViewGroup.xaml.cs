using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

using ReMIND.Client.Business.Models;
using ReMIND.Client.Business.Models.Types;

namespace ReMIND.Client.TasksTab
{
    /// <summary>
    /// Interaction logic for ViewGroup.xaml
    /// </summary>
    public partial class ViewGroup : UserControl
    {
        public event EventHandler<bool> RequestWindowClose;
        JobGroup JobGroup { get; set; }
        public ViewGroup()
        {
            InitializeComponent();
            DataContext = this;
        }
        public ViewGroup(JobGroup group)
            : this()
        {
            JobGroup = group;
        }

        public async void AttachData(JobGroup group)
        {
            JobGroup = group;
            List<Job> activeJobs = new();
            List<JobArchive> archivedJobs = new();
            FromToDate interval = new();
            interval.DateFrom = null;
            interval.DateFrom = null;

            activeJobs = await Utility.GetAllJobsByGroup(interval, JobGroup);//aktivni poslovi (aka nezavrseni)
            archivedJobs = await Utility.GetAllJobArhiveQuerry(interval, "q?groupName=" + JobGroup.Name, 0, 1000);

            StatusStackPanel.Children.Clear();
            foreach(var archivedJob in archivedJobs)
            {
                Rectangle r = new()
                {
                    Height = 10,
                    Width = 10,
                    Fill = new BrushConverter().ConvertFromString("#266986") as SolidColorBrush,
                    Margin = new Thickness(1)
                };
                StatusStackPanel.Children.Add(r);
            }
            foreach(var activeJob in activeJobs)
            {
                Rectangle r = new()
                {
                    Height = 10,
                    Width = 10,
                    Fill = new BrushConverter().ConvertFromString("#E5E5E5") as SolidColorBrush,
                    Margin = new Thickness(1)
                };
                StatusStackPanel.Children.Add(r);
            }
            StatusTextBlock.Text = " " + archivedJobs.Count + "/" + (activeJobs.Count + archivedJobs.Count).ToString();
            DescriptionBox.AppendText(group.Description);
        }

        private void CloseButton_ButtonClicked(object sender, RoutedEventArgs e)
        {
            RequestWindowClose?.Invoke(this, true);
        }
    }
}
