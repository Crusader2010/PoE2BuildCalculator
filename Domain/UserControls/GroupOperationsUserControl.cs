using System.Collections.Immutable;
using System.ComponentModel;

using Domain.Enums;
using Domain.Events;
using Domain.Helpers;
using Domain.Static;
using Domain.Validation;

namespace Domain.UserControls
{
	public partial class GroupOperationsUserControl : UserControl
	{
		public event EventHandler GroupOperationDeleted;

		private ImmutableDictionary<int, string> _groups; // Key: Group ID, Value: Group Name
		private readonly Form _ownerForm;
		private readonly ErrorProvider _errorProvider = new();
		private bool _needsRefresh = false;
		private bool _hasErrors = false;

		/// <summary>
		/// Initializes a new instance of the GroupOperationsUserControl class.
		/// </summary>
		/// <param name="groups">Key: Group ID, Value: Group Name</param>
		public GroupOperationsUserControl(ImmutableDictionary<int, string> groups, Form ownerForm)
		{
			_groups = groups;
			_ownerForm = ownerForm;
			InitializeComponent();

			this.Margin = new Padding(0, 0, 1, 3);
			this.Padding = new Padding(0);
		}

		#region Helper methods
		private void RefreshComboBoxGroup()
		{
			if (_needsRefresh)
			{
				ComboBoxGroup.SuspendLayout();
				if (_groups.Count > 0)
				{
					ComboBoxGroup.DataSource = _groups.OrderBy(x => x.Key).ToList();
					ComboBoxGroup.DisplayMember = "Value";  // Show the group name
					ComboBoxGroup.ValueMember = "Key";
					ComboBoxGroup.SelectedIndex = -1; // No selection initially
				}
				else
				{
					ComboBoxGroup.DataSource = null;
					ComboBoxGroup.Height = ComboBoxGroupLevelOperator.Height;
				}

				_needsRefresh = false;
				ComboBoxGroup.ResumeLayout();
			}
		}

		private void InitializeFormControlsDefaultState()
		{
			this.SuspendLayout();

			ComboBoxGroupLevelOperator.Items.Clear();
			ComboBoxMinMaxOperator.Items.Clear();
			ComboBoxOperatorMax.Items.Clear();
			ComboBoxOperatorMin.Items.Clear();

			ComboBoxGroupLevelOperator.Items.AddRange(EnumDescriptionCache<GroupLevelOperatorsEnum>.DescriptionsArray);
			ComboBoxMinMaxOperator.Items.AddRange(EnumDescriptionCache<MinMaxCombinedOperatorsEnum>.DescriptionsArray);
			ComboBoxOperatorMin.Items.AddRange(EnumDescriptionCache<MinMaxOperatorsEnum>.DescriptionsArray);
			ComboBoxOperatorMax.Items.AddRange(EnumDescriptionCache<MinMaxOperatorsEnum>.DescriptionsArray);

			ComboBoxGroupLevelOperator.SelectedIndex = 0;
			ComboBoxMinMaxOperator.SelectedIndex = 0;
			ComboBoxOperatorMin.SelectedIndex = 0;
			ComboBoxOperatorMax.SelectedItem = MinMaxOperatorsEnum.LessEqual.GetDescription();

			CheckboxMin.Checked = true;
			CheckboxMax.Checked = false;
			CheckboxPercentage.Checked = false;

			OptionSumAll.Checked = true;
			PanelItemCount.Visible = false;
			ComboBoxMinMaxOperator.Enabled = false;

			lblItems.Height = InputBoxItemsCount.Height;
			OptionAtLeast.Text = ValidationTypeEnum.AtLeast.GetDescription();
			OptionAtMost.Text = ValidationTypeEnum.AtMost.GetDescription();
			OptionEachItem.Text = ValidationTypeEnum.EachItem.GetDescription();
			OptionSumAll.Text = ValidationTypeEnum.SumALL.GetDescription();

			ComboBoxGroup.MouseWheel += ComboBox_MouseWheel;
			ComboBoxGroupLevelOperator.MouseWheel += ComboBox_MouseWheel;
			ComboBoxOperatorMin.MouseWheel += ComboBox_MouseWheel;
			ComboBoxOperatorMax.MouseWheel += ComboBox_MouseWheel;
			ComboBoxMinMaxOperator.MouseWheel += ComboBox_MouseWheel;

			this.ResumeLayout();
		}
		#endregion

		#region Public methods

		public void SetComboBoxGroupLevelOperatorEnabled(bool isEnabled)
		{
			ComboBoxGroupLevelOperator.Enabled = isEnabled;
		}

