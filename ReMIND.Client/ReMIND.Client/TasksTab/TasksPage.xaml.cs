using System.Windows.Controls;

namespace ReMIND.Client.TasksTab
{
    /// <summary>
    /// Interaction logic for TasksPage.xaml
    /// </summary>
    public partial class TasksPage : UserControl
    {
        private MainWindow _mainWindow;
        public MainWindow mainWindow
        {
            get { return _mainWindow; }
            set
            {
                _mainWindow = value;
                ItemsList.mainWindow = value;
            }
        }
        public TasksPage()
        {
            InitializeComponent();
            Calendar.ListControl = ItemsList;
        }
    }
}
