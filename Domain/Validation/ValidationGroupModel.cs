using System.ComponentModel;

namespace Domain.Validation
{
	public class ValidationGroupModel : INotifyPropertyChanged
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

		public string MinOperator
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

		public string MaxOperator
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

		public string MinMaxOperator
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


		[Browsable(false)]
		public bool IsActive => IsMinEnabled || IsMaxEnabled; // to do: change

		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
