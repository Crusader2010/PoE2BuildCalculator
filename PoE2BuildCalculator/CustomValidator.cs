using System.Collections.Immutable;
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

		private readonly MainForm _ownerForm;
		private int _nextGroupId = 1;

		// Cached layout calculations
		private (int widthStat, int heightStat, int heightGroupTop, int widthGroup, int widthGroupOperation) _cachedSizes;
		private const int GROUP_ITEMSTATSROWS_VISIBLE = 5;

		private readonly BindingList<Group> _groups = [];
		private readonly BindingList<GroupOperationsUserControl> _operationControls = [];

		private ImmutableDictionary<int, string> _immutableGroups
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
			_groups.ListChanged += (s, e) => _immutableGroups = null;

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

		private void btnAddOperation_Click(object sender, EventArgs e)
		{
			FlowPanelOperations.SuspendLayout();
			try
			{
				var operationControl = new GroupOperationsUserControl(_immutableGroups)
				{
					BackColor = _operationControls.Count % 2 == 0 ? Color.LightGray : Color.LightSlateGray
				};

				FlowPanelOperations.Controls.Add(operationControl);

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

				control.DeleteRequested += (s, e) => DeleteGroup(control);
				FlowPanelGroups.Controls.Add(control);

				_groups.Add(control._group);
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

				// reset operations that have that group as selected
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
				var validatorFunction = BuildValidatorFunction(BuildValidationModelWithOperations());
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

		private static Func<List<Item>, bool> BuildValidatorFunction(List<ValidationModel> groupValidationModels)
		{
			if (groupValidationModels == null || groupValidationModels.Count == 0) return null;

			var activeGroups = groupValidationModels.Where(g => g.IsActive).ToList();
			if (activeGroups.Count == 0) return null;

			return items =>
			{
				return EvaluateValidationModels(activeGroups, items); ;
			};
		}

		private static List<ValidationModel> BuildValidationModelWithOperations()
		{
			return [];
		}

		private static bool EvaluateValidationModels(List<ValidationModel> groupsWithOperations, List<Item> items)
		{
			if (groupsWithOperations == null || groupsWithOperations.Count == 0) return true;

			return true;
		}

		private static (int widthStat, int heightStat, int heightGroupTop, int widthGroup, int widthGroupOperation) GetUserControlSizes()
		{
			using var tempGroupOperation = new GroupOperationsUserControl(ImmutableDictionary<int, string>.Empty);
			using var tempGroup = new ItemStatGroupValidatorUserControl(int.MaxValue, string.Empty);
			using var tempRow = new ItemStatRow(int.MaxValue, string.Empty, tempGroup);
			{
				tempGroup.Width = tempRow.Width + 25; // account for scrollbar
			}

			return (tempRow.Width + 2, tempRow.Height, tempGroup.GetTopRowsHeight(), tempGroup.Width + 2, tempGroupOperation.Width);
		}

		private void CustomValidator_Load(object sender, EventArgs e)
		{
			this.SuspendLayout();

			_cachedSizes = GetUserControlSizes();
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

			this.AutoSize = false;
			this.Width = FlowPanelOperations.Width + FlowPanelOperations.Padding.Left + FlowPanelOperations.Padding.Right
							+ mainPanel.Padding.Left + mainPanel.Padding.Right + delimiter.Width
							+ panel1.Padding.Left + panel1.Padding.Right + _cachedSizes.widthGroup * 2 + 45;
			this.CenterToScreen();

			this.ResumeLayout(true);
		}
	}
}
