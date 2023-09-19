using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ReMIND.Client.Elements
{
    /// <summary>
    /// Interaction logic for DualListPicker.xaml
    /// </summary>
    public partial class DualListPicker : UserControl
    {

        #region Data
        public List<object> OldleftList { get; set; }
        public List<object> OldRightList { get; set; }
        public List<object> LeftList { get; set; }
        public List<object> RightList { get; set; }
        public List<object> OldCheckedList { get; set; }
        public List<object> CheckedList { get; set; }
        #endregion

        public DualListPicker()
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
                typeof(DualListPicker),
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
                typeof(DualListPicker),
                new PropertyMetadata("RIGHT LIST")
                );
        #endregion

        #region Input
        public void AttachData(object[] objects)
        {
            OldleftList = objects.ToList();
            LeftList = objects.ToList();

            LeftListStackPanel.Children.Clear();
            RightListStackPanel.Children.Clear();
            foreach(var obj in objects)
            {
                DualListItem dli = new(obj);
                dli.CheckedChanged += Item_CheckedChanged;
                LeftListStackPanel.Children.Add(dli);
            }

            OldRightList = new();
            RightList = new();
            OldCheckedList = new();
            CheckedList = new();
        }
        public void SelectData(object[] objects)
        {
            List<DualListItem> itemsToMove = new();
            foreach (DualListItem dli in LeftListStackPanel.Children)
                if (objects.Contains(dli.Object))
                    itemsToMove.Add(dli);
            foreach (var item in itemsToMove)
            {
                OldleftList.Remove(item.Object);
                OldRightList.Add(item.Object);
                MoveToRight(item);
            }
        }
        public void CheckButtons(object[] objects)
        {
            foreach (DualListItem dli in RightListStackPanel.Children)
                if (objects.Contains(dli.Object))
                {
                    dli.IsChecked = true; //ovo ce da doda u checkedList (Item_CheckedChanged)
                    dli.IsHighlighted = !dli.IsHighlighted;
                    OldCheckedList.Add(dli.Object);
                }
        }
        public void DeselectAll()
        {
            List<DualListItem> itemsToMove = new();
            foreach (DualListItem dli in RightListStackPanel.Children)
                itemsToMove.Add(dli);
            foreach (var item in itemsToMove)
            {
                MoveToLeft(item);
                item.IsChecked = false;
            }
            LeftFilter.TextValue = "";
            LeftFilter.PlaceholderLabel.Visibility = Visibility.Visible;

            RightFilter.TextValue = "";
            RightFilter.PlaceholderLabel.Visibility = Visibility.Visible;
        }
        public void ResetControl()
        {
            object[] objs = new object[0];
            AttachData(objs);
        }
        public void ResetControlKeepData()
        {
            DeselectAll();
            List<object> oldData = OldleftList.ToList();
            oldData.AddRange(OldRightList.ToList());
            AttachData(oldData.ToArray());
        }
        #endregion

        #region Output
        public object[] GetAdded()
        {
            List<object> addedList = new();
            foreach (DualListItem dli in RightListStackPanel.Children)
                if (!OldRightList.Contains(dli.Object))
                    addedList.Add(dli.Object);

            return addedList.ToArray();
        }
        public object[] GetNewChecked()
        {
            List<object> checkedList = new();
            foreach (DualListItem dli in RightListStackPanel.Children)
                if (dli.IsChecked && !OldRightList.Contains(dli.Object))
                    checkedList.Add(dli.Object);

            return checkedList.ToArray();
        }
        public object[] GetRemoved()
        {
            List<object> removedList = new();
            foreach(DualListItem dli in LeftListStackPanel.Children)
                if (OldRightList.Contains(dli.Object))
                    removedList.Add(dli.Object);

            return removedList.ToArray();
        }
        public object[] GetUpdated() //vraca stvari iz oldRight koji imaju promenjen checked status
        {
            List<object> updatedList = new();
            foreach(DualListItem dli in RightListStackPanel.Children)
            {
                if (OldRightList.Contains(dli.Object)) //postojao u desnom
                {
                    if(OldCheckedList.Contains(dli.Object) //bese cekiran
                        && !CheckedList.Contains(dli.Object)) //sad nije
                    {
                        updatedList.Add(dli.Object);
                    }
                    else if(!OldCheckedList.Contains(dli.Object) //ne bese cekiran
                        && CheckedList.Contains(dli.Object)) //sad jeste
                    {
                        updatedList.Add(dli.Object);
                    }
                }
            }
            return updatedList.ToArray();
        }
        #endregion

        #region MiddleButtons=Move
        private void AddObjectButton_Click(object sender, RoutedEventArgs e)
        {
            List<DualListItem> itemsToMove = new();
            foreach (DualListItem dli in LeftListStackPanel.Children)
                if (dli.IsHighlighted)
                    itemsToMove.Add(dli);
            foreach (var item in itemsToMove)
                MoveToRight(item);
        }
        private void RemoveObjectButton_Click(object sender, RoutedEventArgs e)
        {
            List<DualListItem> itemsToMove = new();
            foreach (DualListItem dli in RightListStackPanel.Children)
                if (dli.IsHighlighted)
                    itemsToMove.Add(dli);
            foreach (var item in itemsToMove)
                MoveToLeft(item);
        }
        private void MoveToRight(DualListItem dli)
        {
            LeftListStackPanel.Children.Remove(dli);
            RightListStackPanel.Children.Insert(0, dli);
            dli.IsHighlighted = false;
            dli.IsSelected = true;

            if(!OldRightList.Contains(dli.Object))
                dli.BackgroundBorder.BorderBrush = new BrushConverter().ConvertFromString("#698626") as SolidColorBrush;
            else
                dli.BackgroundBorder.BorderBrush = new BrushConverter().ConvertFromString("#266986") as SolidColorBrush;
        }
        private void MoveToLeft(DualListItem dli)
        {
            RightListStackPanel.Children.Remove(dli);
            LeftListStackPanel.Children.Insert(0, dli);
            dli.IsHighlighted = false;
            dli.IsSelected = false;

            if(!OldleftList.Contains(dli.Object))
                dli.BackgroundBorder.BorderBrush = new BrushConverter().ConvertFromString("#D0480E") as SolidColorBrush;
            else
                dli.BackgroundBorder.BorderBrush = new BrushConverter().ConvertFromString("#266986") as SolidColorBrush;
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
            foreach(DualListItem dli in LeftListStackPanel.Children)
            {
                if (e == "" || dli.Object.ToString().Contains(e, StringComparison.CurrentCultureIgnoreCase))
                    dli.Visibility = Visibility.Visible;
                else
                    dli.Visibility = Visibility.Collapsed;
            }
        }
        private void RightFilter_TextChanged(object sender, string e)
        {
            foreach (DualListItem dli in RightListStackPanel.Children)
            {
                if (e == "" || dli.Object.ToString().Contains(e, StringComparison.CurrentCultureIgnoreCase))
                    dli.Visibility = Visibility.Visible;
                else
                    dli.Visibility = Visibility.Collapsed;
            }
        }
        #endregion

        #region Check event
        private void Item_CheckedChanged(object sender, bool e)
        {
            DualListItem dli = (DualListItem)sender;
            if (e)
                CheckedList.Add(dli.Object);
            else
                CheckedList.Remove(dli.Object);
        }
        #endregion

        #region UI Stuff
        public void flickerWithRed()
        {
            Utility.flickerPropertyWithRed(RightSideBorder, BackgroundProperty, ((SolidColorBrush)RightSideBorder.Background).Color);
        }
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
        #endregion

    }
}