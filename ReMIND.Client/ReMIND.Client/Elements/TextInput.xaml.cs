using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ReMIND.Client.Elements
{
    /// <summary>
    /// Interaction logic for TextInput.xaml
    /// </summary>
    public partial class TextInput : UserControl
    {
        public event EventHandler<string> InputTextChanged;
        public TextInput()
        {
            InitializeComponent();
            DataContext = this;

            ElementBorder.PreviewMouseDown += Border_Focused;
            ElementBorder.GotKeyboardFocus += Border_Focused;
            TextInputBox.LostFocus += TextInput_LostFocus;
            TextInputBox.LostKeyboardFocus += TextInput_LostFocus;
        }

        #region Properties
        public string TextValue
        {
            get => (string)GetValue(TextValueProperty);
            set
            {
                SetValue(TextValueProperty, value);
                if (value != "" || value != null)
                    PlaceholderLabel.Visibility = Visibility.Collapsed;
                if (value == "")
                    PlaceholderLabel.Visibility = Visibility.Visible;
            }
        }
        public static readonly DependencyProperty TextValueProperty
            = DependencyProperty.Register(
                "TextValue",
                typeof(string),
                typeof(TextInput),
                new PropertyMetadata("")
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
                typeof(TextInput)
                );


        public string TextBackground
        {
            get => (string)GetValue(InputBackgroundProperty);
            set => SetValue(InputBackgroundProperty, value);
        }
        public static readonly DependencyProperty InputBackgroundProperty
            = DependencyProperty.Register(
                "InputBackground",
                typeof(string),
                typeof(TextInput),
                new PropertyMetadata("#FFF")
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
                typeof(TextInput),
                new PropertyMetadata("#000")
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
                typeof(TextInput),
                new PropertyMetadata("#266986")
                );


        public ImageSource ImageSource
        {
            get => (ImageSource)GetValue(ImageSourceProperty);
            set
            {
                SetValue(ImageSourceProperty, value);
                if (value == null)
                {
                    Icon.Visibility = Visibility.Collapsed;
                }
                else
                    Icon.Visibility = Visibility.Visible;
            }
        }
        public static readonly DependencyProperty ImageSourceProperty
            = DependencyProperty.Register(
                "ImageSource",
                typeof(ImageSource),
                typeof(TextInput)
                );

        public double ShadowOpacity
        {
            get => (double)GetValue(ShadowOpacityProperty);
            set => SetValue(ShadowOpacityProperty, value);
        }
        public static readonly DependencyProperty ShadowOpacityProperty
            = DependencyProperty.Register(
                "ShadowOpacity",
                typeof(double),
                typeof(TextInput),
                new PropertyMetadata(0.25)
                );

        #endregion

        #region Focus
        private void Border_Focused(object sender, EventArgs e)
        {
            Border b = (Border)sender;
            StackPanel sp = (StackPanel)b.Child;
            sp.Children[1].Visibility = Visibility.Collapsed;
            TextBox elem = (TextBox)sp.Children[2];
            elem.Focus();
            elem.SelectAll();
        }
        private void TextInput_LostFocus(object sender, RoutedEventArgs e)
        {
            if (TextInputBox.Text != "")
                return;

            PlaceholderLabel.Visibility = Visibility.Visible;
        }

        private void TextInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                TextValue = "";
                unFocus();
            }
            if(e.Key == Key.Enter)
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

        private void TextInputBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            InputTextChanged?.Invoke(this, TextInputBox.Text);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (Icon.Source == null || Icon.Source.ToString() == "")
                Icon.Visibility = Visibility.Collapsed;
        }

        #region UI Stuff
        public void flickerWithRed()
        {
            Utility.flickerPropertyWithRed(ElementBorder, Border.BackgroundProperty, ((SolidColorBrush)ElementBorder.Background).Color);
        }
        private void UserControl_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue == true)
                Opacity = 1;
            else
                Opacity = 0.5;
        }
        #endregion

    }
}
