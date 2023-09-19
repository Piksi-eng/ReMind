using System;
using System.Windows;
using System.Windows.Controls;

namespace ReMIND.Client.Elements
{
    /// <summary>
    /// Interaction logic for CleanButton.xaml
    /// </summary>
    public partial class CleanButton : UserControl
    {
        public event EventHandler<RoutedEventArgs> ButtonClicked;

        public CleanButton()
        {
            InitializeComponent();
            DataContext = this;
        }

        #region Properties
        public string DisplayText
        {
            get => (string)GetValue(DisplayTextProperty);
            set => SetValue(DisplayTextProperty, value.ToUpper());
        }
        public static readonly DependencyProperty DisplayTextProperty
            = DependencyProperty.Register(
                "DisplayText",
                typeof(string),
                typeof(CleanButton),
                new PropertyMetadata("BUTTON")
                );

        public string ButtonBackground
        {
            get => (string)GetValue(ButtonBackgroundProperty);
            set => SetValue(ButtonBackgroundProperty, value.ToUpper());
        }
        public static readonly DependencyProperty ButtonBackgroundProperty
            = DependencyProperty.Register(
                "ButtonBackground",
                typeof(string),
                typeof(CleanButton),
                new PropertyMetadata("#FFF")
                );

        public string ButtonForeground
        {
            get => (string)GetValue(ButtonForegroundProperty);
            set => SetValue(ButtonForegroundProperty, value.ToUpper());
        }
        public static readonly DependencyProperty ButtonForegroundProperty
            = DependencyProperty.Register(
                "ButtonForeground",
                typeof(string),
                typeof(CleanButton),
                new PropertyMetadata("#266986")
                );
        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ButtonClicked?.Invoke(this, e);
        }
    }
}
