using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

using ReMIND.Client.Business.Models;

namespace ReMIND.Client.Management
{
    /// <summary>
    /// Interaction logic for Manager.xaml
    /// </summary>
    public partial class Manager : UserControl
    {
        public Manager()
        {
            Utility.managerControl = this;
            InitializeComponent();
            ReloadData();
        }

        public void ReloadData()
        {
            LoadAccounts();
            LoadTeams();
            LoadTags();
        }

        #region Accounts
        public async void LoadAccounts()
        {
            List<Person> accounts = new();

            accounts = await Utility.GetAllPeople();

            AccountsList.Children.Clear();
            foreach (Person account in accounts)
            {
                AccountItem item = new(account);
                item.Margin = new Thickness(0, 0, 0, 3);
                item.EditClicked += AccountEdit_Clicked;
                AccountsList.Children.Add(item);
            }
        }

        public event EventHandler<Person> AccountEditClicked;
        private void AccountEdit_Clicked(object sender, Person e)
        {
            AccountEditClicked?.Invoke(this, e);
        }

        private void Accounts_SearchChanged(object sender, string e)
        {
            foreach (AccountItem item in AccountsList.Children)
                item.Visibility = item.SearchString.Contains(e.ToLower()) ?
                    Visibility.Visible : Visibility.Collapsed;
        }
        private void AccountScroller_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            double marginTop = AccountScroller.VerticalOffset * AccountScroller.ViewportHeight / AccountScroller.ExtentHeight;
            if (double.IsNaN(marginTop)) //ako delimo nulom onda je bljak pa moramo da mu stavimo value na 0
                marginTop = 0;
            AccountScrollBorder.Margin = new Thickness(0, marginTop, 0, 0);

            double scrollbarHeight = AccountScroller.ViewportHeight / AccountScroller.ExtentHeight * AccountScroller.ViewportHeight;
            if (double.IsInfinity(scrollbarHeight)) //similar thing here
                scrollbarHeight = AccountScroller.ViewportHeight;
            AccountScrollBorder.Height = scrollbarHeight;
        }
        #endregion

        #region Teams
        public async void LoadTeams()
        {
            List<Team> teams = new();

            teams = await Utility.GetAllTeams();

            TeamsList.Children.Clear();
            foreach (Team team in teams)
            {
                TeamItem item = new(team);
                item.Margin = new Thickness(0, 0, 0, 3);
                item.EditClicked += TeamEdit_Clicked;
                TeamsList.Children.Add(item);
            }
            //todo foreach kreiranje kontrola
        }

        public event EventHandler<Team> TeamEditClicked;
        private void TeamEdit_Clicked(object sender, Team e)
        {
            TeamEditClicked?.Invoke(this, e);
        }

        private void Teams_SearchChanged(object sender, string e)
        {
            foreach (TeamItem item in TeamsList.Children)
                item.Visibility = item.SearchString.Contains(e.ToLower()) ?
                    Visibility.Visible : Visibility.Collapsed;
        }
        private void TeamScroller_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            double marginTop = TeamScroller.VerticalOffset * TeamScroller.ViewportHeight / TeamScroller.ExtentHeight;
            if (double.IsNaN(marginTop)) //ako delimo nulom onda je bljak pa moramo da mu stavimo value na 0
                marginTop = 0;
            TeamScrollBorder.Margin = new Thickness(0, marginTop, 0, 0);

            double scrollbarHeight = TeamScroller.ViewportHeight / TeamScroller.ExtentHeight * TeamScroller.ViewportHeight;
            if (double.IsInfinity(scrollbarHeight)) //similar thing here
                scrollbarHeight = TeamScroller.ViewportHeight;
            TeamScrollBorder.Height = scrollbarHeight;
        }
        #endregion

        #region Tags
        public async void LoadTags()
        {
            List<JobTag> tags = new();

            tags = await Utility.GetAllTags();

            TagsList.Children.Clear();
            foreach(JobTag tag in tags)
            {
                TagItem item = new(tag);
                item.Margin = new Thickness(0, 0, 0, 3);
                item.EditClicked += TagEdit_Clicked;
                TagsList.Children.Add(item);
            }
        }
        public event EventHandler<JobTag> TagEditClicked;
        private void TagEdit_Clicked(object sender, JobTag e)
        {
            TagEditClicked?.Invoke(this, e);
        }
        private void Tags_SearchChanged(object sender, string e)
        {
            foreach (TagItem item in TagsList.Children)
                item.Visibility = item.SearchString.Contains(e.ToLower()) ?
                    Visibility.Visible : Visibility.Collapsed;
        }
        private void TagScroller_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            double marginTop = TagScroller.VerticalOffset * TagScroller.ViewportHeight / TagScroller.ExtentHeight;
            if (double.IsNaN(marginTop)) //ako delimo nulom onda je bljak pa moramo da mu stavimo value na 0
                marginTop = 0;
            TagScrollBorder.Margin = new Thickness(0, marginTop, 0, 0);

            double scrollbarHeight = TagScroller.ViewportHeight / TagScroller.ExtentHeight * TagScroller.ViewportHeight;
            if (double.IsInfinity(scrollbarHeight)) //similar thing here
                scrollbarHeight = TagScroller.ViewportHeight;
            TagScrollBorder.Height = scrollbarHeight;
        }
        #endregion
    }
}