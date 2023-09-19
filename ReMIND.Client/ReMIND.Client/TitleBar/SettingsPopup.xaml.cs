using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

using ReMIND.Client.Business;
using ReMIND.Client.Business.Models;

namespace ReMIND.Client.TitleBar
{
    /// <summary>
    /// Interaction logic for SettingsPopup.xaml
    /// </summary>
    public partial class SettingsPopup : UserControl
    {
        public event EventHandler<bool> OnPopupOutsideClicked;

        #region Data
        public MainWindow mainWindow { get; set; }

        private List<Job> UnreadJobs;
        private List<JobGroup> UnreadGroups;
        #endregion

        public SettingsPopup()
        {
            InitializeComponent();
            ChangePassButton.Click += ChangePass_Click;
            ConfirmPassButton.Click += ConfirmPass_Click;
        }

        public async void Refresh()
        {
            #region Account details

            NameBlock.Text = Utility.User.Name;
            EmailBlock.Text = Utility.User.Email;
            PhoneBlock.Text = Utility.User.Phone;

            #endregion

            #region Load Notifications

            UnreadGroups = await Utility.GetAllUnreadJobGroupsNotifications();
            UnreadJobs = await Utility.GetAllUnreadJobsNotifications();

            foreach (JobGroup group in UnreadGroups)
                NotificationsList.Children.Add(new NotificationItem(group));

            foreach (Job job in UnreadJobs)
                NotificationsList.Children.Add(new NotificationItem(job));

            #endregion

            if (NotificationsList.Children.Count > 0)
                Utility.mainWindow.showNotificationIndicator();
            else
                Utility.mainWindow.hideNotificationIndicator();
        }

        #region Password logic
        private void ConfirmPass_Click(object sender, RoutedEventArgs e)
        {
            if(NewPassInput.PasswordValue == "")
            {
                NewPassInput.flickerWithRed();
                return;
            }
            if(ConfirmPassInput.PasswordValue == "")
            {
                ConfirmPassInput.flickerWithRed();
                return;
            }

            if(NewPassInput.PasswordValue != ConfirmPassInput.PasswordValue)
            {
                NewPassInput.flickerWithRed();
                ConfirmPassInput.flickerWithRed();
                ReMINDMessage.Show("The two passowrd don't match! Please retry.");
                return;
            }
            ReMINDPrompt.Show(NewPassInput.PasswordValue); //handles everything pass-change wise
        }

        private void ChangePass_Click(object sender, RoutedEventArgs e)
        {
            if (ConfirmPassPanel.Height == 0) //it's closed. open.
            {
                if(NewPassInput.PasswordValue == "")
                {
                    NewPassInput.flickerWithRed();
                    return;
                }
                var animation = new DoubleAnimation(0, 20, TimeSpan.FromMilliseconds(150));
                ConfirmPassPanel.BeginAnimation(HeightProperty, animation);
                ChangePassButton.Content = "CANCEL";
            }
            else //it's open. close it.
            {
                var animation = new DoubleAnimation(20, 0, TimeSpan.FromMilliseconds(150));
                ConfirmPassPanel.BeginAnimation(HeightProperty, animation);
                NewPassInput.PasswordValue = "";
                ConfirmPassInput.PasswordValue = "";
                ChangePassButton.Content = "CHANGE";
            }
        }
        #endregion

        #region Closing
        private void Border_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.Source.GetType() != typeof(Border))
                return;

            Border b = (Border)e.Source;
            if (b.Background == Brushes.Transparent)
                OnPopupOutsideClicked?.Invoke(this, true);
        }
        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                OnPopupOutsideClicked?.Invoke(this, true);
            }
        }
        #endregion

        #region Scroll
        private void Scroller_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            double marginTop = Scroller.VerticalOffset * Scroller.ViewportHeight / Scroller.ExtentHeight;
            if (double.IsNaN(marginTop)) //ako delimo nulom onda je bljak pa moramo da mu stavimo value na 0
                marginTop = 0;
            ScrollBorder.Margin = new Thickness(2, marginTop, 2, 5);

            double scrollbarHeight = Scroller.ViewportHeight * Scroller.ViewportHeight / Scroller.ExtentHeight;
            if (double.IsInfinity(scrollbarHeight)) //similar thing here
                scrollbarHeight = Scroller.ViewportHeight;

            ScrollBorder.Height = Scroller.ExtentHeight >= Scroller.ViewportHeight ? //ne pitaj zasto
                scrollbarHeight : Scroller.ExtentHeight;
        }
        #endregion

        #region Buttons Logout, Clear all
        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            bool result = false; //true ako je logout uspeo

            //piksi do your thing, logging out
            //result = Utility.LogOut() ??? idk
            //client nema potrebe da komunicira sa serverskom stranom za logout 
            //jedino ako hoce da server moze da keepuje track kolko je korisnik ulogovan ali nema potrebe za time
            
            result = true; //testing purposes

            if (!result)
                return;
            
            Properties.Settings.Default.SessionID = "";
            Utility.RestartApp();
        }
        private async void ClearAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (Job job in UnreadJobs)
            {
                job.IsRead = true;
                await Utility.UpdateJob(job);
            }


            foreach (JobGroup group in UnreadGroups)
            {
                group.IsRead = true;
                await Utility.UpdateJobGroup(group);
            }

            NotificationsList.Children.Clear();
            Utility.mainWindow.hideNotificationIndicator();
        }
        #endregion

    }
}