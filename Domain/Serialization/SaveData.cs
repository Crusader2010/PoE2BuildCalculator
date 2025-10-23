using Domain.Main;
using Domain.Validation;

namespace Domain.Serialization
{
	public class SaveData
	{
		/// <summary>
		/// Configuration schema version (Semantic Versioning: Major.Minor.Patch).
		/// </summary>
		public string Version { get; set; } = "1.0.0";

		public DateTime SavedAt { get; set; }

		// Section: Tiers
		public List<Tier> Tiers { get; set; } = [];

		// Section: Validator
		public List<GroupDto> Groups { get; set; } = [];
		public List<ValidationModel> Operations { get; set; } = [];
	}

	public class GroupDto
	{
		public int GroupId { get; set; }
		public string GroupName { get; set; }
		public List<GroupStatDto> Stats { get; set; } = [];
	}

	public class GroupStatDto
	{
		public string PropertyName { get; set; }
		public string Operator { get; set; }
	}
}
