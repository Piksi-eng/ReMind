using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ReMIND.Client.Elements
{
    /// <summary>
    /// Interaction logic for PasswordInput.xaml
    /// </summary>
    public partial class PasswordInput : UserControl
    {
        public PasswordInput()
        {
            InitializeComponent();
            DataContext = this;
            ElementBorder.PreviewMouseDown += Border_Focused;
            ElementBorder.GotKeyboardFocus += Border_Focused;
            PasswordInputBox.LostFocus += PasswordInput_LostFocus;
            PasswordInputBox.LostKeyboardFocus += PasswordInput_LostFocus;
        }
        public string PasswordValue
        {
            get => PasswordInputBox.Password;
            set => PasswordInputBox.Password = value;
        }

        #region Properties

        public double InputWidth
        {
            get => (double)GetValue(InputWidthProperty);
            set => SetValue(InputWidthProperty, value);
        }
        public static readonly DependencyProperty InputWidthProperty
            = DependencyProperty.Register(
                "InputWidth",
                typeof(double),
                typeof(PasswordInput),
                new PropertyMetadata()
                );


        public double InputHeight
        {
            get => (double)GetValue(InputHeightProperty);
            set => SetValue(InputHeightProperty, value);
        }
        public static readonly DependencyProperty InputHeightProperty
            = DependencyProperty.Register(
                "InputHeight",
                typeof(double),
                typeof(PasswordInput),
                new PropertyMetadata()
                );


        public string BorderColor
        {
            get => (string)GetValue(BorderColorProperty);
            set => SetValue(BorderColorProperty, value);
        }
        public static readonly DependencyProperty BorderColorProperty
            = DependencyProperty.Register(
                "BorderColor",
                typeof(string),
                typeof(PasswordInput),
                new PropertyMetadata("#266986")
                );


        public string InputBackground
        {
            get => (string)GetValue(InputBackgroundProperty);
            set => SetValue(InputBackgroundProperty, value);
        }
        public static readonly DependencyProperty InputBackgroundProperty
            = DependencyProperty.Register(
                "InputBackground",
                typeof(string),
                typeof(PasswordInput),
                new PropertyMetadata("#FFF")
                );


        public string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }
        public static readonly DependencyProperty PlaceholderProperty
            = DependencyProperty.Register(
                "Placeholder",
                typeof(string),
                typeof(PasswordInput)
                );


        public double PlaceholderOpacity
        {
            get => (double)GetValue(PlaceholderOpacityProperty);
            set => SetValue(PlaceholderProperty, value);
        }
        public static readonly DependencyProperty PlaceholderOpacityProperty
            = DependencyProperty.Register(
                "PlaceholderOpacity",
                typeof(double),
                typeof(PasswordInput),
                new PropertyMetadata(0.5)
                );

        public string TextForeground
        {
            get => (string)GetValue(TextForegroundProperty);
            set => SetValue(TextForegroundProperty, value);
        }
        public static readonly DependencyProperty TextForegroundProperty
            = DependencyProperty.Register(
                "TextForeground",
                typeof(string),
                typeof(PasswordInput),
                new PropertyMetadata("#000")
                );

        #endregion

        #region Focus
        private void Border_Focused(object sender, EventArgs e)
        {
            Border b = (Border)sender;
            StackPanel sp = (StackPanel)b.Child;
            sp.Children[0].Visibility = Visibility.Collapsed;
            PasswordBox elem = (PasswordBox)sp.Children[1];
            elem.Focus();
            elem.SelectAll();
        }
        private void PasswordInput_LostFocus(object sender, RoutedEventArgs e)
        {
            if (PasswordInputBox.Password != "")
                return;

            PlaceholderLabel.Visibility = Visibility.Visible;
        }

        private void PasswordInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                PasswordInputBox.Password = "";
                unFocus();
            }
            if (e.Key == Key.Enter)
            {
                unFocus();
            }
        }

        private void unFocus()
        {
            FrameworkElement parent = (FrameworkElement)Parent;
            while (parent != null && parent is IInputElement element && !element.Focusable)
            {
                parent = (FrameworkElement)parent.Parent;
            }

            DependencyObject scope = FocusManager.GetFocusScope(this);
            FocusManager.SetFocusedElement(scope, parent);
        }
        #endregion

        public void flickerWithRed()
        {
            Utility.flickerPropertyWithRed(ElementBorder, BackgroundProperty, ((SolidColorBrush)ElementBorder.Background).Color);
        }

    }
}
