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
        private bool _isClosing = false;

        public CustomValidator(MainForm ownerForm)
        {
            ArgumentNullException.ThrowIfNull(ownerForm, nameof(ownerForm));
            InitializeComponent();
            _ownerForm = ownerForm;
        }

        private void CustomValidator_Load(object sender, EventArgs e)
        {
            SetupUI();
        }

        private void SetupUI()
        {
            this.Text = "Custom Validator - Group-Based Configuration";
            this.Size = new Size(1000, 700);
            this.MinimumSize = new Size(800, 500);

            // Main layout
            var mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(10)
            };

            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50)); // Header
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // Groups
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 60)); // Buttons

            // Header
            var headerPanel = new Panel { Dock = DockStyle.Fill };
            var lblTitle = new Label
            {
                Text = "Validation Groups",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(5, 10),
                AutoSize = true
            };

            var btnAddGroup = new Button
            {
                Text = "+ Add Group",
                Location = new Point(200, 8),
                Size = new Size(120, 35),
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnAddGroup.FlatAppearance.BorderSize = 0;
            btnAddGroup.Click += BtnAddGroup_Click;
            btnAddGroup.MouseEnter += (s, e) => btnAddGroup.BackColor = Color.FromArgb(90, 150, 200);
            btnAddGroup.MouseLeave += (s, e) => btnAddGroup.BackColor = Color.FromArgb(70, 130, 180);

            headerPanel.Controls.Add(lblTitle);
            headerPanel.Controls.Add(btnAddGroup);

            // Groups container (scrollable)
            var groupsPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.FromArgb(240, 240, 240),
                Name = "groupsPanel"
            };

            // Bottom buttons
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
            btnCreateValidator.MouseEnter += (s, e) => btnCreateValidator.BackColor = Color.FromArgb(54, 159, 54);
            btnCreateValidator.MouseLeave += (s, e) => btnCreateValidator.BackColor = Color.FromArgb(34, 139, 34);

            var btnClose = new Button
            {
                Text = "Close",
                Location = new Point(170, 10),
                Size = new Size(100, 40),
                BackColor = Color.FromArgb(220, 220, 220),
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10)
            };
            btnClose.FlatAppearance.BorderSize = 1;
            btnClose.Click += BtnClose_Click;

            bottomPanel.Controls.Add(btnCreateValidator);
            bottomPanel.Controls.Add(btnClose);

            mainPanel.Controls.Add(headerPanel, 0, 0);
            mainPanel.Controls.Add(groupsPanel, 0, 1);
            mainPanel.Controls.Add(bottomPanel, 0, 2);

            this.Controls.Add(mainPanel);
        }

        private void BtnAddGroup_Click(object sender, EventArgs e)
        {
            var group = new ValidationGroupModel
            {
                GroupId = _nextGroupId++,
                GroupName = $"Group {_groups.Count + 1}"
            };

            _groups.Add(group);
            CreateGroupPanel(group);
            UpdateGroupOperatorVisibility();
        }

        private void CreateGroupPanel(ValidationGroupModel group)
        {
            if (this.Controls.Find("groupsPanel", true).FirstOrDefault() is not Panel groupsPanel) return;

            var groupPanel = new Panel
            {
                Width = groupsPanel.ClientSize.Width - 30,
                Height = 250,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                Margin = new Padding(5),
                Tag = group,
                AllowDrop = true,
                Location = new Point(10, _groups.IndexOf(group) * 260 + 10)
            };

            // Header
            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 40,
                BackColor = Color.FromArgb(70, 130, 180)
            };

            var lblGroupName = new Label
            {
                Text = group.GroupName,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Location = new Point(10, 10),
                AutoSize = true
            };

            var btnDelete = new Button
            {
                Text = "×",
                Size = new Size(30, 30),
                Location = new Point(groupPanel.Width - 40, 5),
                BackColor = Color.FromArgb(200, 50, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnDelete.FlatAppearance.BorderSize = 0;
            btnDelete.Click += (s, e) => DeleteGroup(group, groupPanel);

            headerPanel.Controls.Add(lblGroupName);
            headerPanel.Controls.Add(btnDelete);

            // Content area
            var contentPanel = new Panel
            {
                Location = new Point(5, 45),
                Size = new Size(groupPanel.Width - 10, 200),
                AutoScroll = true
            };

            // Stat selector
            var lblAddStat = new Label
            {
                Text = "Add Stat:",
                Location = new Point(5, 5),
                AutoSize = true
            };

            var cmbStats = new ComboBox
            {
                Location = new Point(70, 3),
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDown,
                AutoCompleteMode = AutoCompleteMode.SuggestAppend,
                AutoCompleteSource = AutoCompleteSource.ListItems
            };

            // Populate with available stats
            var properties = typeof(ItemStats).GetProperties()
                .Where(p => p.Name != nameof(ItemStats.Enchant) &&
                           (p.PropertyType == typeof(int) || p.PropertyType == typeof(double)))
                .OrderBy(p => p.Name);

            foreach (var prop in properties)
            {
                cmbStats.Items.Add(prop.Name);
            }

            var btnAddStat = new Button
            {
                Text = "+",
                Location = new Point(280, 2),
                Size = new Size(30, 25),
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnAddStat.FlatAppearance.BorderSize = 0;
            btnAddStat.Click += (s, e) => AddStatToGroup(group, cmbStats, contentPanel);

            // Stats list
            var statsListBox = new ListBox
            {
                Location = new Point(5, 35),
                Size = new Size(310, 80),
                Name = "statsListBox",
                DrawMode = DrawMode.OwnerDrawFixed
            };
            statsListBox.DrawItem += StatsListBox_DrawItem;

            // Min/Max controls
            var chkMin = new CheckBox
            {
                Text = "Min:",
                Location = new Point(5, 125),
                Width = 50
            };
            chkMin.CheckedChanged += (s, e) =>
            {
                group.IsMinEnabled = chkMin.Checked;
                UpdateGroupOperatorVisibility();
            };

            var numMin = new NumericUpDown
            {
                Location = new Point(60, 123),
                Width = 80,
                DecimalPlaces = 2,
                Minimum = -99999,
                Maximum = 99999,
                Enabled = false
            };
            numMin.ValueChanged += (s, e) => group.MinValue = (double)numMin.Value;
            chkMin.CheckedChanged += (s, e) => numMin.Enabled = chkMin.Checked;

            var chkMax = new CheckBox
            {
                Text = "Max:",
                Location = new Point(160, 125),
                Width = 50
            };
            chkMax.CheckedChanged += (s, e) =>
            {
                group.IsMaxEnabled = chkMax.Checked;
                UpdateGroupOperatorVisibility();
            };

            var numMax = new NumericUpDown
            {
                Location = new Point(215, 123),
                Width = 80,
                DecimalPlaces = 2,
                Minimum = -99999,
                Maximum = 99999,
                Enabled = false
            };
            numMax.ValueChanged += (s, e) => group.MaxValue = (double)numMax.Value;
            chkMax.CheckedChanged += (s, e) => numMax.Enabled = chkMax.Checked;

            // Group operator
            var lblOperator = new Label
            {
                Text = "→ Operator:",
                Location = new Point(5, 160),
                AutoSize = true,
                Visible = false,
                Name = "lblOperator"
            };

            var cmbOperator = new ComboBox
            {
                Location = new Point(85, 158),
                Width = 80,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Visible = false,
                Name = "cmbOperator"
            };
            cmbOperator.Items.AddRange(["AND", "OR", "XOR"]);
            cmbOperator.SelectedIndex = 0;
            cmbOperator.SelectedIndexChanged += (s, e) => group.GroupOperator = cmbOperator.SelectedItem?.ToString();

            contentPanel.Controls.Add(lblAddStat);
            contentPanel.Controls.Add(cmbStats);
            contentPanel.Controls.Add(btnAddStat);
            contentPanel.Controls.Add(statsListBox);
            contentPanel.Controls.Add(chkMin);
            contentPanel.Controls.Add(numMin);
            contentPanel.Controls.Add(chkMax);
            contentPanel.Controls.Add(numMax);
            contentPanel.Controls.Add(lblOperator);
            contentPanel.Controls.Add(cmbOperator);

            groupPanel.Controls.Add(headerPanel);
            groupPanel.Controls.Add(contentPanel);

            // Drag-drop for reordering
            groupPanel.MouseDown += GroupPanel_MouseDown;
            groupPanel.MouseMove += GroupPanel_MouseMove;
            groupPanel.DragOver += GroupPanel_DragOver;
            groupPanel.DragDrop += GroupPanel_DragDrop;

            groupsPanel.Controls.Add(groupPanel);
            RearrangeGroupPanels();
        }

        private Point _dragStartPoint;
        private Panel _draggedPanel;

        private void GroupPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _dragStartPoint = e.Location;
                _draggedPanel = sender as Panel;
            }
        }

        private void GroupPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && _draggedPanel != null)
            {
                if (Math.Abs(e.X - _dragStartPoint.X) > 5 || Math.Abs(e.Y - _dragStartPoint.Y) > 5)
                {
                    _draggedPanel.DoDragDrop(_draggedPanel, DragDropEffects.Move);
                }
            }
        }

        private void GroupPanel_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void GroupPanel_DragDrop(object sender, DragEventArgs e)
        {
            if (sender is not Panel targetPanel || e.Data.GetData(typeof(Panel)) is not Panel sourcePanel || targetPanel == sourcePanel) return;

            var sourceGroup = sourcePanel.Tag as ValidationGroupModel;
            var targetGroup = targetPanel.Tag as ValidationGroupModel;

            int sourceIndex = _groups.IndexOf(sourceGroup);
            int targetIndex = _groups.IndexOf(targetGroup);

            _groups.RemoveAt(sourceIndex);
            _groups.Insert(targetIndex, sourceGroup);

            RearrangeGroupPanels();
            UpdateGroupOperatorVisibility();
        }

        private static void AddStatToGroup(ValidationGroupModel group, ComboBox cmbStats, Panel contentPanel)
        {
            if (cmbStats.SelectedItem == null)
            {
                MessageBox.Show("Please select a stat.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string propName = cmbStats.SelectedItem.ToString();
            var propInfo = typeof(ItemStats).GetProperty(propName);

            if (group.Stats.Any(s => s.PropertyName == propName))
            {
                MessageBox.Show("This stat is already in the group.", "Duplicate", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            group.Stats.Add(new GroupStatModel
            {
                PropInfo = propInfo,
                PropertyName = propName,
                Operator = "+"
            });

            if (contentPanel.Controls.Find("statsListBox", false).FirstOrDefault() is ListBox listBox)
            {
                listBox.Items.Add($"{propName} (+)");
            }

            cmbStats.SelectedIndex = -1;
        }

        private void StatsListBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            var listBox = sender as ListBox;
            var item = listBox.Items[e.Index].ToString();

            e.DrawBackground();

            // Draw stat name
            using (var brush = new SolidBrush(e.ForeColor))
            {
                e.Graphics.DrawString(item, e.Font, brush, e.Bounds.Left + 5, e.Bounds.Top + 2);
            }

            // Draw remove button
            var btnRect = new Rectangle(e.Bounds.Right - 25, e.Bounds.Top + 2, 20, e.Bounds.Height - 4);
            using (var btnBrush = new SolidBrush(Color.FromArgb(200, 50, 50)))
            {
                e.Graphics.FillRectangle(btnBrush, btnRect);
            }
            using (var textBrush = new SolidBrush(Color.White))
            {
                var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                e.Graphics.DrawString("×", e.Font, textBrush, btnRect, sf);
            }

            e.DrawFocusRectangle();
        }

        private void DeleteGroup(ValidationGroupModel group, Panel panel)
        {
            _groups.Remove(group);

            if (this.Controls.Find("groupsPanel", true).FirstOrDefault() is Panel groupsPanel)
            {
                groupsPanel.Controls.Remove(panel);
            }

            RearrangeGroupPanels();
            UpdateGroupOperatorVisibility();
        }

        private void RearrangeGroupPanels()
        {
            if (this.Controls.Find("groupsPanel", true).FirstOrDefault() is not Panel groupsPanel) return;

            int yOffset = 10;
            foreach (var group in _groups)
            {
                var panel = groupsPanel.Controls.OfType<Panel>()
                    .FirstOrDefault(p => p.Tag == group);

                if (panel != null)
                {
                    panel.Location = new Point(10, yOffset);
                    yOffset += panel.Height + 10;
                }
            }
        }

        private void UpdateGroupOperatorVisibility()
        {
            if (this.Controls.Find("groupsPanel", true).FirstOrDefault() is not Panel groupsPanel) return;

            for (int i = 0; i < _groups.Count; i++)
            {
                var group = _groups[i];
                var panel = groupsPanel.Controls.OfType<Panel>().FirstOrDefault(p => p.Tag == group);
                if (panel == null) continue;

                var contentPanel = panel.Controls.OfType<Panel>().FirstOrDefault();
                if (contentPanel == null) continue;

                var lblOperator = contentPanel.Controls.Find("lblOperator", false).FirstOrDefault();
                var cmbOperator = contentPanel.Controls.Find("cmbOperator", false).FirstOrDefault();

                bool isLastGroup = (i == _groups.Count - 1);
                bool hasNextActiveGroup = !isLastGroup && _groups.Skip(i + 1).Any(g => g.IsActive);

                if (lblOperator != null && cmbOperator != null)
                {
                    lblOperator.Visible = group.IsActive && hasNextActiveGroup;
                    cmbOperator.Visible = group.IsActive && hasNextActiveGroup;
                }
            }
        }

        private void BtnCreateValidator_Click(object sender, EventArgs e)
        {
            try
            {
                var activeGroups = _groups.Where(g => g.IsActive).ToList();

                if (activeGroups.Count == 0)
                {
                    MessageBox.Show("No active groups with conditions. Validator will always return true.",
                        "Validator Created", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _masterValidator = x => true;
                    _ownerForm._itemValidatorFunction = _masterValidator;
                    return;
                }

                // Build validator function
                _masterValidator = BuildValidatorFunction(activeGroups);
                _ownerForm._itemValidatorFunction = _masterValidator;

                MessageBox.Show("Validator function created successfully!",
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
            return (items) =>
            {
                bool result = EvaluateGroup(activeGroups[0], items);

                for (int i = 1; i < activeGroups.Count; i++)
                {
                    bool nextResult = EvaluateGroup(activeGroups[i], items);
                    string op = activeGroups[i - 1].GroupOperator;

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

            // Calculate expression for each item
            var values = items.Select(item => EvaluateExpression(group.Stats, item.ItemStats)).ToList();

            // Sum all values
            double sum = values.Sum();

            // Check min/max constraints
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

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CustomValidator_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                _isClosing = true;
                this.Hide();
                this.Owner?.BringToFront();
                _isClosing = false;
            }
        }
    }
}