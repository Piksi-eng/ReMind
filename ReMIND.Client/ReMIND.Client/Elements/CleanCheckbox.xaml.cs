using System;
using System.Windows;
using System.Windows.Controls;

namespace ReMIND.Client.Elements
{
    /// <summary>
    /// Interaction logic for CleanCheckbox.xaml
    /// </summary>
    public partial class CleanCheckbox : UserControl
    {
        public event EventHandler<bool> CheckedChanged;
        public CleanCheckbox()
        {
            InitializeComponent();
            DataContext = this;
        }

        public bool IsChecked
        {
            get => MainCB.IsChecked == true;
            set => MainCB.IsChecked = value;
        }

        private void MainCB_Checked(object sender, RoutedEventArgs e)
        {
            CheckedChanged?.Invoke(this, true);
        }

        private void MainCB_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckedChanged?.Invoke(this, false);
        }
    }
}
