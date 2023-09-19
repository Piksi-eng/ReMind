using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using ReMIND.Client.Business;
using ReMIND.Client.Business.Models;
using ReMIND.Client.Business.Models.Types;

namespace ReMIND.Client.TasksTab
{
    /// <summary>
    /// Interaction logic for GroupViewWindow.xaml
    /// </summary>
    public partial class GroupViewWindow : Window
    {
        public JobGroup AttachedGroup { get; set; }
        public bool ChangesMade { get; set; }

        List<Job> activeJobs;
        List<JobArchive> archivedJobs;

        #region Constructors
        public GroupViewWindow()
        {
            InitializeComponent();
            DataContext = this;
        }
        public GroupViewWindow(MainWindow parent, JobGroup group = null)
            : this()
        {
            AttachedGroup = group;
            Owner = parent;

            MaxHeight = parent.Height;
            MaxWidth = parent.Width;
            WindowState = parent.WindowState;
        }
        #endregion

        #region Load
        private async void JobViewWindow_Loaded(object sender, RoutedEventArgs e)
        {
            #region Status
            FromToDate interval = new();
            interval.DateFrom = null;
            interval.DateFrom = null;
            activeJobs = await Utility.GetAllJobsByGroup(interval, AttachedGroup);//aktivni poslovi (aka nezavrseni)
            archivedJobs = await Utility.GetAllJobArhiveQuerry(interval, "q?groupName=" + AttachedGroup.Name, 0, 1000);

            StatusStackPanel.Children.Clear();
            for (int i = 0; i < archivedJobs.Count; i++)//brze je nego foreach, ne koristimo objekte
            {
                Rectangle r = new()
                {
                    Height = 10,
                    Width = 10,
                    Fill = new BrushConverter().ConvertFromString("#005478") as SolidColorBrush,
                    Margin = new Thickness(1)
                };
                StatusStackPanel.Children.Add(r);
            }
            for (int i = 0; i < activeJobs.Count; i++)
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
            StatusTextBlock.Text = $"STATUS: {archivedJobs.Count}/{activeJobs.Count + archivedJobs.Count}";
            #endregion

            //piksi kada se implementira removal ovde samo stavi na true ili false
            bool isRemovalButtonVisible = false;

            ReqRemovalButton.Visibility = isRemovalButtonVisible ? Visibility.Visible : Visibility.Collapsed;

            double scaleX = MaxWidth / 1280;
            double scaleY = MaxHeight / 720;

            DoubleAnimation animationX = new(0, scaleX, TimeSpan.FromMilliseconds(150));
            DoubleAnimation animationY = new(0, scaleY, TimeSpan.FromMilliseconds(150));
            WindowBorder.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, animationX);
            WindowBorder.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, animationY);
        }
        #endregion

        #region Buttons
        private void CloseButton_ButtonClicked(object sender, RoutedEventArgs e)
        {
            if (ChangesMade)
                if (!ReMINDMessage.Show("There are some unsaved changes. Close without saving the job?", true, "Confirm"))
                    return;
            Close();
        }
        private void Removal_ButtonClicked(object sender, RoutedEventArgs e)
        {
            //piksi ovde implementiraj request removal za AttachedGroup
        }
        #endregion

    }
}
