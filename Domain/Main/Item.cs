namespace Domain.Main
{
    public class Item : IEquatable<Item>
    {
        public int Id { get; set; }

        public ItemStats ItemStats { get; set; }

        public string Name { get; set; } = "Placeholder name";

        public string Class { get; set; } = "Placeholder class";

        public bool IsMine { get; set; } = false;

        public bool IsCorrupted { get; set; } = false;

        public bool Equals(Item other)
        {
            if (other == null || this == null) return false;
            return Id == other.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name, Class);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Item);
        }
    }
}
