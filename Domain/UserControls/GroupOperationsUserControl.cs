using System.Collections.Immutable;

using Domain.Enums;
using Domain.Helpers;
using Domain.Static;

namespace Domain.UserControls
{
    public partial class GroupOperationsUserControl : UserControl
    {
        private ImmutableDictionary<int, string> _groups; // Key: Group ID, Value: Group Name

        /// <summary>
        /// Initializes a new instance of the GroupOperationsUserControl class.
        /// </summary>
        /// <param name="groups">Key: Group ID, Value: Group Name</param>
        public GroupOperationsUserControl(ImmutableDictionary<int, string> groups)
        {
            _groups = groups;
            InitializeComponent();

            this.Margin = new Padding(0, 0, 1, 3);
            this.Padding = new Padding(0);
        }

        #region Helper methods
        private void InitializeComboboxGroup()
        {
            ComboBoxGroup.SuspendLayout();
            ComboBoxGroup.Items.Clear();

            if (_groups.Count > 0)
            {
                ComboBoxGroup.DataSource = new BindingSource(_groups, null);
                ComboBoxGroup.DisplayMember = "Value";  // Show the group name
                ComboBoxGroup.ValueMember = "Key";
            }

            ComboBoxGroup.SelectedIndex = -1; // No selection initially
            ComboBoxGroup.ResumeLayout();
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
            ComboBoxOperatorMax.Items.AddRange(EnumDescriptionCache<MinMaxOperatorsEnum>.DescriptionsArray);
            ComboBoxOperatorMin.Items.AddRange(EnumDescriptionCache<MinMaxOperatorsEnum>.DescriptionsArray);

            ComboBoxGroupLevelOperator.SelectedIndex = 0;
            ComboBoxMinMaxOperator.SelectedIndex = 0;
            ComboBoxOperatorMin.SelectedIndex = 0;
            ComboBoxOperatorMax.SelectedIndex = 0;

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

        public void SetGroupComboBoxDictionary(ImmutableDictionary<int, string> groups)
        {
            _groups = groups;
            InitializeComboboxGroup();
        }



        #endregion

        private void GroupValidatorListUserControl_Load(object sender, EventArgs e)
        {
            InitializeFormControlsDefaultState();
            InitializeComboboxGroup();
        }

        private void ComboBoxEachItemOperator_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void ButtonHelp_Click(object sender, EventArgs e)
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
            if (OptionEachItem.Checked) PanelItemCount.Visible = false;
        }

        private void OptionAtMost_CheckedChanged(object sender, EventArgs e)
        {
            if (OptionAtMost.Checked) PanelItemCount.Visible = true;
        }

        private void OptionAtLeast_CheckedChanged(object sender, EventArgs e)
        {
            if (OptionAtLeast.Checked) PanelItemCount.Visible = true;
        }

        private void OptionSumAll_CheckedChanged(object sender, EventArgs e)
        {
            if (OptionSumAll.Checked) PanelItemCount.Visible = false;
        }

        private void CheckboxMax_CheckedChanged(object sender, EventArgs e)
        {
            ComboBoxOperatorMax.Enabled = CheckboxMax.Checked;
            ComboBoxMinMaxOperator.Enabled = CheckboxMax.Checked && CheckboxMin.Checked;
        }

        private void CheckboxMin_CheckedChanged(object sender, EventArgs e)
        {
            ComboBoxOperatorMin.Enabled = CheckboxMin.Checked;
            ComboBoxMinMaxOperator.Enabled = CheckboxMax.Checked && CheckboxMin.Checked;
        }

        private void ComboBoxGroup_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void ButtonDeleteOperation_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
