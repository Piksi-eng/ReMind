using System.Windows.Controls;

namespace ReMIND.Client.Loading
{
    /// <summary>
    /// Interaction logic for LoadingScreen.xaml
    /// </summary>
    public partial class LoadingScreen : UserControl
    {
        public string Message { get; set; }
        public LoadingScreen()
        {
            InitializeComponent();
            Message = "Loading...";
            DataContext = this;
        }
    }
}
