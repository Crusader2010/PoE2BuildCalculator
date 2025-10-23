using System.Collections.Immutable;
using System.ComponentModel;
using System.Reflection;

using Domain.Events;
using Domain.Helpers;
using Domain.Serialization;
using Domain.Validation;

namespace Domain.UserControls
{
	public partial class ItemStatGroupValidatorUserControl : UserControl
	{
		private static readonly Color HEADER_COLOR = Color.FromArgb(100, 160, 210);

		public event EventHandler GroupDeleted;
		public Group _group { get; private set; }

		// Cached data - static to share across all instances
		private static readonly Lazy<ImmutableDictionary<string, PropertyInfo>> _availableProperties = new(() =>
		{
			return ItemStatsHelper.GetStatDescriptors()
				.Where(p => p.PropertyType == typeof(int) ||
							p.PropertyType == typeof(double) ||
							p.PropertyType == typeof(long))
				.OrderBy(p => p.PropertyName)
				.ToImmutableDictionary(x => x.PropertyName, x => x.Property);
		});

		private readonly BindingList<ItemStatRow> _statRows = [];
		private readonly HashSet<string> _usedStats = new(StringComparer.OrdinalIgnoreCase);
		private bool _needsRefresh = false;

		public ItemStatGroupValidatorUserControl(int groupId, string groupName)
		{
			_group = new Group()
			{
				GroupId = groupId,
				GroupName = groupName,
				Stats = []
			};

			InitializeComponent();  // MUST be called here!
			_needsRefresh = true;

			// Set the group name in the label
			lblGroupName.Text = groupName;

			// Subscribe to data changes
			_statRows.ListChanged += Stats_ListChanged;

			this.Padding = new Padding(0);
			this.Margin = new Padding(0, 0, 2, 2);
		}

		private void ItemStatGroupValidatorUserControl_Load(object sender, EventArgs e)
		{
			this.SuspendLayout();
			headerPanel.BackColor = HEADER_COLOR;
			lblGroupName.DataBindings.Add("Text", _group, nameof(_group.GroupName));

			// Enable double buffering for smoother rendering
			SetStyle(ControlStyles.OptimizedDoubleBuffer |
					 ControlStyles.AllPaintingInWmPaint |
					 ControlStyles.UserPaint, true);
			UpdateStyles();
			this.ResumeLayout();

			if (_availableProperties.Value != null) return; // Force initialization of static cached data
		}

		private void UpdateStatsComboBox()
		{
			if (_needsRefresh)
			{
				ComboboxItemStats.BeginUpdate();
				try
				{
					ComboboxItemStats.Items.Clear();

					var availableItems = _availableProperties.Value
						.Where(p => !_usedStats.Any(k => string.Equals(k, p.Key, StringComparison.OrdinalIgnoreCase)))
						.Select(p => p.Key)
						.OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
						.ToArray();

					if (availableItems.Length > 0) ComboboxItemStats.Items.AddRange(availableItems);
				}
				finally
				{
					_needsRefresh = false;
					ComboboxItemStats.EndUpdate();
				}
			}
		}

		private void ButtonDeleteGroup_Click(object sender, EventArgs e)
		{
			GroupDeleted?.Invoke(this, EventArgs.Empty);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				components?.Dispose();
			}
			base.Dispose(disposing);
		}

