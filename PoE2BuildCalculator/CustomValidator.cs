using Domain.Main;
using Domain.Validation;
using System.ComponentModel;

namespace PoE2BuildCalculator
{
    public partial class CustomValidator : Form
    {
        private Func<List<Item>, bool> _masterValidator = x => true;
        private readonly BindingList<ValidationGroupModel> _groups = [];
        private readonly MainForm _ownerForm;
        private int _nextGroupId = 1;
        private Panel _groupsContainer;
        private Button _btnAddGroup;

        // Calculated layout constants (set on form load)
        private int _calculatedComboBoxWidth;
        private int _calculatedContentWidth;
        private int _calculatedPanelWidth;
        private int _calculatedPanelHeight;

        // Grid constants
        private const int GROUP_MARGIN = 10;
        private const int CONTENT_PADDING = 8;
        private const int ELEMENT_SPACING = 5;
        private const int STATS_LISTBOX_HEIGHT = 140;

        // Styling
        private static readonly Color HEADER_COLOR = Color.FromArgb(70, 130, 180);

        public CustomValidator(MainForm ownerForm)
        {
            ArgumentNullException.ThrowIfNull(ownerForm);
            InitializeComponent();
            _ownerForm = ownerForm;
        }

        private void CustomValidator_Load(object sender, EventArgs e) => SetupUI();

        private void SetupUI()
        {
            // Calculate optimal combobox width based on stat names
            CalculateDynamicSizes();

            // Calculate form size for 3x2 grid
            int formWidth = 3 * _calculatedPanelWidth + 4 * GROUP_MARGIN + 40;
            int formHeight = 2 * _calculatedPanelHeight + 3 * GROUP_MARGIN + 150;

            this.Text = "Custom Validator - Group-Based Configuration";
            this.Size = new Size(formWidth, formHeight);
            this.MinimumSize = new Size(_calculatedPanelWidth + 100, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            var mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(10)
            };

            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));

            // Header
            var headerPanel = new Panel { Dock = DockStyle.Fill };
            var lblTitle = new Label
            {
                Text = "Validation Groups",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(5, 10),
                AutoSize = true
            };

            _btnAddGroup = new Button
            {
                Text = "+ Add Group",
                Location = new Point(200, 8),
                Size = new Size(120, 35),
                BackColor = HEADER_COLOR,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Enabled = true
            };
            _btnAddGroup.FlatAppearance.BorderSize = 0;
            _btnAddGroup.Click += BtnAddGroup_Click;

            var btnHelp = new Button
            {
                Text = "?",
                Location = new Point(330, 8),
                Size = new Size(35, 35),
                BackColor = Color.FromArgb(100, 149, 237),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Cursor = Cursors.Help
            };
            btnHelp.FlatAppearance.BorderSize = 0;
            btnHelp.Click += BtnHelp_Click;

            headerPanel.Controls.Add(lblTitle);
            headerPanel.Controls.Add(_btnAddGroup);
            headerPanel.Controls.Add(btnHelp);

