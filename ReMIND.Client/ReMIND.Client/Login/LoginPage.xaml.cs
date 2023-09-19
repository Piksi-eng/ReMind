using System.Windows;
using System.Windows.Controls;

namespace ReMIND.Client.Login
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : UserControl
    {
        public LoginWindow parentWindow;
        public LoginPage()
        {
            InitializeComponent();
        }

        private void LoginForm_OnLoginAction(object sender, bool e)
        {
            if (e)
                parentWindow.Close();
            parentWindow.loadingScreen.Visibility = Visibility.Hidden;
        }

        private void LoginForm_OnLoginStarted(object sender, string e)
        {
            parentWindow.loadingScreen.Message = e;
            parentWindow.loadingScreen.Visibility = Visibility.Visible;
        }

        public void AttemptLogin()
        {
            loginForm.AttemptLogin();
        }
    }
}