		private void AddNewStatRow(string statName)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(statName)) return;

				var itemStatRow = new ItemStatRow(_statRows.Count, statName);
				itemStatRow.ItemStatRowDeleted += RemoveStatRow;
				itemStatRow.ItemStatRowSwapped += SwapStats;
				itemStatRow.OperatorChanged += (s, e) => ReCompileStatsForGroup();

				foreach (var statRow in FlowPanelStats.Controls.OfType<ItemStatRow>())
				{
					statRow.SetupStatOperatorSelection(true);
				}

				// Triggers flow layout update via ListChanged event
				_statRows.Add(itemStatRow); // stat operator combo is disabled by default, so we add the new row after enabling existing ones
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void RemoveStatRow(object sender, EventArgs args)
		{
			if (sender == null || sender is not ItemStatRow row) return;

			_usedStats.Remove(row._selectedStatName);
			_statRows.Remove(row); // Triggers flow layout update via ListChanged event

			for (int i = 0; i < _statRows.Count; i++)
			{
				_statRows[i].ChangeCurrentRowIndex(i);
				_statRows[i].SetupStatOperatorSelection(i != _statRows.Count - 1);
			}

			_needsRefresh = true;
		}

		private void ButtonAddItemStat_Click(object sender, EventArgs e)
		{
			if (ComboboxItemStats.SelectedItem is null)
			{
				MessageBox.Show("Please select a stat.", "Missing item stat", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			string propName = ComboboxItemStats.SelectedItem.ToString();
			_usedStats.Add(propName);

			int selectedIndex = ComboboxItemStats.SelectedIndex;
			ComboboxItemStats.Items.RemoveAt(selectedIndex);
			ComboboxItemStats.SelectedIndex = -1;

			AddNewStatRow(propName);
			_needsRefresh = true;
		}

		private void Stats_ListChanged(object sender, ListChangedEventArgs e)
		{
			switch (e.ListChangedType)
			{
				case ListChangedType.ItemAdded:
					AddControlToFlowPanel(_statRows[e.NewIndex], e.NewIndex);
					break;
				case ListChangedType.ItemDeleted:
					RemoveControlFromFlowPanel(e.NewIndex);
					break;
				case ListChangedType.Reset:
					RebuildAllControlsInFlowPanel();
					break;
				default:
					break;
			}

			ReCompileStatsForGroup();
		}

		private void SwapStats(object sender, EventArgs args)
		{
			if (sender is null or not ItemStatRow) return;
			if (args is not ItemStatRowSwapEventArgs swapArgs) return;

			var indexSource = swapArgs.SourceIndex;
			var indexDestination = swapArgs.TargetIndex;

			if (indexSource < 0 || indexSource >= _statRows.Count ||
				indexDestination < 0 || indexDestination >= _statRows.Count ||
				indexSource == indexDestination) return;

			try
			{
				int lowerIndex = Math.Min(indexSource, indexDestination);
				int higherIndex = Math.Max(indexSource, indexDestination);

				// Swap in the data source
				(_statRows[indexSource], _statRows[indexDestination]) = (_statRows[indexDestination], _statRows[indexSource]);

				// Swap in UI - need TWO SetChildIndex calls for proper swap
				FlowPanelStats.SuspendLayout();

				var lowerControl = FlowPanelStats.Controls[lowerIndex] as ItemStatRow;
				var higherControl = FlowPanelStats.Controls[higherIndex] as ItemStatRow;

				// Move higher control to lower position first
				FlowPanelStats.Controls.SetChildIndex(higherControl, lowerIndex);

				// Move lower control to higher position
				FlowPanelStats.Controls.SetChildIndex(lowerControl, higherIndex);

				// Update their internal indices and operator states
				higherControl?.ChangeCurrentRowIndex(lowerIndex);
				lowerControl?.ChangeCurrentRowIndex(higherIndex);
				higherControl?.SetupStatOperatorSelection(lowerIndex != _statRows.Count - 1);
				lowerControl?.SetupStatOperatorSelection(higherIndex != _statRows.Count - 1);

				FlowPanelStats.ResumeLayout(true);
				ReCompileStatsForGroup();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void AddControlToFlowPanel(ItemStatRow stat, int index)
		{
			FlowPanelStats.SuspendLayout();
			FlowPanelStats.Controls.Add(stat);
			FlowPanelStats.Controls.SetChildIndex(stat, index);
			FlowPanelStats.ResumeLayout(true);
		}

		private void RemoveControlFromFlowPanel(int index)
		{
			if (index < 0 || index >= FlowPanelStats.Controls.Count) return;

			var control = FlowPanelStats.Controls[index];
			if (control is not ItemStatRow)
			{
				MessageBox.Show($"Control at FlowPanel's index {index} is not an ItemStatRow.", "Invalid item", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			FlowPanelStats.SuspendLayout();
			FlowPanelStats.Controls.Remove(control);
			FlowPanelStats.ResumeLayout(true);
		}

		private void RebuildAllControlsInFlowPanel()
		{
			FlowPanelStats.SuspendLayout();

			// Clear all
			foreach (var c in FlowPanelStats.Controls.OfType<ItemStatRow>())
			{
				c.Dispose();
			}

			FlowPanelStats.Controls.Clear();
			FlowPanelStats.Controls.AddRange([.. _statRows]);
			FlowPanelStats.ResumeLayout(true);
		}

		private void ReCompileStatsForGroup()
		{
			if (_group == null)
			{
				MessageBox.Show("Group is not initialized.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			_group.Stats ??= [];
			_group.Stats.Clear();
			var currentRows = FlowPanelStats.Controls.OfType<ItemStatRow>().OrderBy(r => r._currentRowIndex).ToList();

			for (int i = 0; i < currentRows.Count; i++)
			{
				var statRow = currentRows[i];
				_group.Stats.Add(new GroupStatModel
				{
					PropertyName = statRow._selectedStatName,
					PropInfo = _availableProperties.Value.TryGetValue(statRow._selectedStatName, out var dictResult) ? dictResult : null,
					Operator = i == currentRows.Count - 1 ? null : statRow._selectedOperator
				});
			}
		}

		public int GetTopRowsHeight()
		{
			return this.Height - PanelMainArea.Height - 2;
		}

		public void LoadStatsFromConfig(List<GroupStatDto> statDtos)
		{
			FlowPanelStats.SuspendLayout();
			_statRows.Clear();
			_usedStats.Clear();

			foreach (var control in FlowPanelStats.Controls.OfType<ItemStatRow>())
				control.Dispose();
			FlowPanelStats.Controls.Clear();

			var propLookup = ItemStatsHelper.GetStatDescriptors()
				.ToImmutableDictionary(d => d.PropertyName, d => d.Property, StringComparer.OrdinalIgnoreCase);

			for (int i = 0; i < statDtos.Count; i++)
			{
				var dto = statDtos[i];
				if (!propLookup.ContainsKey(dto.PropertyName)) continue;

				var row = new ItemStatRow(i, dto.PropertyName);
				row.ItemStatRowDeleted += RemoveStatRow;
				row.ItemStatRowSwapped += SwapStats;
				row.OperatorChanged += (s, e) => ReCompileStatsForGroup();

				if (!string.IsNullOrEmpty(dto.Operator) && i < statDtos.Count - 1)
				{
					row.SetupStatOperatorSelection(true);
					// Set operator after control is added to form
				}

				_statRows.Add(row);
				_usedStats.Add(dto.PropertyName);
				FlowPanelStats.Controls.Add(row);
			}

			FlowPanelStats.ResumeLayout(true);
			ReCompileStatsForGroup();
		}

		private void ComboboxItemStats_DropDown(object sender, EventArgs e)
		{
			UpdateStatsComboBox();
		}
	}
}