            // Groups container
            _groupsContainer = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.FromArgb(240, 240, 240),
                Name = "groupsContainer"
            };
            _groupsContainer.Resize += (s, e) => ArrangeGroupsInGrid();

            // Bottom panel
            var bottomPanel = new Panel { Dock = DockStyle.Fill };

            var btnCreateValidator = new Button
            {
                Text = "Create Validator",
                Location = new Point(10, 10),
                Size = new Size(150, 40),
                BackColor = Color.FromArgb(34, 139, 34),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };
            btnCreateValidator.FlatAppearance.BorderSize = 0;
            btnCreateValidator.Click += BtnCreateValidator_Click;

            var btnClose = new Button
            {
                Text = "Close",
                Location = new Point(170, 10),
                Size = new Size(100, 40),
                BackColor = Color.FromArgb(220, 220, 220),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10)
            };
            btnClose.Click += (s, e) => this.Close();

            bottomPanel.Controls.Add(btnCreateValidator);
            bottomPanel.Controls.Add(btnClose);

            mainPanel.Controls.Add(headerPanel, 0, 0);
            mainPanel.Controls.Add(_groupsContainer, 0, 1);
            mainPanel.Controls.Add(bottomPanel, 0, 2);

            this.Controls.Add(mainPanel);
        }

        private void CalculateDynamicSizes()
        {
            var tempCombo = new ComboBox
            {
                Font = new Font("Segoe UI", 8.5f),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            var availableProps = typeof(ItemStats).GetProperties()
                .Where(p => p.Name != nameof(ItemStats.Enchant) &&
                           (p.PropertyType == typeof(int) || p.PropertyType == typeof(double)))
                .Select(p => p.Name)
                .ToList();

            foreach (var prop in availableProps)
                tempCombo.Items.Add(prop);

            using var g = tempCombo.CreateGraphics();
            int maxWidth = availableProps
                .Select(item => (int)g.MeasureString(item, tempCombo.Font).Width)
                .DefaultIfEmpty(200)
                .Max();

            _calculatedComboBoxWidth = maxWidth + 30; // Add space for dropdown arrow

            // Calculate content width: "Add Stat:" label + spacing + combo + spacing + "+" button
            int labelWidth = 60;
            int buttonWidth = 28;
            _calculatedContentWidth = labelWidth + ELEMENT_SPACING + _calculatedComboBoxWidth + ELEMENT_SPACING + buttonWidth;

            // Panel width = content width + 2 * padding
            _calculatedPanelWidth = _calculatedContentWidth + 2 * CONTENT_PADDING;

            // Panel height calculation
            int headerHeight = 32;
            int addStatRowHeight = 25;
            int constraintsHeight = 65;
            int operatorRowHeight = 35;
            int totalContentHeight = addStatRowHeight + ELEMENT_SPACING + STATS_LISTBOX_HEIGHT +
                                    ELEMENT_SPACING + constraintsHeight + ELEMENT_SPACING + operatorRowHeight;

            _calculatedPanelHeight = headerHeight + totalContentHeight + 2 * CONTENT_PADDING + 10;

            tempCombo.Dispose();
        }

        private void BtnHelp_Click(object sender, EventArgs e)
        {
            string helpText = @"=== ORDER OF OPERATIONS ===

WITHIN A GROUP (Stats):
Stats are evaluated LEFT-TO-RIGHT in the order they appear.
Example: If you have:
  • MaxLife (+)
  • Armour% (-)
  • Spirit (*)

Calculation: ((MaxLife + Armour%) - Spirit) * next_stat
This is LEFT-ASSOCIATIVE evaluation.

To control order:
1. Reorder stats using ▲▼ buttons
2. First stat evaluated first
3. Each operator applies between result and next stat

BETWEEN GROUPS:
Groups evaluated in grid order (left→right, top→bottom).
Each group produces TRUE/FALSE based on Min/Max constraints.

Results combined using group operators (AND/OR/XOR):
  • AND: Both groups must pass
  • OR: At least one group must pass  
  • XOR: Exactly one group must pass

Example with 3 groups:
  Group1 (TRUE) → AND
  Group2 (FALSE) → OR
  Group3 (TRUE)

Evaluation: (TRUE AND FALSE) OR TRUE = FALSE OR TRUE = TRUE

CONSTRAINTS:
Each group sums all item stats per its expression,
then checks if sum is within Min/Max bounds.

Min/Max can be 0 or negative.
At least one constraint (Min OR Max) must be enabled.

Example:
If calculated value is 150:
  • Min=100, Max=200 → PASS ✓
  • Min=200, Max=300 → FAIL ✗";

            using var helpForm = new Form
            {
                Text = "Validator Help - Order of Operations",
                Size = new Size(650, 600),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            var txtHelp = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Dock = DockStyle.Fill,
                Font = new Font("Consolas", 9.5f),
                Text = helpText,
                Padding = new Padding(10)
            };

            // Prevent auto-selection of text
            txtHelp.Select(0, 0);
            txtHelp.GotFocus += (s, e) => txtHelp.Select(0, 0);

            var btnOk = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Dock = DockStyle.Bottom,
                Height = 40,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            helpForm.Controls.Add(txtHelp);
            helpForm.Controls.Add(btnOk);
            helpForm.ShowDialog(this);
        }

        private void BtnAddGroup_Click(object sender, EventArgs e)
        {
            var group = new ValidationGroupModel
            {
                GroupId = _nextGroupId++,
                GroupName = $"Group {_groups.Count + 1}",
                IsMinEnabled = true,
                MinValue = 0.00
            };

            _groups.Add(group);
            CreateGroupPanel(group);

            ArrangeGroupsInGrid();
            RevalidateAllGroups();
        }

        private void CreateGroupPanel(ValidationGroupModel group)
        {
            var groupPanel = new Panel
            {
                Width = _calculatedPanelWidth,
                Height = _calculatedPanelHeight,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                Tag = group
            };

            // Header
            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 32,
                BackColor = HEADER_COLOR
            };

            var lblGroupName = new Label
            {
                Text = group.GroupName,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(8, 7),
                AutoSize = true
            };

            var btnDelete = new Button
            {
                Text = "×",
                Size = new Size(24, 24),
                Location = new Point(_calculatedPanelWidth - 28, 4),
                BackColor = Color.FromArgb(200, 50, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnDelete.FlatAppearance.BorderSize = 0;
            btnDelete.Click += (s, e) => DeleteGroup(group, groupPanel);

            headerPanel.Controls.Add(lblGroupName);
            headerPanel.Controls.Add(btnDelete);

            // Content panel
            var contentPanel = new Panel
            {
                Location = new Point(CONTENT_PADDING, 37),
                Size = new Size(_calculatedContentWidth, _calculatedPanelHeight - 42),
                AutoScroll = false
            };

            int yPos = 0;

            // Add Stat row
            var lblAddStat = new Label
            {
                Text = "Add Stat:",
                Location = new Point(0, yPos + 3),
                Size = new Size(60, 18),
                Font = new Font("Segoe UI", 8.5f)
            };

            var cmbStats = new ComboBox
            {
                Location = new Point(60 + ELEMENT_SPACING, yPos),
                Width = _calculatedComboBoxWidth,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Name = "cmbStats",
                Font = new Font("Segoe UI", 8.5f)
            };

            var availableProps = typeof(ItemStats).GetProperties()
                .Where(p => p.Name != nameof(ItemStats.Enchant) &&
                           (p.PropertyType == typeof(int) || p.PropertyType == typeof(double)))
                .OrderBy(p => p.Name)
                .ToList();

            foreach (var prop in availableProps)
                cmbStats.Items.Add(prop.Name);

            var btnAddStat = new Button
            {
                Text = "+",
                Location = new Point(60 + ELEMENT_SPACING + _calculatedComboBoxWidth + ELEMENT_SPACING, yPos - 1),
                Size = new Size(28, 23),
                BackColor = HEADER_COLOR,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnAddStat.FlatAppearance.BorderSize = 0;
            btnAddStat.Click += (s, e) => AddStatToGroup(group, cmbStats, contentPanel);

            yPos += 25 + ELEMENT_SPACING;

            // Stats ListBox
            var statsListBox = new ListBox
            {
                Location = new Point(0, yPos),
                Width = _calculatedContentWidth,
                Height = STATS_LISTBOX_HEIGHT,
                Name = "statsListBox",
                DrawMode = DrawMode.OwnerDrawFixed,
                ItemHeight = 28,
                SelectionMode = SelectionMode.One
            };
            statsListBox.DrawItem += (_, e) => StatsListBox_DrawItem(e, group);
            statsListBox.MouseClick += (s, e) => StatsListBox_MouseClick(s, e, group, cmbStats);

            yPos += STATS_LISTBOX_HEIGHT + ELEMENT_SPACING;

            // Constraints GroupBox
            var grpConstraints = new GroupBox
            {
                Text = "Constraints",
                Location = new Point(0, yPos),
                Width = _calculatedContentWidth,
                Height = 65,
                Font = new Font("Segoe UI", 8.5f, FontStyle.Bold)
            };

            // Calculate centered positions for checkboxes and inputs
            int constraintContentWidth = grpConstraints.Width - 16;
            int halfWidth = constraintContentWidth / 2;

            var chkMin = new CheckBox
            {
                Text = "Min:",
                Location = new Point(8, 23),
                Width = 48,
                Font = new Font("Segoe UI", 8.5f),
                Name = "chkMin",
                Checked = group.IsMinEnabled
            };

            var numMin = new NumericUpDown
            {
                Location = new Point(60, 21),
                Width = halfWidth - 64,
                DecimalPlaces = 2,
                Minimum = -99999,
                Maximum = 99999,
                Value = (decimal)(group.MinValue ?? 0.00),
                Enabled = group.IsMinEnabled,
                Name = "numMin",
                Font = new Font("Segoe UI", 8.5f)
            };

            chkMin.CheckedChanged += (s, e) =>
            {
                group.IsMinEnabled = chkMin.Checked;
                numMin.Enabled = chkMin.Checked;
                ValidateAndUpdate(group, contentPanel);
            };

            numMin.ValueChanged += (s, e) =>
            {
                group.MinValue = (double)numMin.Value;
                ValidateAndUpdate(group, contentPanel);
            };

            numMin.Leave += (s, e) => ValidateAndUpdate(group, contentPanel);

            var chkMax = new CheckBox
            {
                Text = "Max:",
                Location = new Point(halfWidth + 8, 23),
                Width = 48,
                Font = new Font("Segoe UI", 8.5f),
                Name = "chkMax"
            };

            var numMax = new NumericUpDown
            {
                Location = new Point(halfWidth + 60, 21),
                Width = halfWidth - 64,
                DecimalPlaces = 2,
                Minimum = -99999,
                Maximum = 99999,
                Enabled = false,
                Name = "numMax",
                Font = new Font("Segoe UI", 8.5f)
            };

            chkMax.CheckedChanged += (s, e) =>
            {
                group.IsMaxEnabled = chkMax.Checked;
                numMax.Enabled = chkMax.Checked;
                if (chkMax.Checked && !group.MaxValue.HasValue)
                    group.MaxValue = (double)numMax.Value;
                ValidateAndUpdate(group, contentPanel);
            };

            numMax.ValueChanged += (s, e) =>
            {
                if (chkMax.Checked)
                {
                    group.MaxValue = (double)numMax.Value;
                    ValidateAndUpdate(group, contentPanel);
                }
            };

            numMax.Leave += (s, e) => ValidateAndUpdate(group, contentPanel);

            var lblValidation = new Label
            {
                Location = new Point(8, 45),
                Size = new Size(constraintContentWidth, 14),
                ForeColor = Color.Red,
                Font = new Font("Segoe UI", 7f),
                Name = "lblValidation",
                Text = "",
                TextAlign = ContentAlignment.MiddleLeft
            };

            grpConstraints.Controls.AddRange([chkMin, numMin, chkMax, numMax, lblValidation]);

            yPos += 65 + ELEMENT_SPACING;

            // Group Operator Row
            var pnlOperator = new Panel
            {
                Location = new Point(0, yPos),
                Width = _calculatedContentWidth,
                Height = 30,
                Name = "pnlOperator",
                Visible = false
            };

            var cmbOperator = new ComboBox
            {
                Location = new Point(0, 4),
                Width = _calculatedContentWidth - 45,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Name = "cmbOperator",
                Font = new Font("Segoe UI", 8.5f),
                Enabled = false
            };
            cmbOperator.Items.AddRange(["AND", "OR", "XOR"]);
            cmbOperator.SelectedIndex = 0;
            cmbOperator.SelectedIndexChanged += (s, e) => group.GroupOperator = cmbOperator.SelectedItem?.ToString();

            var lblArrow = new Label
            {
                Text = "→",
                Location = new Point(_calculatedContentWidth - 35, 0),
                Size = new Size(35, 30),
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = HEADER_COLOR,
                TextAlign = ContentAlignment.MiddleCenter
            };

            pnlOperator.Controls.AddRange([cmbOperator, lblArrow]);

            contentPanel.Controls.AddRange([lblAddStat, cmbStats, btnAddStat, statsListBox, grpConstraints, pnlOperator]);
            groupPanel.Controls.AddRange([headerPanel, contentPanel]);

            // Drag-drop
            groupPanel.AllowDrop = true;
            groupPanel.MouseDown += GroupPanel_MouseDown;
            groupPanel.MouseMove += GroupPanel_MouseMove;
            groupPanel.DragOver += (s, e) => e.Effect = DragDropEffects.Move;
            groupPanel.DragDrop += GroupPanel_DragDrop;

            _groupsContainer.Controls.Add(groupPanel);

            ValidateAndUpdate(group, contentPanel);
        }

        private void ValidateAndUpdate(ValidationGroupModel group, Panel contentPanel)
        {
            var lblValidation = contentPanel.Controls.Find("lblValidation", true).FirstOrDefault() as Label;

            bool isValid = true;
            string errorMsg = "";

            if (!group.IsMinEnabled && !group.IsMaxEnabled)
            {
                errorMsg = "Enable Min or Max";
                isValid = false;
            }
            else if (group.IsMinEnabled && group.IsMaxEnabled &&
                     group.MinValue.HasValue && group.MaxValue.HasValue &&
                     group.MinValue.Value >= group.MaxValue.Value)
            {
                errorMsg = "Min must be < Max";
                isValid = false;
            }

            if (lblValidation is not null)
            {
                lblValidation.Text = errorMsg;
                lblValidation.ForeColor = isValid ? Color.Green : Color.Red;
            }

            RevalidateAllGroups();
        }

        private void RevalidateAllGroups()
        {
            // Update "Add Group" button state
            if (_groups.Count == 0)
            {
                UpdateAddGroupButton(true);
            }
            else
            {
                var lastGroup = _groups[^1];
                bool hasConstraint = lastGroup.IsMinEnabled || lastGroup.IsMaxEnabled;
                bool hasStats = lastGroup.Stats.Count > 0;
                bool isValid = !(lastGroup.IsMinEnabled && lastGroup.IsMaxEnabled &&
                               lastGroup.MinValue.HasValue && lastGroup.MaxValue.HasValue &&
                               lastGroup.MinValue.Value >= lastGroup.MaxValue.Value);

                UpdateAddGroupButton(hasConstraint && hasStats && isValid);
            }

            // Update group operator visibility for all groups
            UpdateAllGroupOperatorVisibility();
        }

        private void UpdateAddGroupButton(bool enabled)
        {
            _btnAddGroup.Enabled = enabled;
            _btnAddGroup.ForeColor = enabled ? Color.White : Color.Gray;
        }

        private void UpdateAllGroupOperatorVisibility()
        {
            for (int i = 0; i < _groups.Count; i++)
            {
                var group = _groups[i];
                var panel = _groupsContainer.Controls.OfType<Panel>().FirstOrDefault(p => p.Tag == group);
                if (panel is null) continue;

                var contentPanel = panel.Controls.OfType<Panel>().FirstOrDefault();
                if (contentPanel is null) continue;

                if (contentPanel.Controls.Find("pnlOperator", false).FirstOrDefault() is not Panel pnlOperator ||
                    pnlOperator?.Controls.Find("cmbOperator", false).FirstOrDefault() is not ComboBox cmbOperator) continue;

                bool currentHasConstraint = group.IsMinEnabled || group.IsMaxEnabled;
                bool currentHasStats = group.Stats.Count > 0;

                bool hasNextValidGroup = false;
                if (i < _groups.Count - 1)
                {
                    for (int j = i + 1; j < _groups.Count; j++)
                    {
                        var nextGroup = _groups[j];
                        bool nextHasConstraint = nextGroup.IsMinEnabled || nextGroup.IsMaxEnabled;
                        bool nextHasStats = nextGroup.Stats.Count > 0;

                        if (nextHasConstraint && nextHasStats)
                        {
                            hasNextValidGroup = true;
                            break;
                        }
                    }
                }

                bool shouldShow = currentHasConstraint && currentHasStats && hasNextValidGroup;
                pnlOperator.Visible = shouldShow;
                cmbOperator.Enabled = shouldShow;
            }
        }

        private static void StatsListBox_DrawItem(DrawItemEventArgs e, ValidationGroupModel group)
        {
            if (e.Index < 0 || e.Index >= group.Stats.Count) return;

            var stat = group.Stats[e.Index];
            bool isLastStat = e.Index == group.Stats.Count - 1;

            e.DrawBackground();

            var bounds = e.Bounds;

            // Draw stat name
            using (var brush = new SolidBrush(e.ForeColor))
            {
                var textRect = new Rectangle(bounds.Left + 4, bounds.Top + 7, bounds.Width - 100, bounds.Height);
                e.Graphics.DrawString(stat.PropertyName, e.Font, brush, textRect);
            }

            // Operator dropdown (only if NOT last stat)
            if (!isLastStat)
            {
                var opRect = new Rectangle(bounds.Right - 95, bounds.Top + 2, 50, bounds.Height - 4);
                using var bgBrush = new SolidBrush(Color.FromArgb(240, 240, 240));
                e.Graphics.FillRectangle(bgBrush, opRect);
                e.Graphics.DrawRectangle(Pens.Gray, opRect);

                using var textBrush = new SolidBrush(Color.Black);
                var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                e.Graphics.DrawString(stat.Operator, e.Font, textBrush, opRect, sf);
            }

            // Up button
            var upRect = new Rectangle(bounds.Right - 42, bounds.Top + 2, 18, 11);
            using (var btnBrush = new SolidBrush(Color.FromArgb(100, 150, 200)))
                e.Graphics.FillRectangle(btnBrush, upRect);
            e.Graphics.DrawRectangle(Pens.DarkBlue, upRect);
            using (var textBrush = new SolidBrush(Color.White))
            {
                var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                e.Graphics.DrawString("▲", new Font("Arial", 6), textBrush, upRect, sf);
            }

            // Down button
            var downRect = new Rectangle(bounds.Right - 42, bounds.Top + 15, 18, 11);
            using (var btnBrush = new SolidBrush(Color.FromArgb(100, 150, 200)))
                e.Graphics.FillRectangle(btnBrush, downRect);
            e.Graphics.DrawRectangle(Pens.DarkBlue, downRect);
            using (var textBrush = new SolidBrush(Color.White))
            {
                var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                e.Graphics.DrawString("▼", new Font("Arial", 6), textBrush, downRect, sf);
            }

            // Remove button
            var removeRect = new Rectangle(bounds.Right - 22, bounds.Top + 2, 18, bounds.Height - 4);
            using (var btnBrush = new SolidBrush(Color.FromArgb(200, 50, 50)))
                e.Graphics.FillRectangle(btnBrush, removeRect);
            using (var textBrush = new SolidBrush(Color.White))
            {
                var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                e.Graphics.DrawString("×", new Font("Segoe UI", 10, FontStyle.Bold), textBrush, removeRect, sf);
            }

            e.DrawFocusRectangle();
        }

        private void StatsListBox_MouseClick(object sender, MouseEventArgs e, ValidationGroupModel group, ComboBox cmbStats)
        {
            var listBox = sender as ListBox;
            int index = listBox.IndexFromPoint(e.Location);
            if (index < 0 || index >= group.Stats.Count) return;

            var bounds = listBox.GetItemRectangle(index);
            bool isLastStat = index == group.Stats.Count - 1;

            // Operator dropdown
            if (!isLastStat)
            {
                var opRect = new Rectangle(bounds.Right - 95, bounds.Top + 2, 50, bounds.Height - 4);
                if (opRect.Contains(e.Location))
                {
                    ShowOperatorMenu(group.Stats[index], listBox, e.Location);
                    return;
                }
            }

            // Up button
            var upRect = new Rectangle(bounds.Right - 42, bounds.Top + 2, 18, 11);
            if (upRect.Contains(e.Location) && index > 0)
            {
                (group.Stats[index], group.Stats[index - 1]) = (group.Stats[index - 1], group.Stats[index]);
                RefreshStatsListBox(listBox, group);
                return;
            }

            // Down button
            var downRect = new Rectangle(bounds.Right - 42, bounds.Top + 15, 18, 11);
            if (downRect.Contains(e.Location) && index < group.Stats.Count - 1)
            {
                (group.Stats[index], group.Stats[index + 1]) = (group.Stats[index + 1], group.Stats[index]);
                RefreshStatsListBox(listBox, group);
                return;
            }

            // Remove button
            var removeRect = new Rectangle(bounds.Right - 22, bounds.Top + 2, 18, bounds.Height - 4);
            if (removeRect.Contains(e.Location))
            {
                string propName = group.Stats[index].PropertyName;
                group.Stats.RemoveAt(index);
                RefreshStatsListBox(listBox, group);

                // Re-add to dropdown
                if (!cmbStats.Items.Contains(propName))
                {
                    var items = cmbStats.Items.Cast<string>().Append(propName).OrderBy(x => x).ToList();
                    cmbStats.Items.Clear();
                    cmbStats.Items.AddRange([.. items]);
                }

                RevalidateAllGroups();
            }
        }

        private static void ShowOperatorMenu(GroupStatModel stat, ListBox listBox, Point location)
        {
            using var menu = new ContextMenuStrip();
            foreach (var op in new[] { "+", "-", "*", "/" })
            {
                var item = new ToolStripMenuItem(op) { Checked = stat.Operator == op };
                item.Click += (s, e) =>
                {
                    stat.Operator = op;
                    listBox.Invalidate();
                };
                menu.Items.Add(item);
            }
            menu.Show(listBox, location);
        }

        private static void RefreshStatsListBox(ListBox listBox, ValidationGroupModel group)
        {
            listBox.Items.Clear();
            listBox.Items.AddRange([.. group.Stats.Select(s => s.PropertyName)]);
        }

        private void AddStatToGroup(ValidationGroupModel group, ComboBox cmbStats, Panel contentPanel)
        {
            if (cmbStats.SelectedItem is null)
            {
                MessageBox.Show("Please select a stat.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string propName = cmbStats.SelectedItem.ToString();
            var propInfo = typeof(ItemStats).GetProperty(propName);

            if (group.Stats.Any(s => s.PropertyName == propName))
            {
                MessageBox.Show("Stat already in group.", "Duplicate", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            group.Stats.Add(new GroupStatModel
            {
                PropInfo = propInfo,
                PropertyName = propName,
                Operator = "+"
            });

            cmbStats.Items.Remove(propName);
            cmbStats.SelectedIndex = -1;

            if (contentPanel.Controls.Find("statsListBox", false).FirstOrDefault() is ListBox listBox)
                RefreshStatsListBox(listBox, group);

            RevalidateAllGroups();
        }

        private Point _dragStartPoint;
        private Panel _draggedPanel;

        private void GroupPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && e.Y < 32)
            {
                _dragStartPoint = e.Location;
                _draggedPanel = sender as Panel;
            }
        }

        private void GroupPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && _draggedPanel is not null)
            {
                if (Math.Abs(e.X - _dragStartPoint.X) > 5 || Math.Abs(e.Y - _dragStartPoint.Y) > 5)
                    _draggedPanel.DoDragDrop(_draggedPanel, DragDropEffects.Move);
            }
        }

        private void GroupPanel_DragDrop(object sender, DragEventArgs e)
        {
            if (sender is not Panel targetPanel ||
                e.Data.GetData(typeof(Panel)) is not Panel sourcePanel ||
                targetPanel == sourcePanel)
                return;

            var sourceGroup = sourcePanel.Tag as ValidationGroupModel;
            var targetGroup = targetPanel.Tag as ValidationGroupModel;

            int sourceIndex = _groups.IndexOf(sourceGroup);
            int targetIndex = _groups.IndexOf(targetGroup);

            _groups.RemoveAt(sourceIndex);
            _groups.Insert(targetIndex, sourceGroup);

            ArrangeGroupsInGrid();
            RevalidateAllGroups();
        }

        private void DeleteGroup(ValidationGroupModel group, Panel panel)
        {
            _groups.Remove(group);
            _groupsContainer.Controls.Remove(panel);
            panel.Dispose();

            ArrangeGroupsInGrid();
            RevalidateAllGroups();
        }

        private void ArrangeGroupsInGrid()
        {
            if (_groupsContainer is null || _groups.Count == 0) return;

            int containerWidth = _groupsContainer.ClientSize.Width - 5;
            int columnsPerRow = Math.Max(1, (containerWidth - GROUP_MARGIN) / (_calculatedPanelWidth + GROUP_MARGIN));

            int currentRow = 0;
            int currentCol = 0;

            foreach (var group in _groups)
            {
                var panel = _groupsContainer.Controls.OfType<Panel>()
                    .FirstOrDefault(p => p.Tag == group);

                if (panel is not null)
                {
                    int x = GROUP_MARGIN + currentCol * (_calculatedPanelWidth + GROUP_MARGIN);
                    int y = GROUP_MARGIN + currentRow * (_calculatedPanelHeight + GROUP_MARGIN);

                    panel.Location = new Point(x, y);

                    currentCol++;
                    if (currentCol >= columnsPerRow)
                    {
                        currentCol = 0;
                        currentRow++;
                    }
                }
            }

            int totalRows = (int)Math.Ceiling((double)_groups.Count / columnsPerRow);
            int minHeight = totalRows * (_calculatedPanelHeight + GROUP_MARGIN) + GROUP_MARGIN;
            _groupsContainer.AutoScrollMinSize = new Size(0, minHeight);
        }

        private void BtnCreateValidator_Click(object sender, EventArgs e)
        {
            try
            {
                var activeGroups = _groups.Where(g => g.IsMinEnabled || g.IsMaxEnabled).ToList();

                if (activeGroups.Count == 0)
                {
                    MessageBox.Show("No active groups. Validator will always return true.",
                        "Validator Created", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _masterValidator = x => true;
                    _ownerForm._itemValidatorFunction = _masterValidator;
                    return;
                }

                foreach (var group in activeGroups)
                {
                    if (group.IsMinEnabled && group.IsMaxEnabled &&
                        group.MinValue.HasValue && group.MaxValue.HasValue &&
                        group.MinValue.Value >= group.MaxValue.Value)
                    {
                        MessageBox.Show($"{group.GroupName}: Min must be less than Max.",
                            "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                _masterValidator = BuildValidatorFunction(activeGroups);
                _ownerForm._itemValidatorFunction = _masterValidator;

                MessageBox.Show($"Validator created with {activeGroups.Count} group(s)!",
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating validator: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static Func<List<Item>, bool> BuildValidatorFunction(List<ValidationGroupModel> activeGroups)
        {
            return items =>
            {
                if (activeGroups.Count == 0) return true;

                bool result = EvaluateGroup(activeGroups[0], items);

                for (int i = 1; i < activeGroups.Count; i++)
                {
                    bool nextResult = EvaluateGroup(activeGroups[i], items);
                    string op = activeGroups[i - 1].GroupOperator ?? "AND";

                    result = op switch
                    {
                        "AND" => result && nextResult,
                        "OR" => result || nextResult,
                        "XOR" => result ^ nextResult,
                        _ => result && nextResult
                    };
                }

                return result;
            };
        }

        private static bool EvaluateGroup(ValidationGroupModel group, List<Item> items)
        {
            if (group.Stats.Count == 0) return true;

            var values = items.Select(item => EvaluateExpression(group.Stats, item.ItemStats)).ToList();
            double sum = values.Sum();

            if (group.IsMinEnabled && group.MinValue.HasValue && sum < group.MinValue.Value)
                return false;

            if (group.IsMaxEnabled && group.MaxValue.HasValue && sum > group.MaxValue.Value)
                return false;

            return true;
        }

        private static double EvaluateExpression(List<GroupStatModel> stats, ItemStats itemStats)
        {
            if (stats.Count == 0) return 0;

            double result = Convert.ToDouble(stats[0].PropInfo.GetValue(itemStats));

            for (int i = 1; i < stats.Count; i++)
            {
                double nextValue = Convert.ToDouble(stats[i].PropInfo.GetValue(itemStats));
                string op = stats[i].Operator;

                result = op switch
                {
                    "+" => result + nextValue,
                    "-" => result - nextValue,
                    "*" => result * nextValue,
                    "/" => nextValue != 0 ? result / nextValue : result,
                    _ => result + nextValue
                };
            }

            return result;
        }

        private void CustomValidator_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
                this.Owner?.BringToFront();
            }
        }
    }
}