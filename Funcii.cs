using ExcelDataReader;
using Experiments;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace DIPLOM
{
    public static class Functions
    {

		public static void ImportExcelFile(Microsoft.Win32.OpenFileDialog dialog, ExperimentsContext context)
		{
		
			if (dialog.ShowDialog() == true)
			{
				if (IsFileOpen(dialog.FileName))
				{
					MessageBox.Show("Файл уже открыт. Пожалуйста, закройте файл перед загрузкой.");
					return;
				}

				FileStream stream = File.Open(dialog.FileName, FileMode.Open, FileAccess.Read);
				IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
				DataSet result = excelReader.AsDataSet();
				DataTable dt = result.Tables[0];

				var rows = dt.Rows;
				int? timeIndex = null;
				int? fzIndex = null;
				int startRow = 0;

				
				for (int i = 0; i < rows.Count; i++)
				{
					for (int j = 0; j < dt.Columns.Count; j++)
					{
						if (rows[i][j].ToString().StartsWith("Time"))
						{
							timeIndex = j;
							startRow = i + 1;
						}
						if (rows[i][j].ToString().StartsWith("Fz"))
						{
							fzIndex = j;
						}
					}
					
					if (timeIndex.HasValue && fzIndex.HasValue)
					{
						break;
					}
				}

				if (timeIndex.HasValue && fzIndex.HasValue && rows.Count > startRow)
				{
					if (rows[startRow][timeIndex.Value].ToString() == "s" &&
						rows[startRow][fzIndex.Value].ToString() == "N")
					{
						startRow += 1; 
					}

					var experiment = new Experiment
					{
						Name = Path.GetFileNameWithoutExtension(dialog.FileName),
						Time = DateTime.Now
					};

					context?.Experiments?.Add(experiment);
					context?.SaveChanges();

					var data = new List<Data>();

					for (int i = startRow; i < rows.Count; i++)
					{
						if (double.TryParse(rows[i][timeIndex.Value].ToString(), out double time) &&
							double.TryParse(rows[i][fzIndex.Value].ToString(), out double fz))
						{
							data.Add(new Data()
							{
								Time = time,
								Fz = fz,
								ExperementId = experiment.Id
							});
						}
						else
						{
							MessageBox.Show("Некорректный файл");
							return;
						}
					}

					context?.Data?.AddRange(data);
					context?.SaveChanges();
				}
				else
				{
					MessageBox.Show("Некорректный файл");
				}
			}
		}

		public static bool IsFileOpen(string filename)
		{
			try
			{
				using (var stream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.None))
				{
					return false;
				}
			}
			catch (IOException)
			{
				return true;
			}
		}


		public static void ImportFolderExcelFiles(string s, ExperimentsContext context)
        {

			try
			{
				if (IsFileOpen(s))
				{
					MessageBox.Show("Файл уже открыт. Пожалуйста, закройте файл перед загрузкой.");
					return;
				}

				FileStream stream = File.Open(s, FileMode.Open, FileAccess.Read);
				IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
				DataSet result = excelReader.AsDataSet();
				DataTable dt = result.Tables[0];

				var rows = dt.Rows;
				int? timeIndex = null;
				int? fzIndex = null;
				int startRow = 0;


				for (int i = 0; i < rows.Count; i++)
				{
					for (int j = 0; j < dt.Columns.Count; j++)
					{
						if (rows[i][j].ToString().StartsWith("Time"))
						{
							timeIndex = j;
							startRow = i + 1;
						}
						if (rows[i][j].ToString().StartsWith("Fz"))
						{
							fzIndex = j;
						}
					}

					if (timeIndex.HasValue && fzIndex.HasValue)
					{
						break;
					}
				}

				if (timeIndex.HasValue && fzIndex.HasValue && rows.Count > startRow)
				{
					if (rows[startRow][timeIndex.Value].ToString() == "s" &&
						rows[startRow][fzIndex.Value].ToString() == "N")
					{
						startRow += 1;
					}

					var experiment = new Experiment
					{
						Name = Path.GetFileNameWithoutExtension(s),
						Time = DateTime.Now
					};

					context?.Experiments?.Add(experiment);
					context?.SaveChanges();

					var data = new List<Data>();

					for (int i = startRow; i < rows.Count; i++)
					{
						if (double.TryParse(rows[i][timeIndex.Value].ToString(), out double time) &&
							double.TryParse(rows[i][fzIndex.Value].ToString(), out double fz))
						{
							data.Add(new Data()
							{
								Time = time,
								Fz = fz,
								ExperementId = experiment.Id
							});
						}
						else
						{
							MessageBox.Show("Некорректный файл");
							return;
						}
					}

					context?.Data?.AddRange(data);
					context?.SaveChanges();
				}
				else
				{
					MessageBox.Show("Некорректный файл");
				}
			}
			catch (Exception)
			{

				MessageBox.Show("Ошибка!");
			}
		}        
    }
}
