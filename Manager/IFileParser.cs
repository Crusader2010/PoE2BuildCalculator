namespace Manager
{
	public interface IFileParser
	{
		Task ParseFileAsync(IProgress<int> progress, CancellationToken cancellationToken);
	}
}
