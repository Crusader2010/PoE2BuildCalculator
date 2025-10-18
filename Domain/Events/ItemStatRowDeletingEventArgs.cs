namespace Domain.Events
{
	public class ItemStatRowDeletingEventArgs : EventArgs
	{
		public bool IsDeleting { get; set; }
	}
}