		public void RefreshGroupOperationAfterDelete(int totalCount, int currentIndexInList)
		{
			this.SuspendLayout();
			ComboBoxGroupLevelOperator.Enabled = currentIndexInList != totalCount - 1;
			this.BackColor = currentIndexInList % 2 == 0 ? Color.LightGray : Color.LightSlateGray;
			this.ResumeLayout();
		}

		public void UpdateGroups(ImmutableDictionary<int, string> groups)
		{
			_groups = groups;
			_needsRefresh = true;  // Mark as stale, don't refresh yet			
		}

		public ValidationModel GetValidationModel()
		{
			if (_hasErrors) return null;

			int groupId = (ComboBoxGroup.SelectedValue is int selectedGroupId) ? selectedGroupId : -1;
			var groupLevelOperator = ComboBoxGroupLevelOperator.SelectedItem == null ? (GroupLevelOperatorsEnum?)null : (EnumDescriptionCache<GroupLevelOperatorsEnum>.DescriptionToEnum.TryGetValue(ComboBoxGroupLevelOperator.SelectedItem.ToString(), out var op1) ? op1 : null);
			var minOperator = ComboBoxOperatorMin.SelectedItem == null ? (MinMaxOperatorsEnum?)null : (EnumDescriptionCache<MinMaxOperatorsEnum>.DescriptionToEnum.TryGetValue(ComboBoxOperatorMin.SelectedItem.ToString(), out var op2) ? op2 : null);
			var maxOperator = ComboBoxOperatorMax.SelectedItem == null ? (MinMaxOperatorsEnum?)null : (EnumDescriptionCache<MinMaxOperatorsEnum>.DescriptionToEnum.TryGetValue(ComboBoxOperatorMax.SelectedItem.ToString(), out var op3) ? op3 : null);
			var minMaxOperator = ComboBoxMinMaxOperator.SelectedItem == null ? (MinMaxCombinedOperatorsEnum?)null : (EnumDescriptionCache<MinMaxCombinedOperatorsEnum>.DescriptionToEnum.TryGetValue(ComboBoxMinMaxOperator.SelectedItem.ToString(), out var op4) ? op4 : null);
			var validationType = OptionAtLeast.Checked ? ValidationTypeEnum.AtLeast :
									OptionAtMost.Checked ? ValidationTypeEnum.AtMost :
									OptionEachItem.Checked ? ValidationTypeEnum.EachItem :
									ValidationTypeEnum.SumALL;

			return new ValidationModel
			{
				GroupId = groupId,
				GroupLevelOperator = ComboBoxGroupLevelOperator.Enabled ? groupLevelOperator : null,
				MinOperator = CheckboxMin.Checked ? minOperator : null,
				MaxOperator = CheckboxMax.Checked ? maxOperator : null,
				MinMaxOperator = CheckboxMax.Checked && CheckboxMin.Checked ? minMaxOperator : null,
				MaxValue = CheckboxMax.Checked ? (double)InputBoxMax.Value : null,
				MinValue = CheckboxMin.Checked ? (double)InputBoxMin.Value : null,
				NumberOfItems = (validationType is ValidationTypeEnum.AtLeast or ValidationTypeEnum.AtMost) ? (int)InputBoxItemsCount.Value : 0,
				NumberOfItemsAsPercentage = CheckboxPercentage.Checked,
				ValidationType = validationType
			};
		}

		public void LoadFromValidationModel(ValidationModel model)
		{
			this.SuspendLayout();

			// Set group
			if (ComboBoxGroup.Items.Count > 0)
			{
				for (int i = 0; i < ComboBoxGroup.Items.Count; i++)
				{
					if (ComboBoxGroup.Items[i] is KeyValuePair<int, string> kvp && kvp.Key == model.GroupId)
					{
						ComboBoxGroup.SelectedIndex = i;
						break;
					}
				}
			}

			// Set validation type
			OptionSumAll.Checked = model.ValidationType == ValidationTypeEnum.SumALL;
			OptionEachItem.Checked = model.ValidationType == ValidationTypeEnum.EachItem;
			OptionAtLeast.Checked = model.ValidationType == ValidationTypeEnum.AtLeast;
			OptionAtMost.Checked = model.ValidationType == ValidationTypeEnum.AtMost;

			// Set min/max
			CheckboxMin.Checked = model.MinOperator.HasValue;
			CheckboxMax.Checked = model.MaxOperator.HasValue;

			if (model.MinValue.HasValue) InputBoxMin.Value = (decimal)model.MinValue.Value;
			if (model.MaxValue.HasValue) InputBoxMax.Value = (decimal)model.MaxValue.Value;

			// Set operators
			if (model.MinOperator.HasValue)
				ComboBoxOperatorMin.SelectedItem = model.MinOperator.Value.GetDescription();
			if (model.MaxOperator.HasValue)
				ComboBoxOperatorMax.SelectedItem = model.MaxOperator.Value.GetDescription();
			if (model.MinMaxOperator.HasValue)
				ComboBoxMinMaxOperator.SelectedItem = model.MinMaxOperator.Value.GetDescription();
			if (model.GroupLevelOperator.HasValue)
				ComboBoxGroupLevelOperator.SelectedItem = model.GroupLevelOperator.Value.GetDescription();

			// Set item count
			if (model.ValidationType is ValidationTypeEnum.AtLeast or ValidationTypeEnum.AtMost)
			{
				InputBoxItemsCount.Value = model.NumberOfItems;
				CheckboxPercentage.Checked = model.NumberOfItemsAsPercentage;
			}

			this.ResumeLayout();
		}

