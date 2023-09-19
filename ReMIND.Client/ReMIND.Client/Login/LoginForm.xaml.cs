using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using ReMIND.Client.Business.Models;
using ReMIND.Client.Business.Models.Types;
using ReMIND.Client.Properties;

namespace ReMIND.Client.Login
{
    /// <summary>
    /// Interaction logic for LoginForm.xaml
    /// </summary>
    public partial class LoginForm : UserControl
    {
        public event EventHandler<bool> OnLoginCompleted;
        public event EventHandler<string> OnLoginStarted;

        public LoginForm()
        {
            InitializeComponent();

            #region Event registration
            //email
            EmailBorder.PreviewMouseDown += Border_Focused;
            EmailBorder.GotKeyboardFocus += Border_Focused;
            EmailInput.LostFocus += EmailInput_LostFocus;
            EmailInput.LostKeyboardFocus += EmailInput_LostFocus;

            //password
            PasswordBorder.PreviewMouseDown += Border_Focused;
            PasswordBorder.GotKeyboardFocus += Border_Focused;
            PasswordInput.LostFocus += PasswordInput_LostFocus;
            PasswordInput.LostKeyboardFocus += PasswordInput_LostFocus;

            //login
            LoginButton.Click += Login_Click;

            //support
            SupportLink.Click += SupportLink_Click;
            #endregion

        }

        #region Input UI
        private void Border_Focused(object sender, EventArgs e)
        {
            Border b = (Border)sender;
            StackPanel sp = (StackPanel)b.Child;
            sp.Children[1].Visibility = Visibility.Collapsed;
            UIElement elem = sp.Children[2];
            elem.Focus();
        }
        private void EmailInput_LostFocus(object sender, RoutedEventArgs e)
        {
            if (EmailInput.Text != "")
                return;

            EmailLabel.Visibility = Visibility.Visible;
        }
        private void PasswordInput_LostFocus(object sender, RoutedEventArgs e)
        {
            if (PasswordInput.Password.ToString() != "")
                return;

            PasswordLabel.Visibility = Visibility.Visible;
        }
        #endregion

        #region Login
        private void Login_Click(object sender, EventArgs e)
        {
            if (EmailInput.Text == "")
            {
                Utility.flickerPropertyWithRed(EmailBorder, Border.BackgroundProperty, EmailBorder.Background);
                return;
            }
            if (PasswordInput.Password == "")
            {
                Utility.flickerPropertyWithRed(PasswordBorder, Border.BackgroundProperty, PasswordBorder.Background);
                return;
            }
                AttemptLogin();
        }
        public async void AttemptLogin()
        {
            OnLoginStarted?.Invoke(this, "Logging in...");
            bool result = false; //true at the end if login was successful

            Person user;
            string email = EmailInput.Text;
            string password = PasswordInput.Password;


            string auth = email + ":" + password;
            //dok nema kontrolera ovo ce biti iskomentarisano da moze da radi code
            try
            {
                user = await Utility.LoginUser(auth);
            }
            catch (Exception ex)
            {
                ErrorLabel.Text = ex.Message;
                user = null;
                result = false;
            }

            if (user != null)
            {
                try
                {
                    result = await Utility.SetMyTittle(user.ID);
                    await Utility.GetAllTags();
                    await Utility.GetAllMyTeams();
                    await Utility.GetAllMyJobs();
                    //await Utility.GetAllMyPeople();
                    if (Utility.UserTittle != TitleType.Employee)
                        await Utility.GetTeamsByLeader(user);
                }
                catch (Exception ex)
                {
                    ErrorLabel.Text = "Your Account is not yet setup.";
                    user = null;
                    result = false;
                }
            }

            if (result)
            {
                MainWindow app = new(user);
                app.Show();
            }
            else
            {
                //MessageBox.Show("Invalid credentials. Please try again.", "Login unsuccesful.", MessageBoxButton.OK, MessageBoxImage.Error);
                //ovaj pop up handlujemo u utiliy zato sto tamo moze i da nam stigne poruka do koje greske je doslo
            }

            OnLoginCompleted?.Invoke(this, result);
        }
        #endregion

        #region SupportLink
        private void SupportLink_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
                UseShellExecute = true
            });
        }
        #endregion

        #region Login Check
        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            OnLoginStarted?.Invoke(this, "Loading...");
            OnLoginCompleted?.Invoke(this, await IsAlreadyLoggedIn());
        }
        private async Task<bool> IsAlreadyLoggedIn()
        {
            if (Settings.Default.SessionID == "")
                return false; //logged out

            bool result = false; //true na kraju ako je user logged in
            //result = await Utility.CheckSession(Settings.Default.SessionID);

            return result;
        }
        #endregion

    }
}
