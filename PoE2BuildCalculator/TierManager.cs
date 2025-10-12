using System.ComponentModel;
using System.Data;
using System.Reflection;

using Domain.Helpers;
using Domain.Main;
using Domain.Static;

using TextBox = System.Windows.Forms.TextBox;
using Timer = System.Windows.Forms.Timer;

namespace PoE2BuildCalculator
{
    public partial class TierManager : Form
    {
        private double _totalTierWeight => _bindingTiers.Sum(t => t.TierWeight);

        private Color _validationBackColorSuccess;
        private Color _validationForeColorSuccess;
        private int _minFormSize;
        private DataGridViewCell _previousSelectedCell;
        private Timer _flashTimer;
        private DateTime _flashStartTime;
        private Color _originalBackColor;

        private readonly Color _editBackColor = Color.LightGreen;
        private readonly Color _editInvalidBackColor = Color.LightPink;
        private readonly Color _selectionBackColor = Color.SpringGreen;
        private readonly Color _selectionTextColor = Color.DarkRed;
        private readonly Color _tierWeightTextColor = Color.Blue;
        private readonly Color _tierWeightBackColor = Color.Wheat;

        private readonly BindingList<Tier> _bindingTiers = [];
        private readonly IReadOnlyList<ItemStatsHelper.StatDescriptor> _itemStatsDescriptors;

