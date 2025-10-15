using System.ComponentModel;
using System.Reflection;

using Domain.Main;

namespace Domain.UserControls
{
    public partial class ItemStatGroupValidatorUserControl(int groupId, string groupName) : UserControl
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
        private readonly BindingList<ItemStatRow> _statRows = [];
        private readonly int _groupId = groupId;
        private readonly string _groupName = groupName;

        private readonly HashSet<string> _usedStats = new(StringComparer.OrdinalIgnoreCase);

        public event EventHandler DeleteRequested;
        public event EventHandler ValidationChanged;

        private void ItemStatGroupValidatorUserControl_Load(object sender, EventArgs e)
        {
            InitializeComponent();
            InitializeComboBoxCache();
            UpdateStatsComboBox();

            // Subscribe to data changes
            _statRows.ListChanged += Stats_ListChanged;
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
            ComboboxItemStats.BeginUpdate();
            try
            {
                ComboboxItemStats.Items.Clear();

                // Use LINQ for efficient filtering and ordering
                var availableItems = _availableProperties.Value
                    .Where(p => !_usedStats.Contains(p.Name))
                    .Select(p => p.Name)
                    .ToArray();

                ComboboxItemStats.Items.AddRange(availableItems);
            }
            finally
            {
                ComboboxItemStats.EndUpdate();
            }
        }

        private void ButtonDeleteGroup_Click(object sender, EventArgs e)
        {
            DeleteRequested?.Invoke(this, EventArgs.Empty);
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

                var itemStatRow = new ItemStatRow(_statRows.Count + 1, statName, this);
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

        public void RemoveStatRow(ItemStatRow row)
        {
            if (row == null) return;

            _usedStats.Remove(row._selectedStatName);
            _statRows.Remove(row); // Triggers flow layout update via ListChanged event

            for (int i = 0; i < _statRows.Count; i++)
            {
                _statRows[i].ChangeCurrentRowIndex(i);
                _statRows[i].SetupStatOperatorSelection(i != _statRows.Count - 1);
            }

            UpdateStatsComboBox();
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
            UpdateStatsComboBox();
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
        }

        public void SwapStats(int index1, int index2)
        {
            if (index1 < 0 || index1 >= _statRows.Count ||
                index2 < 0 || index2 >= _statRows.Count ||
                index1 == index2) return;

            try
            {
                // Swap in the data source
                (_statRows[index1], _statRows[index2]) = (_statRows[index2], _statRows[index1]);

                // Swap in UI - need TWO SetChildIndex calls for proper swap
                FlowPanelStats.SuspendLayout();
                int lowerIndex = Math.Min(index1, index2);
                int higherIndex = Math.Max(index1, index2);

                var lowerControl = FlowPanelStats.Controls[lowerIndex] as ItemStatRow;
                var higherControl = FlowPanelStats.Controls[higherIndex] as ItemStatRow;

                // Move higher control to lower position first
                FlowPanelStats.Controls.SetChildIndex(higherControl, lowerIndex);

                // Move lower control to higher position
                FlowPanelStats.Controls.SetChildIndex(lowerControl, higherIndex);
                FlowPanelStats.ResumeLayout(true);
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
    }
}
