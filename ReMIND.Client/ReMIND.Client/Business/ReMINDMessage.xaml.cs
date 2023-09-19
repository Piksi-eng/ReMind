using System.Windows;

namespace ReMIND.Client.Business
{
    /// <summary>
    /// Interaction logic for ReMINDMessage.xaml
    /// </summary>
    public partial class ReMINDMessage : Window
    {
        bool result;
        public ReMINDMessage(string message, bool YesNo, string caption)
        {
            Owner = Utility.mainWindow;
            InitializeComponent();

            Message.Text = message;

            Caption.Text = caption == "" ?
                "ReMIND Message" : caption;

            if (YesNo)
            {
                OKButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                YESButton.Visibility = Visibility.Collapsed;
                NOButton.Visibility = Visibility.Collapsed;
            }
        }

        public static bool Show(string message, bool YesNo = false, string caption = "")
        {
            Utility.mainWindow.Blur();
            ReMINDMessage msg = new(message, YesNo, caption);
            msg.ShowDialog();
            Utility.mainWindow.Unblur();
            return msg.result;
        }

        private void OKButton_ButtonClicked(object sender, RoutedEventArgs e)
        {
            result = true;
            Close();
        }

        private void NOButton_ButtonClicked(object sender, RoutedEventArgs e)
        {
            result = false;
            Close();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            result = false;
            Close();
        }
    }
}
