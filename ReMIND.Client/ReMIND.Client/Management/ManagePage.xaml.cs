using System.Windows.Controls;

namespace ReMIND.Client.Management
{
    /// <summary>
    /// Interaction logic for ManageTab.xaml
    /// </summary>
    public partial class ManagePage : UserControl
    {
        public ManagePage()
        {
            InitializeComponent();
        }

        private void ManagerControl_AccountEditClicked(object sender, Business.Models.Person e)
        {
            AddEditViewer.SelectAccount(e);
        }

        private void ManagerControl_TeamEditClicked(object sender, Business.Models.Team e)
        {
            AddEditViewer.SelectTeam(e);
        }

        private void ManagerControl_TagEditClicked(object sender, Business.Models.JobTag e)
        {
            AddEditViewer.SelectTag(e);
        }
    }
}
