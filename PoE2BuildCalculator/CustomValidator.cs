using Domain.Main;
using Domain.UserControls;
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

        // Grid constants
        private const int GROUP_MARGIN = 10;
        private const int CONTROL_WIDTH = 350;
        private const int CONTROL_HEIGHT = 370;

        public CustomValidator(MainForm ownerForm)
        {
            ArgumentNullException.ThrowIfNull(ownerForm);
            InitializeComponent();
            _ownerForm = ownerForm;
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
            CreateGroupControl(group);
            ArrangeGroupsInGrid();
            RevalidateAllGroups();
        }

        private void CreateGroupControl(ValidationGroupModel group)
        {
            var control = new ItemStatGroupValidatorUserControl
            {
                Group = group,
                Width = CONTROL_WIDTH,
                Height = CONTROL_HEIGHT,
                Tag = group
            };

            control.DeleteRequested += (s, e) => DeleteGroup(group, control);
            control.ValidationChanged += (s, e) => RevalidateAllGroups();

            // Drag-drop support
            control.AllowDrop = true;
            control.MouseDown += GroupControl_MouseDown;
            control.MouseMove += GroupControl_MouseMove;
            control.DragOver += (s, e) => e.Effect = DragDropEffects.Move;
            control.DragDrop += GroupControl_DragDrop;

            groupsContainer.Controls.Add(control);
        }

        private void RevalidateAllGroups()
        {
            // Update "Add Group" button state
            if (_groups.Count == 0)
            {
                btnAddGroup.Enabled = true;
            }
            else
            {
                var lastGroup = _groups[^1];
                bool hasConstraint = lastGroup.IsMinEnabled || lastGroup.IsMaxEnabled;
                bool hasStats = lastGroup.Stats.Count > 0;
                bool isValid = !(lastGroup.IsMinEnabled && lastGroup.IsMaxEnabled &&
                               lastGroup.MinValue.HasValue && lastGroup.MaxValue.HasValue &&
                               lastGroup.MinValue.Value >= lastGroup.MaxValue.Value);

                btnAddGroup.Enabled = hasConstraint && hasStats && isValid;
            }

            // Update operator visibility for all groups
            UpdateAllGroupOperatorVisibility();
        }

        private void UpdateAllGroupOperatorVisibility()
        {
            for (int i = 0; i < _groups.Count; i++)
            {
                var group = _groups[i];
                var control = groupsContainer.Controls.OfType<ItemStatGroupValidatorUserControl>()
                    .FirstOrDefault(c => c.Tag == group);

                if (control == null) continue;

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
                control.UpdateOperatorVisibility(shouldShow);
            }
        }

        private Point _dragStartPoint;
        private ItemStatGroupValidatorUserControl _draggedControl;

        private void GroupControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && e.Y < 32)
            {
                _dragStartPoint = e.Location;
                _draggedControl = sender as ItemStatGroupValidatorUserControl;
            }
        }

        private void GroupControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && _draggedControl != null)
            {
                if (Math.Abs(e.X - _dragStartPoint.X) > 5 || Math.Abs(e.Y - _dragStartPoint.Y) > 5)
                    _draggedControl.DoDragDrop(_draggedControl, DragDropEffects.Move);
            }
        }

        private void GroupControl_DragDrop(object sender, DragEventArgs e)
        {
            if (sender is not ItemStatGroupValidatorUserControl targetControl ||
                e.Data.GetData(typeof(ItemStatGroupValidatorUserControl)) is not ItemStatGroupValidatorUserControl sourceControl ||
                targetControl == sourceControl)
                return;

            var sourceGroup = sourceControl.Tag as ValidationGroupModel;
            var targetGroup = targetControl.Tag as ValidationGroupModel;

            int sourceIndex = _groups.IndexOf(sourceGroup);
            int targetIndex = _groups.IndexOf(targetGroup);

            _groups.RemoveAt(sourceIndex);
            _groups.Insert(targetIndex, sourceGroup);

            ArrangeGroupsInGrid();
            RevalidateAllGroups();
        }

        private void DeleteGroup(ValidationGroupModel group, ItemStatGroupValidatorUserControl control)
        {
            _groups.Remove(group);
            groupsContainer.Controls.Remove(control);
            control.Dispose();

            ArrangeGroupsInGrid();
            RevalidateAllGroups();
        }

        private void ArrangeGroupsInGrid()
        {
            if (groupsContainer == null || _groups.Count == 0) return;

            int containerWidth = groupsContainer.ClientSize.Width - 5;
            int columnsPerRow = Math.Max(1, (containerWidth - GROUP_MARGIN) / (CONTROL_WIDTH + GROUP_MARGIN));

            int currentRow = 0;
            int currentCol = 0;

            foreach (var group in _groups)
            {
                var control = groupsContainer.Controls.OfType<ItemStatGroupValidatorUserControl>()
                    .FirstOrDefault(c => c.Tag == group);

                if (control != null)
                {
                    int x = GROUP_MARGIN + currentCol * (CONTROL_WIDTH + GROUP_MARGIN);
                    int y = GROUP_MARGIN + currentRow * (CONTROL_HEIGHT + GROUP_MARGIN);

                    control.Location = new Point(x, y);

                    currentCol++;
                    if (currentCol >= columnsPerRow)
                    {
                        currentCol = 0;
                        currentRow++;
                    }
                }
            }

            int totalRows = (int)Math.Ceiling((double)_groups.Count / columnsPerRow);
            int minHeight = totalRows * (CONTROL_HEIGHT + GROUP_MARGIN) + GROUP_MARGIN;
            groupsContainer.AutoScrollMinSize = new Size(0, minHeight);
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

        private void GroupsContainer_Resize(object sender, EventArgs e)
        {
            ArrangeGroupsInGrid();
        }
    }
}