using Domain.Main;
using Domain.Validation;

namespace Domain.UserControls
{
    public partial class ItemStatGroupValidatorUserControl : UserControl
    {
        private ValidationGroupModel _group;
        private static readonly Color HEADER_COLOR = Color.FromArgb(70, 130, 180);

        public event EventHandler DeleteRequested;
        public event EventHandler ValidationChanged;

        public ValidationGroupModel Group
        {
            get => _group;
            set
            {
                _group = value;
                UpdateDisplay();
            }
        }

        public ItemStatGroupValidatorUserControl()
        {
            InitializeComponent();
        }

        private void ItemStatGroupValidatorUserControl_Load(object sender, EventArgs e)
        {
            if (_group != null)
            {
                UpdateDisplay();
            }
        }

        private void UpdateDisplay()
        {
            if (_group == null) return;

            lblGroupName.Text = _group.GroupName;

            // Update constraints
            chkMin.Checked = _group.IsMinEnabled;
            numMin.Value = (decimal)(_group.MinValue ?? 0.00);
            numMin.Enabled = _group.IsMinEnabled;

            chkMax.Checked = _group.IsMaxEnabled;
            numMax.Value = (decimal)(_group.MaxValue ?? 0.00);
            numMax.Enabled = _group.IsMaxEnabled;

            // Update stats combobox
            UpdateStatsComboBox();

            // Update stats listbox
            RefreshStatsListBox();

            // Update operator
            cmbOperator.SelectedItem = _group.GroupOperator ?? "AND";

            ValidateGroup();
        }

        private void UpdateStatsComboBox()
        {
            var availableProps = typeof(ItemStats).GetProperties()
                .Where(p => p.Name != nameof(ItemStats.Enchant) &&
                           (p.PropertyType == typeof(int) || p.PropertyType == typeof(double)))
                .OrderBy(p => p.Name)
                .ToList();

            cmbStats.Items.Clear();
            foreach (var prop in availableProps)
            {
                if (!_group.Stats.Any(s => s.PropertyName == prop.Name))
                {
                    cmbStats.Items.Add(prop.Name);
                }
            }
        }

        private void RefreshStatsListBox()
        {
            statsListBox.Items.Clear();
            statsListBox.Items.AddRange([.. _group.Stats.Select(s => s.PropertyName)]);
        }

        private void ValidateGroup()
        {
            bool isValid = true;
            string errorMsg = "";

            if (!_group.IsMinEnabled && !_group.IsMaxEnabled)
            {
                errorMsg = "Enable Min or Max";
                isValid = false;
            }
            else if (_group.IsMinEnabled && _group.IsMaxEnabled &&
                     _group.MinValue.HasValue && _group.MaxValue.HasValue &&
                     _group.MinValue.Value >= _group.MaxValue.Value)
            {
                errorMsg = "Min must be < Max";
                isValid = false;
            }

            lblValidation.Text = errorMsg;
            lblValidation.ForeColor = isValid ? Color.Green : Color.Red;

            ValidationChanged?.Invoke(this, EventArgs.Empty);
        }

        public void UpdateOperatorVisibility(bool visible)
        {
            pnlOperatorContainer.Visible = visible;
            cmbOperator.Enabled = visible;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DeleteRequested?.Invoke(this, EventArgs.Empty);
        }

