using System.ComponentModel;

using Domain.Main;
using Domain.Static;
using Domain.UserControls;
using Domain.Validation;

namespace PoE2BuildCalculator
{
	public partial class CustomValidator : Form
	{
		private Func<List<Item>, bool> _masterValidator = x => true;
		private readonly BindingList<ValidationGroupModel> _groups = [];
		private readonly MainForm _ownerForm;
		private int _nextGroupId = 1;
		private Point _dragStartPoint;
		private ItemStatGroupValidatorUserControl _draggedControl;

		// Cached layout calculations
		private int _lastContainerWidth = -1;
		private int _cachedColumnsPerRow = -1;
		private (int widthStat, int heightStat, int heightGroupTop) _cachedSizes;

		private const int GROUP_MARGIN = 10;
		private const int GROUP_ITEMSTATSROWS_VISIBLE = 5;
		private const int GROUP_CONTROL_WIDTH = 350;
		private const int GROUP_CONTROL_HEIGHT = 350;

		public CustomValidator(MainForm ownerForm)
		{
			ArgumentNullException.ThrowIfNull(ownerForm);
			InitializeComponent();
			_ownerForm = ownerForm;

			// Enable double buffering for smoother rendering
			SetStyle(ControlStyles.OptimizedDoubleBuffer |
					 ControlStyles.AllPaintingInWmPaint |
					 ControlStyles.UserPaint, true);
			UpdateStyles();
		}

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
				Font = new Font("Consolas", 9.5f),
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

			groupsContainer.SuspendLayout();
			try
			{
				CreateGroupControl(group);
				//ArrangeGroupsInGrid();
				//RevalidateAllGroups();
			}
			finally
			{
				groupsContainer.ResumeLayout(true);
			}
		}

