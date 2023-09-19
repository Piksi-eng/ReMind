using System;
using System.Windows;
using System.Windows.Controls;

namespace ReMIND.Client.TitleBar
{
    /// <summary>
    /// Interaction logic for Tab.xaml
    /// </summary>
    public partial class Tab : UserControl
    {
        public UIElement Page { get; set; }
        public event EventHandler<bool> OnTabActivated;
        public Tab()
        {
            InitializeComponent();
            DataContext = this;
        }

        #region Property registering
        public string DisplayText
        {
            get { return (string)GetValue(DisplayTextProperty); }
            set { SetValue(DisplayTextProperty, value); }
        }
        public string DisplayImage
        {
            get { return (string)GetValue(DisplayImageProperty); }
            set { SetValue(DisplayImageProperty, value); }
        }
        public bool Active
        {
            get { return (bool)GetValue(ActiveProperty); }
            set
            {
                SetValue(ActiveProperty, value);
                TabButton.IsChecked = value;

                if (value)
                    OnTabActivated?.Invoke(this, value);
            }
        }

        public static readonly DependencyProperty DisplayTextProperty
            = DependencyProperty.Register(
                "DisplayText",
                typeof(string),
                typeof(Tab),
                new PropertyMetadata("Tab")
                );
        public static readonly DependencyProperty DisplayImageProperty
            = DependencyProperty.Register(
                "DisplayImage",
                typeof(string),
                typeof(Tab),
                new PropertyMetadata("")
                );
        public static readonly DependencyProperty ActiveProperty
            = DependencyProperty.Register(
                "Active",
                typeof(bool),
                typeof(Tab),
                new PropertyMetadata(false));

        #endregion

        private void TabButton_Checked(object sender, RoutedEventArgs e)
        {
            if (!Active)
                Active = true;
            TabButton.IsEnabled = false;
            OnTabActivated?.Invoke(this, true);
        }
        private void TabButton_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Active)
                Active = false;
            TabButton.IsEnabled = true;
        }
    }
}
