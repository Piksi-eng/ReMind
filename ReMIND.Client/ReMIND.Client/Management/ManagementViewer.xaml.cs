using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

using ReMIND.Client.Business.Models;

namespace ReMIND.Client.Management
{
    /// <summary>
    /// Interaction logic for ManagementViewer.xaml
    /// </summary>
    public partial class ManagementViewer : UserControl
    {
        public ManagementViewer()
        {
            Utility.managerViewer = this;
            InitializeComponent();
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            ToggleButton tg = (ToggleButton)sender;
            foreach(ToggleButton child in ToggleButtonsPanel.Children)
                if (!tg.Equals(child))
                    child.IsChecked = false;

            if((string)tg.Content == "ACCOUNT")
                ContainerBorder.Child = new AddEditAccount();

            else if((string)tg.Content == "TEAM")
                ContainerBorder.Child = new AddEditTeam();

            else if((string)tg.Content == "TYPE")
                ContainerBorder.Child = new AddEditTag();

            else
                return; //inicijalizacija je cudna, pa bez ovoga puca program, ne pitaj me nista
        }
        public void Refresh()
        {
            foreach (ToggleButton child in ToggleButtonsPanel.Children)
                if (child.IsChecked == true)
                    switch (child.Content)
                    {
                        case "ACCOUNT":
                            ContainerBorder.Child = new AddEditAccount();
                            break;
                        case "TEAM:":
                            ContainerBorder.Child = new AddEditTeam();
                            break;
                        case "EVENT":
                            ContainerBorder.Child = new AddEditTag();
                            break;
                    }
        }

        public void SelectAccount(Person person)
        {
            ToggleButton tg = (ToggleButton)ToggleButtonsPanel.Children[0];
            tg.IsChecked = true;
            ContainerBorder.Child = new AddEditAccount(person);
        }
        public void SelectTeam(Team team)
        {
            ToggleButton tg = (ToggleButton)ToggleButtonsPanel.Children[1];
            tg.IsChecked = true;
            ContainerBorder.Child = new AddEditTeam(team);
        }
        public void SelectTag(JobTag tag)
        {
            ToggleButton tg = (ToggleButton)ToggleButtonsPanel.Children[2];
            tg.IsChecked = true;
            ContainerBorder.Child = new AddEditTag(tag);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ContainerBorder.Child = new AddEditAccount();
            ToggleButton tg = (ToggleButton)ToggleButtonsPanel.Children[0];
            tg.IsChecked = true;
        }
    }
}
