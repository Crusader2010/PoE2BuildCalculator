using Domain.Static;

namespace Domain.UserControls
{
    public partial class ItemStatRow : UserControl
    {
        public int _currentRowIndex { get; private set; }
        public string _selectedStatName { get; private set; }
        public string _selectedOperator { get; private set; }

        private readonly ItemStatGroupValidatorUserControl _ownerGroupControl;

        public ItemStatRow(int currentRowIndex, string statName, ItemStatGroupValidatorUserControl ownerGroupControl)
        {
            ArgumentNullException.ThrowIfNull(ownerGroupControl, nameof(ownerGroupControl));

            _currentRowIndex = currentRowIndex;
            _ownerGroupControl = ownerGroupControl;
            _selectedStatName = statName ?? string.Empty;

            // required in constructor
            InitializeComponent();
            InitializeOperatorCombobox();
        }

        private void ItemStatRow_Load(object sender, EventArgs e)
        {
            TextboxItemStat.Text = _selectedStatName;
            ComboboxOperator.Enabled = false;
        }

        private void InitializeOperatorCombobox()
        {
            ComboboxOperator.SuspendLayout();
            ComboboxOperator.Items.Clear();

            ComboboxOperator.BeginUpdate();
            foreach (var item in Constants.LOGICAL_OPERATORS)
            {
                ComboboxOperator.Items.Add(item);
            }
            ComboboxOperator.EndUpdate();

            ComboboxOperator.SelectedIndex = 0;
            ComboboxOperator.DropDownWidth = ComboboxOperator.Width;
            ComboboxOperator.ResumeLayout();

            _selectedOperator = ComboboxOperator.Items[0]?.ToString() ?? Constants.LOGICAL_OPERATORS.First();
        }

        public void ChangeCurrentRowIndex(int newRowIndex)
        {
            _currentRowIndex = newRowIndex;
        }

        public void SetupStatOperatorSelection(bool isEnabled)
        {
            ComboboxOperator.Enabled = isEnabled;
        }

        private void ButtonRemove_Click(object sender, EventArgs e)
        {
            _ownerGroupControl.RemoveStatRow(this);
            Dispose();
        }

        private void ButtonMoveUp_Click(object sender, EventArgs e)
        {
            int destinationIndex = _currentRowIndex - 1 < 0 ? 0 : _currentRowIndex - 1;

            _ownerGroupControl.SwapStats(_currentRowIndex, destinationIndex);
            _currentRowIndex = destinationIndex;
        }

        private void ButtonMoveDown_Click(object sender, EventArgs e)
        {
            _ownerGroupControl.SwapStats(_currentRowIndex, _currentRowIndex + 1);
            _currentRowIndex += 1;
        }

        private void ComboboxOperator_SelectedIndexChanged(object sender, EventArgs e)
        {
            _selectedOperator = ComboboxOperator.SelectedItem?.ToString() ?? Constants.LOGICAL_OPERATORS.First();
        }
    }
}
