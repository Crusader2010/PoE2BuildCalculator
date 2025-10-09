using System.ComponentModel;
using System.Reflection;

namespace Domain.Validation
{
    public class ValidationGroupModel : INotifyPropertyChanged
    {
        private string _groupName;
        private double? _minValue;
        private double? _maxValue;
        private string _groupOperator;
        private bool _isMinEnabled;
        private bool _isMaxEnabled;

        public int GroupId { get; set; }

        public string GroupName
        {
            get => _groupName;
            set
            {
                if (_groupName != value)
                {
                    _groupName = value;
                    OnPropertyChanged(nameof(GroupName));
                }
            }
        }

        public List<GroupStatModel> Stats { get; set; } = [];

        public bool IsMinEnabled
        {
            get => _isMinEnabled;
            set
            {
                if (_isMinEnabled != value)
                {
                    _isMinEnabled = value;
                    OnPropertyChanged(nameof(IsMinEnabled));
                }
            }
        }

        public double? MinValue
        {
            get => _minValue;
            set
            {
                if (_minValue != value)
                {
                    _minValue = value;
                    OnPropertyChanged(nameof(MinValue));
                }
            }
        }

        public bool IsMaxEnabled
        {
            get => _isMaxEnabled;
            set
            {
                if (_isMaxEnabled != value)
                {
                    _isMaxEnabled = value;
                    OnPropertyChanged(nameof(IsMaxEnabled));
                }
            }
        }

        public double? MaxValue
        {
            get => _maxValue;
            set
            {
                if (_maxValue != value)
                {
                    _maxValue = value;
                    OnPropertyChanged(nameof(MaxValue));
                }
            }
        }

        public string GroupOperator
        {
            get => _groupOperator ?? "AND";
            set
            {
                if (_groupOperator != value)
                {
                    _groupOperator = value;
                    OnPropertyChanged(nameof(GroupOperator));
                }
            }
        }

        [Browsable(false)]
        public bool IsActive => (IsMinEnabled && MinValue.HasValue) || (IsMaxEnabled && MaxValue.HasValue);

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class GroupStatModel
    {
        public PropertyInfo PropInfo { get; set; }
        public string PropertyName { get; set; }
        public string Operator { get; set; } = "+"; // +, -, *, /
    }
}