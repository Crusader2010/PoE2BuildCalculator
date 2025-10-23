namespace Domain.Serialization
{
	/// <summary>
	/// Interface for forms that can save/load their configuration.
	/// </summary>
	public interface IConfigurable
	{
		/// <summary>
		/// Exports the form's configuration data.
		/// </summary>
		/// <returns>Configuration object (should be JSON-serializable).</returns>
		object ExportConfig();

		/// <summary>
		/// Imports configuration data into the form.
		/// </summary>
		/// <param name="data">Configuration object to import.</param>
		void ImportConfig(object data);

		/// <summary>
		/// Gets whether the form has valid data to export.
		/// </summary>
		bool HasData { get; }
	}
}
