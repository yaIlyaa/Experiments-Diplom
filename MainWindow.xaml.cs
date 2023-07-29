using DIPLOM;
using ExcelDataReader;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics.Metrics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace Experiments
{

    public partial class MainWindow : Window  
    {
        public bool AllChecked  
        {
            set
            {
                foreach (var experiment in context?.Experiments?.Local ?? 
                    new System.Collections.ObjectModel.ObservableCollection<Experiment>{ })
                    experiment.Selected = value;
                ExperimentsDataGrid.ItemsSource = null;
                ExperimentsDataGrid.ItemsSource = context?.Experiments?.Local;
            }
        }

        private readonly ExperimentsContext? context;  

        public MainWindow()
        {
            InitializeComponent();

            context = new ExperimentsContext();
            context?.Experiments?.Load();
            ExperimentsDataGrid.ItemsSource = context?.Experiments?.Local;
        }


        private void AddExperiment_Click(object sender, RoutedEventArgs e)  
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();  
            dialog.Filter = "Excel files|*.xlsx";
       
			Functions.ImportExcelFile(dialog, context);
			ExperimentsDataGrid.ItemsSource = null;
            ExperimentsDataGrid.ItemsSource = context?.Experiments?.Local;
        }


        private void AddManyExperiment_Click(object sender, RoutedEventArgs e)
        {

			var dialog = new FolderBrowserDialog();
			List<string> file = null;
			DialogResult result = dialog.ShowDialog();

			if (result == System.Windows.Forms.DialogResult.OK)
			{
				string[] files = Directory.GetFiles(dialog.SelectedPath);
				if (files.Length == 0)
				{
					System.Windows.Forms.MessageBox.Show("Выбранная папка пуста.");

				}
				else
				{
					file = files.ToList();
					foreach (var s in file)
					{
						Functions.ImportFolderExcelFiles(s, context);
					}
				}
			}
			else
			{
				System.Windows.Forms.MessageBox.Show("Не выбрана папка для импорта.");
			}

		}


        private void ButtonOpen_Click(object sender, RoutedEventArgs e)  
        {
            if (ExperimentsDataGrid.SelectedItem != null)  
            {
                var window = new ExperimentWindow(context,  
                    new List<Experiment>() { (Experiment)ExperimentsDataGrid.SelectedItem });
                window.Show();  
            }
        }

		private void ButtonDelete_Click(object sender, RoutedEventArgs e)  
		{
			if (ExperimentsDataGrid.SelectedItem != null)  
			{
				context?.Experiments?.Remove((Experiment)ExperimentsDataGrid.SelectedItem);  
				context?.SaveChanges();
			}
		}

		private void ButtonOpenSelected_Click(object sender, RoutedEventArgs e)  
        {
            var selected = context?.Experiments?.Local.Where(x => x.Selected).ToList();  
            if (selected != null && selected.Count > 0) 
            {
                var window = new ExperimentWindow(context, selected); 
                window.Show(); 
            }
        }

        private void ButtonDeleteSelected_Click(object sender, RoutedEventArgs e)
        {
            var selected = context?.Experiments?.Local.Where(x => x.Selected); 
            if (selected != null && selected.Count() > 0)  
             {
                foreach (var s in selected)  
                    context?.Data?.RemoveRange(context?.Data?  
                        .Where(x => x.ExperementId == s.Id));

                context?.Experiments?.RemoveRange(selected); 
                context?.SaveChanges(); 
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) 
        {
            context?.SaveChanges();  
        }
    }
}
