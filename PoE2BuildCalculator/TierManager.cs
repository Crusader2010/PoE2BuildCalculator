using Domain;
using Manager;
using System.ComponentModel;
using System.Data;
using TextBox = System.Windows.Forms.TextBox;
using System.Reflection;

namespace PoE2BuildCalculator
{
    public partial class TierManager : Form
    {
        private Color _validationBackColorSuccess;
        private Color _validationForeColorSuccess;
        private int _minFormSize;
        private DataGridViewCell _previousSelectedCell;

        private readonly Color _editBackColor = Color.LightGreen;
        private readonly Color _editInvalidBackColor = Color.MistyRose;
        private readonly Color _selectionColor = Color.SpringGreen;
        private readonly Color _textColor = Color.IndianRed;

        private readonly StringFormat _cellPaintingStringFormat = new()
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center,
            Trimming = StringTrimming.EllipsisCharacter
        };

        private readonly BindingList<Tier> _bindingTiers = [];
        private readonly IReadOnlyList<ItemStatsHelper.StatDescriptor> _itemStatsDescriptors;
        private readonly DataGridViewCellStyle _defaultDoubleCellStyle = new()
        {
            Format = "0.00",
            Alignment = DataGridViewContentAlignment.MiddleCenter,
            DataSourceNullValue = 0.0d,
            NullValue = 0.0d,
            FormatProvider = System.Globalization.CultureInfo.InvariantCulture
        };

        private readonly DataGridViewCellStyle _defaultStringCellStyle = new()
        {
            Alignment = DataGridViewContentAlignment.MiddleCenter,
            DataSourceNullValue = string.Empty,
            NullValue = string.Empty,
            FormatProvider = System.Globalization.CultureInfo.InvariantCulture
        };

        private double _totalTierWeight => _bindingTiers.Sum(t => t.TierWeight);

        public TierManager()
        {
            _itemStatsDescriptors = ItemStatsHelper.GetStatDescriptors();
            InitializeComponent();

            this.Load += TierManager_Load;
            this.FormClosing += TierManager_FormClosing;
            this.TableTiers.CellValidating += TableTiers_CellValidating;
            this.TableTiers.CellFormatting += TableTiers_CellFormatting;
            this.TableTiers.KeyDown += TableTiers_KeyDown;
            this.TableStatsWeightSum.CellFormatting += TableStatsWeightSum_CellFormatting;
            this.TableTiers.CellPainting += TableTiers_CellPainting;
            this.TableTiers.EditingControlShowing += TableTiers_EditingControlShowing;
            this.TableTiers.SelectionChanged += TableTiers_SelectionChanged;
        }

        public List<Tier> GetTiers()
        {
            return [.. _bindingTiers.OrderBy(t => t.TierId)];
        }

        private void TierManager_Load(object sender, EventArgs e)
        {
            this.AutoSize = false; // Turn off AutoSize so the form can be sized by code.
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            InitializeTableTiers();
            InitializeTableStatsWeightSum();

            _minFormSize = ComputeMinRequiredFormHeight();
            AdjustFormSizeToDataGrid();

            this.TextboxTotalTierWeights.Enabled = true;
            this.TextboxTotalTierWeights.BackColor = this.TextboxTotalTierWeights.BackColor; // this is needed for the stupid textbox to start considering the Forecolor :/
            this.TextboxTotalTierWeights.ForeColor = this.TextboxTotalTierWeights.ForeColor;

            SetTotalTierWeights();
        }

        private void AddTierButton_Click(object sender, EventArgs e)
        {
            var newTier = new Tier
            {
                TierId = _bindingTiers.Count + 1,
                TierName = $"Tier {_bindingTiers.Count + 1}",
                TierWeight = 0.0d,
                StatWeights = new Dictionary<string, double>()
            };

            // Initialize StatWeights with 0 for all possible stats
            foreach (var descriptor in _itemStatsDescriptors)
            {
                newTier.StatWeights.Add(descriptor.PropertyName, 0.0d);
            }

            _bindingTiers.Add(newTier);
            SetTotalTierWeights();

            if (TableTiers.RowCount > 0) TableTiers.FirstDisplayedScrollingRowIndex = TableTiers.RowCount - 1;
        }

