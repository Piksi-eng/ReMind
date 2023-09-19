using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using LiveChartsCore.SkiaSharpView.Painting;

namespace ReMIND.Client.AnalyticsTab.Tools
{
    /// <summary>
    /// Interaction logic for DualListPicker.xaml
    /// </summary>
    public partial class GraphDualListPicker : UserControl
    {
        public event EventHandler<bool> SelectedItemsChanged;

        #region Data
        public List<object> OldleftList { get; set; }
        public List<object> OldRightList { get; set; }
        public List<object> LeftList { get; set; }
        public List<object> RightList { get; set; }
        #endregion

        public GraphDualListPicker()
        {
            InitializeComponent();
            DataContext = this;
        }

        #region Properties
        public string LeftText
        {
            get => (string)GetValue(LeftTextProperty);
            set => SetValue(LeftTextProperty, value);
        }
        public static readonly DependencyProperty LeftTextProperty
            = DependencyProperty.Register(
                "LeftText",
                typeof(string),
                typeof(GraphDualListPicker),
                new PropertyMetadata("LEFT LIST")
                );

        public string RightText
        {
            get => (string)GetValue(RightTextProperty);
            set => SetValue(RightTextProperty, value);
        }
        public static readonly DependencyProperty RightTextProperty
            = DependencyProperty.Register(
                "RightText",
                typeof(string),
                typeof(GraphDualListPicker),
                new PropertyMetadata("RIGHT LIST")
                );
        #endregion

        #region Input
        public void AttachData(object[] objects)
        {
            OldRightList = objects.ToList();
            RightList = objects.ToList();

            RightListStackPanel.Children.Clear();
            LeftListStackPanel.Children.Clear();
            foreach (var obj in objects)
            {
                GraphDualListItem gli = new(obj);
                RightListStackPanel.Children.Add(gli);
            }

            OldleftList = new();
            LeftList = new();
        }
        public void SelectData(object[] objects)
        {
            List<GraphDualListItem> itemsToMove = new();
            foreach (GraphDualListItem dli in RightListStackPanel.Children)
                if (objects.Contains(dli.Object))
                    itemsToMove.Add(dli);

            foreach (var item in itemsToMove)
            {
                OldRightList.Remove(item.Object);
                OldleftList.Add(item.Object);
                MoveToLeft(item);
            }

            SelectedItemsChanged?.Invoke(this, true);
        }
        public void DeselectAll()
        {
            List<GraphDualListItem> itemsToMove = new();
            foreach (GraphDualListItem dli in LeftListStackPanel.Children)
                itemsToMove.Add(dli);

            foreach (var item in itemsToMove)
                MoveToRight(item);

            LeftFilter.TextValue = "";
            LeftFilter.PlaceholderLabel.Visibility = Visibility.Visible;

            RightFilter.TextValue = "";
            RightFilter.PlaceholderLabel.Visibility = Visibility.Visible;

            SelectedItemsChanged?.Invoke(this, true);
        }
        public void ResetControl()
        {
            object[] objs = Array.Empty<object>();
            AttachData(objs);

            SelectedItemsChanged?.Invoke(this, true);
        }
        public void ResetControlKeepData()
        {
            DeselectAll();
            List<object> oldData = OldRightList.ToList();
            oldData.AddRange(OldleftList.ToList());
            AttachData(oldData.ToArray());

            SelectedItemsChanged?.Invoke(this, true);
        }
        #endregion

        #region Output
        public Dictionary<T, SolidColorPaint> GetAllSelected<T>()
        {
            Dictionary<T, SolidColorPaint> allSelected = new Dictionary<T, SolidColorPaint>();
            foreach (GraphDualListItem dli in LeftListStackPanel.Children)
                allSelected.Add(ObjectConverter.ToTypedInstance<T>(dli.Object), dli.currentPaint);

            return allSelected;
        }
        #endregion

