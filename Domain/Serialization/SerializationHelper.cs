using System.Text.Json;
using System.Text.Json.Serialization;

namespace Domain.Serialization
{
	public static class SerializationHelper
	{
		private static readonly JsonSerializerOptions _options = new()
		{
			WriteIndented = true,
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
			DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
			AllowTrailingCommas = true,
			PropertyNameCaseInsensitive = true,
			NumberHandling = JsonNumberHandling.AllowReadingFromString,
			Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, false) }
		};

		/// <summary>
		/// Asynchronously saves configuration data to a file.
		/// </summary>
		public static async Task SaveToFileAsync(SaveData data, string filePath, CancellationToken cancellationToken = default)
		{
			var json = JsonSerializer.Serialize(data, _options);
			await File.WriteAllTextAsync(filePath, json, System.Text.Encoding.UTF8, cancellationToken).ConfigureAwait(false);
		}

		/// <summary>
		/// Asynchronously loads configuration data from a file.
		/// </summary>
		public static async Task<SaveData> LoadFromFileAsync(string filePath, CancellationToken cancellationToken = default)
		{
			var json = await File.ReadAllTextAsync(filePath, System.Text.Encoding.UTF8, cancellationToken).ConfigureAwait(false);
			return JsonSerializer.Deserialize<SaveData>(json, _options);
		}
	}
}
