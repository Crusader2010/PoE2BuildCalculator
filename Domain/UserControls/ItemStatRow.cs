using Domain.Enums;
using Domain.Events;
using Domain.Static;

namespace Domain.UserControls
{
	public partial class ItemStatRow : UserControl
	{
		public int _currentRowIndex { get; private set; }
		public string _selectedStatName { get; private set; }
		public ArithmeticOperationsEnum _selectedOperator { get; private set; }

		public event EventHandler ItemStatRowDeleted;
		public event EventHandler ItemStatRowSwapped;

		public ItemStatRow(int currentRowIndex, string statName)
		{
			_currentRowIndex = currentRowIndex;
			_selectedStatName = statName ?? string.Empty;

			// required in constructor
			InitializeComponent();
			InitializeOperatorCombobox();

			this.Padding = new Padding(0);
			this.Margin = new Padding(0);
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
			ComboboxOperator.Items.AddRange(EnumDescriptionCache<ArithmeticOperationsEnum>.DescriptionsArray);
			ComboboxOperator.EndUpdate();

			if (ComboboxOperator.Items.Count > 0) ComboboxOperator.SelectedIndex = 0;
			ComboboxOperator.DropDownWidth = ComboboxOperator.Width;
			ComboboxOperator.ResumeLayout();

			_selectedOperator = ComboboxOperator.SelectedValue == null ? ArithmeticOperationsEnum.Sum : (EnumDescriptionCache<ArithmeticOperationsEnum>.DescriptionToEnum.TryGetValue(ComboboxOperator.SelectedValue.ToString(), out var op) ? op : ArithmeticOperationsEnum.Sum);

			ComboboxOperator.MouseWheel += ComboBox_MouseWheel;
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
			ItemStatRowDeleted?.Invoke(this, new ItemStatRowDeletingEventArgs() { IsDeleting = true });
			Dispose();
		}

		private void ButtonMoveUp_Click(object sender, EventArgs e)
		{
			ItemStatRowSwapped?.Invoke(this, new ItemStatRowSwapEventArgs() { SourceIndex = _currentRowIndex, TargetIndex = _currentRowIndex - 1 });
		}

		private void ButtonMoveDown_Click(object sender, EventArgs e)
		{
			ItemStatRowSwapped?.Invoke(this, new ItemStatRowSwapEventArgs() { SourceIndex = _currentRowIndex, TargetIndex = _currentRowIndex + 1 });
		}

		private void ComboboxOperator_SelectedIndexChanged(object sender, EventArgs e)
		{
			_selectedOperator = ComboboxOperator.SelectedValue == null ? ArithmeticOperationsEnum.Sum : (EnumDescriptionCache<ArithmeticOperationsEnum>.DescriptionToEnum.TryGetValue(ComboboxOperator.SelectedValue.ToString(), out var op) ? op : ArithmeticOperationsEnum.Sum);
		}

		private void ComboBox_MouseWheel(object sender, MouseEventArgs e)
		{
			if (e is HandledMouseEventArgs handledE)
			{
				handledE.Handled = true;
			}
		}
	}
}
