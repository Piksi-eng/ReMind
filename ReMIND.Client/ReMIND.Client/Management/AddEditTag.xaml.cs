using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using ReMIND.Client.Business;
using ReMIND.Client.Business.Models;

namespace ReMIND.Client.Management
{
    /// <summary>
    /// Interaction logic for AddEditTag.xaml
    /// </summary>
    public partial class AddEditTag : UserControl
    {
        Function _function = Function.Create;
        Function function
        {
            get => _function;
            set
            {
                _function = value;
                SaveButton.DisplayText = value == Function.Create ?
                    "SAVE" : "UPDATE";
            }
        }

        JobTag JobTag;
        public AddEditTag(JobTag t = null)
        {
            InitializeComponent();
            DataContext = this;
            
            if(t == null)
                JobTag = new();
            else
            {
                JobTag = t;
                function = Function.Update;
            }

            JobItemPreview1._TestMode = true;
            JobItemPreview2._TestMode = true;
        }

        #region Tag UI stuff
        private void TagColorBorder_Click(object sender, RoutedEventArgs e)
        {
            ColorPicker.IsOpen = true;
        }
        private void ColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            TagColorBorder.Background = new SolidColorBrush(ColorPicker.SelectedColor.Value);
            var color = (TagColorBorder.Background as SolidColorBrush).Color;
            TagColorBorder.Content = color.ToString().Remove(1, 2); // index 1 and 2 is alpha channel value. that's why we remove them
            Refresh();
        }
        private void TagNameInput_InputTextChanged(object sender, string e)
        {
            Refresh();
        }
        private void Refresh()
        {
            #region First Cell test tag
            CalendarPreviewCell1.ClearJobs();
            CalendarPreviewCell1.AddTestTag(ColorPicker.SelectedColor.Value);
            #endregion

            #region Second Cell test tags
            CalendarPreviewCell2.ClearJobs();
            List<string> tagColors = new();
            foreach(JobTag tag in Utility.JobTags)
                tagColors.Add(tag.Color);
            tagColors.Add(ColorPicker.SelectedColor.Value.ToString());

            Random r = new Random();
            for (int i = 0; i < 8; i++)
            {
                int randIndex = (r.Next() % tagColors.Count);
                Color c = (Color)ColorConverter.ConvertFromString(tagColors[randIndex]);
                CalendarPreviewCell2.AddTestTag(c);
            }
            #endregion

            #region JobItems
            JobTag jt1 = new(1, TagNameInput.TextInputBox.Text, ColorPicker.SelectedColor.Value.ToString());
            JobTag jt2 = new(1, TagNameInput.TextInputBox.Text, ColorPicker.SelectedColor.Value.ToString());

            Job testJob1 = new()
            {
                Name = "Test job",
                JobGroup = new() { Name = "Test task" },
                Deadline = DateTime.Now.AddDays(1),
                JobTag = jt1,
                IsDone = false
            };
            JobItemPreview1.newData(testJob1);

            Job testJob2 = new()
            {
                Name = "Test job",
                JobGroup = new() { Name = "Test task" },
                Deadline = DateTime.Now.AddDays(-1),
                JobTag = jt2,
                IsDone = true
            };
            JobItemPreview2.newData(testJob2);
            #endregion

        }
        #endregion

        #region Buttons
        private async void SaveButton_ButtonClicked(object sender, RoutedEventArgs e)
        {
            if (!CheckFields())
                return;

            JobTag jt = new();
            jt.Name = TagNameInput.TextValue;
            jt.Color = TagColorBorder.Content.ToString();
            if (function == Function.Create)
            {
                bool Answer = ReMINDMessage.Show("Are you sure you want to create a Job Type : " + jt.Name + "", true, "Confirm");
                if (Answer)
                {
                    bool res = await Utility.CreateJobTag(jt);
                    if (res)
                    {
                        ReMINDMessage.Show("Job Type Created", false, "Success");
                        Utility.ReloadManagementData();
                        Utility.ReloadTasksData();
                    }
                }
            }
            else //update deo
            {
                jt.ID = JobTag.ID;
                bool Answer = ReMINDMessage.Show("Are you sure you want to updated a Job Type : " + jt.Name + "", true, "Confirm");
                if (Answer)
                {
                    bool res = await Utility.UpdatedJobTag(jt);
                    if (res)
                    {
                        ReMINDMessage.Show("Job Type Updated", false, "Success");
                        Utility.ReloadManagementData();
                        Utility.ReloadTasksData();
                    }
                }
            }


        }

        private async void DeleteButton_ButtonClicked(object sender, RoutedEventArgs e)
        {
            if(function == Function.Create)
            {
                ReMINDMessage.Show("Please select a job type", false, "Nothing selected");
                return;
            }

            JobTag jt = JobTag;
            bool res = ReMINDMessage.Show("Are you sure you want to delete the Job Type" + jt.Name + 
                                          ".All Jobs with that taype will be tranfered to The Deafult Type", true, "Confirm");
            if(res)
            {
                await Utility.DeleteJobTag(jt);
                ReMINDMessage.Show("Job Type Removed", false, "Success");
                Utility.ReloadManagementData();
            }

            clearAll();
        }
        #endregion

        private void ClearButton_ButtonClicked(object sender, RoutedEventArgs e)
        {
            clearAll();
        }

        private void clearAll()
        {
            JobTag = new();
            ColorPicker.SelectedColor = Color.FromRgb(38, 105, 134);
            TagNameInput.TextValue = "";
            function = Function.Create;
            Refresh();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            CalendarPreviewCell1.DateText = DateTime.Now.Day.ToString();
            CalendarPreviewCell2.DateText = DateTime.Now.AddDays(1).Day.ToString();

            if(function == Function.Create)
            {
                ColorPicker.SelectedColor = Color.FromRgb(38, 105, 134); //this should be #266986;
            }
            else
            {
                ColorPicker.SelectedColor = (Color)ColorConverter.ConvertFromString(JobTag.Color);
                TagNameInput.TextValue = JobTag.Name;
            }
            Refresh();
        }

        private bool CheckFields()
        {
            bool passed = true;
            if(TagNameInput.TextValue == "")
            {
                TagNameInput.flickerWithRed();
                passed = false;
            }
            return passed;
        }
    }
}