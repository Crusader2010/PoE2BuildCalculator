using System.ComponentModel;

namespace Domain.Validation
{
    public class Group : INotifyPropertyChanged
    {
        public int GroupId { get; set; }

        public string GroupName
        {
            get;
            set
            {
                if (field != value)
                {
                    field = value;
                    OnPropertyChanged(nameof(GroupName));
                }
            }
        }

        public List<GroupStatModel> Stats
        {
            get;
            set
            {
                if (field != value)
                {
                    field = value;
                    OnPropertyChanged(nameof(Stats));
                }
            }
        } = [];

        public bool IsActive => Stats != null && Stats.Count > 0;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
