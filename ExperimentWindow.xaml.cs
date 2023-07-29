using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows;
using ZedGraph;

namespace Experiments
{

    public partial class ExperimentWindow : Window  
    {
        private static readonly Random random = new Random();  

        public static string RandomString(int length) 
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";  
            return new string(Enumerable.Repeat(chars, length)  
                .Select(s => s[random.Next(s.Length)]).ToArray()); 
        }

        public ExperimentWindow(ExperimentsContext? context, List<Experiment> experiments)  
        {
            InitializeComponent();  

            var colors = new List<Color> 
            { 
                Color.FromArgb(69, 218, 170),
                Color.FromArgb(238, 112, 106),
                Color.FromArgb(73, 169, 225),
                Color.FromArgb(226, 194, 93),
                Color.FromArgb(140, 118, 200)
            };

            GraphPane pane = GraphUp.GraphPane;  
            pane.CurveList.Clear(); 
            pane.YAxis.Title.Text = "Cutting Force, Fz (Newton)";  
            pane.XAxis.Title.Text = "Time (seconds)"; 
            pane.Title.Text = "(CrAlSi)N - Epilam";  

            GraphPane paneDown = GraphDown.GraphPane;  
            paneDown.CurveList.Clear();  
            paneDown.YAxis.Title.Text = "Cutting Force, Fz (Newton)";  
            paneDown.XAxis.Title.Text = "Time (seconds)";  
            paneDown.Title.Text = "(CrAlSi)N - Epilam";  

            int index = 0;  
            var table = new DataTable();  

            foreach (var experiment in experiments)  
            {
                var id = experiment.Id;  
                if (index > 0)  
                    table.Columns.Add(RandomString(5));  
                table.Columns.Add(RandomString(8));  
                table.Columns.Add(RandomString(8));  

                var query = context?.Data?.Where(x => x.ExperementId == id).ToList();  
                if (query != null && query.Count > 0)  
                {
                    while (table.Rows.Count - 2 < query.Count)  
                        table.Rows.Add();  

                    int column1 = 0; 
                    int column2 = 1;  

                    if (index > 0)  
                    {
                        column1 = table.Columns.Count - 2;  
                        column2 = table.Columns.Count - 1;  
                    }

                    table.Rows[0][column1] = experiment.Name;  
                    table.Rows[1][column1] = "Time (s)";  
                    table.Rows[1][column2] = "Fz (N)";  

                    for (int i = 0; i < query.Count; i++)  
                    {
                        table.Rows[i + 2][column1] = query[i].Time.ToString();  
                        table.Rows[i + 2][column2] = query[i].Fz.ToString();  
                    }

                    PointPairList list = new PointPairList(); 
                    list.AddRange(query.Select(x => new PointPair(x.Time, x.Fz))); 
                    LineItem myCurve = pane.AddCurve(experiment.Name, list, colors[index % colors.Count], 
                        SymbolType.None);  

                    var xValues = list.Select(x => x.X).ToArray();  
                    var yValues = list.Select(x => x.Y).ToArray();  
                    var cubicSpline = new CubicSpline(xValues, yValues);  
                    PointPairList list2 = new PointPairList(); 
                    var min = xValues.Min(); 
                    var max = xValues.Max();  
                    var step = (max - min ) / 30.0; 
                    for (double x = min; x < max; x += step) 
                        list2.Add(new PointPair(x, cubicSpline.GetY(x))); 
                    LineItem myCurve2 = paneDown.AddCurve(experiment.Name, list2, colors[index++ % colors.Count],
                        SymbolType.None); 
                }

                DataGrid.ItemsSource = table.AsDataView();  

                GraphUp.AxisChange(); 
                GraphUp.Invalidate();  

                GraphDown.AxisChange();  
                GraphDown.Invalidate();  
            }       
        }


	}
}