        private readonly StringFormat _cellPaintingStringFormat = new()
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center,
            Trimming = StringTrimming.EllipsisCharacter
        };
        private readonly DataGridViewCellStyle _defaultDoubleCellStyle = new()
        {
            Format = Constants.DOUBLE_NUMBER_FORMAT,
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

        public TierManager()
        {
            _itemStatsDescriptors = ItemStatsHelper.GetStatDescriptors();
            InitializeComponent();
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

            foreach (var descriptor in _itemStatsDescriptors)
            {
                newTier.StatWeights.Add(descriptor.PropertyName, 0.0d);
            }

            _bindingTiers.Add(newTier);

            // UPDATE ROW COUNT for virtual mode
            TableTiers.RowCount = _bindingTiers.Count;

            SetTotalTierWeights();

            if (TableTiers.RowCount > 0)
                TableTiers.FirstDisplayedScrollingRowIndex = TableTiers.RowCount - 1;
        }
        private void RemoveTierButton_Click(object sender, EventArgs e)
        {
            RemoveSelectedGridRows();
        }
        private void TierManager_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Dispose timer if it exists
            try
            {
                _flashTimer?.Stop();
                _flashTimer?.Dispose();
            }
            catch { }

            // Detach handlers that Designer may have attached in InitializeComponent
            try
            {
                this.TableTiers.CellValidating -= TableTiers_CellValidating;
                this.TableTiers.CellFormatting -= TableTiers_CellFormatting;
                this.TableTiers.CellPainting -= TableTiers_CellPainting;
                this.TableTiers.KeyDown -= TableTiers_KeyDown;
                this.TableTiers.SelectionChanged -= TableTiers_SelectionChanged;
                this.TableTiers.EditingControlShowing -= TableTiers_EditingControlShowing;
                this.TableTiers.CellEndEdit -= TableTiers_CellEndEdit;

                this.TableStatsWeightSum.CellFormatting -= TableStatsWeightSum_CellFormatting;
            }
            catch
            {
                // ignore any detach errors on shutdown
            }
        }
        private void TextboxTotalTierWeights_TextChanged(object sender, EventArgs e)
        {
            StartFlashingIfNeeded();
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
                var desiredBack = GetCellEditBackColor(grid, e.RowIndex, true, false);
                ApplyEditingControlStyling(editingControl, desiredBack);

                // Invalidate the specific cell to ensure borders are drawn correctly
                grid.InvalidateCell(e.ColumnIndex, e.RowIndex);
            }
        }

        private void TableTiers_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= _bindingTiers.Count || e.ColumnIndex <= 1) return;

            var grid = (DataGridView)sender;
            string statName = grid.Columns[e.ColumnIndex].DataPropertyName;

            var tier = _bindingTiers[e.RowIndex];

            if (string.Equals(statName, nameof(Tier.TierWeight), StringComparison.OrdinalIgnoreCase))
            {
                e.Value = tier.TierWeight.ToString(_defaultDoubleCellStyle.Format);
                e.FormattingApplied = true;
            }
            else if (tier.StatWeights.TryGetValue(statName, out double value))
            {
                e.Value = value.ToString(_defaultDoubleCellStyle.Format);
                e.FormattingApplied = true;
            }
            else
            {
                e.Value = 0.0d.ToString(_defaultDoubleCellStyle.Format);
                e.FormattingApplied = true;
            }
        }

        private void TableTiers_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode is Keys.Enter or Keys.Return)
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
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            var grid = (DataGridView)sender;
            var cell = grid.Rows[e.RowIndex].Cells[e.ColumnIndex];
            bool isCurrentCell = grid.CurrentCell != null && grid.CurrentCell.RowIndex == e.RowIndex && grid.CurrentCell.ColumnIndex == e.ColumnIndex;
            bool isTierWeightColumn = string.Equals(grid.Columns[e.ColumnIndex].Name, nameof(Tier.TierWeight), StringComparison.OrdinalIgnoreCase);

            Color backColor = Color.Empty;
            bool isColorSet = false;

            if (isCurrentCell && cell.IsInEditMode)
            {
                backColor = GetCellEditBackColor(grid, e.RowIndex, true, false);
                isColorSet = true;
            }
            else if (isCurrentCell && (e.State & DataGridViewElementStates.Selected) == DataGridViewElementStates.Selected)
            {
                backColor = GetCellEditBackColor(grid, e.RowIndex, false, true);
                isColorSet = true;
            }
            else if (isTierWeightColumn)
            {
                backColor = _tierWeightBackColor;
                isColorSet = true;
            }

            if (!isColorSet)
            {
                e.Handled = false;
                return;
            }

            // Paint full background; +1 pixel height is defensive against 1px artifacts on some themes/DPI.
            var fillRect = e.CellBounds;
            fillRect.Height += 1;

            using (var backBrush = new SolidBrush(backColor))
            {
                e.Graphics.FillRectangle(backBrush, fillRect);
            }

            // Only draw text when not editing (editing control draws it)
            if (!cell.IsInEditMode && e.FormattedValue != null)
            {
                var brushColor = isTierWeightColumn ? _tierWeightTextColor : _selectionTextColor;
                using var foreBrush = new SolidBrush(brushColor);
                e.Graphics.DrawString(e.FormattedValue.ToString(), e.CellStyle.Font, foreBrush, e.CellBounds, _cellPaintingStringFormat);
            }

            // Draw borders on top
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
                        if (dgv.CurrentCell == null) return;

                        var desiredBack = GetCellEditBackColor(dgv, dgv.CurrentCell.RowIndex, true, false);
                        ApplyEditingControlStyling(textBox, desiredBack);

                        // Ensure cell repainted so our background/border alignment is correct
                        dgv.InvalidateCell(dgv.CurrentCell.ColumnIndex, dgv.CurrentCell.RowIndex);
                    }
                    catch
                    {
                        // swallow — do not break edit flow
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
            if (oldSelectedCell != null && oldSelectedCell.DataGridView == TableTiers)
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

            if (value is < 0.00d or > 100.00d)
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
                else
                {
                    var (isValid, rowIndexOther) = ValidateNoStatWeightDuplication(rowIndex, columnName);
                    if (!isValid) return (false, $"You cannot set the weight, for the same stat, in more than one tier. Stat name: {columnName}; Other tier: {_bindingTiers[rowIndexOther].TierName}");
                }

                tier.SetStatWeight(columnName, value);
                TableStatsWeightSum.Refresh();
            }

            return (true, string.Empty); // Validation and commit successful
        }

        private Color GetCellEditBackColor(DataGridView grid, int rowIndex, bool isEditing, bool isSelected)
        {
            if (isEditing)
            {
                return string.IsNullOrWhiteSpace(grid.Rows[rowIndex].ErrorText) ? _editBackColor : _editInvalidBackColor;
            }
            else if (isSelected)
            {
                return _selectionBackColor;
            }

            return Color.Empty;
        }

        /// <summary>
        /// Ensures the editing TextBox and its host share the same background and remove inner padding.
        /// Keeps the control filling the host client area (avoids repositioning host which causes flicker).
        /// </summary>
        private void ApplyEditingControlStyling(TextBox editingControl, Color desiredBack)
        {
            if (editingControl == null) return;

            // save original colors for restoration (existing fields used elsewhere)
            _validationBackColorSuccess = editingControl.BackColor;
            _validationForeColorSuccess = editingControl.ForeColor;

            editingControl.BackColor = desiredBack;
            editingControl.ForeColor = _selectionTextColor;
            editingControl.Margin = Padding.Empty;
            editingControl.BorderStyle = BorderStyle.FixedSingle; // keeps vertical text alignment correct
            editingControl.Multiline = false;
            editingControl.TextAlign = HorizontalAlignment.Center;

            if (editingControl.Parent != null)
            {
                editingControl.Parent.BackColor = desiredBack;
                editingControl.Parent.Padding = Padding.Empty;
                editingControl.Parent.Margin = Padding.Empty;

                // make editing control exactly fill the host client area
                editingControl.Location = new Point(0, 0);
                editingControl.Size = editingControl.Parent.ClientSize;
            }
            else
            {
                editingControl.Dock = DockStyle.Fill;
            }

            // Do a localized refresh — avoid grid.RefreshEdit()
            editingControl.Invalidate();
            editingControl.Refresh();
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
                var rowsToRemove = TableTiers.SelectedRows
                    .Cast<DataGridViewRow>()
                    .Select(row => row.Index)
                    .Where(index => index >= 0 && index < _bindingTiers.Count)
                    .OrderByDescending(i => i)
                    .ToList();

                foreach (var rowIndex in rowsToRemove)
                {
                    _bindingTiers.RemoveAt(rowIndex);
                }

                RefreshTierIds();

                // UPDATE ROW COUNT for virtual mode
                TableTiers.RowCount = _bindingTiers.Count;

                SetTotalTierWeights();
            }
        }
        private void InitializeTableTiers()
        {
            TableTiers.SuspendLayout();

            // ENABLE VIRTUAL MODE for lazy loading
            TableTiers.VirtualMode = true;
            TableTiers.AutoGenerateColumns = false;
            TableTiers.Columns.Clear();
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
            TableTiers.DefaultCellStyle.Padding = Padding.Empty;
            TableTiers.RowTemplate.DefaultCellStyle.Padding = Padding.Empty;
            TableTiers.RowTemplate.DefaultCellStyle.FormatProvider = System.Globalization.CultureInfo.InvariantCulture;

            // WIRE UP VIRTUAL MODE EVENTS (ONLY REAL EVENTS)
            TableTiers.CellValueNeeded += TableTiers_CellValueNeeded;
            TableTiers.CellValuePushed += TableTiers_CellValuePushed;

            try
            {
                typeof(DataGridView).GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic)?.SetValue(TableTiers, true);
            }
            catch
            {
                // ignore; double-buffering is a best-effort optimization
            }

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

            // Add columns for StatWeights
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

            // SET INITIAL ROW COUNT for virtual mode
            TableTiers.RowCount = _bindingTiers.Count;

            // Add context menu
            var contextMenu = new ContextMenuStrip();
            var deleteItem = new ToolStripMenuItem("Delete Selected Tiers");
            var addItem = new ToolStripMenuItem("Add New Tier");
            deleteItem.Click += (s, e) => RemoveSelectedGridRows();
            addItem.Click += AddTierButton_Click;

            contextMenu.Items.AddRange([addItem, deleteItem]);
            TableTiers.ContextMenuStrip = contextMenu;

            TableTiers.ResumeLayout(true);
        }

        private void TableTiers_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= _bindingTiers.Count) return;

            var tier = _bindingTiers[e.RowIndex];
            var columnName = TableTiers.Columns[e.ColumnIndex].Name;

            e.Value = string.Equals(columnName, nameof(Tier.TierId), StringComparison.OrdinalIgnoreCase)
                ? tier.TierId
                : string.Equals(columnName, nameof(Tier.TierName), StringComparison.OrdinalIgnoreCase)
                    ? tier.TierName
                    : string.Equals(columnName, nameof(Tier.TierWeight), StringComparison.OrdinalIgnoreCase)
                        ? tier.TierWeight
                        : tier.StatWeights.TryGetValue(columnName, out double value) ? value : 0.0d;
        }

        private void TableTiers_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= _bindingTiers.Count) return;

            var tier = _bindingTiers[e.RowIndex];
            var columnName = TableTiers.Columns[e.ColumnIndex].Name;

            if (string.Equals(columnName, nameof(Tier.TierName), StringComparison.OrdinalIgnoreCase))
            {
                tier.TierName = e.Value?.ToString() ?? string.Empty;
            }
            else if (string.Equals(columnName, nameof(Tier.TierWeight), StringComparison.OrdinalIgnoreCase))
            {
                if (double.TryParse(e.Value?.ToString(), out double weight))
                {
                    tier.TierWeight = weight;
                    SetTotalTierWeights();
                }
            }
            else if (tier.StatWeights.ContainsKey(columnName))
            {
                if (double.TryParse(e.Value?.ToString(), out double statWeight))
                {
                    tier.SetStatWeight(columnName, statWeight);
                }
            }
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
            TextboxTotalTierWeights.Text = _totalTierWeight.ToString(Constants.DOUBLE_NUMBER_FORMAT) + Constants.TOTAL_TIERS_WEIGHT_SUFFIX;
        }

        private void StartFlashingIfNeeded()
        {
            var raw = TextboxTotalTierWeights.Text[..^2]; // ignore last 2 characters " %"

            if (!double.TryParse(raw, out double value) || value <= 80.00d) return;

            if (_flashTimer == null)
            {
                _originalBackColor = TextboxTotalTierWeights.BackColor;
                _flashTimer = new Timer
                {
                    Interval = 500 // toggle every 0.5 seconds
                };

                _flashTimer.Tick += FlashTimer_Tick;
            }

            _flashStartTime = DateTime.Now;
            _flashTimer.Start();
        }

        private void FlashTimer_Tick(object sender, EventArgs e)
        {
            // Stop after 10 seconds
            if ((DateTime.Now - _flashStartTime).TotalSeconds > 10)
            {
                _flashTimer.Stop();
                TextboxTotalTierWeights.BackColor = _originalBackColor;
                return;
            }

            // Toggle between original and light reddish
            TextboxTotalTierWeights.BackColor =
                TextboxTotalTierWeights.BackColor == _originalBackColor
                ? Color.Red
                : _originalBackColor;
        }

        private (bool isValid, int rowIndexOther) ValidateNoStatWeightDuplication(int rowIndexToExclude, string statName)
        {
            for (int i = 0; i < _bindingTiers.Count; i++)
            {
                if (i != rowIndexToExclude && _bindingTiers[i].StatWeights.TryGetValue(statName, out double value) && value > 0.0d)
                {
                    return (false, i);
                }
            }

            return (true, -1);
        }

        #endregion




        #region TableStatsWeightSum events

        private void TableStatsWeightSum_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= _bindingTiers.Count) return;

            var grid = (DataGridView)sender;
            string statName = grid.Columns[e.ColumnIndex].DataPropertyName;

            var tier = _bindingTiers[e.RowIndex];

            if (string.Equals(statName, nameof(Tier.TotalStatWeight), StringComparison.OrdinalIgnoreCase))
            {
                e.Value = tier.TotalStatWeight.ToString(_defaultDoubleCellStyle.Format);
                e.FormattingApplied = true;
            }
        }

        #endregion
    }
}