using System.ComponentModel;

namespace Domain.Main
{
    public class Tier : INotifyPropertyChanged
    {
        private IDictionary<string, double> _statWeights = new Dictionary<string, double>();
        private int _tierId;
        private string _tierName;
        private double _tierWeight;

        public int TierId
        {
            get { return _tierId; }
            set
            {
                if (_tierId == value) return;
                _tierId = value;
                OnPropertyChanged(nameof(TierId));
            }
        }

        public string TierName
        {
            get { return _tierName; }
            set
            {
                if (_tierName == value) return;
                _tierName = value;
                OnPropertyChanged(nameof(TierName));
            }
        }

        public double TierWeight
        {
            get { return _tierWeight; }
            set
            {
                if (_tierWeight == value) return;
                _tierWeight = value;
                OnPropertyChanged(nameof(TierWeight));
            }
        }

        /// <summary>
        /// A dictionary with the weight of each item stat (by PropertyName).
        /// Keys should match the PropertyName values from PropertyDescriptionHelper.StatDescriptor.
        /// </summary>
        public IDictionary<string, double> StatWeights
        {
            get { return _statWeights; }
            set
            {
                _statWeights = value;
                OnPropertyChanged(nameof(StatWeights));
                OnPropertyChanged(nameof(TotalStatWeight)); // Notify the DataGridView of the change
            }
        }

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
