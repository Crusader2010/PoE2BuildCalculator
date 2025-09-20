using Domain;
using Manager;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;

namespace PoE2BuildCalculator
{
    public partial class Compute : Form
    {
        private readonly List<Tier> _tiers = [];
        private readonly BindingList<Tier> _bindingTiers = [];
        private int _minFormSize;

        private readonly IReadOnlyList<ItemStatsHelper.StatDescriptor> _itemStatsDescriptors;
        private readonly DataGridViewCellStyle _defaultCellStyle = new()
        {
            Alignment = DataGridViewContentAlignment.MiddleCenter,
            DataSourceNullValue = string.Empty,
            NullValue = string.Empty,
            FormatProvider = System.Globalization.CultureInfo.InvariantCulture
        };

        private double _totalTierWeight => _tiers.Sum(t => t.TierWeight);

        public Compute()
        {
            _itemStatsDescriptors = ItemStatsHelper.GetStatDescriptors();
            InitializeComponent();

            this.Load += Compute_Load;
            this.FormClosing += Compute_FormClosing; // Add this line
            this.TableTiers.CellValidating += TableTiers_CellValidating;
        }

        private void Compute_Load(object sender, EventArgs e)
        {
            this.AutoSize = false; // Turn off AutoSize so the form can be sized by code.
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            InitializeTableTiers();
            InitializeTableStatsWeightSum();
            SetTotalTierWeights();

            _minFormSize = ComputeMinRequiredFormHeight();
            AdjustFormSizeToDataGrid();
        }

        private void AddTierButton_Click(object sender, EventArgs e)
        {
            var newTier = new Tier
            {
                TierId = TableTiers.Rows.Count + 1,
                TierName = $"Tier {TableTiers.Rows.Count + 1}",
                TierWeight = 0.0d,
                StatWeights = new Dictionary<string, double>()
            };

            // Initialize StatWeights with 0 for all possible stats
            foreach (var descriptor in _itemStatsDescriptors)
            {
                newTier.StatWeights.Add(descriptor.PropertyName, 0.0d);
            }

            _tiers.Add(newTier);
            _bindingTiers.Add(newTier);

            TableTiers.SuspendLayout();
            string[] rowValues = new string[TableTiers.Columns.Count];
            rowValues[0] = newTier.TierId.ToString();
            rowValues[1] = newTier.TierName;
            rowValues[2] = newTier.TierWeight.ToString("0.00");
            for (int i = 3; i < rowValues.Length; i++)
            {
                rowValues[i] = "0.00";
            }

            // Add the row by passing the values directly. This is the most reliable method.
            TableTiers.Rows.Add(rowValues);
            TableTiers.ResumeLayout(true);
        }

        private void TableTiers_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (!ValidateAndCommitTierData(e.RowIndex, e.ColumnIndex, e.FormattedValue, TableTiers, true))
            {
                e.Cancel = true; // Cancel the event if validation fails.
            }
        }

