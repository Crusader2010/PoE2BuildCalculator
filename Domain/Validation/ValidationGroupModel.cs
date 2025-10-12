using System.ComponentModel;
using System.Linq.Expressions; // ✅ ADDED MISSING USING
using System.Reflection;

using Domain.Main;

namespace Domain.Validation
{
    public class ValidationGroupModel : INotifyPropertyChanged
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

        public List<GroupStatModel> Stats { get; set; } = [];

        public bool IsMinEnabled
        {
            get;
            set
            {
                if (field != value)
                {
                    field = value;
                    OnPropertyChanged(nameof(IsMinEnabled));
                }
            }
        }

        public double? MinValue
        {
            get;
            set
            {
                if (field != value)
                {
                    field = value;
                    OnPropertyChanged(nameof(MinValue));
                }
            }
        }

        public bool IsMaxEnabled
        {
            get;
            set
            {
                if (field != value)
                {
                    field = value;
                    OnPropertyChanged(nameof(IsMaxEnabled));
                }
            }
        }

        public double? MaxValue
        {
            get;
            set
            {
                if (field != value)
                {
                    field = value;
                    OnPropertyChanged(nameof(MaxValue));
                }
            }
        }

        public string GroupOperator
        {
            get => field ?? "AND";
            set
            {
                if (field != value)
                {
                    field = value;
                    OnPropertyChanged(nameof(GroupOperator));
                }
            }
        }

        [Browsable(false)]
        public bool IsActive => Stats.Count > 0 &&
                    (
                        (IsMinEnabled && MinValue.HasValue && !IsMaxEnabled) ||
                        (IsMaxEnabled && MaxValue.HasValue && !IsMinEnabled) ||
                        (IsMinEnabled && MinValue.HasValue && IsMaxEnabled && MaxValue.HasValue && MaxValue.Value >= MinValue.Value)
                    );

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
        public string Operator { get; set; } = "+";

        // Using Lazy<T> for thread-safe initialization
        private readonly Lazy<Func<ItemStats, double>> _cachedGetter;

        public GroupStatModel()
        {
            _cachedGetter = new Lazy<Func<ItemStats, double>>(BuildGetter);
        }

        private Func<ItemStats, double> BuildGetter()
        {
            if (PropInfo == null)
                return _ => 0.0;

            var param = Expression.Parameter(typeof(ItemStats), "stats");
            var propAccess = Expression.Property(param, PropInfo);
            var convert = Expression.Convert(propAccess, typeof(double));
            return Expression.Lambda<Func<ItemStats, double>>(convert, param).Compile();
        }

        public Func<ItemStats, double> GetCachedGetter()
        {
            return _cachedGetter.Value;
        }
    }
}