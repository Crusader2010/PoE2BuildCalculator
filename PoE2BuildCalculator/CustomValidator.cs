using System.Collections.Immutable;
using System.ComponentModel;
using System.Reflection;

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
			var activeOperations = operations?.Where(g => g != null && g.IsActive)?.ToList() ?? [];
			var activeGroups = _groups?.Where(x => x.IsActive)?.ToDictionary(g => g.GroupId, g => g) ?? [];
			if (activeGroups.Count == 0 || activeOperations.Count == 0) return null;

			return items =>
			{
				return EvaluateValidationModels(activeOperations, items, activeGroups); ;
			};
		}

		private List<ValidationModel> BuildValidationModels()
		{
			var result = _operationControls.Select(x => x.GetValidationModel()).Where(x => x != null && x.IsActive).ToList();
			return result;
		}

		private bool EvaluateValidationModels(List<ValidationModel> activeOperations, List<Item> items, Dictionary<int, Group> activeGroups)
		{
			if (items == null || items.Count == 0) return true;
			bool overallResult = true;

			foreach (var operation in activeOperations)
			{
				var group = operation.GroupId >= 0 && activeGroups.TryGetValue(operation.GroupId, out var dictGroup) ? dictGroup : null;
				if (group == null) continue;

				foreach (var item in items)
				{
					foreach (var stat in group.Stats)
					{
						//if (item.ItemStats.TryGetValue(stat.StatId, out var itemStat))
						//{
						//	bool statResult = ValidationModel.EvaluateStatCondition(itemStat.Value, stat.Condition, stat.Value);
						//	overallResult = operation.GroupLevelOperator switch
						//	{
						//		GroupLevelOperator.AND => overallResult && statResult,
						//		GroupLevelOperator.OR => overallResult || statResult,
						//		_ => overallResult
						//	};
						//}
					}
				}
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
	}
}
