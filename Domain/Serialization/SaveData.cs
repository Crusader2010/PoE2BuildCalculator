using Domain.Main;
using Domain.Validation;

namespace Domain.Serialization
{
	public class SaveData
	{
		public string Version { get; set; } = "1.0";
		public DateTime SavedAt { get; set; }
		public List<Tier> Tiers { get; set; } = [];
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