        private void RemoveTierButton_Click(object sender, EventArgs e)
        {
            RemoveSelectedGridRows();
        }

        private void TierManager_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Detach the event handler to prevent validation during form closure
            this.TableTiers.CellValidating -= TableTiers_CellValidating;
            this.TableTiers.CellFormatting -= TableTiers_CellFormatting;
            this.TableTiers.CellPainting -= TableTiers_CellPainting;
            this.TableTiers.KeyDown -= TableTiers_KeyDown;
            this.TableStatsWeightSum.CellFormatting -= TableStatsWeightSum_CellFormatting;
            this.TableTiers.SelectionChanged -= TableTiers_SelectionChanged;
        }

        #region TableTiers events

        private void TableTiers_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            var grid = (DataGridView)sender;
            if (!grid.IsCurrentCellDirty || e.ColumnIndex <= 1 || e.ColumnIndex >= grid.Columns.Count || e.RowIndex < 0 || e.RowIndex >= _bindingTiers.Count || e.RowIndex >= grid.Rows.Count) return; // Skip validation if the cell is not dirty or for non-editable columns

            //var cell = grid.Rows[e.RowIndex].Cells[e.ColumnIndex];
            var (isValid, errorMessage) = ValidateAndCommitTierData(e.RowIndex, e.ColumnIndex, e.FormattedValue);

            if (!isValid)
            {
                grid.Rows[e.RowIndex].ErrorText = errorMessage;
                e.Cancel = true; // Prevent leaving the cell
            }
            else
            {
                grid.Rows[e.RowIndex].ErrorText = string.Empty; // Clear error on success
            }

            if (grid.EditingControl is TextBox editingControl)
            {
                // compute color from the current row error state which we already set above
                var desiredBack = string.IsNullOrWhiteSpace(grid.Rows[e.RowIndex].ErrorText) ? _editBackColor : _editInvalidBackColor;

                // save old colors so you can restore later as before
                _validationBackColorSuccess = editingControl.BackColor;
                _validationForeColorSuccess = editingControl.ForeColor;

                // set both editing control and its host background to avoid any white edges
                editingControl.BackColor = desiredBack;
                editingControl.ForeColor = _textColor;
                editingControl.Multiline = false;
                editingControl.TextAlign = HorizontalAlignment.Center;

                if (editingControl.Parent != null)
                {
                    editingControl.Parent.BackColor = desiredBack;
                    editingControl.Parent.Padding = Padding.Empty;
                    editingControl.Parent.Margin = Padding.Empty;
                }

                // Do NOT call grid.RefreshEdit() — it can recreate the editing control and cause flicker/position loss.
                // Instead perform a local refresh for the editing control and invalidate the specific cell.
                editingControl.Invalidate();
                editingControl.Refresh();
                grid.InvalidateCell(e.ColumnIndex, e.RowIndex);
            }
        }

        private void TableTiers_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex <= 1) return; // ignore tier id, name

            var grid = (DataGridView)sender;
            string statName = grid.Columns[e.ColumnIndex].DataPropertyName;

            if (grid.Rows[e.RowIndex].DataBoundItem is Tier tier)
            {
                if (string.Equals(statName, nameof(Tier.TierWeight), StringComparison.OrdinalIgnoreCase))
                {
                    e.Value = tier.TierWeight.ToString(_defaultDoubleCellStyle.Format);
                }
                else if (tier.StatWeights.TryGetValue(statName, out double value))
                {
                    e.Value = value.ToString(_defaultDoubleCellStyle.Format);
                    e.FormattingApplied = true;
                }
                else
                {
                    e.Value = (0.0d).ToString(_defaultDoubleCellStyle.Format);
                    e.FormattingApplied = true;
                }
            }
        }

        private void TableTiers_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
            {
                // This is the key change: we use BeginInvoke to ensure the edit starts after the KeyDown event has been fully processed.
                // This is a more robust way to trigger edit mode manually.
                if (!TableTiers.IsCurrentCellInEditMode)
                {
                    if (TableTiers.CurrentCell != null && !TableTiers.CurrentCell.ReadOnly)
                    {
                        TableTiers.BeginInvoke(new Action(() =>
                        {
                            TableTiers.BeginEdit(true);
                        }));
                    }
                }
                else
                {
                    // If already in edit mode, commit the changes.
                    TableTiers.EndEdit();
                }

                // Mark the event as handled to prevent the default behavior (moving to the next row).
                e.Handled = true;
            }
        }

        private void TableTiers_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            // Make sure we are in a valid cell and not a header
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            var grid = (DataGridView)sender;
            var cell = grid.Rows[e.RowIndex].Cells[e.ColumnIndex];
            bool isCurrentCell = (grid.CurrentCell != null && grid.CurrentCell.RowIndex == e.RowIndex && grid.CurrentCell.ColumnIndex == e.ColumnIndex);

            Color backColor = Color.Empty;
            bool isColorSet;

            // State 1: Cell is in Edit Mode
            if (isCurrentCell && cell.IsInEditMode)
            {
                // Check if there is a validation error on the row
                backColor = string.IsNullOrWhiteSpace(grid.Rows[e.RowIndex].ErrorText)
                    ? _editBackColor // Normal edit color
                    : _editInvalidBackColor;   // Error edit color
                isColorSet = true;
            }
            // State 2: Cell is selected but NOT in Edit Mode
            else if (isCurrentCell && (e.State & DataGridViewElementStates.Selected) == DataGridViewElementStates.Selected)
            {
                backColor = _selectionColor; // selection color
                isColorSet = true;
            }
            else
            {
                isColorSet = false;
            }

            if (!isColorSet)
            {
                e.Handled = false;
                return;
            }

            // Paint the background ourselves — do NOT call e.PaintBackground (it can introduce small gaps).
            // Expand height by 1 to be defensive against 1px artifacts on some themes/DPI combos.
            var fillRect = e.CellBounds;
            fillRect.Height += 1;

            using (var backBrush = new SolidBrush(backColor))
            {
                e.Graphics.FillRectangle(backBrush, fillRect);
            }

            // Only draw text if NOT in edit mode (editing control paints text)
            if (!cell.IsInEditMode)
            {
                var foreColor = _textColor;
                using (var foreBrush = new SolidBrush(foreColor))
                {
                    if (e.FormattedValue != null)
                    {
                        e.Graphics.DrawString(e.FormattedValue.ToString(), e.CellStyle.Font, foreBrush, e.CellBounds, _cellPaintingStringFormat);
                    }
                }
            }

            // Draw the cell border on top of our painting
            e.Paint(e.CellBounds, DataGridViewPaintParts.Border);
            e.Handled = true;
        }

        private void TableTiers_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control is DataGridViewTextBoxEditingControl textBox)
            {
                // Use BeginInvoke so we run after the internal layout is settled.
                BeginInvoke(new Action(() =>
                {
                    try
                    {
                        var dgv = TableTiers;

                        // determine the desired background based on row error state (if any)
                        Color desiredBack = _editBackColor;
                        if (dgv.CurrentCell != null)
                        {
                            var r = dgv.Rows[dgv.CurrentCell.RowIndex];
                            if (!string.IsNullOrWhiteSpace(r.ErrorText))
                            {
                                desiredBack = _editInvalidBackColor;
                            }
                        }

                        // Make parent (editing host) use the same background and remove padding/margins
                        var host = textBox.Parent;
                        if (host != null)
                        {
                            host.Margin = Padding.Empty;
                            host.Padding = Padding.Empty;
                            host.BackColor = desiredBack;
                        }

                        // Remove textbox chrome and make it fill the host client area exactly
                        textBox.Margin = Padding.Empty;
                        textBox.BorderStyle = BorderStyle.FixedSingle;
                        if (host != null)
                        {
                            textBox.Location = new Point(0, 0);
                            textBox.Size = host.ClientSize; // fill host exactly
                        }
                        else
                        {
                            textBox.Dock = DockStyle.Fill;
                        }

                        // Set colors with high-contrast foreground to avoid invisible text
                        textBox.BackColor = desiredBack;
                        textBox.ForeColor = _textColor;
                        textBox.Multiline = false;
                        textBox.TextAlign = HorizontalAlignment.Center;

                        // Bring to front to avoid painting order issues, then refresh only the editing control.
                        textBox.BringToFront();
                        textBox.Invalidate();
                        textBox.Refresh();

                        // Invalidate the cell to force a consistent repaint of borders
                        if (dgv.CurrentCell != null)
                        {
                            dgv.InvalidateCell(dgv.CurrentCell.ColumnIndex, dgv.CurrentCell.RowIndex);
                        }
                    }
                    catch
                    {
                        // swallow exceptions to keep editing flow stable
                    }
                }));
            }
        }

        private void TableTiers_SelectionChanged(object sender, EventArgs e)
        {
            // Store the current cell before the selection changes to a new cell
            var oldSelectedCell = _previousSelectedCell;

            // Get the new current cell and store it for the next selection change
            if (TableTiers.CurrentCell != null)
            {
                _previousSelectedCell = TableTiers.CurrentCell;
            }

            // Invalidate the old cell to remove the custom painting
            if (oldSelectedCell != null)
            {
                TableTiers.InvalidateCell(oldSelectedCell);
            }

            // Invalidate the new current cell to trigger repainting
            if (TableTiers.CurrentCell != null)
            {
                TableTiers.InvalidateCell(TableTiers.CurrentCell);
            }
        }

        private void TableTiers_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            var grid = (DataGridView)sender;
            if (e.ColumnIndex < 0 || e.ColumnIndex >= grid.Columns.Count || e.RowIndex < 0 || e.RowIndex >= grid.Rows.Count) return;

            var row = grid.Rows[e.RowIndex];
            row.ErrorText = string.Empty; // Clear the error text for the row
        }

        #endregion

        #region Private helpers

        internal (bool Result, string ErrorMessage) ValidateAndCommitTierData(int rowIndex, int colIndex, object formattedValue)
        {
            // Skip validation for TierId and TierName columns
            if (colIndex <= 1 || rowIndex < 0 || rowIndex >= _bindingTiers.Count) return (true, string.Empty);

            // Validate numeric columns
            if (!double.TryParse(formattedValue?.ToString() ?? "0.00", out double value))
            {
                return (false, "Please enter a valid floating point number.");
            }

            if (value < 0.00d || value > 100.00d)
            {
                return (false, "The value of the weight needs to be between 0.00 and 100.00");
            }

            var tier = _bindingTiers[rowIndex];
            var columnName = TableTiers.Columns[colIndex].Name;

            if (string.Equals(columnName, nameof(Tier.TierWeight), StringComparison.OrdinalIgnoreCase))
            {
                if (_totalTierWeight - tier.TierWeight + value > 100.00d)
                {
                    return (false, $"The total value of tier weights cannot exceed 100%. Attempted value: {_totalTierWeight - tier.TierWeight + value}");
                }

                tier.TierWeight = value;
                SetTotalTierWeights();
                TableStatsWeightSum.Refresh();
            }
            else // It's a StatWeight
            {
                if (tier.TotalStatWeight - tier.StatWeights[columnName] + value > 100.00d)
                {
                    return (false, $"The total value of stats' weights for '{tier.TierName}' (ID: {tier.TierId}) cannot exceed 100%. Attempted value: {tier.TotalStatWeight - tier.StatWeights[columnName] + value}");
                }

                tier.SetStatWeight(columnName, value);
                TableStatsWeightSum.Refresh();
            }

            return (true, string.Empty); // Validation and commit successful
        }

        private void RefreshTierIds()
        {
            for (int i = 0; i < _bindingTiers.Count; i++)
            {
                _bindingTiers[i].TierId = i + 1;
            }
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
                ErrorHelper.ShowError(ex, $"{nameof(TierManager)} - {nameof(AdjustFormSizeToDataGrid)}");
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
            if (TableTiers.SelectedCells.Count > 0)
            {
                var tiersToRemove = TableTiers.SelectedRows
                    .Cast<DataGridViewRow>()
                    .Select(row => row.DataBoundItem as Tier)
                    .Where(tier => tier != null)
                    .ToList();

                foreach (var tier in tiersToRemove)
                {
                    _bindingTiers.Remove(tier);
                }

                RefreshTierIds();
                SetTotalTierWeights();
            }
        }

        private void InitializeTableTiers()
        {
            TableTiers.SuspendLayout();

            // Configure the DataGridView            
            TableTiers.AutoGenerateColumns = false;
            TableTiers.Columns.Clear();
            TableTiers.DataSource = _bindingTiers;
            TableTiers.ShowEditingIcon = true;
            TableTiers.ShowCellErrors = true;
            TableTiers.ShowRowErrors = true;
            TableTiers.AllowUserToAddRows = false;
            TableTiers.AllowUserToDeleteRows = false;
            TableTiers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            TableTiers.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;
            TableTiers.MultiSelect = true;
            TableTiers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            TableTiers.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            TableTiers.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            TableTiers.DefaultCellStyle.FormatProvider = System.Globalization.CultureInfo.InvariantCulture;
            TableTiers.RowTemplate.DefaultCellStyle.FormatProvider = System.Globalization.CultureInfo.InvariantCulture;
            typeof(DataGridView).GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(TableTiers, true);

            // Add basic columns
            var idColumn = new DataGridViewTextBoxColumn
            {
                Name = nameof(Tier.TierId),
                HeaderText = "Tier ID",
                DataPropertyName = nameof(Tier.TierId),
                ValueType = typeof(int),
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "0", Alignment = DataGridViewContentAlignment.MiddleCenter, FormatProvider = System.Globalization.CultureInfo.InvariantCulture }
            };

            var nameColumn = new DataGridViewTextBoxColumn
            {
                Name = nameof(Tier.TierName),
                HeaderText = "Tier Name",
                DataPropertyName = nameof(Tier.TierName),
                ValueType = typeof(string),
                ReadOnly = false,
                DefaultCellStyle = _defaultStringCellStyle,
                HeaderCell = { ToolTipText = "Tier name" },
            };

            var weightColumn = new DataGridViewTextBoxColumn
            {
                Name = nameof(Tier.TierWeight),
                HeaderText = "Tier Weight",
                DataPropertyName = nameof(Tier.TierWeight),
                ValueType = typeof(double),
                ReadOnly = false,
                DefaultCellStyle = _defaultDoubleCellStyle,
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
                    DataPropertyName = descriptor.PropertyName,
                    ValueType = typeof(double),
                    ReadOnly = false,
                    DefaultCellStyle = _defaultDoubleCellStyle,
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
            TableStatsWeightSum.Columns.Clear();
            TableStatsWeightSum.DataSource = _bindingTiers;
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
                ReadOnly = true,
                ValueType = typeof(int),
                DataPropertyName = nameof(Tier.TierId),
                DefaultCellStyle = new DataGridViewCellStyle() { Alignment = DataGridViewContentAlignment.MiddleCenter }
            };

            var weightColumn = new DataGridViewTextBoxColumn
            {
                Name = nameof(Tier.TotalStatWeight),
                HeaderText = "Total Stat Weight (%)",
                ReadOnly = true,
                ValueType = typeof(double),
                DataPropertyName = nameof(Tier.TotalStatWeight),
                DefaultCellStyle = _defaultDoubleCellStyle
            };
            TableStatsWeightSum.Columns.AddRange([tierIdColumn, weightColumn]);

            TableStatsWeightSum.ResumeLayout(true);
        }

        private void SetTotalTierWeights()
        {
            TextboxTotalTierWeights.Text = _totalTierWeight.ToString("0.00") + " %";
        }

        #endregion

        #region TableStatsWeightSum events

        private void TableStatsWeightSum_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            var grid = (DataGridView)sender;
            string statName = grid.Columns[e.ColumnIndex].DataPropertyName;

            if (e.RowIndex >= 0
                && grid.Rows[e.RowIndex].DataBoundItem is Tier tier
                && string.Equals(statName, nameof(Tier.TotalStatWeight), StringComparison.OrdinalIgnoreCase))
            {
                e.Value = tier.TotalStatWeight.ToString(_defaultDoubleCellStyle.Format);
                e.FormattingApplied = true;
            }
        }

        #endregion
    }
}