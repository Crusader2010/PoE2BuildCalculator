using System.ComponentModel;

namespace Domain.Main
{
	public class Tier : INotifyPropertyChanged
	{
		public int TierId
		{
			get;
			set
			{
				if (field == value) return;
				field = value;
				OnPropertyChanged(nameof(TierId));
			}
		}

		public string TierName
		{
			get;
			set
			{
				if (field == value) return;
				field = value;
				OnPropertyChanged(nameof(TierName));
			}
		}

		public double TierWeight
		{
			get;
			set
			{
				if (field == value) return;
				field = value;
				OnPropertyChanged(nameof(TierWeight));
			}
		}

		/// <summary>
		/// A dictionary with the weight of each item stat (by PropertyName).
		/// Keys should match the PropertyName values from PropertyDescriptionHelper.StatDescriptor.
		/// </summary>
		public IDictionary<string, double> StatWeights
		{
			get;
			set
			{
				field = value;
				OnPropertyChanged(nameof(StatWeights));
				OnPropertyChanged(nameof(TotalStatWeight)); // Notify the DataGridView of the change
			}
		} = new Dictionary<string, double>();

		/// <summary>
		/// The sum of the weights for each stat of the tier.
		/// </summary>
		public double TotalStatWeight
		{
			get { return StatWeights.Values.Count == 0 ? 0.0d : StatWeights.Values.Sum(); }
		}

		// This method provides a clean way to update a stat weight and ensures
		// the UI is notified that the TotalStatWeight needs to be refreshed.
		public void SetStatWeight(string statName, double value)
		{
			if (StatWeights.TryGetValue(statName, out double valueDict) && valueDict != value)
			{
				StatWeights[statName] = value;
				OnPropertyChanged(nameof(StatWeights));
				OnPropertyChanged(nameof(TotalStatWeight));
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
