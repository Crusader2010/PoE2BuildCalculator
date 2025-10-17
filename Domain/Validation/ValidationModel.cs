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

		public GroupLevelOperatorsEnum? GroupOperator
		{
			get => field ?? GroupLevelOperatorsEnum.AND;
			set
			{
				if (field != value)
				{
					field = value;
					OnPropertyChanged(nameof(GroupOperator));
				}
			}
		}

		public MinMaxOperatorsEnum? MinOperator
		{
			get => field ?? MinMaxOperatorsEnum.GreaterEqual;
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
			get => field ?? MinMaxOperatorsEnum.GreaterEqual;
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
			get => field ?? MinMaxCombinedOperatorsEnum.AND;
			set
			{
				if (field != value)
				{
					field = value;
					OnPropertyChanged(nameof(MinMaxOperator));
				}
			}
		}


		[Browsable(false)]
		public bool IsActive => IsMinEnabled || IsMaxEnabled; // to do: change

		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
