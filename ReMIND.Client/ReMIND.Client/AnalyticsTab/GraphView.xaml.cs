using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;

using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

using ReMIND.Client.Business.Models;
using ReMIND.Client.Business.Models.Types;

namespace ReMIND.Client.AnalyticsTab
{
    /// <summary>
    /// Interaction logic for GraphView.xaml
    /// </summary>
    public partial class GraphView : UserControl
    {

        #region Data & Properties

        private static readonly SolidColorPaint TransparentPaint = new SolidColorPaint(SKColor.Parse("#00000000"));
        private static readonly SolidColorPaint RedPaint = new SolidColorPaint(SKColor.Parse("#D0480E"));
        private static readonly SolidColorPaint ECECEC = new SolidColorPaint(SKColor.Parse("#ECECEC"));

        int maxEmployeeSeries = 10;
        int maxTeamSeries = 10;

        public ObservableCollection<ISeries> GraphSeries { get; set; }
        public ObservableCollection<ObservableCollection<ObservableValue>> GraphData { get; set; }
        public Axis[] XAxis { get; set; }
        public Axis[] YAxis { get; set; }

        private ISeries ColumnSeriesTemplate =>
            new ColumnSeries<ObservableValue>
            {
                Stroke = null,
                Fill = RedPaint,
                IsHoverable = true,
                Rx = 0,
                Ry = 0,
            };
        private ISeries LineSeriesTemplate =>
            new LineSeries<ObservableValue>
            {
                Fill = TransparentPaint,
                IsHoverable = true,
                LineSmoothness = 0.3,
            };
        #endregion

        public GraphView()
        {
            InitializeComponent();
            DataContext = this;

            GraphSeries = new ObservableCollection<ISeries>();
            GraphData = new ObservableCollection<ObservableCollection<ObservableValue>>();
            XAxis = new Axis[]
            {
                new Axis
                {
                    NamePaint = ECECEC,
                    LabelsPaint = new SolidColorPaint(SKColor.Parse("#AAA")),
                    ShowSeparatorLines = false,
                    LabelsRotation = -60,
                    TextSize = 12
                }
            };
            YAxis = new Axis[]
            {
                new Axis
                {
                    LabelsPaint = new SolidColorPaint(SKColor.Parse("#AAA")),
                    NamePaint = new SolidColorPaint(SKColor.Parse("#0000")),
                    ShowSeparatorLines = false,
                    TextSize = 12,
                    MinStep = 1,
                    MinLimit = 0
                }
            };
            setupSeries();
        }

        #region Input
        public void updateLabels(FromToDate interval)
        {
            XAxis[0].Labels = new List<string>();

            int days = (int)(interval.DateTo - interval.DateFrom).Value.TotalDays;
            DateTime start = interval.DateFrom.Value;
            for(int i = 0; i <= days; i++)
            {
                XAxis[0].Labels.Add(start.AddDays(i).ToString("dd.MM.yy"));
            }
        }

        public void addEmployeeData(Dictionary<Person, List<JobArchive>> jobs, Dictionary<Person, SolidColorPaint> paints)
        {
            int index = 0;//index da znamo u koji graphData ubacujemo
            foreach(var pair in jobs)//za svaku osobu i njene poslove, radimo stuff
            {
                #region DateTime dictionary
                Dictionary<DateTime, List<JobArchive>> data = new Dictionary<DateTime, List<JobArchive>>();
                foreach (var job in pair.Value)
                {
                    if (job.Finished == null)
                        continue; //ne racunamo nezavrsene poslove

                    List<JobArchive> outList = new();
                    if (data.TryGetValue(job.Finished.Value.Date, out outList))//ako postoji entry dodajemo posao u njega,
                    {
                        outList.Add(job);
                    }
                    else//ako ne postoji entry pravimo novi da dodamo
                    {
                        outList = new();
                        outList.Add(job);
                        data.Add(job.Finished.Value.Date, outList);
                    }
                }
                #endregion

                #region Sorting
                //sortiramo nove podatke po datumu i dodajemo ih u graf (prvo i poslednje mesto je prazno)
                var orderedPairs = data.OrderBy(x => x.Key).ToList();
                foreach(var orderedPair in orderedPairs)
                {
                    int sum = orderedPair.Value.Sum(x => x.JobWeight);
                    GraphData[index].Insert(GraphData[index].Count-1, new(sum));
                }
                #endregion

                #region Color
                //color assignment
                SolidColorPaint currentColumnPaint;
                if (paints.TryGetValue(pair.Key, out currentColumnPaint))
                {
                    var currentSerie = (ColumnSeries<ObservableValue>)GraphSeries[index];
                    currentSerie.Fill = new SolidColorPaint(SKColor.Parse(currentColumnPaint.Color.ToString()));//currentColumnPaint;
                }
                #endregion

                GraphSeries[index].IsVisible = true;
                GraphSeries[index].Name = pair.Key.Name;
                index++;
            }
        }

