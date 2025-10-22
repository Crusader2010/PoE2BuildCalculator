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
			Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, false) }
		};

		public static void SaveToFile(SaveData data, string filePath) =>
			File.WriteAllText(filePath, JsonSerializer.Serialize(data, _options), System.Text.Encoding.UTF8);

		public static SaveData LoadFromFile(string filePath) =>
			JsonSerializer.Deserialize<SaveData>(File.ReadAllText(filePath, System.Text.Encoding.UTF8), _options);
	}
}
