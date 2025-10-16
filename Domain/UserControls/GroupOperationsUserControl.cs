using System.Collections.Immutable;

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

        private void InitializeFormControls()
        {
            this.SuspendLayout();
            ComboBoxGroupLevelOperator.Items.Clear();
            ComboBoxMinMaxOperator.Items.Clear();
            ComboBoxOperatorMax.Items.Clear();
            ComboBoxOperatorMin.Items.Clear();

            ComboBoxGroupLevelOperator.Items.AddRange([.. Constants.LOGICAL_OPERATORS]);
            ComboBoxMinMaxOperator.Items.AddRange([.. Constants.GROUP_MIN_MAX_LOGICAL_OPERATORS]);
            ComboBoxOperatorMax.Items.AddRange([.. Constants.GROUP_VALUES_OPERATORS]);
            ComboBoxOperatorMin.Items.AddRange([.. Constants.GROUP_VALUES_OPERATORS]);

            ComboBoxGroupLevelOperator.SelectedIndex = 0;
            ComboBoxMinMaxOperator.SelectedIndex = 0;
            ComboBoxOperatorMin.SelectedIndex = 0;
            ComboBoxOperatorMax.SelectedIndex = 0;

            CheckboxMin.Checked = true;
            CheckboxMax.Checked = false;
            CheckboxPercentage.Checked = false;

            OptionSumAll.Checked = true;
            PanelItemCount.Visible = false;

            lblItems.Height = InputBoxItemsCount.Height;
            this.ResumeLayout();
        }
        #endregion


        public void SetGroupComboBoxDictionary(ImmutableDictionary<int, string> groups)
        {
            _groups = groups;
            InitializeComboboxGroup();
        }

        private void GroupValidatorListUserControl_Load(object sender, EventArgs e)
        {
            InitializeFormControls();
            InitializeComboboxGroup();
        }

        private void ComboBoxEachItemOperator_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void ButtonHelp_Click(object sender, EventArgs e)
        {

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
    }
}
