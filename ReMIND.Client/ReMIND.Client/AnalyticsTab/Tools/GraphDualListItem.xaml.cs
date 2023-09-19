using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using LiveChartsCore.SkiaSharpView.Painting;

namespace ReMIND.Client.AnalyticsTab.Tools
{
    /// <summary>
    /// Interaction logic for DualListItem.xaml
    /// </summary>
    public partial class GraphDualListItem : UserControl
    {
        public object Object { get; set; }
        public SolidColorPaint currentPaint { get; set; }

        public GraphDualListItem(object obj)
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
                typeof(GraphDualListItem),
                new PropertyMetadata("FailedToLoad")
                );

        //ako je item highlighted(oznacen), na njega ce se odnositi funkcije dugmica iz sredine liste
        public bool IsHighlighted
        {
            get
            {
                Brush compareBrush = new BrushConverter().ConvertFromString("#E9E9FF") as SolidColorBrush;
                return BackgroundBorder.Background.ToString() == compareBrush.ToString();
            }
            set
            {
                BackgroundBorder.Background = value ?
                    new BrushConverter().ConvertFromString("#E9E9FF") as SolidColorBrush :
                    new BrushConverter().ConvertFromString("#FAFAFA") as SolidColorBrush;
            }
        }


        //ako je item selected(na levoj strani), vidi se border, koji je iste boje kao this item's graph part
        public bool IsSelected
        {
            get => ColorBorder.Visibility == Visibility.Visible;
            set
            {
                ColorBorder.Visibility = value ?
                    Visibility.Visible : Visibility.Collapsed;

                DisplayLabel.Margin = value ?
                    new Thickness(2, 1, 20, 1) : new Thickness(2, 1, 2, 1);
            }
        }

        #endregion

        private void DualListItem_Loaded(object sender, RoutedEventArgs e)
        {
            DisplayText = Object.ToString();
        }

        public void AssignColor(SolidColorPaint paint)
        {
            currentPaint = paint;
            ColorBorder.Background = new BrushConverter().ConvertFromString(paint.Color.ToString()) as SolidColorBrush;
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
        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            ItemScale.ScaleX = 1;
            ItemScale.ScaleY = 1;
        }
        #endregion

    }
}
