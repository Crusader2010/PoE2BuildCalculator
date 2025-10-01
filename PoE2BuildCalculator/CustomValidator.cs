using Domain.Combinations;
using Domain.Main;

namespace PoE2BuildCalculator
{
    public partial class CustomValidator : Form
    {
        // This will hold the final, combined validation function.
        private Func<List<Item>, bool> _masterValidator;
        private List<ValidationRuleModel> _rules;
        private readonly MainForm _ownerForm;

        private DataGridViewCellStyle _enabledOpStyle;
        private DataGridViewCellStyle _disabledOpStyle;

        public CustomValidator(MainForm ownerForm)
        {
            ArgumentNullException.ThrowIfNull(ownerForm, nameof(ownerForm));
            InitializeComponent();
            _ownerForm = ownerForm;
        }

        private void dgvRules_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            // Check if the changed cell is one of our checkbox columns
            bool isCheckboxColumn = e.ColumnIndex == 1 || e.ColumnIndex == 4 || e.ColumnIndex == 7 || e.ColumnIndex == 10;
            if (isCheckboxColumn)
            {
                dgvRules.CommitEdit(DataGridViewDataErrorContexts.Commit); // Ensure the model is updated first

                var ruleModel = _rules[e.RowIndex];
                bool isChecked = (bool)(dgvRules[e.ColumnIndex, e.RowIndex]?.Value ?? false);

                // --- New: Clear corresponding value if unchecked ---
                if (!isChecked)
                {
                    switch (e.ColumnIndex)
                    {
                        case 1: ruleModel.SumAtLeastValue = string.Empty; break;
                        case 4: ruleModel.SumAtMostValue = string.Empty; break;
                        case 7: ruleModel.EachAtLeastValue = string.Empty; break;
                        case 10: ruleModel.EachAtMostValue = string.Empty; break;
                    }
                }

                UpdateCellStates();
            }
        }

        private void dgvRules_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            // We only care about the value columns
            bool isValueColumn = e.ColumnIndex == 2 || e.ColumnIndex == 4 || e.ColumnIndex == 7 || e.ColumnIndex == 9 || e.ColumnIndex == 11;
            if (!isValueColumn) return;

            var rule = _rules[e.RowIndex];
            bool isValidationRequired = false;

            // Check if the corresponding checkbox is checked
            switch (e.ColumnIndex)
            {
                case 2: isValidationRequired = rule.SumAtLeastEnabled; break;
                case 5: isValidationRequired = rule.SumAtMostEnabled; break;
                case 8: isValidationRequired = rule.EachAtLeastEnabled; break;
                case 11: isValidationRequired = rule.EachAtMostEnabled; break;
            }

            // --- New: Only validate if the checkbox is checked ---
            if (!isValidationRequired)
            {
                e.Cancel = false;
                dgvRules.Rows[e.RowIndex].ErrorText = null;
                return;
            }

            var propType = rule.PropInfo.PropertyType;
            string value = e.FormattedValue?.ToString();

            // Allow empty values for checked boxes, but treat as an error during final validation
            if (string.IsNullOrWhiteSpace(value)) return;

            bool isValid = propType == typeof(int)
                ? int.TryParse(value, out _)
                : double.TryParse(value, out _);

