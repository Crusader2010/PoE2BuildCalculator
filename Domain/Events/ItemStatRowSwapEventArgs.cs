namespace Domain.Events
{
	public class ItemStatRowSwapEventArgs : EventArgs
	{
		public int SourceIndex { get; set; }
		public int TargetIndex { get; set; }
	}
}
