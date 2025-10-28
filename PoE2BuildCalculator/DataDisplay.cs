using System.Collections.Immutable;

using Domain.HelperForms;
using Domain.Helpers;
using Domain.Main;

namespace PoE2BuildCalculator
{
	public partial class DataDisplay : BaseForm
	{
		private ImmutableList<Item> _itemsToDisplay = [];
		private IReadOnlyList<ItemStatsHelper.StatDescriptor> _descriptors;
		private List<object[]> _cachedRows;
		private const int BaseColumnCount = 4;

		public DataDisplay(ImmutableList<Item> itemsToDisplay)
		{
			InitializeComponent();
			this.Load += DataDisplay_Load;
			if (itemsToDisplay?.Count > 0) _itemsToDisplay = itemsToDisplay;
		}

		private void DataDisplay_Load(object sender, EventArgs e)
		{
			ConfigureGrid();
			SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
			UpdateStyles();
			ImportDataToDisplay_Click(sender, e);
		}

		private void ConfigureGrid()
		{
			TableDisplayData.SuspendLayout();

			TableDisplayData.VirtualMode = true;
			TableDisplayData.ReadOnly = true;
			TableDisplayData.AllowUserToAddRows = false;
			TableDisplayData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
			TableDisplayData.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
			TableDisplayData.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			TableDisplayData.RowHeadersVisible = false;
			TableDisplayData.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
			TableDisplayData.ColumnHeadersHeight = 25;
			TableDisplayData.RowTemplate.Height = 22;
			TableDisplayData.AllowUserToResizeRows = false;

			TableDisplayData.CellValueNeeded += TableDisplayData_CellValueNeeded;

			TableDisplayData.ResumeLayout();
		}

		private async void ImportDataToDisplay_Click(object sender, EventArgs e)
		{
			if (_itemsToDisplay?.Count == 0)
			{
				CustomMessageBox.Show("No parsed items to display.", "Missing parsed items");
				return;
			}

			this.Cursor = Cursors.WaitCursor;

			try
			{
				await Task.Run(() =>
				{
					_descriptors = ItemStatsHelper.GetStatDescriptors();
					BuildColumns();
					CacheRowData();
				});

				TableDisplayData.RowCount = _cachedRows.Count;
				ResizeColumnsToContent();
			}
			catch (Exception ex)
			{
				ErrorHelper.ShowError(ex, $"{nameof(DataDisplay)} - {nameof(ImportDataToDisplay_Click)}");
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void BuildColumns()
		{
			if (TableDisplayData.InvokeRequired)
			{
				TableDisplayData.Invoke(BuildColumns);
				return;
			}

			TableDisplayData.Columns.Clear();

			// Base columns
			TableDisplayData.Columns.Add(new DataGridViewTextBoxColumn
			{
				Name = "Id",
				HeaderText = "Id",
				ValueType = typeof(int),
				Width = 50,
				DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
			});

			TableDisplayData.Columns.Add(new DataGridViewTextBoxColumn
			{
				Name = "Name",
				HeaderText = "Name",
				ValueType = typeof(string),
				Width = 150
			});

			TableDisplayData.Columns.Add(new DataGridViewTextBoxColumn
			{
				Name = "Class",
				HeaderText = "Class",
				ValueType = typeof(string),
				Width = 100
			});

			TableDisplayData.Columns.Add(new DataGridViewTextBoxColumn
			{
				Name = "Mine",
				HeaderText = "Mine?",
				ValueType = typeof(string),
				Width = 50,
				DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
			});

			// Stat columns
			foreach (var d in _descriptors)
			{
				TableDisplayData.Columns.Add(new DataGridViewTextBoxColumn
				{
					Name = d.PropertyName,
					HeaderText = d.Header,
					ValueType = d.PropertyType,
					Width = d.PropertyType == typeof(string) ? 100 : 70,
					DefaultCellStyle = new DataGridViewCellStyle
					{
						Alignment = d.PropertyType == typeof(string)
							? DataGridViewContentAlignment.MiddleLeft
							: DataGridViewContentAlignment.MiddleRight,
						Format = d.PropertyType == typeof(double) ? "F2" : null
					}
				});
			}
		}

		private void CacheRowData()
		{
			_cachedRows = new List<object[]>(_itemsToDisplay.Count);

			foreach (var item in _itemsToDisplay)
			{
				var statsMap = ItemStatsHelper.ToDictionary(item.ItemStats);
				var rowValues = new object[BaseColumnCount + _descriptors.Count];

				rowValues[0] = item.Id;
				rowValues[1] = item.Name;
				rowValues[2] = item.Class;
				rowValues[3] = item.IsMine ? "YES" : "NO";

				for (int i = 0; i < _descriptors.Count; i++)
				{
					var d = _descriptors[i];
					rowValues[BaseColumnCount + i] = statsMap.TryGetValue(d.PropertyName, out var v)
						? v
						: (d.PropertyType == typeof(string) ? string.Empty : ItemStatsHelper.ConvertToType(d.PropertyType, 0));
				}

				_cachedRows.Add(rowValues);
			}
		}

		private void TableDisplayData_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
		{
			if (e.RowIndex < 0 || e.RowIndex >= _cachedRows.Count || e.ColumnIndex < 0) return;
			e.Value = _cachedRows[e.RowIndex][e.ColumnIndex];
		}

		private void ResizeColumnsToContent()
		{
			if (TableDisplayData.InvokeRequired)
			{
				TableDisplayData.Invoke(ResizeColumnsToContent);
				return;
			}

			// Sample first 100 rows for sizing
			int sampleSize = Math.Min(100, _cachedRows.Count);
			for (int col = 0; col < TableDisplayData.Columns.Count; col++)
			{
				int maxWidth = TableDisplayData.Columns[col].Width;

				using var g = TableDisplayData.CreateGraphics();
				var headerWidth = (int)g.MeasureString(TableDisplayData.Columns[col].HeaderText, TableDisplayData.Font).Width + 20;
				maxWidth = Math.Max(maxWidth, headerWidth);

				for (int row = 0; row < sampleSize; row++)
				{
					var value = _cachedRows[row][col]?.ToString() ?? "";
					var cellWidth = (int)g.MeasureString(value, TableDisplayData.Font).Width + 20;
					maxWidth = Math.Max(maxWidth, cellWidth);
				}

				TableDisplayData.Columns[col].Width = Math.Min(maxWidth, 300);
			}
		}

		private void ButtonClose_Click(object sender, EventArgs e)
		{
			this.Close();
			this.Dispose();
		}
	}
}
