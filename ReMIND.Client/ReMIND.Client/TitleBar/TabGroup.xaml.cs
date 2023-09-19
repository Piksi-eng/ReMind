using System;
using System.Windows;
using System.Windows.Controls;

namespace ReMIND.Client.TitleBar
{
    /// <summary>
    /// Interaction logic for TabGroup.xaml
    /// </summary>
    public partial class TabGroup : UserControl
    {
        public event EventHandler<UIElement> OnTabChanged;

        private MainWindow _mainWindow;
        public MainWindow mainWindow
        {
            get => _mainWindow;
            set
            {
                _mainWindow = value;
            }
        }
        public TabGroup()
        {
            InitializeComponent();
            setupTasks();
        }

        #region Load/Setup
        private void setupTasks()
        {
            Tab tasks = new Tab()
            {
                DisplayText = "Tasks",
                DisplayImage = "Icons/Tasks.png",
                Page = new TasksTab.TasksPage() { mainWindow = Utility.mainWindow },
                Width = 106,
                Height = 32
            };
            tasks.OnTabActivated += Tab_Activated;
            TabsContainer.Children.Add(tasks);
        }
        public void setupLeader()
        {
            Tab analytics = new Tab()
            {
                DisplayText = "Analytics",
                DisplayImage = "Icons/Analytics.png",
                Page = new AnalyticsTab.AnalyticsPage(),
                Width = 106,
                Height = 32
            };
            TabsContainer.Children.Add(analytics);
            analytics.OnTabActivated += Tab_Activated;

            Tab archive = new Tab()
            {
                DisplayText = "Archive",
                DisplayImage = "Icons/Archive.png",
                Page = new ArchiveTab.ArchivePage(),
                Width = 106,
                Height = 32
            };
            TabsContainer.Children.Add(archive);
            archive.OnTabActivated += Tab_Activated;
        }
        public void setupAdmin()
        {
            setupLeader();
            Tab manage = new Tab()
            {
                DisplayText = "Manage",
                DisplayImage = "Icons/Manage.png",
                Page = new Management.ManagePage(),
                Width = 106,
                Height = 32
            };
            TabsContainer.Children.Add(manage);
            manage.OnTabActivated += Tab_Activated;
        }
        public void ActivateIndex(int index)
        {
            if (index < 0)
                return;
            if (index > TabsContainer.Children.Count)
                return;
            Tab tab = (Tab)TabsContainer.Children[index];

            tab.Active = true;
            ActivateTab(tab);
        }
        #endregion

        #region Tab Activation
        private void Tab_Activated(object sender, bool e)
        {
            ActivateTab((Tab)sender);
        }
        private void ActivateTab(Tab tab)
        {
            foreach (var child in TabsContainer.Children)
            {
                Tab t = (Tab)child;
                if (t != tab)
                    t.Active = false;
            }
            OnTabChanged?.Invoke(this, tab.Page);
        }
        #endregion
    }
}
