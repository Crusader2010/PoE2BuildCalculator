using System.Collections.Immutable;
using System.ComponentModel;
using System.Reflection;

using Domain.Enums;
using Domain.Helpers;
using Domain.Main;
using Domain.Static;
using Domain.UserControls;
using Domain.Validation;

namespace PoE2BuildCalculator
{
    public partial class CustomValidator : Form
    {
        private Func<List<Item>, bool> _masterValidator = x => true;

        private readonly MainForm _ownerForm;
        private int _nextGroupId = 1;

        // Cached layout calculations
        private readonly (int widthStat, int heightStat, int heightGroupTop, int widthGroup, int widthGroupOperation) _cachedSizes;
        private const int GROUP_ITEMSTATSROWS_VISIBLE = 5;

        private readonly BindingList<Group> _groups = [];
        private readonly BindingList<GroupOperationsUserControl> _operationControls = [];

        private ImmutableDictionary<int, string> _immutableGroupDescriptions
        {
            get
            {
                field ??= _groups.Count == 0
                        ? ImmutableDictionary<int, string>.Empty
                        : _groups.ToImmutableDictionary(g => g.GroupId, g => g.GroupName);

                return field;
            }

            set;
        }

        public CustomValidator(MainForm ownerForm)
        {
            ArgumentNullException.ThrowIfNull(ownerForm);
            InitializeComponent();

            _ownerForm = ownerForm;
            _groups.ListChanged += (s, e) => _immutableGroupDescriptions = null;
            _cachedSizes = GetUserControlSizes();

            // Enable double buffering for smoother rendering
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint, true);
            UpdateStyles();
        }

        #region Validation and parsing methods

        private Func<List<Item>, bool> BuildValidatorFunction(List<ValidationModel> operations)
        {
            var groups = _groups.ToDictionary(g => g.GroupId, g => g);
            if (groups.Count == 0 || operations.Count == 0) return null;

            return items =>
            {
                return EvaluateValidationModels(operations, items, groups); ;
            };
        }

        private List<ValidationModel> BuildValidationModels()
        {
            var result = _operationControls.Select(x => x.GetValidationModel()).ToList(); // all are presumed active!
            return result;
        }

        /// <summary>
        /// The input groups and operations need to be active already, with proper values and valid stats.
        /// </summary>
        /// <param name="activeOperations"></param>
        /// <param name="items"></param>
        /// <param name="activeGroups"></param>
        /// <returns></returns>
        private static bool EvaluateValidationModels(List<ValidationModel> activeOperations, List<Item> items, Dictionary<int, Group> activeGroups)
        {
            if (items == null || items.Count == 0) return true;
            bool overallResult = true;

            ValidationModel prevOperation = activeOperations[0];
            for (int i1 = 1; i1 < activeOperations.Count; i1++)
            {
                var prevGroup = prevOperation.GroupId >= 0 && activeGroups.TryGetValue(prevOperation.GroupId, out var dictGroupPrev) ? dictGroupPrev : null;
                if (prevGroup == null) continue;

                ValidationModel nextOperation = activeOperations[i1];
                var nextGroup = nextOperation.GroupId >= 0 && activeGroups.TryGetValue(nextOperation.GroupId, out var dictGroupNext) ? dictGroupNext : null;
                if (nextGroup == null) continue;

                foreach (var item in items)
                {
                    var itemStats = ItemStatsHelper.ToDictionary(item.ItemStats);
                    double finalItemValue = itemStats.TryGetValue(nextGroup.Stats[0].PropertyName, out var initialStat) ? (double)initialStat : 0.0;

                    for (int i = 1; i < nextGroup.Stats.Count; i++)
                    {
                        var nextValue = itemStats.TryGetValue(nextGroup.Stats[i].PropertyName, out var nextStat) ? (double)nextStat : 0.0;
                        switch (nextGroup.Stats[i - 1].Operator)
                        {
                            case ArithmeticOperationsEnum.Sum:
                                finalItemValue += nextValue;
                                break;
                            case ArithmeticOperationsEnum.Diff:
                                finalItemValue -= nextValue;
                                break;
                            case ArithmeticOperationsEnum.Mult:
                                finalItemValue *= nextValue;
                                break;
                            case ArithmeticOperationsEnum.Div:
                                finalItemValue /= nextValue;
                                break;
                            default:
                                break;
                        }
                    }

                    if (nextOperation.ValidationType == ValidationTypeEnum.EachItem && !ValidateOperationTypeEachItem(nextOperation, finalItemValue)) return false;
                }

                prevOperation = nextOperation; // this probably needs to be a clone so they dont end up referecing the same object
            }

            return overallResult;
        }

        #endregion