		private void CreateGroupControl(ValidationGroupModel group)
		{
			try
			{
				var control = new ItemStatGroupValidatorUserControl(group.GroupId, group.GroupName)
				{
					Width = _cachedSizes.widthStat + 25, // account for scrollbar
					Height = _cachedSizes.heightGroupTop + (_cachedSizes.heightStat * GROUP_ITEMSTATSROWS_VISIBLE) + 5,
					Tag = group,
					AllowDrop = true
				};

				control.DeleteRequested += (s, e) => DeleteGroup(group, control);
				//control.ValidationChanged += (s, e) => RevalidateAllGroups();
				control.MouseDown += GroupControl_MouseDown;
				control.MouseMove += GroupControl_MouseMove;
				control.DragOver += (s, e) => e.Effect = DragDropEffects.Move;
				control.DragDrop += GroupControl_DragDrop;

				groupsContainer.Controls.Add(control);
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error creating group control: {ex}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void RevalidateAllGroups()
		{
			btnAddGroup.Enabled = _groups.Count == 0 || ValidateLastGroup();
			UpdateAllGroupOperatorVisibility();
		}

		private bool ValidateLastGroup()
		{
			var lastGroup = _groups[^1];
			bool hasConstraint = lastGroup.IsMinEnabled || lastGroup.IsMaxEnabled;
			bool hasStats = lastGroup.Stats.Count > 0;
			bool isValid = !(lastGroup.IsMinEnabled && lastGroup.IsMaxEnabled &&
						   lastGroup.MinValue.HasValue && lastGroup.MaxValue.HasValue &&
						   lastGroup.MinValue.Value > lastGroup.MaxValue.Value);

			return hasConstraint && hasStats && isValid;
		}

		private void UpdateAllGroupOperatorVisibility()
		{
			for (int i = 0; i < _groups.Count; i++)
			{
				var group = _groups[i];
				var control = FindControlForGroup(group);
				if (control == null) continue;

				bool currentHasConstraint = group.IsMinEnabled || group.IsMaxEnabled;
				bool currentHasStats = group.Stats.Count > 0;
				bool hasNextValidGroup = HasNextValidGroup(i);

				//ItemStatGroupValidatorUserControl.UpdateOperatorVisibility(currentHasConstraint && currentHasStats && hasNextValidGroup);
			}
		}

		private bool HasNextValidGroup(int currentIndex)
		{
			for (int j = currentIndex + 1; j < _groups.Count; j++)
			{
				var nextGroup = _groups[j];
				if ((nextGroup.IsMinEnabled || nextGroup.IsMaxEnabled) && nextGroup.Stats.Count > 0)
					return true;
			}
			return false;
		}

		private ItemStatGroupValidatorUserControl FindControlForGroup(ValidationGroupModel group)
		{
			return groupsContainer.Controls.OfType<ItemStatGroupValidatorUserControl>()
				.FirstOrDefault(c => c.Tag == group);
		}

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
			if (e.Button == MouseButtons.Left && _draggedControl != null &&
				(Math.Abs(e.X - _dragStartPoint.X) > 5 || Math.Abs(e.Y - _dragStartPoint.Y) > 5))
			{
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

			groupsContainer.SuspendLayout();
			try
			{
				ArrangeGroupsInGrid();
				RevalidateAllGroups();
			}
			finally
			{
				groupsContainer.ResumeLayout(true);
			}
		}

		private void DeleteGroup(ValidationGroupModel group, ItemStatGroupValidatorUserControl control)
		{
			_groups.Remove(group);

			groupsContainer.SuspendLayout();
			try
			{
				groupsContainer.Controls.Remove(control);
				control.Dispose();
				ArrangeGroupsInGrid();
				RevalidateAllGroups();
			}
			finally
			{
				groupsContainer.ResumeLayout(true);
			}
		}

		private void ArrangeGroupsInGrid()
		{
			if (groupsContainer == null || _groups.Count == 0) return;

			int containerWidth = groupsContainer.ClientSize.Width - 5;

			// Use cached calculation if width hasn't changed
			int columnsPerRow;
			if (containerWidth == _lastContainerWidth && _cachedColumnsPerRow > 0)
			{
				columnsPerRow = _cachedColumnsPerRow;
			}
			else
			{
				columnsPerRow = Math.Max(1, (containerWidth - GROUP_MARGIN) / (GROUP_CONTROL_WIDTH + GROUP_MARGIN));
				_lastContainerWidth = containerWidth;
				_cachedColumnsPerRow = columnsPerRow;
			}

			int currentRow = 0, currentCol = 0;

			foreach (var group in _groups)
			{
				var control = FindControlForGroup(group);
				if (control != null)
				{
					control.Location = new Point(
						GROUP_MARGIN + currentCol * (GROUP_CONTROL_WIDTH + GROUP_MARGIN),
						GROUP_MARGIN + currentRow * (GROUP_CONTROL_HEIGHT + GROUP_MARGIN)
					);

					if (++currentCol >= columnsPerRow)
					{
						currentCol = 0;
						currentRow++;
					}
				}
			}

			int totalRows = (int)Math.Ceiling((double)_groups.Count / columnsPerRow);
			groupsContainer.AutoScrollMinSize = new Size(0, totalRows * (GROUP_CONTROL_HEIGHT + GROUP_MARGIN) + GROUP_MARGIN);
		}

		private void BtnCreateValidator_Click(object sender, EventArgs e)
		{
			try
			{
				var validatorFunction = BuildValidatorFunction([.. _groups]);
				if (validatorFunction == null)
				{
					MessageBox.Show("No validation function can be computed based on the existing groups.\n\nKeeping default logic -> all combinations are valid.", "No usable groups", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return;
				}

				_masterValidator = validatorFunction;
				_ownerForm._itemValidatorFunction = _masterValidator;
				MessageBox.Show($"Validator created with {_groups.Count(x => x.IsActive)} ACTIVE group(s)!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error creating validator: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private static Func<List<Item>, bool> BuildValidatorFunction(List<ValidationGroupModel> groups)
		{
			if (groups == null || groups.Count == 0) return null;

			var activeGroups = groups.Where(g => g.IsActive).ToList();
			if (activeGroups.Count == 0) return null;

			return items =>
			{
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
			double sum = items.Sum(item => EvaluateExpression(group.Stats, item.ItemStats));

			if (group.IsMinEnabled && group.MinValue.HasValue && sum <= group.MinValue.Value)
				return false;

			if (group.IsMaxEnabled && group.MaxValue.HasValue && sum >= group.MaxValue.Value)
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

				result = stats[i].Operator switch
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
			// Invalidate cache on resize
			_lastContainerWidth = -1;
			_cachedColumnsPerRow = -1;

			groupsContainer.SuspendLayout();
			try
			{
				ArrangeGroupsInGrid();
			}
			finally
			{
				groupsContainer.ResumeLayout(true);
			}
		}

		private static (int widthStat, int heightStat, int heightGroupTop) GetUserControlSizes()
		{
			using var tempGroup = new ItemStatGroupValidatorUserControl(int.MaxValue, string.Empty);
			using var tempRow = new ItemStatRow(int.MaxValue, string.Empty, tempGroup);
			return (tempRow.Width + 2, tempRow.Height, tempGroup.GetTopRowsHeight());
		}

		private void CustomValidator_Load(object sender, EventArgs e)
		{
			_cachedSizes = GetUserControlSizes();
		}
	}
}
