namespace PoE2BuildCalculator.Helpers
{
	public partial class BaseForm : Form
	{
		public BaseForm()
		{
			InitializeComponent();
		}

		protected override void OnCreateControl()
		{
			base.OnCreateControl();
			this.Font = new Font("Verdana", 9F);
		}
	}
}
