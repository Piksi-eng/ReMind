using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

using ReMIND.Client.Business.Models;

namespace ReMIND.Client.TasksTab
{
    /// <summary>
    /// Interaction logic for AddEditJob.xaml
    /// </summary>
    public partial class JobAndGroupViewerContainer : Window
    {
        public Job Job { get; set; }
        public JobAndGroupViewerContainer(Job job = null, JobGroup group = null)
        {
            Owner = Utility.mainWindow;
            InitializeComponent();
            DataContext = this;

            if (job != null)
            {
                JobWindow.Job = job;
                JobWindow.loadDefaults();
                JobWindow.populateFields();
                TaskButton.IsChecked = true;
                GroupButton.Visibility = Visibility.Collapsed;
            }
            else if (group != null)
            {
                GroupWindow.JobGroup = group;
                GroupWindow.loadDefaults();
                GroupWindow.populateFields();
                GroupButton.IsChecked = true;
                TaskButton.Visibility = Visibility.Collapsed;
            }
        }

        #region Load
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            setupWindow();
        }
        private void setupWindow()
        {
            ShowInTaskbar = false;
            Left = Utility.mainWindow.Left;
            Top = Utility.mainWindow.Top;
            Height = Utility.mainWindow.Height;
            Width = Utility.mainWindow.Width;
            WindowState = Utility.mainWindow.WindowState;

            double scaleX = Width / 1280;
            double scaleY = Height / 720;
            DoubleAnimation animationX = new(0, scaleX, TimeSpan.FromMilliseconds(150));
            DoubleAnimation animationY = new(0, scaleY, TimeSpan.FromMilliseconds(150));

            JobWindow.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, animationX);
            JobWindow.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, animationY);

            GroupWindow.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, animationX);
            GroupWindow.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, animationY);

            ToggleButtonStackPanel.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, animationX);
            ToggleButtonStackPanel.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, animationY);
        }
        #endregion

        #region Close
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
        }

        private void Window_RequestWindowClose(object sender, bool e)
        {
            Close();
        }
        #endregion

        #region Job/Group switching
        private void TaskButton_Checked(object sender, RoutedEventArgs e)
        {
            TaskButton.IsEnabled = false;
            GroupButton.IsChecked = false;
            GroupButton.IsEnabled = true;
            GroupWindow.Visibility = Visibility.Collapsed;
            JobWindow.IsEnabled = true;
        }

        private void GroupButton_Checked(object sender, RoutedEventArgs e)
        {
            GroupButton.IsEnabled = false;
            TaskButton.IsChecked = false;
            TaskButton.IsEnabled = true;
            GroupWindow.Visibility = Visibility.Visible;
            JobWindow.IsEnabled = false;
        }
        #endregion

    }
}
