using Domain;
using Manager;
using System.Data;

namespace PoE2BuildCalculator
{
    public partial class Compute : Form
    {
        private readonly List<Tier> _tiers = [];
        private readonly IReadOnlyList<ItemStatsHelper.StatDescriptor> _itemStatsDescriptors;
        private int _minFormSize;
        private readonly DataGridViewCellStyle _defaultCellStyle = new()
        {
            Format = "0.00",
            Alignment = DataGridViewContentAlignment.MiddleCenter,
            DataSourceNullValue = 0.0d,
            NullValue = 0.0d,
            FormatProvider = System.Globalization.CultureInfo.InvariantCulture
        };

        public Compute()
        {
            _itemStatsDescriptors = ItemStatsHelper.GetStatDescriptors();

            InitializeComponent();
            InitializeCustomComponents();
            this.Load += Compute_Load;
        }

        private void Compute_Load(object sender, EventArgs e)
        {
            this.AutoSize = false; // Turn off AutoSize so the form can be sized by code.
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            _minFormSize = ComputeMinRequiredFormHeight();
            AdjustFormSizeToDataGrid();
        }

        private void AddTierButton_Click(object sender, EventArgs e)
        {
            TableTiers.DataSource = null;
            var newTier = new Tier
            {
                TierId = TableTiers.Rows.Count + 1,
                TierName = string.Empty,
                TierWeight = 0,
                StatWeights = new Dictionary<string, double>()
            };

            // Initialize StatWeights with 0 for all possible stats
            foreach (var descriptor in _itemStatsDescriptors)
            {
                newTier.StatWeights.Add(descriptor.PropertyName, 0.0);
            }

            _tiers.Add(newTier);
            TableTiers.SuspendLayout();

            object[] rowValues = new object[TableTiers.Columns.Count];
            rowValues[0] = newTier.TierId;
            rowValues[1] = newTier.TierName;
            rowValues[2] = newTier.TierWeight;
            for (int i = 3; i < rowValues.Length; i++)
            {
                rowValues[i] = 0.0d;
            }

            // Add the row by passing the values directly. This is the most reliable method.
            TableTiers.Rows.Add(rowValues);

            TableTiers.ResumeLayout(true);
            TableTiers.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            //AdjustFormSizeToDataGrid();
        }

        private void TableTiers_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.ColumnIndex <= 1) return; // Skip validation for TierId and TierName columns

            // Validate numeric columns (TierWeight and StatWeights)
            if (!double.TryParse(e.FormattedValue.ToString(), out _))
            {
                e.Cancel = true;
                MessageBox.Show("Please enter a valid floating point number.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void TableTiers_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            var index = e.Row.Index;
            _tiers.RemoveAt(index);
            RefreshTierIds();
        }

        private void TableTiers_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void RemoveTierButton_Click(object sender, EventArgs e)
        {
            RemoveSelectedGridRows();
        }

        private void TableTiers_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
        }

        #region Private helpers

        private void RefreshTierIds()
        {
            for (int i = 0; i < TableTiers.Rows.Count; i++)
            {
                TableTiers.Rows[i].Cells["TierId"].Value = i + 1;
                if (i < _tiers.Count)
                {
                    _tiers[i].TierId = i + 1;
                }
            }
        }

        public List<Tier> GetTiers()
        {
            return [.. _tiers.OrderBy(t => t.TierId)];
        }

        private void AdjustFormSizeToDataGrid()
        {
            try
            {
                if (TableTiers.Columns.Count == 0) return;

                // Ensure layout is up-to-date so column widths are valid.
                TableTiers.PerformLayout();
                TableTiers.Update();

                // Compute required client size for the grid (width = row header + all column widths; height = column header + sum of row heights).
                int requiredWidth = TableTiers.RowHeadersVisible ? TableTiers.RowHeadersWidth : 0;
                foreach (DataGridViewColumn col in TableTiers.Columns)
                    requiredWidth += col.Width;

                int requiredHeight = TableTiers.ColumnHeadersHeight;
                // Sum the heights of rows (handles variable row heights)
                foreach (DataGridViewRow row in TableTiers.Rows)
                    requiredHeight += row.Height;

                // Small padding so borders/lines aren't cut off.
                const int gridPadding = 6;
                requiredWidth += gridPadding;
                requiredHeight += gridPadding;

                // Convert grid client size to a desired form size by adding non-client chrome.
                int chromeWidth = this.Width - this.ClientSize.Width;
                int chromeHeight = this.Height - this.ClientSize.Height;

                int desiredFormWidth = requiredWidth + chromeWidth;
                int desiredFormHeight = requiredHeight + chromeHeight;

                // Clamp to screen working area with a margin so form isn't fullscreen or off-screen.
                Rectangle wa = Screen.GetWorkingArea(this);
                const int margin = 36;
                int maxWidth = Math.Max(wa.Width - margin, 200);
                int maxHeight = Math.Max(wa.Height - margin, 100);

                if (desiredFormHeight < _minFormSize) desiredFormHeight = _minFormSize; // adjust according to other controls too
                desiredFormWidth = Math.Min(desiredFormWidth, maxWidth);
                desiredFormHeight = Math.Min(desiredFormHeight, maxHeight);

                // Apply new size
                this.Size = new Size(desiredFormWidth, desiredFormHeight);

                // Ensure form is still on-screen; nudge if necessary.
                int newX = Math.Min(this.Location.X, wa.Right - this.Width - 10);
                int newY = Math.Min(this.Location.Y, wa.Bottom - this.Height - 10);
                newX = Math.Max(newX, wa.Left + 10);
                newY = Math.Max(newY, wa.Top + 10);
                this.Location = new Point(newX, newY);
            }
            catch (Exception ex)
            {
                ErrorHelper.ShowError(ex, $"{nameof(Compute)} - {nameof(AdjustFormSizeToDataGrid)}");
                throw;
            }
        }