        private void BtnHelp_Click(object sender, EventArgs e)
        {
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
                Font = new Font("Verdana", 10f),
                Text = Constants.VALIDATOR_HELP_TEXT,
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
                Font = new Font("Verdana", 10, FontStyle.Bold)
            };

            helpForm.Controls.Add(txtHelp);
            helpForm.Controls.Add(btnOk);
            helpForm.ShowDialog(this);
        }

        private void BtnAddOperation_Click(object sender, EventArgs e)
        {
            FlowPanelOperations.SuspendLayout();
            try
            {
                var operationControl = new GroupOperationsUserControl(_immutableGroupDescriptions, this)
                {
                    BackColor = _operationControls.Count % 2 == 0 ? Color.LightGray : Color.LightSlateGray
                };

                FlowPanelOperations.Controls.Add(operationControl);

                if (_operationControls.Count > 0) _operationControls[^1].SetComboBoxGroupLevelOperatorEnabled(true);
                operationControl.GroupOperationDeleted += OperationControl_GroupOperationDeleted;

                _operationControls.Add(operationControl);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating group operation control: {ex}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                FlowPanelOperations.ResumeLayout();
            }
        }

        private void OperationControl_GroupOperationDeleted(object sender, EventArgs e)
        {
            this.SuspendLayout();
            FlowPanelOperations.SuspendLayout();
            try
            {
                if (sender is not null and GroupOperationsUserControl operationControl)
                {
                    _operationControls.Remove(operationControl);
                    RefreshGroupOperationsAfterDelete();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error when deleting a group operation control: {ex}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                FlowPanelOperations.ResumeLayout();
                this.ResumeLayout();
            }
        }

        private void BtnAddGroup_Click(object sender, EventArgs e)
        {
            FlowPanelGroups.SuspendLayout();
            try
            {
                var control = new ItemStatGroupValidatorUserControl(_nextGroupId++, $"Group {_groups.Count + 1}")
                {
                    Width = _cachedSizes.widthStat + 25, // account for scrollbar
                    Height = _cachedSizes.heightGroupTop + (_cachedSizes.heightStat * GROUP_ITEMSTATSROWS_VISIBLE) + 5,
                    AllowDrop = false
                };

                control.GroupDeleted += (s, e) => DeleteGroup(control);
                FlowPanelGroups.Controls.Add(control);

                _groups.Add(control._group);
                RefreshGroupOperationsAfterGroupsChanged();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating group control: {ex}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                FlowPanelGroups.ResumeLayout(true);
            }
        }

        private void DeleteGroup(ItemStatGroupValidatorUserControl control)
        {
            _groups.Remove(control._group);

            FlowPanelGroups.SuspendLayout();
            try
            {
                FlowPanelGroups.Controls.Remove(control);
                control.Dispose();

                RefreshGroupOperationsAfterGroupsChanged();
            }
            finally
            {
                FlowPanelGroups.ResumeLayout(true);
            }
        }

        private void BtnCreateValidator_Click(object sender, EventArgs e)
        {
            try
            {
                var check = CheckInactiveSelectedGroupsOrOperations();
                if (!string.IsNullOrWhiteSpace(check))
                {
                    MessageBox.Show(check, "Inactive groups or operations", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var validatorFunction = BuildValidatorFunction(BuildValidationModels());
                if (validatorFunction == null)
                {
                    MessageBox.Show("No validation function can be computed based on the existing groups.\n\nKeeping default logic -> all combinations are valid.", "No usable groups", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _masterValidator = validatorFunction;
                _ownerForm._itemValidatorFunction = _masterValidator;
                MessageBox.Show($"Validator created with {_groups.Count(x => x.Stats.Count > 0)} ACTIVE group(s)!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating validator: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static (int widthStat, int heightStat, int heightGroupTop, int widthGroup, int widthGroupOperation) GetUserControlSizes()
        {
            using var tempGroupOperation = new GroupOperationsUserControl(ImmutableDictionary<int, string>.Empty, new Form());
            using var tempGroup = new ItemStatGroupValidatorUserControl(int.MaxValue, string.Empty);
            using var tempRow = new ItemStatRow(int.MaxValue, string.Empty);
            {
                tempGroup.Width = tempRow.Width + 25; // account for scrollbar
            }

            return (tempRow.Width + 2, tempRow.Height, tempGroup.GetTopRowsHeight(), tempGroup.Width + 2, tempGroupOperation.Width);
        }

        private void CustomValidator_Load(object sender, EventArgs e)
        {
            this.SuspendLayout();

            FlowPanelOperations.Padding = new Padding(0, 0, 1, 0);
            FlowPanelOperations.Width = _cachedSizes.widthGroupOperation + 20;

            Panel delimiter = new()
            {
                Width = 2,
                Dock = DockStyle.Left,
                BackColor = Color.YellowGreen
            };
            panel1.Controls.Add(delimiter);

            FlowPanelOperations.BringToFront();
            delimiter.BringToFront();
            FlowPanelGroups.BringToFront();

            typeof(Panel).InvokeMember("DoubleBuffered", BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic, null, FlowPanelGroups, [true]);
            typeof(Panel).InvokeMember("DoubleBuffered", BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic, null, FlowPanelOperations, [true]);

            this.AutoSize = false;
            this.Width = FlowPanelOperations.Width + FlowPanelOperations.Padding.Left + FlowPanelOperations.Padding.Right
                            + mainPanel.Padding.Left + mainPanel.Padding.Right + delimiter.Width
                            + panel1.Padding.Left + panel1.Padding.Right + _cachedSizes.widthGroup * 2 + 45;
            this.CenterToScreen();

            this.ResumeLayout(true);
        }

        private void RefreshGroupOperationsAfterDelete()
        {
            for (int i = 0; i < _operationControls.Count; i++)
            {
                _operationControls[i].RefreshGroupOperationAfterDelete(_operationControls.Count, i);
            }
        }

        private void RefreshGroupOperationsAfterGroupsChanged()
        {
            for (int i = 0; i < _operationControls.Count; i++)
            {
                _operationControls[i].UpdateGroups(_immutableGroupDescriptions);
            }
        }

        private string CheckInactiveSelectedGroupsOrOperations()
        {
            var grpMsg = string.Empty;
            var operMsg = string.Empty;

            var inactiveGroups = _groups.Where(x => x == null || !x.IsActive).ToList();
            if (inactiveGroups.Count > 0)
            {
                grpMsg = $"The following groups are inactive: {string.Join(",", inactiveGroups.Select(x => x.GroupName))}.";
            }

            var inactiveGroupIds = inactiveGroups.Select(x => x.GroupId).ToHashSet();
            int inactiveOperations = _operationControls
                .Select(x => x.GetValidationModel())
                .Count(x => x == null || !x.IsActive || x.GroupId == -1 || inactiveGroupIds.Contains(x.GroupId));

            if (inactiveOperations > 0)
            {
                operMsg = $"There are currently {inactiveOperations} that are either inactive or have an inactive or no group selected.";
            }

            return string.Join("\r\n", grpMsg, operMsg, "\r\nBefore a validation function can be created, you must remove or edit the inactive or invalid groups and operations.");
        }

        private static bool ValidateOperationTypeEachItem(ValidationModel validationModel, double value)
        {
            if (validationModel.ValidationType == ValidationTypeEnum.EachItem)
            {
                if (validationModel.MinValue.HasValue && validationModel.MaxValue.HasValue)
                {
                    switch (validationModel.MinMaxOperator)
                    {
                        case MinMaxCombinedOperatorsEnum.AND:
                            if (!(EvaluateArithmeticOperation(validationModel.MinOperator, value, validationModel.MinValue ?? 0.0) && EvaluateArithmeticOperation(validationModel.MaxOperator, value, validationModel.MaxValue ?? 0.0)))
                            {
                                return false;
                            }
                            break;
                        case MinMaxCombinedOperatorsEnum.OR:
                            if (!(EvaluateArithmeticOperation(validationModel.MinOperator, value, validationModel.MinValue ?? 0.0) || EvaluateArithmeticOperation(validationModel.MaxOperator, value, validationModel.MaxValue ?? 0.0)))
                            {
                                return false;
                            }
                            break;
                        case null:
                        default:
                            return false;
                    }
                }
                else if (validationModel.MinValue.HasValue)
                {
                    if (!EvaluateArithmeticOperation(validationModel.MinOperator, value, validationModel.MinValue.Value)) return false;
                }
                else if (validationModel.MaxValue.HasValue)
                {
                    if (!EvaluateArithmeticOperation(validationModel.MaxOperator, value, validationModel.MaxValue.Value)) return false;
                }
            }

            return true;
        }

        private static bool EvaluateArithmeticOperation(MinMaxOperatorsEnum? arithOperator, double existingValue, double testValue)
        {
            return arithOperator switch
            {
                MinMaxOperatorsEnum.Greater => existingValue > testValue,
                MinMaxOperatorsEnum.GreaterEqual => existingValue >= testValue,
                MinMaxOperatorsEnum.Equal => existingValue == testValue,
                MinMaxOperatorsEnum.Less => existingValue < testValue,
                MinMaxOperatorsEnum.LessEqual => existingValue <= testValue,
                _ => false,
            };
        }
    }
}