        private void TableTiers_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            var index = e.Row.Index;
            _tiers.RemoveAt(index);
            _bindingTiers.RemoveAt(index);
            RefreshTierIds();
        }

        private void TableTiers_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex <= 0) return; // Ignore header changes and TierId column

            if (TableTiers.Columns[e.ColumnIndex].Name == nameof(Tier.TierName))
            {
                var row = TableTiers.Rows[e.RowIndex];
                var tier = _tiers[e.RowIndex];
                tier.TierName = row.Cells[nameof(Tier.TierName)].FormattedValue?.ToString() ?? string.Empty;
                tier.TriggerPropertyChange(nameof(Tier.TierName));
            }
        }

        private void RemoveTierButton_Click(object sender, EventArgs e)
        {
            RemoveSelectedGridRows();
        }

        private void TableTiers_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            // Make sure we are in a valid cell and not a header
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            // Check if the current cell is the selected one
            if ((e.State & DataGridViewElementStates.Selected) == DataGridViewElementStates.Selected &&
                TableTiers.CurrentCell.RowIndex == e.RowIndex &&
                TableTiers.CurrentCell.ColumnIndex == e.ColumnIndex)
            {
                // Define your custom colors
                Color selectedBackColor = Color.SpringGreen;
                Color selectedForeColor = Color.IndianRed;

                // Erase the default background
                e.PaintBackground(e.ClipBounds, false);

                // Draw the custom background
                using (SolidBrush backBrush = new(selectedBackColor))
                {
                    e.Graphics.FillRectangle(backBrush, e.CellBounds);
                }

                // Create a StringFormat object for alignment
                StringFormat stringFormat = new()
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center,
                    Trimming = StringTrimming.EllipsisCharacter
                };

                // Now, draw the cell content (text, etc.)
                using (SolidBrush foreBrush = new(selectedForeColor))
                {
                    e.Graphics.DrawString(e.FormattedValue.ToString(), e.CellStyle.Font, foreBrush, e.CellBounds, stringFormat);
                }

                // Prevent the default cell painting
                e.Handled = true;
            }
            else
            {
                // Let the default painting occur for non-selected cells
                e.Handled = false;
            }
        }

        private void Compute_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Detach the event handler to prevent validation during form closure
            this.TableTiers.CellValidating -= TableTiers_CellValidating;
        }

        private void TableTiers_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
        }





        #region Private helpers

        internal bool ValidateAndCommitTierData(int rowIndex, int colIndex, object formattedValue, DataGridView grid, bool performCellValueFormat)
        {
            // Skip validation for TierId and TierName columns
            if (colIndex <= 1) return true;

            // Validate numeric columns
            if (!double.TryParse(formattedValue?.ToString() ?? "0.00", out double value))
            {
                MessageBox.Show("Please enter a valid floating point number.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            else if (value < 0.00d || value > 100.00d)
            {
                MessageBox.Show("The value of the weight needs to be between 0.00 and 100.00", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // Update the cell value with decimals even if the user didn't type them
            if (performCellValueFormat)
            {
                SetFormattedCellValue(grid, rowIndex, colIndex, value);
            }

            var tier = _tiers[rowIndex];
            if (colIndex == grid.Columns[nameof(Tier.TierWeight)].Index)
            {
                if (_totalTierWeight - tier.TierWeight + value > 100.00d)
                {
                    MessageBox.Show("The total value of tier weights cannot exceed 100%", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                tier.TierWeight = value;
                tier.TriggerPropertyChange(nameof(Tier.TierWeight));
                SetTotalTierWeights();
            }
            else
            {
                var columnName = grid.Columns[colIndex].Name;
                if (tier.TotalStatWeight - tier.StatWeights[columnName] + value > 100.00d)
                {
                    MessageBox.Show($"The total value of stats' weights for '{tier.TierName}' (ID: {tier.TierId}) cannot exceed 100%", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                tier.StatWeights[columnName] = value;
                tier.TriggerPropertyChange(nameof(Tier.TotalStatWeight));
            }

            return true; // Validation and commit successful
        }

        private void RefreshTierIds()
        {
            for (int i = 0; i < TableTiers.Rows.Count; i++)
            {
                TableTiers.Rows[i].Cells[nameof(Tier.TierId)].Value = i + 1;
                if (i < _tiers.Count)
                {
                    _tiers[i].TierId = i + 1;
                    _tiers[i].TriggerPropertyChange(nameof(Tier.TierId));
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
            int offset = 136;

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
                var tiersDictionary = _tiers.ToDictionary(x => x.TierId, x => x);

                for (int i = TableTiers.SelectedRows.Count - 1; i >= 0; i--) // must be in reverse for list update
                {
                    var row = TableTiers.SelectedRows[i];
                    var tierId = int.Parse(row.Cells[nameof(Tier.TierId)].Value?.ToString() ?? "0");

                    _tiers.Remove(tiersDictionary[tierId]);
                    _bindingTiers.Remove(tiersDictionary[tierId]);
                    TableTiers.Rows.Remove(row);
                }

                RefreshTierIds();
                TableTiers.ClearSelection();
            }
        }

        private void InitializeTableTiers()
        {
            TableTiers.SuspendLayout();

            // Configure the DataGridView
            TableTiers.AutoGenerateColumns = false;
            TableTiers.DataSource = null;
            TableTiers.Columns.Clear();
            TableTiers.Rows.Clear();
            TableTiers.AllowUserToAddRows = false;
            TableTiers.AllowUserToDeleteRows = false;
            TableTiers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            TableTiers.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;
            TableTiers.ShowEditingIcon = true;
            TableTiers.MultiSelect = true;
            TableTiers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            TableTiers.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            TableTiers.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            TableTiers.DefaultCellStyle.FormatProvider = System.Globalization.CultureInfo.InvariantCulture;
            TableTiers.RowTemplate.DefaultCellStyle.FormatProvider = System.Globalization.CultureInfo.InvariantCulture;

            // Add basic columns
            var idColumn = new DataGridViewTextBoxColumn
            {
                Name = nameof(Tier.TierId),
                HeaderText = "Tier ID",
                //DataPropertyName = "TierId",
                ValueType = typeof(string),
                ReadOnly = true,
                DefaultCellStyle = _defaultCellStyle
            };

            var nameColumn = new DataGridViewTextBoxColumn
            {
                Name = nameof(Tier.TierName),
                HeaderText = "Tier Name",
                //DataPropertyName = "TierName",
                ValueType = typeof(string),
                ReadOnly = false,
                DefaultCellStyle = _defaultCellStyle,
                HeaderCell = { ToolTipText = "Tier name" },
            };

            var weightColumn = new DataGridViewTextBoxColumn
            {
                Name = nameof(Tier.TierWeight),
                HeaderText = "Tier Weight",
                //DataPropertyName = "TierWeight",
                ValueType = typeof(string),
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
                    ValueType = typeof(string),
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
        }

        private void InitializeTableStatsWeightSum()
        {
            TableStatsWeightSum.SuspendLayout();

            TableStatsWeightSum.AutoGenerateColumns = false;
            TableStatsWeightSum.DataSource = null;
            TableStatsWeightSum.Columns.Clear();
            TableStatsWeightSum.Rows.Clear();
            TableStatsWeightSum.AutoSize = false;
            TableStatsWeightSum.AllowUserToAddRows = false;
            TableStatsWeightSum.AllowUserToDeleteRows = false;
            TableStatsWeightSum.SelectionMode = DataGridViewSelectionMode.CellSelect;
            TableStatsWeightSum.ReadOnly = true;
            TableStatsWeightSum.MultiSelect = false;
            TableStatsWeightSum.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            TableStatsWeightSum.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Raised;
            TableStatsWeightSum.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            TableStatsWeightSum.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedHeaders;
            TableStatsWeightSum.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
            TableStatsWeightSum.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            TableStatsWeightSum.DefaultCellStyle.FormatProvider = System.Globalization.CultureInfo.InvariantCulture;
            TableStatsWeightSum.RowTemplate.DefaultCellStyle.FormatProvider = System.Globalization.CultureInfo.InvariantCulture;

            var tierIdColumn = new DataGridViewTextBoxColumn
            {
                Name = nameof(Tier.TierId),
                HeaderText = "Tier Id",
                ValueType = typeof(string),
                ReadOnly = true,
                DataPropertyName = nameof(Tier.TierId),
                DefaultCellStyle = _defaultCellStyle
            };

            var weightColumn = new DataGridViewTextBoxColumn
            {
                Name = nameof(Tier.TotalStatWeight),
                HeaderText = "Total Stat Weight (%)",
                ValueType = typeof(string),
                ReadOnly = true,
                DataPropertyName = nameof(Tier.TotalStatWeight),
                DefaultCellStyle = _defaultCellStyle
            };
            TableStatsWeightSum.Columns.AddRange([tierIdColumn, weightColumn]);

            TableStatsWeightSum.DataSource = _bindingTiers;
            TableStatsWeightSum.ResumeLayout(true);
            TableStatsWeightSum.Update();
        }

        private void SetTotalTierWeights()
        {
            TextboxTotalTierWeights.Text = _totalTierWeight.ToString("0.00") + " %";
        }

        internal static void SetFormattedCellValue(DataGridView grid, int rowIndex, int colIndex, object unformattedValue)
        {
            if (rowIndex < 0 || rowIndex >= grid.Rows.Count) return;
            if (colIndex < 0 || colIndex >= grid.Columns.Count) return;

            grid.SuspendLayout();

            var cell = grid.Rows[rowIndex].Cells[colIndex];
            if (cell.ValueType != typeof(string))
            {
                cell.Value = ItemStatsHelper.ConvertToType(cell.ValueType, unformattedValue); // set the value directly
            }
            else
            {
                cell.Value = Convert.ToDouble(unformattedValue).ToString("0.00"); // force double value with decimals
            }

            grid.UpdateCellValue(colIndex, rowIndex);
            grid.ResumeLayout(true);
        }

        #endregion

        internal class CustomDataGridView : DataGridView
        {
            protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
            {
                if (keyData == Keys.Enter || keyData == Keys.Return)
                {
                    if (this.IsCurrentCellInEditMode)
                    {
                        // Find the parent form to call its validation method
                        if (this.FindForm() is Compute computeForm)
                        {
                            var cell = this.CurrentCell;
                            // Get the text directly from the editing control
                            var editedValue = this.EditingControl.Text;

                            // Manually invoke the form's validation logic.
                            // If it passes, we then commit the edit.
                            // If it fails, the message box is shown and we do nothing, which leaves the cell in edit mode.
                            if (computeForm.ValidateAndCommitTierData(cell.RowIndex, cell.ColumnIndex, editedValue, this, false))
                            {
                                this.EndEdit();
                                SetFormattedCellValue(this, cell.RowIndex, cell.ColumnIndex, editedValue); // Update the cell value with decimals even if the user didn't type them
                            }
                        }
                        else
                        {
                            // Fallback to default behavior if form isn't found for some reason
                            this.EndEdit();
                        }

                        return true; // The Enter key has been handled.
                    }
                    else if (this.CurrentCell != null)
                    {
                        this.BeginEdit(true);
                        return true;
                    }
                }

                // Allow other keys to be processed normally
                return base.ProcessCmdKey(ref msg, keyData);
            }
        }
    }
}