        private int ComputeMinRequiredFormHeight()
        {
            int maxY = 0;
            int offset = 36;

            if (this.Controls.Count > 0)
            {
                foreach (Control control in this.Controls)
                {
                    int height = control.Location.Y + control.Height + control.Margin.Bottom + control.Margin.Top + offset;
                    if (height > maxY) maxY = height;
                }
            }

            return maxY;
        }

        private void RemoveSelectedGridRows()
        {
            if (TableTiers.SelectedRows.Count > 0)
            {
                for (int i = 0; i < TableTiers.SelectedRows.Count; i++)
                {
                    TableTiers.Rows.Remove(TableTiers.SelectedRows[i]);
                }

                RefreshTierIds();
                TableTiers.ClearSelection();
            }

        }

        private void InitializeCustomComponents()
        {
            // Clear any existing columns/rows and ensure base columns exist.
            TableTiers.Columns.Clear();
            TableTiers.Rows.Clear();

            // Configure the DataGridView
            TableTiers.AllowUserToAddRows = false;
            TableTiers.AllowUserToDeleteRows = false;
            TableTiers.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;
            TableTiers.ShowEditingIcon = true;
            TableTiers.MultiSelect = true;
            TableTiers.AutoGenerateColumns = false;
            TableTiers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            TableTiers.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            TableTiers.SelectionMode = DataGridViewSelectionMode.CellSelect;
            TableTiers.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            TableTiers.DefaultCellStyle.FormatProvider = System.Globalization.CultureInfo.InvariantCulture;
            TableTiers.RowTemplate.DefaultCellStyle.FormatProvider = System.Globalization.CultureInfo.InvariantCulture;

            TableTiers.SuspendLayout();

            // Add basic columns
            var idColumn = new DataGridViewTextBoxColumn
            {
                Name = "TierId",
                HeaderText = "Tier ID",
                //DataPropertyName = "TierId",
                ValueType = typeof(int),
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "0", Alignment = DataGridViewContentAlignment.MiddleCenter, FormatProvider = System.Globalization.CultureInfo.InvariantCulture }
            };

            var nameColumn = new DataGridViewTextBoxColumn
            {
                Name = "TierName",
                HeaderText = "Tier Name",
                //DataPropertyName = "TierName",
                ValueType = typeof(string),
                ReadOnly = false,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter, DataSourceNullValue = string.Empty, NullValue = string.Empty, FormatProvider = System.Globalization.CultureInfo.InvariantCulture },
                HeaderCell = { ToolTipText = "Tier name" }
            };

            var weightColumn = new DataGridViewTextBoxColumn
            {
                Name = "TierWeight",
                HeaderText = "Tier Weight",
                //DataPropertyName = "TierWeight",
                ValueType = typeof(double),
                ReadOnly = false,
                DefaultCellStyle = _defaultCellStyle,
                HeaderCell = { ToolTipText = "Weight of the tier" }
            };

            TableTiers.Columns.AddRange([idColumn, nameColumn, weightColumn]);

            // Add columns for StatWeights if we have any item stats
            foreach (var descriptor in _itemStatsDescriptors)
            {
                var statColumn = new DataGridViewTextBoxColumn
                {
                    Name = descriptor.PropertyName,
                    HeaderText = descriptor.Header,
                    //DataPropertyName = $"StatWeights[{descriptor.PropertyName}]",
                    ValueType = typeof(double),
                    ReadOnly = false,
                    DefaultCellStyle = _defaultCellStyle,
                    HeaderCell = { ToolTipText = descriptor.Header }
                };
                TableTiers.Columns.Add(statColumn);
            }

            // Add context menu for row creation and deletion
            var contextMenu = new ContextMenuStrip();
            var deleteItem = new ToolStripMenuItem("Delete Selected Tiers");
            var addItem = new ToolStripMenuItem("Add New Tier");
            deleteItem.Click += (s, e) =>
            {
                RemoveSelectedGridRows();
            };

            addItem.Click += (s, e) =>
            {
                AddTierButton_Click(s, e);
            };

            contextMenu.Items.AddRange([addItem, deleteItem]);
            TableTiers.ContextMenuStrip = contextMenu;

            TableTiers.ResumeLayout(true);
            AdjustFormSizeToDataGrid();
        }

        #endregion
    }
}