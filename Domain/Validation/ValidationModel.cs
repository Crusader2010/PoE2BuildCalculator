using System.ComponentModel;

using Domain.Enums;

namespace Domain.Validation
{
	public class ValidationModel : INotifyPropertyChanged
	{
		public int GroupId
		{
			get => field;
			set
			{
				if (field != value)
				{
					field = value;
					OnPropertyChanged(nameof(GroupId));
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

		public GroupLevelOperatorsEnum? GroupLevelOperator
		{
			get => field;
			set
			{
				if (field != value)
				{
					field = value;
					OnPropertyChanged(nameof(GroupLevelOperator));
				}
			}
		}

		public MinMaxOperatorsEnum? MinOperator
		{
			get => field;
			set
			{
				if (field != value)
				{
					field = value;
					OnPropertyChanged(nameof(MinOperator));
				}
			}
		}

		public MinMaxOperatorsEnum? MaxOperator
		{
			get => field;
			set
			{
				if (field != value)
				{
					field = value;
					OnPropertyChanged(nameof(MaxOperator));
				}
			}
		}

		public MinMaxCombinedOperatorsEnum? MinMaxOperator
		{
			get => field;
			set
			{
				if (field != value)
				{
					field = value;
					OnPropertyChanged(nameof(MinMaxOperator));
				}
			}
		}

		public ValidationTypeEnum ValidationType
		{
			get => field;
			set
			{
				if (field != value)
				{
					field = value;
					OnPropertyChanged(nameof(ValidationType));
				}
			}
		}

		public int NumberOfItems { get; set; } = 0;

		public bool NumberOfItemsAsPercentage { get; set; } = false;

		public bool IsMinChecked { get; set; } = true;

		public bool IsMaxChecked { get; set; } = false;


		[Browsable(false)]
		public bool IsActive => MinOperator != null || MaxOperator != null;

		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
