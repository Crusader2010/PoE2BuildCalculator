using Domain.Combinations;
using Domain.Main;
using System.ComponentModel;
using System.Windows.Forms.VisualStyles;

namespace PoE2BuildCalculator
{
    public partial class CustomValidator : Form
    {
        #region Constants
        private const string COL_PROPERTY_NAME = "PropertyName";
        private const string COL_SUM_AT_LEAST_ENABLED = "SumAtLeastEnabled";
        private const string COL_SUM_AT_LEAST_VALUE = "SumAtLeastValue";
        private const string COL_OP1 = "Op1";
        private const string COL_SUM_AT_MOST_ENABLED = "SumAtMostEnabled";
        private const string COL_SUM_AT_MOST_VALUE = "SumAtMostValue";
        private const string COL_OP2 = "Op2";
        private const string COL_EACH_AT_LEAST_ENABLED = "EachAtLeastEnabled";
        private const string COL_EACH_AT_LEAST_VALUE = "EachAtLeastValue";
        private const string COL_OP3 = "Op3";
        private const string COL_EACH_AT_MOST_ENABLED = "EachAtMostEnabled";
        private const string COL_EACH_AT_MOST_VALUE = "EachAtMostValue";
        private const string COL_ROW_OPERATOR = "RowOperator";
        #endregion

        // This will hold the final, combined validation function.
        private Func<List<Item>, bool> _masterValidator;
        private BindingList<ValidationRuleModel> _rules;
        private readonly MainForm _ownerForm;

        private DataGridViewCellStyle _enabledOpStyle;
        private DataGridViewCellStyle _disabledOpStyle;
        private int _rowIndexFromMouseDown;
        private int _rowIndexOfItemUnderMouseToDrop;
        private int _insertionRowIndex = -1;
        private bool _isClosing = false;
        private Color _initialCellSelectionColor;
        private readonly Color _backColorInvalidValue = Color.FromArgb(255, 200, 200);
        private readonly Color _backColorValidValue = Color.White;

        public CustomValidator(MainForm ownerForm)
        {
            ArgumentNullException.ThrowIfNull(ownerForm, nameof(ownerForm));
            InitializeComponent();
            _ownerForm = ownerForm;

            typeof(DataGridView).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)?.SetValue(dgvRules, true);
        }

        private void dgvRules_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var colName = dgvRules.Columns[e.ColumnIndex].DataPropertyName;

            // Check if the changed cell is one of our checkbox columns
            bool isCheckboxColumn = colName == COL_SUM_AT_LEAST_ENABLED ||
                                   colName == COL_SUM_AT_MOST_ENABLED ||
                                   colName == COL_EACH_AT_LEAST_ENABLED ||
                                   colName == COL_EACH_AT_MOST_ENABLED;

