using System.Windows;
using System.Windows.Controls;

using ReMIND.Client.Business.Models;

namespace ReMIND.Client.TitleBar
{
    /// <summary>
    /// Interaction logic for NotificationItem.xaml
    /// </summary>
    public partial class NotificationItem : UserControl
    {
        public Job AttachedJob { get; set; }
        public JobGroup AttachedGroup { get; set; }

        public NotificationItem()
        {
            InitializeComponent();
        }

        public NotificationItem(JobGroup group)
            :this()
        {
            AttachedGroup = group;
        }
        public NotificationItem(Job job)
            :this()
        {
            AttachedJob = job;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if(AttachedJob != null)
            {
                DisplayLabel.Text = AttachedJob.JobTag.Name + " - " + AttachedJob.Name;
            }
            else if(AttachedGroup != null)
            {
                DisplayLabel.Text = AttachedGroup.Name;
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (AttachedJob != null)
            {
                Utility.OpenViewJob(AttachedJob);
            }
            else if (AttachedGroup != null)
            {
                Utility.OpenViewGroup(AttachedGroup);
            }
            StackPanel parent = (StackPanel)Parent;

            //piksi ovde uklanjam notification iz liste
            //ukoliko je lista prazna, iskljucujem notification indicator na ventilatoru
            //treba da se update AttachedJob tj AttachedGroup (u zavisnosti koji nije null, imas gore ifove)
            //sa onim isRead=true ili kako vec... ako treba jos nesto eto i to
            //poy od dzonija <3

            if(AttachedJob != null)
            {
                AttachedJob.IsRead = true;
                await Utility.UpdateJob(AttachedJob);
            }


            if(AttachedGroup != null)
            {
                AttachedGroup.IsRead = true;
                await Utility.UpdateJobGroup(AttachedGroup);
            }


            parent.Children.Remove(this);
            if (parent.Children.Count == 0)
                Utility.mainWindow.hideNotificationIndicator();
        }
    }
}
