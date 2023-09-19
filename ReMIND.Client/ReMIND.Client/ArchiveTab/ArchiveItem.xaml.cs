using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

using ReMIND.Client.Business.Models;

namespace ReMIND.Client.ArchiveTab
{
    /// <summary>
    /// Interaction logic for ArchiveItem.xaml
    /// </summary>
    public partial class ArchiveItem : UserControl
    {
        public ArchiveViewer ArchiveParent { get; set; }
        public JobArchive ArchivedJob { get; set; }
        public int Index;

        public ArchiveItem()
        {
            InitializeComponent();
            DataContext = this;
            ArchivedJob = new();
            Index = 0;
        }
        public ArchiveItem(int index, JobArchive job)
            : this()
        {
            ArchivedJob = job;
            Index = index;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (Index % 2 == 0)
                Background = new BrushConverter().ConvertFromString("#80ECECEC") as SolidColorBrush;

            IndexTextBlock.Text = Index.ToString();

            displayJobData();
        }

        public void AttachData(JobArchive newArchivedJob)
        {
            ArchivedJob = newArchivedJob;
            displayJobData();
        }
        private void displayJobData()
        {
            NameTextBlock.Text = ArchivedJob.Name;
            TeamTextblock.Text = ArchivedJob.TeamName;
            ContactTextBlock.Text = ArchivedJob.Contact;
            GroupTextBlock.Text = ArchivedJob.JobGroupName;
            TagTextBlock.Text = ArchivedJob.JobTagName;
            EmployeeTextBlock.Text = ArchivedJob.Person.Name;
            DeadlineTextBlock.Text = ArchivedJob.Deadline.ToString("dd.MM.yyyy");
            WeightNumberLabel.Text = ArchivedJob.JobWeight.ToString();
            WeightsStackPanel.Children.Clear();
            for (int i = 0; i < ArchivedJob.JobWeight; i++)
            {
                Rectangle r = new()
                {
                    Width = 10,
                    Height = 10,
                    Fill = new BrushConverter().ConvertFromString("#266986") as SolidColorBrush,
                    Margin = new Thickness(1)
                };
                WeightsStackPanel.Children.Add(r);
            }

            if (ArchivedJob.Finished == null)
            {
                FinishedDateTextblock.Text = "---";
                FinishedDateTextblock.FontWeight = FontWeights.ExtraBold;
                FinishedDateTextblock.FontSize = 16;
            }
            else
            {
                FinishedDateTextblock.Text = ((DateTime)ArchivedJob.Finished).ToString("dd.MM.yyyy");
                FinishedDateTextblock.FontSize = 10;
                FinishedDateTextblock.FontWeight = FontWeights.Medium;

            }
        }


        public static bool operator < (ArchiveItem ai1, ArchiveItem ai2)
        {
            if (!ai1.ArchiveParent.Equals(ai2.ArchiveParent))
                return false;
            switch (ai1.ArchiveParent.CurrentSortStyle)
            {
                case SortEnum.NameAsc:
                    return Utility.areStringsSorted(ai1.ArchivedJob.Name, ai2.ArchivedJob.Name);
                case SortEnum.NameDesc:
                    return !Utility.areStringsSorted(ai1.ArchivedJob.Name, ai2.ArchivedJob.Name);
                case SortEnum.TeamAsc:
                    return Utility.areStringsSorted(ai1.ArchivedJob.TeamName, ai2.ArchivedJob.TeamName);
                case SortEnum.TeamDesc:
                    return !Utility.areStringsSorted(ai1.ArchivedJob.TeamName, ai2.ArchivedJob.TeamName);
                case SortEnum.ContactAsc:
                    return Utility.areStringsSorted(ai1.ArchivedJob.Contact, ai2.ArchivedJob.Contact);
                case SortEnum.ContactDesc:
                    return !Utility.areStringsSorted(ai1.ArchivedJob.Contact, ai2.ArchivedJob.Contact);
                case SortEnum.GroupAsc:
                    return Utility.areStringsSorted(ai1.ArchivedJob.JobGroupName, ai2.ArchivedJob.JobGroupName);
                case SortEnum.GroupDesc:
                    return !Utility.areStringsSorted(ai1.ArchivedJob.JobGroupName, ai2.ArchivedJob.JobGroupName);
                case SortEnum.TagAsc:
                    return Utility.areStringsSorted(ai1.ArchivedJob.JobTagName, ai2.ArchivedJob.JobTagName);
                case SortEnum.TagDesc:
                    return !Utility.areStringsSorted(ai1.ArchivedJob.JobTagName, ai2.ArchivedJob.JobTagName);
                case SortEnum.EmployeeAsc:
                    return Utility.areStringsSorted(ai1.ArchivedJob.Person.Name, ai2.ArchivedJob.Person.Name);
                case SortEnum.EmployeeDesc:
                    return !Utility.areStringsSorted(ai1.ArchivedJob.Person.Name, ai2.ArchivedJob.Person.Name);
                case SortEnum.WeightAsc:
                    return ai1.ArchivedJob.JobWeight < ai2.ArchivedJob.JobWeight;
                case SortEnum.WeightDesc:
                    return ai1.ArchivedJob.JobWeight > ai2.ArchivedJob.JobWeight;
                case SortEnum.DateAsc:
                    return ai1.ArchivedJob.Deadline < ai2.ArchivedJob.Deadline;
                case SortEnum.DateDesc:
                    return ai1.ArchivedJob.Deadline > ai2.ArchivedJob.Deadline;
                case SortEnum.FinishedAsc:
                    return !(ai1.ArchivedJob.Finished == null && ai2.ArchivedJob.Finished != null);
                case SortEnum.FinishedDesc:
                    return ai1.ArchivedJob.Finished == null && ai2.ArchivedJob.Finished != null;
                case SortEnum.None:
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Don't use this. mrzelo me da pravim.
        /// </summary>
        /// <param name="ai1"></param>
        /// <param name="ai2"></param>
        /// <returns></returns>
        public static bool operator > (ArchiveItem ai1, ArchiveItem ai2)
        {
            if (!ai1.ArchiveParent.Equals(ai2.ArchiveParent))
                return false;
            return false;
        }
    }
}
