namespace Domain.Helpers
{
	public class CustomColorTable : ProfessionalColorTable
	{
		public override Color MenuItemBorder => Color.DarkBlue;
		public override Color MenuItemSelected => Color.LightSkyBlue;
		public override Color MenuBorder => Color.Navy;
	}
}