        #region MiddleButtons
        private void AddObjectButton_Click(object sender, RoutedEventArgs e)
        {
            List<GraphDualListItem> itemsToMove = new();
            foreach (GraphDualListItem dli in RightListStackPanel.Children)
                if (dli.IsHighlighted)
                    itemsToMove.Add(dli);
            foreach (var item in itemsToMove)
                MoveToLeft(item);

            SelectedItemsChanged?.Invoke(this, true);
        }
        private void RemoveObjectButton_Click(object sender, RoutedEventArgs e)
        {
            List<GraphDualListItem> itemsToMove = new();
            foreach (GraphDualListItem dli in LeftListStackPanel.Children)
                if (dli.IsHighlighted)
                    itemsToMove.Add(dli);
            foreach (var item in itemsToMove)
                MoveToRight(item);

            SelectedItemsChanged?.Invoke(this, true);
        }
        private void MoveToRight(GraphDualListItem dli)
        {
            GraphColorManager.release(dli.currentPaint);
            LeftListStackPanel.Children.Remove(dli);
            RightListStackPanel.Children.Insert(0, dli);
            dli.IsHighlighted = false;
            dli.IsSelected = false;
        }
        private void MoveToLeft(GraphDualListItem dli)
        {
            dli.AssignColor(GraphColorManager.takeNext());
            RightListStackPanel.Children.Remove(dli);
            LeftListStackPanel.Children.Insert(0, dli);
            dli.IsHighlighted = false;
            dli.IsSelected = true;
        }
        #endregion

        #region Scroll
        private void LeftListScroller_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            double marginTop = LeftListScroller.VerticalOffset * LeftListScroller.ViewportHeight / LeftListScroller.ExtentHeight;
            if (double.IsNaN(marginTop)) //ako delimo nulom onda je bljak pa moramo da mu stavimo value na 0
                marginTop = 0;
            LeftListScrollBorder.Margin = new Thickness(0, marginTop, 0, 0);

            double scrollbarHeight = (LeftListScroller.ViewportHeight * LeftListScroller.ViewportHeight) / LeftListScroller.ExtentHeight;
            if (double.IsInfinity(scrollbarHeight)) //similar thing here
                scrollbarHeight = LeftListScroller.ViewportHeight;

            LeftListScrollBorder.Height = LeftListScroller.ExtentHeight >= LeftListScroller.ViewportHeight ? //ne pitaj zasto
                scrollbarHeight : LeftListScroller.ExtentHeight;
        }

        private void RightListScroller_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            double marginTop = RightListScroller.VerticalOffset * RightListScroller.ViewportHeight / RightListScroller.ExtentHeight;
            if (double.IsNaN(marginTop)) //ako delimo nulom onda je bljak pa moramo da mu stavimo value na 0
                marginTop = 0;
            RightListScrollBorder.Margin = new Thickness(0, marginTop, 0, 0);

            double scrollbarHeight = (RightListScroller.ViewportHeight * RightListScroller.ViewportHeight) / RightListScroller.ExtentHeight;
            if (double.IsInfinity(scrollbarHeight)) //similar thing here
                scrollbarHeight = RightListScroller.ViewportHeight;

            RightListScrollBorder.Height = RightListScroller.ExtentHeight >= RightListScroller.ViewportHeight ? //ne pitaj zasto
                scrollbarHeight : RightListScroller.ExtentHeight;
        }
        #endregion

        #region Search
        private void LeftFilter_TextChanged(object sender, string e)
        {
            foreach (GraphDualListItem dli in LeftListStackPanel.Children)
            {
                if (e == "" || dli.Object.ToString().Contains(e, StringComparison.CurrentCultureIgnoreCase))
                    dli.Visibility = Visibility.Visible;
                else
                    dli.Visibility = Visibility.Collapsed;
            }
        }
        private void RightFilter_TextChanged(object sender, string e)
        {
            foreach (GraphDualListItem dli in RightListStackPanel.Children)
            {
                if (e == "" || dli.Object.ToString().Contains(e, StringComparison.CurrentCultureIgnoreCase))
                    dli.Visibility = Visibility.Visible;
                else
                    dli.Visibility = Visibility.Collapsed;
            }
        }
        #endregion

        private void UserControl_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            bool newVal = (bool)e.NewValue;
            if (newVal)
            {
                Opacity = 1;
            }
            else
            {
                Opacity = 0.5;
            }
        }
    }
}