        private void btnAddStat_Click(object sender, EventArgs e)
        {
            if (cmbStats.SelectedItem is null)
            {
                MessageBox.Show("Please select a stat.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string propName = cmbStats.SelectedItem.ToString();
            var propInfo = typeof(ItemStats).GetProperty(propName);

            if (_group.Stats.Any(s => s.PropertyName == propName))
            {
                MessageBox.Show("Stat already in group.", "Duplicate", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _group.Stats.Add(new GroupStatModel
            {
                PropInfo = propInfo,
                PropertyName = propName,
                Operator = "+"
            });

            cmbStats.Items.Remove(propName);
            cmbStats.SelectedIndex = -1;
            RefreshStatsListBox();
            ValidateGroup();
        }

        private void chkMin_CheckedChanged(object sender, EventArgs e)
        {
            _group.IsMinEnabled = chkMin.Checked;
            numMin.Enabled = chkMin.Checked;
            ValidateGroup();
        }

        private void numMin_ValueChanged(object sender, EventArgs e)
        {
            _group.MinValue = (double)numMin.Value;
            ValidateGroup();
        }

        private void numMin_Leave(object sender, EventArgs e)
        {
            ValidateGroup();
        }

        private void chkMax_CheckedChanged(object sender, EventArgs e)
        {
            _group.IsMaxEnabled = chkMax.Checked;
            numMax.Enabled = chkMax.Checked;
            if (chkMax.Checked && !_group.MaxValue.HasValue)
                _group.MaxValue = (double)numMax.Value;
            ValidateGroup();
        }

        private void numMax_ValueChanged(object sender, EventArgs e)
        {
            if (chkMax.Checked)
            {
                _group.MaxValue = (double)numMax.Value;
                ValidateGroup();
            }
        }

        private void numMax_Leave(object sender, EventArgs e)
        {
            ValidateGroup();
        }

        private void cmbOperator_SelectedIndexChanged(object sender, EventArgs e)
        {
            _group.GroupOperator = cmbOperator.SelectedItem?.ToString();
        }

        private void contentPanel_Click(object sender, EventArgs e)
        {
            ValidateGroup();
        }

        private void statsListBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= _group.Stats.Count) return;

            var stat = _group.Stats[e.Index];
            bool isLastStat = e.Index == _group.Stats.Count - 1;

            e.DrawBackground();
            var bounds = e.Bounds;

            // Draw stat name
            using (var brush = new SolidBrush(e.ForeColor))
            {
                var textRect = new Rectangle(bounds.Left + 4, bounds.Top + 9, bounds.Width - 100, bounds.Height);
                e.Graphics.DrawString(stat.PropertyName, e.Font, brush, textRect);
            }

            // Operator dropdown (only if NOT last stat)
            if (!isLastStat)
            {
                var opRect = new Rectangle(bounds.Right - 95, bounds.Top + 5, 50, bounds.Height - 10);
                using var bgBrush = new SolidBrush(Color.FromArgb(240, 240, 240));
                e.Graphics.FillRectangle(bgBrush, opRect);
                e.Graphics.DrawRectangle(Pens.Gray, opRect);

                using var textBrush = new SolidBrush(Color.Black);
                var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                e.Graphics.DrawString(stat.Operator, e.Font, textBrush, opRect, sf);
            }

            // Up button - taller
            var upRect = new Rectangle(bounds.Right - 42, bounds.Top + 3, 18, 14);
            using (var btnBrush = new SolidBrush(Color.FromArgb(100, 150, 200)))
                e.Graphics.FillRectangle(btnBrush, upRect);
            e.Graphics.DrawRectangle(Pens.DarkBlue, upRect);
            using (var textBrush = new SolidBrush(Color.White))
            {
                var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                e.Graphics.DrawString("▲", new Font("Arial", 7), textBrush, upRect, sf);
            }

            // Down button - taller
            var downRect = new Rectangle(bounds.Right - 42, bounds.Top + 18, 18, 14);
            using (var btnBrush = new SolidBrush(Color.FromArgb(100, 150, 200)))
                e.Graphics.FillRectangle(btnBrush, downRect);
            e.Graphics.DrawRectangle(Pens.DarkBlue, downRect);
            using (var textBrush = new SolidBrush(Color.White))
            {
                var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                e.Graphics.DrawString("▼", new Font("Arial", 7), textBrush, downRect, sf);
            }

            // Remove button - matches combined height of up+down buttons including gap
            var removeRect = new Rectangle(bounds.Right - 22, bounds.Top + 3, 18, 29);
            using (var btnBrush = new SolidBrush(Color.FromArgb(200, 50, 50)))
                e.Graphics.FillRectangle(btnBrush, removeRect);
            using (var textBrush = new SolidBrush(Color.White))
            {
                var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                e.Graphics.DrawString("×", new Font("Segoe UI", 10, FontStyle.Bold), textBrush, removeRect, sf);
            }

            e.DrawFocusRectangle();
        }

        private void statsListBox_MouseClick(object sender, MouseEventArgs e)
        {
            int index = statsListBox.IndexFromPoint(e.Location);
            if (index < 0 || index >= _group.Stats.Count) return;

            var bounds = statsListBox.GetItemRectangle(index);
            bool isLastStat = index == _group.Stats.Count - 1;

            // Operator dropdown
            if (!isLastStat)
            {
                var opRect = new Rectangle(bounds.Right - 95, bounds.Top + 5, 50, bounds.Height - 10);
                if (opRect.Contains(e.Location))
                {
                    ShowOperatorMenu(_group.Stats[index], e.Location);
                    return;
                }
            }

            // Up button
            var upRect = new Rectangle(bounds.Right - 42, bounds.Top + 3, 18, 14);
            if (upRect.Contains(e.Location) && index > 0)
            {
                (_group.Stats[index], _group.Stats[index - 1]) = (_group.Stats[index - 1], _group.Stats[index]);
                RefreshStatsListBox();
                return;
            }

            // Down button
            var downRect = new Rectangle(bounds.Right - 42, bounds.Top + 18, 18, 14);
            if (downRect.Contains(e.Location) && index < _group.Stats.Count - 1)
            {
                (_group.Stats[index], _group.Stats[index + 1]) = (_group.Stats[index + 1], _group.Stats[index]);
                RefreshStatsListBox();
                return;
            }

            // Remove button
            var removeRect = new Rectangle(bounds.Right - 22, bounds.Top + 3, 18, 29);
            if (removeRect.Contains(e.Location))
            {
                string propName = _group.Stats[index].PropertyName;
                _group.Stats.RemoveAt(index);
                RefreshStatsListBox();

                // Re-add to dropdown
                if (!cmbStats.Items.Contains(propName))
                {
                    var items = cmbStats.Items.Cast<string>().Append(propName).OrderBy(x => x).ToList();
                    cmbStats.Items.Clear();
                    cmbStats.Items.AddRange([.. items]);
                }

                ValidateGroup();
            }
        }

        private void ShowOperatorMenu(GroupStatModel stat, Point location)
        {
            var menu = new ContextMenuStrip();
            foreach (var op in new[] { "+", "-", "*", "/" })
            {
                var item = new ToolStripMenuItem(op) { Checked = stat.Operator == op };
                item.Click += (s, e) =>
                {
                    stat.Operator = op;
                    statsListBox.Invalidate();
                };
                menu.Items.Add(item);
            }
            menu.Closed += (s, e) => { menu.Close(); };
            menu.Show(statsListBox, location);
        }
    }
}