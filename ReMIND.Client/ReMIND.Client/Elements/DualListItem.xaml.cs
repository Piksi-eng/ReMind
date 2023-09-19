using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ReMIND.Client.Elements
{
    /// <summary>
    /// Interaction logic for DualListItem.xaml
    /// </summary>
    public partial class DualListItem : UserControl
    {
        public event EventHandler<bool> CheckedChanged;
        public object Object { get; set; }

        public DualListItem(object obj)
        {
            InitializeComponent();
            DataContext = this;
            Object = obj;
        }

        #region Properties
        public string DisplayText
        {
            get => (string)GetValue(DisplayTextProperty);
            set => SetValue(DisplayTextProperty, value);
        }
        public static readonly DependencyProperty DisplayTextProperty
            = DependencyProperty.Register(
                "DisplayText",
                typeof(string),
                typeof(DualListItem),
                new PropertyMetadata("FailedToLoad")
                );

        //ako je item highlighted(oznacen), na njega ce se odnositi funkcije dugmica iz sredine liste
        public bool IsHighlighted
        {
            get
            {
                Brush compareBrush = new BrushConverter().ConvertFromString("#377A97") as SolidColorBrush;
                return BackgroundBorder.Background.ToString() == compareBrush.ToString();
            }
            set
            {
                BackgroundBorder.Background = value ?
                    new BrushConverter().ConvertFromString("#377A97") as SolidColorBrush :
                    new BrushConverter().ConvertFromString("#266986") as SolidColorBrush;
            }
        }


        //ako je item selected(na desnoj strani), vidi se dugme
        public bool IsSelected
        {
            get => CheckButton.Visibility == Visibility.Visible;
            set
            {
                CheckButton.Visibility = value ?
                    Visibility.Visible : Visibility.Hidden;

                DisplayLabel.Margin = value ?
                    new Thickness(2, 1, 20, 1) : new Thickness(2, 1, 2, 1);
            }
        }


        //ako je item checked, oznacen je kao lider u izabranom timu(zavrsio posao u izabranoj listi radnika itd. za sta vec moze da se koristi)
        public bool IsChecked
        {
            get => CheckButton.IsChecked == true;
            set
            {
                CheckButton.IsChecked = value;
            }
        }

        #endregion

        private void DualListItem_Loaded(object sender, RoutedEventArgs e)
        {
            DisplayText = Object.ToString();
        }

        #region Mouse Events
        private void DualListItem_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            IsHighlighted = !IsHighlighted;
        }

        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ItemScale.ScaleX = 0.95;
            ItemScale.ScaleY = 0.95;
        }

        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ItemScale.ScaleX = 1;
            ItemScale.ScaleY = 1;
        }

        #endregion

        #region Checkbutton
        private void CheckButton_Checked(object sender, RoutedEventArgs e)
        {
            CheckedChanged?.Invoke(this, true);
            IsHighlighted = !IsHighlighted;
        }

        private void CheckButton_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckedChanged?.Invoke(this, false);
            IsHighlighted = !IsHighlighted;
        }
        #endregion

    }
}
