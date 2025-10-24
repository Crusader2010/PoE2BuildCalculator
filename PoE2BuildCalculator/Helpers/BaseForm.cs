namespace PoE2BuildCalculator.Helpers
{
	public partial class BaseForm : Form
	{
		protected override void OnCreateControl()
		{
			base.OnCreateControl();
			this.Font = new Font("Nirmala UI", this.Font.Size, this.Font.Style, this.Font.Unit);
		}
	}
}
