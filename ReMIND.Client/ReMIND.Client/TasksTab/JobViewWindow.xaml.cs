using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

using ReMIND.Client.Business.Models;
using ReMIND.Client.Business;

namespace ReMIND.Client.TasksTab
{
    /// <summary>
    /// Interaction logic for JobViewWindow.xaml
    /// </summary>
    public partial class JobViewWindow : Window
    {
        public Job AttachedJob { get; set; }
        public bool ChangesMade { get; set; }

        #region Constructors
        public JobViewWindow()
        {
            InitializeComponent();
            DataContext = this;
        }
        public JobViewWindow(MainWindow parent, Job job = null)
            :this()
        {
            AttachedJob = job;
            Owner = parent;

            FinishedCB.IsChecked = AttachedJob.IsDone;

            MaxHeight = parent.Height;
            MaxWidth = parent.Width;
            WindowState = parent.WindowState;
        }
        #endregion

        #region Load
        private void JobViewWindow_Loaded(object sender, RoutedEventArgs e)
        {

            double scaleX = MaxWidth / 1280;
            double scaleY = MaxHeight / 720;

            DoubleAnimation animationX = new(0, scaleX, TimeSpan.FromMilliseconds(150));
            DoubleAnimation animationY = new(0, scaleY, TimeSpan.FromMilliseconds(150));
            WindowBorder.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, animationX);
            WindowBorder.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, animationY);
        }
        #endregion

        #region Buttons
        private async void SaveButton_ButtonClicked(object sender, RoutedEventArgs e)
        {
            string notFinished = AttachedJob.IsDone ? "not " : "";
            bool answer = ReMINDMessage.Show($"Are you sure you want to update {AttachedJob.Name} as {notFinished}finished?", true, "Confirm");

            if (answer)
            {
                AttachedJob.IsDone = FinishedCB.IsChecked;
                bool res = await Utility.UpdateJob(AttachedJob);

                //if (res)
                //ReMINDMessage.Show(AttachedJob.Name + "has been marked as finished", false, "Succsus");


                Utility.ReloadTasksData();
            }

            Close();
        }

        private void CloseButton_ButtonClicked(object sender, RoutedEventArgs e)
        {
            if (ChangesMade)
                if (!Business.ReMINDMessage.Show("There are some unsaved changes. Close without saving the job?", true, "Confirm"))
                    return;
            Close();
        }
        #endregion

        #region Checkbox
        private void FinishedCheck_CheckedChanged(object sender, bool e)
        {
            ChangesMade = e != AttachedJob.IsDone;
            SaveButton.Visibility = ChangesMade ? Visibility.Visible : Visibility.Collapsed;
        }
        #endregion

    }
}
