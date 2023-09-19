using System.Windows;
using System.Windows.Controls;

using ReMIND.Client.Business.Models;

namespace ReMIND.Client.TasksTab
{
    /// <summary>
    /// Interaction logic for JobTagEllipse.xaml
    /// </summary>
    public partial class JobTagEllipse : UserControl
    {
        public Job Job { get; set; }

        private bool _visible;
        public bool Visible
        {
            get => TagEllipse.Visibility == Visibility.Visible;
            set
            {
                TagEllipse.Visibility = value ?
                    Visibility.Visible : Visibility.Collapsed;
            }
        }

        public JobTagEllipse(Job _job)
        {
            InitializeComponent();
            DataContext = this;
            Job = _job;
        }
    }
}