            if (!isValid)
            {
                e.Cancel = true;
                dgvRules.Rows[e.RowIndex].ErrorText = $"Invalid value for rule {rule.PropertyName}";
                //MessageBox.Show($"Please enter a valid {propType.Name} for {rule.PropertyName}.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                dgvRules.Rows[e.RowIndex].ErrorText = null;
            }
        }

        private void btnCreateValidator_Click(object sender, EventArgs e)
        {
            // A helper function to combine two boolean results based on an operator string
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
                var activeRules = _rules.Where(r => r.SumAtLeastEnabled || r.SumAtMostEnabled || r.EachAtLeastEnabled || r.EachAtMostEnabled).ToList();

                if (activeRules.Count == 0)
                {
                    _masterValidator = (items) => true;
                    MessageBox.Show("No rules defined. Validator will always return true.", "Validator Created", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Generate a final validator function for each active row
                var rowValidators = new List<Func<List<Item>, bool>>();
                foreach (var rule in activeRules)
                {
                    bool propertyValidator(List<Item> items)
                    {
                        var conditions = new List<Func<List<Item>, bool>>();
                        var operators = new List<string>();

                        // Gather all active conditions and their subsequent operators for the current row
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

                        if (conditions.Count == 0) return true; // Should not happen due to activeRules filter

                        // Sequentially apply the conditions using the selected operators
                        bool result = conditions[0](items);
                        for (int i = 1; i < conditions.Count; i++)
                        {
                            result = combine(operators[i - 1], result, conditions[i](items));
                        }
                        return result;
                    }
                    rowValidators.Add(propertyValidator);
                }

                // Combine all the row validators using the RowOperators
                _masterValidator = (items) =>
                {
                    if (rowValidators.Count == 0) return true;

                    // Sequentially apply the row validators using the selected RowOperators
                    bool finalResult = rowValidators[0](items);
                    for (int i = 1; i < rowValidators.Count; i++)
                    {
                        // The operator is taken from the previous rule in the list
                        string rowOp = activeRules[i - 1].RowOperator;
                        finalResult = combine(rowOp, finalResult, rowValidators[i](items));
                    }
                    return finalResult;
                };

                _ownerForm._itemValidatorFunction = _masterValidator;

                MessageBox.Show("Validator function created successfully! Ready to test.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (FormatException)
            {
                MessageBox.Show("Error: A checked condition has an empty or invalid value.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred: {ex}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CustomValidator_Load(object sender, EventArgs e)
        {
            SetupDataGridView();
            PopulateRules();
        }

        private void SetupDataGridView()
        {
            _enabledOpStyle = new DataGridViewCellStyle
            {
                BackColor = Color.GreenYellow,
                ForeColor = Color.Black,
                SelectionBackColor = Color.AliceBlue,
            };
            _disabledOpStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(224, 224, 224), // A light gray
                ForeColor = Color.DarkGray, // "Washed out" text
                SelectionBackColor = Color.FromArgb(224, 224, 224)
            };

            dgvRules.SuspendLayout();
            dgvRules.AutoGenerateColumns = false;
            dgvRules.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgvRules.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvRules.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            var opItems = new[] { "AND", "OR", "XOR" };

            // Helper function to create a combo box column
            DataGridViewComboBoxColumn createOpColumn(string header, string dataProperty)
            {
                var col = new DataGridViewComboBoxColumn
                {
                    HeaderText = header,
                    DataPropertyName = dataProperty,
                    FlatStyle = FlatStyle.Standard,
                    FillWeight = 45,
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton
                };
                col.Items.AddRange(opItems);
                return col;
            }

            dgvRules.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Statistic", DataPropertyName = "PropertyName", ReadOnly = true, FillWeight = 150 });

            // Condition 1
            dgvRules.Columns.Add(new DataGridViewCheckBoxColumn { HeaderText = "Sum ≥", DataPropertyName = "SumAtLeastEnabled", FillWeight = 50 });
            dgvRules.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Value", DataPropertyName = "SumAtLeastValue", FillWeight = 70 });

            dgvRules.Columns.Add(createOpColumn("Op", "Op1")); // Operator 1

            // Condition 2
            dgvRules.Columns.Add(new DataGridViewCheckBoxColumn { HeaderText = "Sum ≤", DataPropertyName = "SumAtMostEnabled", FillWeight = 50 });
            dgvRules.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Value", DataPropertyName = "SumAtMostValue", FillWeight = 70 });

            dgvRules.Columns.Add(createOpColumn("Op", "Op2")); // Operator 2

            // Condition 3
            dgvRules.Columns.Add(new DataGridViewCheckBoxColumn { HeaderText = "Each ≥", DataPropertyName = "EachAtLeastEnabled", FillWeight = 50 });
            dgvRules.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Value", DataPropertyName = "EachAtLeastValue", FillWeight = 70 });

            dgvRules.Columns.Add(createOpColumn("Op", "Op3")); // Operator 3

            // Condition 4
            dgvRules.Columns.Add(new DataGridViewCheckBoxColumn { HeaderText = "Each ≤", DataPropertyName = "EachAtMostEnabled", FillWeight = 50 });
            dgvRules.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Value", DataPropertyName = "EachAtMostValue", FillWeight = 70 });

            // Final Row Operator
            dgvRules.Columns.Add(createOpColumn("Row Op", "RowOperator"));
            dgvRules.ResumeLayout();
        }

        private void PopulateRules()
        {
            _rules = [];

            var properties = typeof(ItemStats).GetProperties()
                .Where(p => p.Name != nameof(ItemStats.Enchant) && (p.PropertyType == typeof(int) || p.PropertyType == typeof(double)));

            foreach (var prop in properties)
            {
                _rules.Add(new ValidationRuleModel { PropertyName = prop.Name, PropInfo = prop });
            }

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

                // --- Value Cells (Unchanged) ---
                row.Cells[2].ReadOnly = !ruleModel.SumAtLeastEnabled;
                row.Cells[5].ReadOnly = !ruleModel.SumAtMostEnabled;
                row.Cells[8].ReadOnly = !ruleModel.EachAtLeastEnabled;
                row.Cells[11].ReadOnly = !ruleModel.EachAtMostEnabled;

                // --- Operator Cells (Updated with Style logic) ---
                // Helper to apply style and readonly state
                void setOpCellState(int colIndex, bool isEnabled)
                {
                    row.Cells[colIndex].ReadOnly = !isEnabled;
                    row.Cells[colIndex].Style = isEnabled ? _enabledOpStyle : _disabledOpStyle;
                }

                setOpCellState(3, ruleModel.SumAtLeastEnabled && ruleModel.SumAtMostEnabled);    // Op1
                setOpCellState(6, ruleModel.SumAtMostEnabled && ruleModel.EachAtLeastEnabled);   // Op2
                setOpCellState(9, ruleModel.EachAtLeastEnabled && ruleModel.EachAtMostEnabled);  // Op3

                // --- RowOperator (Updated with Style logic) ---
                bool isLastRow = (i == dgvRules.Rows.Count - 1);
                if (isLastRow)
                {
                    setOpCellState(12, false); // Always disabled
                }
                else
                {
                    var nextRuleModel = dgvRules.Rows[i + 1].DataBoundItem as ValidationRuleModel;
                    setOpCellState(12, ruleModel.IsActive && nextRuleModel.IsActive);
                }
            }
            dgvRules.Invalidate(); // Use Invalidate for a smoother visual update
            dgvRules.ResumeLayout();
        }

        private void dgvRules_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            // We only care about our value columns
            int colIndex = dgvRules.CurrentCell.ColumnIndex;
            bool isValueColumn = colIndex == 2 || colIndex == 5 || colIndex == 8 || colIndex == 11;

            if (isValueColumn && e.Control is TextBox textBox)
            {
                // Remove handler first to avoid multiple subscriptions
                textBox.KeyDown -= EditingControl_KeyDown;
                textBox.KeyDown += EditingControl_KeyDown;
            }
        }

        private void EditingControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                e.SuppressKeyPress = true; // Prevent the "ding" sound
                if (sender is TextBox textBox)
                {
                    textBox.Text = string.Empty; // Clear the text
                }
            }
        }

        /// <summary>
        /// Forces checkbox changes to be committed instantly, triggering CellValueChanged.
        /// </summary>
        private void dgvRules_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvRules.IsCurrentCellDirty)
            {
                if (dgvRules.CurrentCell is DataGridViewCheckBoxCell)
                {
                    dgvRules.CommitEdit(DataGridViewDataErrorContexts.Commit);
                }
            }
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
        }
    }
}
