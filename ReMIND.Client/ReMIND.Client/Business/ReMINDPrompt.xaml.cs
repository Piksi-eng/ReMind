using System.Windows;

namespace ReMIND.Client.Business
{
    /// <summary>
    /// Interaction logic for ReMINDPrompt.xaml
    /// </summary>
    public partial class ReMINDPrompt : Window
    {
        string newPassword;
        public ReMINDPrompt(string newPassword)
        {
            Owner = Utility.mainWindow;
            InitializeComponent();
            this.newPassword = newPassword;
        }

        private async void ConfirmButton_ButtonClicked(object sender, RoutedEventArgs e)
        {
            if (!CheckField())
                return;

            Utility.StartLoading();
            bool result = await Utility.UpdatePasword(OldPasswordInput.PasswordValue, newPassword);
            Utility.StopLoading();
            if (result)
            {
                ReMINDMessage.Show("Password change succesful.");
                Close();
            }
            else
            {
                WrongPasswordLabel.Visibility = Visibility.Visible;
                OldPasswordInput.PasswordValue = "";
            }
        }
        public static void Show(string newPassword)
        {
            ReMINDPrompt prompt = new(newPassword);
            prompt.ShowDialog();
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private bool CheckField()
        {
            if(OldPasswordInput.PasswordValue == "")
            {
                OldPasswordInput.flickerWithRed();
                return false;
            }
            return true;
        }
    }
}
