using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Tier : INotifyPropertyChanged
    {
        private IDictionary<string, double> _statWeights = new Dictionary<string, double>();
        private int _tierId;

        public int TierId
        {
            get { return _tierId; }
            set
            {
                _tierId = value;
                OnPropertyChanged(nameof(TierId));
            }
        }


        public string TierName { get; set; }

        public double TierWeight { get; set; }

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
            get { return StatWeights.Values.Sum(); }
        }

        public void TriggerPropertyChange(string propertyName)
        {
            OnPropertyChanged(propertyName);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