        public void addTeamsData(Dictionary<string, List<JobArchive>> jobs, Dictionary<string, SolidColorPaint> paints)
        {
            int index = 0 + maxEmployeeSeries; //index da znamo u koji graphData ubacujemo
            foreach(var pair in jobs)
            {
                #region DateTime dictionary
                Dictionary<DateTime, List<JobArchive>> data = new Dictionary<DateTime, List<JobArchive>>();
                foreach (var job in pair.Value)
                {
                    if (job.Finished == null)
                        continue; //ne racunamo nezavrsene poslove

                    List<JobArchive> outList = new();
                    if (data.TryGetValue(job.Finished.Value.Date, out outList))//ako postoji entry dodajemo posao u njega,
                    {
                        outList.Add(job);
                    }
                    else//ako ne postoji entry pravimo novi da dodamo u dictionary
                    {
                        outList = new();
                        outList.Add(job);
                        data.Add(job.Finished.Value.Date, outList);
                    }
                }
                #endregion

                #region Sorting
                //sortiramo nove podatke po datumu i dodajemo ih u graf (prvo i poslednje mesto je prazno)
                var orderedPairs = data.OrderBy(x => x.Key).ToList();
                foreach (var orderedPair in orderedPairs)
                {
                    int sum = orderedPair.Value.Sum(x => x.JobWeight);
                    int avg = sum / pair.Value.GroupBy(x => x.PersonID).Distinct().Count(); //piksi nisam siguran da ovo radi ali proveravam x.PersonID, ne znam sta je ucitano
                    GraphData[index].Insert(GraphData[index].Count - 1, new(avg));
                }
                #endregion

                #region Colors
                //color assignment
                SolidColorPaint currentLinePaint;
                if (paints.TryGetValue(pair.Key, out currentLinePaint))
                {
                    var currentSerie = (LineSeries<ObservableValue>)GraphSeries[index];
                    currentSerie.GeometryFill = new SolidColorPaint(SKColor.Parse("#FFF")) { StrokeThickness = 2 }; //currentColumnPaint;
                    currentSerie.GeometryStroke = new SolidColorPaint(SKColor.Parse(currentLinePaint.Color.ToString())) { StrokeThickness = 3 };
                    currentSerie.GeometrySize = 10;
                    currentSerie.Stroke = new SolidColorPaint(SKColor.Parse(currentLinePaint.Color.ToString())) { StrokeThickness = 4 };
                    currentSerie.Fill = null;
                }
                #endregion

                GraphSeries[index].IsVisible = true;
                GraphSeries[index].Name = pair.Key;
                index++;
            }
        }
        #endregion

        #region Setting up
        public void setupSeries()
        {
            for(int i = 0; i< maxEmployeeSeries; i++)
            {
                GraphData.Add(new() { new(0), new(0) });
                var serie = ColumnSeriesTemplate;
                serie.Values = GraphData[i];
                serie.IsVisible = false;
                GraphSeries.Add(serie);
            }

            for (int i = 0; i < maxTeamSeries; i++)
            {
                GraphData.Add(new() { new(0), new(0) });
                var serie = LineSeriesTemplate;
                serie.Values = GraphData[i + maxEmployeeSeries];
                serie.IsVisible = false;
                GraphSeries.Add(serie);
            }
        }
        public void clearEmployeeSeries()
        {
            for (int i = 0; i < maxEmployeeSeries; i++)
            {
                GraphSeries[i].IsVisible = false;

                //cistimo graphdata od starih podataka
                while (GraphData[i].Count > 2)
                    GraphData[i].RemoveAt(1);
            }
        }
        public void clearTeamSeries()
        {
            for (int i = 0; i < maxTeamSeries; i++)
            {
                GraphSeries[i + maxEmployeeSeries].IsVisible = false;

                //cistimo graphdata od starih podataka
                while (GraphData[i + maxEmployeeSeries].Count > 2)
                    GraphData[i + maxEmployeeSeries].RemoveAt(1);
            }
        }
        public void resetGraphData()
        {
            clearEmployeeSeries();
            clearTeamSeries();
        }
        #endregion

    }
}
