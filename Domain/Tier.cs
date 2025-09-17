using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Tier
    {
        public int TierId { get; set; }

        public string TierName { get; set; }

        public double TierWeight { get; set; }

        /// <summary>
        /// A dictionary with the weight of each item stat (by PropertyName).
        /// Keys should match the PropertyName values from PropertyDescriptionHelper.StatDescriptor.
        /// </summary>
        public IDictionary<string, double> StatWeights { get; set; } = new Dictionary<string, double>();
    }
}