            if (isCheckboxColumn)
            {
                dgvRules.CommitEdit(DataGridViewDataErrorContexts.Commit);
                var ruleModel = _rules[e.RowIndex];
                bool isChecked = (bool)(dgvRules[e.ColumnIndex, e.RowIndex]?.Value ?? false);

                if (!isChecked)
                {
                    switch (colName)
                    {
                        case COL_SUM_AT_LEAST_ENABLED: ruleModel.SumAtLeastValue = string.Empty; break;
                        case COL_SUM_AT_MOST_ENABLED: ruleModel.SumAtMostValue = string.Empty; break;
                        case COL_EACH_AT_LEAST_ENABLED: ruleModel.EachAtLeastValue = string.Empty; break;
                        case COL_EACH_AT_MOST_ENABLED: ruleModel.EachAtMostValue = string.Empty; break;
                    }
                }

                UpdateCellStates();
            }
        }

        private void dgvRules_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (_isClosing)
            {
                e.Cancel = false;
                return;
            }

            var colName = dgvRules.Columns[e.ColumnIndex].DataPropertyName;

            // Only validate value columns
            bool isValueColumn = colName == COL_SUM_AT_LEAST_VALUE ||
                                colName == COL_SUM_AT_MOST_VALUE ||
                                colName == COL_EACH_AT_LEAST_VALUE ||
                                colName == COL_EACH_AT_MOST_VALUE;

            if (!isValueColumn) return;

            var rule = _rules[e.RowIndex];
            bool isValidationRequired = colName switch
            {
                COL_SUM_AT_LEAST_VALUE => rule.SumAtLeastEnabled,
                COL_SUM_AT_MOST_VALUE => rule.SumAtMostEnabled,
                COL_EACH_AT_LEAST_VALUE => rule.EachAtLeastEnabled,
                COL_EACH_AT_MOST_VALUE => rule.EachAtMostEnabled,
                _ => false
            };

            if (!isValidationRequired)
            {
                e.Cancel = false;
                dgvRules.Rows[e.RowIndex].ErrorText = string.Empty;
                dgvRules[e.ColumnIndex, e.RowIndex].Style.BackColor = _backColorValidValue;

                // Update editing control if still in edit mode
                if (dgvRules.EditingControl is TextBox tb) tb.BackColor = _backColorValidValue;

                return;
            }

            var propType = rule.PropInfo.PropertyType;
            string value = e.FormattedValue?.ToString();

            if (string.IsNullOrWhiteSpace(value))
            {
                //e.Cancel = true;
                //dgvRules.Rows[e.RowIndex].ErrorText = $"Value required for {rule.PropertyName}";
                //dgvRules[e.ColumnIndex, e.RowIndex].Style.BackColor = _backColorInvalidValue;
                return;
            }

            bool isValid = propType == typeof(int)
                ? int.TryParse(value, out _)
                : double.TryParse(value, out _);

            if (!isValid)
            {
                e.Cancel = true;
                dgvRules.Rows[e.RowIndex].ErrorText = $"Invalid {propType.Name} for {rule.PropertyName}";
                dgvRules[e.ColumnIndex, e.RowIndex].Style.BackColor = _backColorInvalidValue;

                if (dgvRules.EditingControl is TextBox tb) tb.BackColor = _backColorInvalidValue;
            }
            else
            {
                dgvRules.Rows[e.RowIndex].ErrorText = string.Empty;
                dgvRules[e.ColumnIndex, e.RowIndex].Style.BackColor = _backColorValidValue;

                if (dgvRules.EditingControl is TextBox tb) tb.BackColor = _backColorValidValue;
            }
        }

        private void btnCreateValidator_Click(object sender, EventArgs e)
        {
            static bool combine(string op, bool current, bool next)
            {
                return op switch
                {
                    "AND" => current && next,
                    "OR" => current || next,
                    "XOR" => current ^ next,
                    _ => throw new InvalidOperationException($"Unknown operator: {op}"),
                };
            }

            dgvRules.CommitEdit(DataGridViewDataErrorContexts.Commit);

            try
            {
                // Get active rules with their ORIGINAL indices for proper operator lookup
                var activeRulesWithIndices = _rules
                    .Select((rule, index) => new { Rule = rule, OriginalIndex = index })
                    .Where(x => x.Rule.SumAtLeastEnabled || x.Rule.SumAtMostEnabled ||
                               x.Rule.EachAtLeastEnabled || x.Rule.EachAtMostEnabled)
                    .ToList();

                if (activeRulesWithIndices.Count == 0)
                {
                    _masterValidator = (items) => true;
                    MessageBox.Show("No rules defined. Validator will always return true.",
                                  "Validator Created", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Validate that all active rules have values
                foreach (var ruleWithIndex in activeRulesWithIndices)
                {
                    var rule = ruleWithIndex.Rule;
                    if ((rule.SumAtLeastEnabled && string.IsNullOrWhiteSpace(rule.SumAtLeastValue)) ||
                        (rule.SumAtMostEnabled && string.IsNullOrWhiteSpace(rule.SumAtMostValue)) ||
                        (rule.EachAtLeastEnabled && string.IsNullOrWhiteSpace(rule.EachAtLeastValue)) ||
                        (rule.EachAtMostEnabled && string.IsNullOrWhiteSpace(rule.EachAtMostValue)))
                    {
                        MessageBox.Show($"Rule '{rule.PropertyName}' has an enabled condition without a value.",
                                      "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                // Build validators with their operators
                var validatorsWithOps = new List<(Func<List<Item>, bool> Validator, string Operator)>();

                for (int i = 0; i < activeRulesWithIndices.Count; i++)
                {
                    var ruleWithIndex = activeRulesWithIndices[i];
                    var rule = ruleWithIndex.Rule;

                    bool propertyValidator(List<Item> items)
                    {
                        var conditions = new List<Func<List<Item>, bool>>();
                        var operators = new List<string>();

                        if (rule.SumAtLeastEnabled)
                        {
                            var v = Convert.ToDouble(rule.SumAtLeastValue);
                            conditions.Add(list => list.Sum(item => Convert.ToDouble(rule.PropInfo.GetValue(item.ItemStats))) >= v);
                            operators.Add(rule.Op1);
                        }
                        if (rule.SumAtMostEnabled)
                        {
                            var v = Convert.ToDouble(rule.SumAtMostValue);
                            conditions.Add(list => list.Sum(item => Convert.ToDouble(rule.PropInfo.GetValue(item.ItemStats))) <= v);
                            operators.Add(rule.Op2);
                        }
                        if (rule.EachAtLeastEnabled)
                        {
                            var v = Convert.ToDouble(rule.EachAtLeastValue);
                            conditions.Add(list => list.All(item => Convert.ToDouble(rule.PropInfo.GetValue(item.ItemStats)) >= v));
                            operators.Add(rule.Op3);
                        }
                        if (rule.EachAtMostEnabled)
                        {
                            var v = Convert.ToDouble(rule.EachAtMostValue);
                            conditions.Add(list => list.All(item => Convert.ToDouble(rule.PropInfo.GetValue(item.ItemStats)) <= v));
                        }

                        if (conditions.Count == 0) return true;

                        bool result = conditions[0](items);
                        for (int j = 1; j < conditions.Count; j++)
                        {
                            result = combine(operators[j - 1], result, conditions[j](items));
                        }
                        return result;
                    }

                    // Get the row operator from the ORIGINAL rule position
                    string rowOperator = rule.RowOperator ?? "AND";
                    validatorsWithOps.Add((propertyValidator, rowOperator));
                }

                // Combine all validators
                _masterValidator = (items) =>
                {
                    if (validatorsWithOps.Count == 0) return true;

                    bool finalResult = validatorsWithOps[0].Validator(items);
                    for (int i = 1; i < validatorsWithOps.Count; i++)
                    {
                        // Use the operator from the PREVIOUS validator
                        string rowOp = validatorsWithOps[i - 1].Operator;
                        finalResult = combine(rowOp, finalResult, validatorsWithOps[i].Validator(items));
                    }
                    return finalResult;
                };

                _ownerForm._itemValidatorFunction = _masterValidator;

                MessageBox.Show("Validator function created successfully! Ready to test.",
                              "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (FormatException)
            {
                MessageBox.Show("Error: A checked condition has an empty or invalid value.",
                              "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred: {ex.Message}",
                              "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CustomValidator_Load(object sender, EventArgs e)
        {
            SetupDataGridView();
            PopulateRules();

            AutoResizeForm();
        }

        private void SetupDataGridView()
        {
            _enabledOpStyle = new DataGridViewCellStyle
            {
                BackColor = Color.GreenYellow,
                ForeColor = Color.DarkRed,
                SelectionBackColor = Color.AliceBlue,
            };
            _disabledOpStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(224, 224, 224),
                ForeColor = Color.DarkGray,
                SelectionBackColor = Color.FromArgb(224, 224, 224)
            };

            dgvRules.SuspendLayout();
            dgvRules.AutoGenerateColumns = false;
            dgvRules.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgvRules.RowHeadersVisible = true;
            dgvRules.RowHeadersWidth = 30;
            dgvRules.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToFirstHeader;
            dgvRules.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvRules.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            var opItems = new[] { "AND", "OR", "XOR" };

            DataGridViewComboBoxColumn createOpColumn(string header, string dataProperty)
            {
                var col = new DataGridViewComboBoxColumn
                {
                    HeaderText = header,
                    DataPropertyName = dataProperty,
                    Name = dataProperty,
                    FlatStyle = FlatStyle.Standard,
                    FillWeight = 30,
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton
                };
                col.Items.AddRange(opItems);
                return col;
            }

            dgvRules.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Statistic", DataPropertyName = COL_PROPERTY_NAME, Name = COL_PROPERTY_NAME, ReadOnly = true, FillWeight = 150 });
            dgvRules.Columns.Add(new DataGridViewCheckBoxColumn { HeaderText = "Sum ≥", DataPropertyName = COL_SUM_AT_LEAST_ENABLED, Name = COL_SUM_AT_LEAST_ENABLED, FillWeight = 50 });
            dgvRules.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Value", DataPropertyName = COL_SUM_AT_LEAST_VALUE, Name = COL_SUM_AT_LEAST_VALUE, FillWeight = 70 });
            dgvRules.Columns.Add(createOpColumn("Operator", COL_OP1));
            dgvRules.Columns.Add(new DataGridViewCheckBoxColumn { HeaderText = "Sum ≤", DataPropertyName = COL_SUM_AT_MOST_ENABLED, Name = COL_SUM_AT_MOST_ENABLED, FillWeight = 50 });
            dgvRules.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Value", DataPropertyName = COL_SUM_AT_MOST_VALUE, Name = COL_SUM_AT_MOST_VALUE, FillWeight = 70 });
            dgvRules.Columns.Add(createOpColumn("Operator", COL_OP2));
            dgvRules.Columns.Add(new DataGridViewCheckBoxColumn { HeaderText = "Each ≥", DataPropertyName = COL_EACH_AT_LEAST_ENABLED, Name = COL_EACH_AT_LEAST_ENABLED, FillWeight = 50 });
            dgvRules.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Value", DataPropertyName = COL_EACH_AT_LEAST_VALUE, Name = COL_EACH_AT_LEAST_VALUE, FillWeight = 70 });
            dgvRules.Columns.Add(createOpColumn("Operator", COL_OP3));
            dgvRules.Columns.Add(new DataGridViewCheckBoxColumn { HeaderText = "Each ≤", DataPropertyName = COL_EACH_AT_MOST_ENABLED, Name = COL_EACH_AT_MOST_ENABLED, FillWeight = 50 });
            dgvRules.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Value", DataPropertyName = COL_EACH_AT_MOST_VALUE, Name = COL_EACH_AT_MOST_VALUE, FillWeight = 70 });
            dgvRules.Columns.Add(createOpColumn("Row Operator", COL_ROW_OPERATOR));

            dgvRules.CellPainting -= dgvRules_CellPainting;
            dgvRules.CellPainting += dgvRules_CellPainting;

            dgvRules.MouseDown -= dgvRules_MouseDown;
            dgvRules.MouseMove -= dgvRules_MouseMove;
            dgvRules.DragOver -= dgvRules_DragOver;
            dgvRules.DragDrop -= dgvRules_DragDrop;
            dgvRules.DragLeave -= dgvRules_DragLeave;
            dgvRules.RowPostPaint -= dgvRules_RowPostPaint;
            dgvRules.KeyDown -= dgvRules_KeyDown;

            dgvRules.MouseDown += dgvRules_MouseDown;
            dgvRules.MouseMove += dgvRules_MouseMove;
            dgvRules.DragOver += dgvRules_DragOver;
            dgvRules.DragDrop += dgvRules_DragDrop;
            dgvRules.DragLeave += dgvRules_DragLeave;
            dgvRules.RowPostPaint += dgvRules_RowPostPaint;
            dgvRules.KeyDown += dgvRules_KeyDown;

            dgvRules.AllowDrop = true;
            dgvRules.ResumeLayout();
        }

        private void AutoResizeForm()
        {
            // Auto-size columns to fit their content
            dgvRules.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);

            // Calculate total width needed
            int totalWidth = dgvRules.RowHeadersWidth + SystemInformation.VerticalScrollBarWidth + 20; // padding
            foreach (DataGridViewColumn col in dgvRules.Columns)
            {
                totalWidth += col.Width;
            }

            // Calculate total height needed (limit to reasonable size)
            int totalHeight = dgvRules.ColumnHeadersHeight + panelBottom.Height + 50; // padding and borders
            int visibleRowsHeight = Math.Min(dgvRules.Rows.Count * dgvRules.Rows[0].Height, 600); // max 600px for rows
            totalHeight += visibleRowsHeight;

            // Set form size with reasonable limits
            int newWidth = Math.Max(800, Math.Min(totalWidth, Screen.PrimaryScreen.WorkingArea.Width - 100));
            int newHeight = Math.Max(400, Math.Min(totalHeight, Screen.PrimaryScreen.WorkingArea.Height - 100));

            this.Size = new Size(newWidth, newHeight);

            // After sizing, switch back to Fill mode for responsive behavior
            dgvRules.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void PopulateRules()
        {
            var properties = typeof(ItemStats).GetProperties()
                .Where(p => p.Name != nameof(ItemStats.Enchant) && (p.PropertyType == typeof(int) || p.PropertyType == typeof(double)));

            var rulesList = new List<ValidationRuleModel>();
            foreach (var prop in properties)
            {
                rulesList.Add(new ValidationRuleModel { PropertyName = prop.Name, PropInfo = prop });
            }

            _rules = new BindingList<ValidationRuleModel>(rulesList);

            dgvRules.SuspendLayout();
            dgvRules.DataSource = _rules;
            dgvRules.ResumeLayout();

            UpdateCellStates();
        }

        private void UpdateCellStates()
        {
            dgvRules.SuspendLayout();
            for (int i = 0; i < dgvRules.Rows.Count; i++)
            {
                var row = dgvRules.Rows[i];
                if (row.DataBoundItem is not ValidationRuleModel ruleModel) continue;

                // Helper to set cell state by column name
                void setCellState(string colName, bool isEnabled)
                {
                    var cell = row.Cells[colName];
                    cell.ReadOnly = !isEnabled;
                    if (dgvRules.Columns[colName] is DataGridViewComboBoxColumn)
                    {
                        cell.Style = isEnabled ? _enabledOpStyle : _disabledOpStyle;
                    }
                }

                // Value cells
                setCellState(COL_SUM_AT_LEAST_VALUE, ruleModel.SumAtLeastEnabled);
                setCellState(COL_SUM_AT_MOST_VALUE, ruleModel.SumAtMostEnabled);
                setCellState(COL_EACH_AT_LEAST_VALUE, ruleModel.EachAtLeastEnabled);
                setCellState(COL_EACH_AT_MOST_VALUE, ruleModel.EachAtMostEnabled);

                // Operator cells
                setCellState(COL_OP1, ruleModel.SumAtLeastEnabled && ruleModel.SumAtMostEnabled);
                setCellState(COL_OP2, ruleModel.SumAtMostEnabled && ruleModel.EachAtLeastEnabled);
                setCellState(COL_OP3, ruleModel.EachAtLeastEnabled && ruleModel.EachAtMostEnabled);

                // Row operator
                bool isLastRow = (i == dgvRules.Rows.Count - 1);
                if (isLastRow)
                {
                    setCellState(COL_ROW_OPERATOR, false);
                }
                else
                {
                    var nextRuleModel = dgvRules.Rows[i + 1].DataBoundItem as ValidationRuleModel;
                    setCellState(COL_ROW_OPERATOR, ruleModel.IsActive && nextRuleModel?.IsActive == true);
                }
            }
            dgvRules.Invalidate();
            dgvRules.ResumeLayout();
        }

        private void dgvRules_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            var colName = dgvRules.Columns[dgvRules.CurrentCell.ColumnIndex].DataPropertyName;

            bool isValueColumn = colName == COL_SUM_AT_LEAST_VALUE ||
                                colName == COL_SUM_AT_MOST_VALUE ||
                                colName == COL_EACH_AT_LEAST_VALUE ||
                                colName == COL_EACH_AT_MOST_VALUE;

            _initialCellSelectionColor = dgvRules.CurrentCell.Style.SelectionBackColor;
            if (isValueColumn && e.Control is TextBox textBox)
            {
                textBox.KeyDown -= EditingControl_KeyDown;
                textBox.KeyDown += EditingControl_KeyDown;

                // Set the editing control's background to match the cell's background
                var cell = dgvRules.CurrentCell;
                textBox.BackColor = cell.Style.BackColor != Color.Empty
                    ? cell.Style.BackColor
                    : _backColorValidValue;
            }
            else if (e.Control is ComboBox comboBox)
            {
                comboBox.DrawItem -= ComboBox_DrawItem;
                comboBox.DrawMode = DrawMode.OwnerDrawFixed;
                comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
                comboBox.DrawItem += ComboBox_DrawItem;
            }
        }

        private void EditingControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                e.SuppressKeyPress = true;
                if (sender is TextBox textBox)
                {
                    textBox.Text = string.Empty;
                }
            }
            else if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;

                // Commit the edit and move to next cell
                dgvRules.EndEdit();
                dgvRules.CommitEdit(DataGridViewDataErrorContexts.Commit);

                // Move to next row, same column
                if (dgvRules.CurrentCell.RowIndex < dgvRules.Rows.Count - 1)
                {
                    dgvRules.CurrentCell = dgvRules[dgvRules.CurrentCell.ColumnIndex, dgvRules.CurrentCell.RowIndex + 1];
                }
            }
        }

        private void ComboBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            // Ignore if the index is invalid
            if (e.Index < 0) { return; }

            var comboBox = sender as ComboBox;
            string text = comboBox.Items[e.Index].ToString();

            // Draw the background of the item
            e.DrawBackground();

            // Use TextRenderer for high-quality text drawing
            TextRenderer.DrawText(
                e.Graphics,
                text,
                e.Font,
                e.Bounds,
                e.ForeColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
            );

            // Draw the focus rectangle if the mouse is over the item
            e.DrawFocusRectangle();
        }

        /// <summary>
        /// Opens the dropdown list on the first click for combo box cells.
        /// </summary>
        private void dgvRules_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            // Check if the clicked column is a combo box column and not read-only
            if (dgvRules.Columns[e.ColumnIndex] is DataGridViewComboBoxColumn && !dgvRules[e.ColumnIndex, e.RowIndex].ReadOnly)
            {
                dgvRules.BeginEdit(true);
                if (dgvRules.EditingControl is ComboBox comboBox)
                {
                    comboBox.DroppedDown = true;
                }
            }
            else if (dgvRules.Columns[e.ColumnIndex] is DataGridViewCheckBoxColumn)
            {
                bool currentValue = (bool?)dgvRules[e.ColumnIndex, e.RowIndex].Value ?? false;
                dgvRules[e.ColumnIndex, e.RowIndex].Value = !currentValue;
                dgvRules.CommitEdit(DataGridViewDataErrorContexts.Commit);
                dgvRules.InvalidateCell(e.ColumnIndex, e.RowIndex);
                dgvRules.EndEdit();
            }
        }

        private void dgvRules_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            // leave headers untouched
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            // Only handle ComboBox columns
            if (dgvRules.Columns[e.ColumnIndex] is not DataGridViewComboBoxColumn) return;

            // Get the cell and choose the style we want (enabled vs disabled)
            var cell = dgvRules[e.ColumnIndex, e.RowIndex];
            var style = cell.ReadOnly ? _disabledOpStyle : _enabledOpStyle;

            // Fill the complete cell rectangle with our desired backcolor (this ensures no white band)
            using (var b = new SolidBrush(style.BackColor))
            {
                e.Graphics.FillRectangle(b, e.CellBounds);
            }

            string text = e.FormattedValue?.ToString() ?? string.Empty;

            // Use TextRenderer for crisp text and high-DPI correctness
            var textFlags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis;
            int glyphWidth = SystemInformation.VerticalScrollBarWidth + 4; // ~18-20 px: keeps a reasonable button width
            var textRect = new Rectangle(e.CellBounds.X, e.CellBounds.Y, e.CellBounds.Width - glyphWidth, e.CellBounds.Height); // or e.CellBounds
            var buttonRect = new Rectangle(e.CellBounds.Right - glyphWidth, e.CellBounds.Y, glyphWidth, e.CellBounds.Height);

            TextRenderer.DrawText(e.Graphics, text, e.CellStyle.Font, textRect, style.ForeColor, textFlags);
            if (ComboBoxRenderer.IsSupported)
            {
                // Draw native-looking drop-down button on the right
                ComboBoxRenderer.DrawDropDownButton(e.Graphics, buttonRect, ComboBoxState.Hot);
            }
            else
            {
                // Simple fallback: draw a tiny triangle arrow
                Point center = new(buttonRect.Left + buttonRect.Width / 2, buttonRect.Top + buttonRect.Height / 2);
                var p1 = new Point(center.X - 5, center.Y - 1);
                var p2 = new Point(center.X + 5, center.Y - 1);
                var p3 = new Point(center.X, center.Y + 3);
                using (var br = new SolidBrush(style.ForeColor))
                {
                    e.Graphics.FillPolygon(br, new[] { p1, p2, p3 });
                }
            }

            e.Paint(e.CellBounds, DataGridViewPaintParts.Border);

            // Draw a focus rectangle if needed (keeps existing UX consistent)
            if ((e.State & DataGridViewElementStates.Selected) == DataGridViewElementStates.Selected)
            {
                var focusRect = e.CellBounds;
                //focusRect.Inflate(-2, -2);
                ControlPaint.DrawFocusRectangle(e.Graphics, focusRect);
            }

            e.Handled = true;
        }

        private void dgvRules_MouseDown(object sender, MouseEventArgs e)
        {
            var hitTest = dgvRules.HitTest(e.X, e.Y);

            // Only allow drag from row headers or cells, not from column headers
            if (hitTest.RowIndex >= 0)
            {
                _rowIndexFromMouseDown = hitTest.RowIndex;
            }
            else
            {
                _rowIndexFromMouseDown = -1;
            }
        }

        private void dgvRules_MouseMove(object sender, MouseEventArgs e)
        {
            // Don't allow drag if currently editing a cell
            if (dgvRules.IsCurrentCellInEditMode)
            {
                return;
            }

            if ((e.Button & MouseButtons.Left) == MouseButtons.Left && _rowIndexFromMouseDown >= 0)
            {
                dgvRules.DoDragDrop(dgvRules.Rows[_rowIndexFromMouseDown], DragDropEffects.Move);
            }
        }

        private void dgvRules_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;

            Point clientPoint = dgvRules.PointToClient(new Point(e.X, e.Y));
            int rowIndex = dgvRules.HitTest(clientPoint.X, clientPoint.Y).RowIndex;

            // update insertion line
            if (rowIndex != _insertionRowIndex)
            {
                _insertionRowIndex = rowIndex;
                dgvRules.Invalidate();
            }

            // --- auto-scroll ---
            int scrollZone = 30; // px near top/bottom edge
            if (clientPoint.Y < scrollZone)
            {
                // scroll up
                if (dgvRules.FirstDisplayedScrollingRowIndex > 1)
                    dgvRules.FirstDisplayedScrollingRowIndex -= 2;
            }
            else if (clientPoint.Y > dgvRules.Height - scrollZone)
            {
                // scroll down
                int last = dgvRules.Rows.Count - 1;
                if (dgvRules.FirstDisplayedScrollingRowIndex < last - 1)
                    dgvRules.FirstDisplayedScrollingRowIndex += 2;
            }
        }

        private void dgvRules_DragDrop(object sender, DragEventArgs e)
        {
            Point clientPoint = dgvRules.PointToClient(new Point(e.X, e.Y));
            _rowIndexOfItemUnderMouseToDrop = dgvRules.HitTest(clientPoint.X, clientPoint.Y).RowIndex;

            if (_rowIndexOfItemUnderMouseToDrop < 0 || _rowIndexFromMouseDown < 0 || _rowIndexFromMouseDown == _rowIndexOfItemUnderMouseToDrop)
            {
                _insertionRowIndex = -1;
                _rowIndexFromMouseDown = -1;
                dgvRules.Invalidate();
                return;
            }

            var rowToMove = _rules[_rowIndexFromMouseDown];
            _rules.RemoveAt(_rowIndexFromMouseDown);
            _rules.Insert(_rowIndexOfItemUnderMouseToDrop, rowToMove);

            _insertionRowIndex = -1;
            _rowIndexFromMouseDown = -1;

            dgvRules.ClearSelection();
            dgvRules.Rows[_rowIndexOfItemUnderMouseToDrop].Selected = true;
            dgvRules.CurrentCell = dgvRules.Rows[_rowIndexOfItemUnderMouseToDrop].Cells[0];

            dgvRules.Refresh();
            UpdateCellStates();
        }

        private void dgvRules_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            if (_insertionRowIndex < 0) return;

            using (var pen = new Pen(Color.Red, 2))
            {
                if (e.RowIndex == _insertionRowIndex)
                {
                    int y = e.RowBounds.Top;
                    e.Graphics.DrawLine(pen, e.RowBounds.Left, y, e.RowBounds.Right, y);
                }
                else if (_insertionRowIndex == dgvRules.Rows.Count)
                {
                    // line after the last row
                    if (e.RowIndex == dgvRules.Rows.Count - 1)
                    {
                        int y = e.RowBounds.Bottom;
                        e.Graphics.DrawLine(pen, e.RowBounds.Left, y, e.RowBounds.Right, y);
                    }
                }
            }
        }

        private void dgvRules_KeyDown(object sender, KeyEventArgs e)
        {
            if (dgvRules.CurrentRow == null) return;

            int index = dgvRules.CurrentRow.Index;
            if (e.Control && e.KeyCode == Keys.Up && index > 0)
            {
                var item = _rules[index];
                _rules.RemoveAt(index);
                _rules.Insert(index - 1, item);

                dgvRules.ClearSelection();
                dgvRules.Rows[index - 1].Selected = true;
                dgvRules.CurrentCell = dgvRules.Rows[index - 1].Cells[0];
                dgvRules.Refresh();
                UpdateCellStates();
                e.Handled = true;
            }
            else if (e.Control && e.KeyCode == Keys.Down && index < _rules.Count - 1)
            {
                var item = _rules[index];
                _rules.RemoveAt(index);
                _rules.Insert(index + 1, item);

                dgvRules.ClearSelection();
                dgvRules.Rows[index + 1].Selected = true;
                dgvRules.CurrentCell = dgvRules.Rows[index + 1].Cells[0];
                dgvRules.Refresh();
                UpdateCellStates();
                e.Handled = true;
            }
        }

        private void dgvRules_DragLeave(object sender, EventArgs e)
        {
            // Clear the insertion line when drag leaves the control
            _insertionRowIndex = -1;
            dgvRules.Invalidate();
        }

        private void btnCreateValidator_MouseEnter(object sender, EventArgs e)
        {
            btnCreateValidator.BackColor = Color.FromArgb(90, 150, 200);
        }

        private void btnCreateValidator_MouseLeave(object sender, EventArgs e)
        {
            btnCreateValidator.BackColor = Color.FromArgb(70, 130, 180);
        }

        private void CustomValidator_FormClosing(object sender, FormClosingEventArgs e)
        {
            _isClosing = true;

            try
            {
                // Force end any edit operation without validation
                if (dgvRules.IsCurrentCellInEditMode)
                {
                    dgvRules.CancelEdit();
                    dgvRules.EndEdit();
                }

                dgvRules.CurrentCell = null;

                // Clear all errors
                foreach (DataGridViewRow row in dgvRules.Rows)
                {
                    row.ErrorText = string.Empty;
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        cell.ErrorText = string.Empty;
                        if (cell.Style.BackColor == Color.FromArgb(255, 200, 200))
                        {
                            cell.Style.BackColor = Color.White;
                        }
                    }
                }
            }
            catch
            {
                // Ignore errors
            }
        }
    }
}
