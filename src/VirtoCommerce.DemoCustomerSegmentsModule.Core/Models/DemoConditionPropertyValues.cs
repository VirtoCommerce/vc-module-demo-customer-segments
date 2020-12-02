using System.Collections.Generic;
using VirtoCommerce.CoreModule.Core.Conditions;

namespace VirtoCommerce.DemoCustomerSegmentsModule.Core.Models
{
    public class DemoConditionPropertyValues: ConditionTree
    {
        public IDictionary<string, string[]> Properties { get; set; }

        public IDictionary<string, string[]> GetPropertyValues()
        {
            return new Dictionary<string, string[]>();
        }
    }
}
