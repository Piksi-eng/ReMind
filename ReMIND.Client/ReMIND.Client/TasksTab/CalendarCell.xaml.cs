using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using ReMIND.Client.Business.Models;

namespace ReMIND.Client.TasksTab
{
    /// <summary>
    /// Interaction logic for CalendarCellControl.xaml
    /// </summary>
    public partial class CalendarCell : UserControl
    {

        #region Data & Properties
        public List<JobTagEllipse> Jobs;
        public string DateText { get; set; }
        public int DateNumber => Convert.ToInt32(DateText);


        public bool IsActive
        {
            get => CellButton.IsChecked == true;
            set => CellButton.IsChecked = value;
        }

        private bool _enabled;
        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                CellButton.IsEnabled = value;
                _enabled = value;
            }
        }
        public bool Today
        {
            get => CellButton.Foreground == Brushes.Red;
            set
            {
                if (value)
                    CellButton.Foreground = Brushes.Red;
                else
                    CellButton.Foreground = new BrushConverter().ConvertFromString("#266986") as SolidColorBrush;
            }
        }
        #endregion

        public CalendarCell()
        {
            InitializeComponent();
            DataContext = this;
            Jobs = new();
        }

        #region Data IO
        public void AddJob(Job job)
        {
            JobTagEllipse e = new(job);
            Jobs.Add(e);
            TagListContainer.Children.Add(e);
        }
        public void ClearJobs()
        {
            TagListContainer.Children.Clear();
        }
        public void AddTestTag(Color c)
        {
            Job j = new();
            j.JobTag.Color = c.ToString();
            JobTagEllipse jte = new(j);
            TagListContainer.Children.Add(jte);
        }
        #endregion

        #region Select
        public void SelectJobs(List<Job> jobs)
        {
            foreach(JobTagEllipse jte in TagListContainer.Children)
                jte.Visibility = jobs.Contains(jte.Job) ?
                    Visibility.Visible : Visibility.Collapsed;
        }
        public void SelectAll()
        {
            foreach (JobTagEllipse jte in TagListContainer.Children)
                jte.Visibility = Visibility.Visible;
        }
        #endregion

        #region Events

        public event EventHandler<bool> CellActiveChanged;
        private void CellButton_Checked(object sender, RoutedEventArgs e)
        {
            CellActiveChanged?.Invoke(this, true);
        }
        private void CellButton_Unchecked(object sender, RoutedEventArgs e)
        {
            CellActiveChanged?.Invoke(this, false);
        }

        #endregion

    }
}
