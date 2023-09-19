using System.IO;
using System.Windows;
using System.Windows.Input;

namespace ReMIND.Client.Login
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            //if (!File.Exists("./API_URL.txt"))
            //    File.Create("./API_URL.txt");

            //Utility.API_URI = File.ReadAllText("./API_URL.txt");
            Utility.API_URI = "https://localhost:44311";
            //Utility.API_URI = "https://localhost:5001";

            InitializeComponent();
            loginPage.parentWindow = this;
        }
        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
                WindowState = WindowState.Normal;
            else
                WindowState = WindowState.Maximized;
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
                loginPage.AttemptLogin();
        }
    }
}