		#endregion

		private void GroupOperationsUserControl_Load(object sender, EventArgs e)
		{
			InitializeFormControlsDefaultState();
			_needsRefresh = true;
		}

		private void ComboBoxEachItemOperator_SelectedIndexChanged(object sender, EventArgs e)
		{

		}

		private void ComboBox_MouseWheel(object sender, MouseEventArgs e)
		{
			if (e is HandledMouseEventArgs handledE)
			{
				handledE.Handled = true;
			}
		}

		private void OptionEachItem_CheckedChanged(object sender, EventArgs e)
		{
			if (OptionEachItem.Checked) { PanelItemCount.Visible = false; PanelItemCount.Enabled = false; }
		}

		private void OptionAtMost_CheckedChanged(object sender, EventArgs e)
		{
			if (OptionAtMost.Checked) { PanelItemCount.Visible = true; PanelItemCount.Enabled = true; }
		}

		private void OptionAtLeast_CheckedChanged(object sender, EventArgs e)
		{
			if (OptionAtLeast.Checked) { PanelItemCount.Visible = true; PanelItemCount.Enabled = true; }
		}

		private void OptionSumAll_CheckedChanged(object sender, EventArgs e)
		{
			if (OptionSumAll.Checked) { PanelItemCount.Visible = false; PanelItemCount.Enabled = false; }
		}

		private void CheckboxMax_CheckedChanged(object sender, EventArgs e)
		{
			ComboBoxOperatorMax.Enabled = CheckboxMax.Checked;
			InputBoxMax.Enabled = CheckboxMax.Checked;
			ComboBoxMinMaxOperator.Enabled = CheckboxMax.Checked && CheckboxMin.Checked;
		}

		private void CheckboxMin_CheckedChanged(object sender, EventArgs e)
		{
			ComboBoxOperatorMin.Enabled = CheckboxMin.Checked;
			InputBoxMin.Enabled = CheckboxMin.Checked;
			ComboBoxMinMaxOperator.Enabled = CheckboxMax.Checked && CheckboxMin.Checked;
		}

		private void ButtonDeleteOperation_Click(object sender, EventArgs e)
		{
			ButtonDeleteOperation.CausesValidation = false;
			GroupOperationDeleted?.Invoke(this, new ItemStatRowDeletingEventArgs { IsDeleting = true });
			this.Dispose();
		}

		private void ComboBoxGroup_DropDown(object sender, EventArgs e)
		{
			RefreshComboBoxGroup();
		}

		private void InputBoxItemsCount_Validating(object sender, CancelEventArgs e)
		{
			if (CheckboxPercentage.Checked && (InputBoxItemsCount.Value < 0 || InputBoxItemsCount.Value > 100))
			{
				e.Cancel = true;
				_hasErrors = true;
				InputBoxItemsCount.BackColor = Color.Coral;
				_errorProvider.SetError(InputBoxItemsCount, "Value must be between 0 and 100 when 'IsPercentage' is checked!");
			}
			else
			{
				_hasErrors = false;
				InputBoxItemsCount.BackColor = SystemColors.Window;
				_errorProvider.SetError(InputBoxItemsCount, "");
			}
		}

		private void CheckboxPercentage_CheckedChanged(object sender, EventArgs e)
		{
			if (CheckboxPercentage.Checked && (InputBoxItemsCount.Value is > 100 or < 0))
			{
				InputBoxItemsCount.Value = 0;
			}
		}

		private void InputBoxItemsCount_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode is Keys.Enter or Keys.Return)
			{
				var cancelEventArgs = new CancelEventArgs();
				InputBoxItemsCount_Validating(sender, cancelEventArgs);

				if (!cancelEventArgs.Cancel)
				{
					// Validation passed, move on
					this.SelectNextControl(InputBoxItemsCount, true, true, true, true);
				}
				e.SuppressKeyPress = true;
			}
		}
	}
}
