using System.ComponentModel;
using System.Reflection;

using Domain.Main;
using Domain.Static;
using Domain.Validation;

namespace Domain.UserControls
{
	public partial class ItemStatGroupValidatorUserControl : UserControl
	{
		private static readonly Color HEADER_COLOR = Color.FromArgb(70, 130, 180);

		// Cached data - static to share across all instances
		private static readonly Lazy<IReadOnlyList<PropertyInfo>> _availableProperties = new(() =>
		{
			return [.. typeof(ItemStats).GetProperties()
				.Where(p => p.PropertyType == typeof(int) ||
							p.PropertyType == typeof(double) ||
							p.PropertyType == typeof(long))
				.OrderBy(p => p.Name)];
		});

		// Cache for combo box state restoration
		private readonly Dictionary<string, int> _comboBoxIndexCache = [];

		private readonly BindingSource _statRowsBindingSource = [];
		private readonly BindingList<ItemStatRow> _statRows = [];

		public event EventHandler DeleteRequested;
		public event EventHandler ValidationChanged;

		public ValidationGroupModel Group
		{
			get;
			set
			{
				field = value;
				UpdateStatsComboBox();
			}
		}

		public ItemStatGroupValidatorUserControl()
		{
			InitializeComponent();
			InitializeComboBoxCache();
		}

		private void ItemStatGroupValidatorUserControl_Load(object sender, EventArgs e)
		{
			if (Group != null)
			{
				UpdateStatsComboBox();
			}

			_statRowsBindingSource.DataSource = _statRows;
			statsListBox.DataSource = _statRowsBindingSource;
		}

		private void InitializeComboBoxCache()
		{
			for (int i = 0; i < _availableProperties.Value.Count; i++)
			{
				_comboBoxIndexCache[_availableProperties.Value[i].Name] = i;
			}
		}

		private void UpdateStatsComboBox()
		{
			var usedStats = new HashSet<string>(Group.Stats.Select(s => s.PropertyName), StringComparer.OrdinalIgnoreCase);

			cmbStats.BeginUpdate();
			try
			{
				cmbStats.Items.Clear();

				// Use LINQ for efficient filtering and ordering
				var availableItems = _availableProperties.Value
					.Where(p => !usedStats.Contains(p.Name))
					.Select(p => p.Name)
					.ToArray();

				cmbStats.Items.AddRange(availableItems);
			}
			finally
			{
				cmbStats.EndUpdate();
			}
		}

		private void SwapStatsListBoxItems(int index1, int index2)
		{
			if (index1 < 0 || index1 >= _statRows.Count ||
				index2 < 0 || index2 >= _statRows.Count ||
				index1 == index2)
			{
				return;
			}

			statsListBox.BeginUpdate();
			try
			{
				// Swap in the data source
				(_statRows[index1], _statRows[index2]) = (_statRows[index2], _statRows[index1]);
				_statRowsBindingSource.ResetBindings(false);
			}
			finally
			{
				statsListBox.EndUpdate();
			}
		}

		private void btnDelete_Click(object sender, EventArgs e)
		{
			DeleteRequested?.Invoke(this, EventArgs.Empty);
		}

		private void btnAddStat_Click(object sender, EventArgs e)
		{
			if (cmbStats.SelectedItem is null)
			{
				MessageBox.Show("Please select a stat.", "Missing item stat", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			string propName = cmbStats.SelectedItem.ToString();
			var propInfo = _availableProperties.Value.First(p => p.Name == propName);

			Group.Stats.Add(new GroupStatModel
			{
				PropInfo = propInfo,
				PropertyName = propName,
				Operator = "+"
			});

			// Remove from combo using cached index for efficiency
			int selectedIndex = cmbStats.SelectedIndex;
			cmbStats.Items.RemoveAt(selectedIndex);
			cmbStats.SelectedIndex = -1;

			AddNewStatRow(propName, statsListBox.Items.Count);
			UpdateStatsComboBox();
		}

		public void SwapStats(int index1, int index2)
		{
			(Group.Stats[index1], Group.Stats[index2]) = (Group.Stats[index2], Group.Stats[index1]);
			SwapStatsListBoxItems(index1, index2);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				components?.Dispose();
			}
			base.Dispose(disposing);
		}

		private void AddNewStatRow(string statName, int index)
		{
			if (string.IsNullOrWhiteSpace(statName)) return;

			try
			{
				var itemStatRow = new ItemStatRow(index, statName, this);

				foreach (var statRow in statsListBox.Items)
				{
					if (statRow is ItemStatRow row)
					{
						row.SetupStatOperatorSelection(true);
					}
				}

				_statRows.Add(itemStatRow); // stat operator combo is disabled by default, so we add the new row after enabling existing ones
				_statRowsBindingSource.ResetBindings(false);
				statsListBox.Refresh();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		public void RemoveStatRow(ItemStatRow row)
		{
			if (row == null) return;

			statsListBox.SuspendLayout();
			_statRows.Remove(row);
			_statRowsBindingSource.ResetBindings(false);
			statsListBox.ResumeLayout();

			for (int i = 0; i < statsListBox.Items.Count; i++)
			{
				if (statsListBox.Items[i] is ItemStatRow itemStatRow)
				{
					itemStatRow.ChangeCurrentRowIndex(i);
					itemStatRow.SetupStatOperatorSelection(i != statsListBox.Items.Count - 1);
				}
			}
		}
	}
}
