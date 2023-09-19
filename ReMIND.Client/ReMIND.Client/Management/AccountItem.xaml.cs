using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

using ReMIND.Client.Business.Models;

namespace ReMIND.Client.Management
{
    /// <summary>
    /// Interaction logic for AccountItem.xaml
    /// </summary>
    public partial class AccountItem : UserControl
    {
        public event EventHandler<Person> EditClicked;
        public Person Account { get; set; }
        public AccountItem(Person account)
        {
            InitializeComponent();
            DataContext = this;
            Account = account;


            if (!Account.IsEmployed)
                EmployeeImage.Source = new BitmapImage(new Uri("Icons/Unemployed.png", UriKind.Relative));
        }

        public string SearchString => $"{Account.Name.ToLower()}{Account.Email.ToLower()}{Account.Phone.ToLower()}";

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
            EditClicked?.Invoke(this, Account);
        }
    }
}
