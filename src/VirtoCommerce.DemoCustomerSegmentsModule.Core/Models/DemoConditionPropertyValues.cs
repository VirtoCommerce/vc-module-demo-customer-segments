using System.Collections.Generic;
using VirtoCommerce.CoreModule.Core.Conditions;
using VirtoCommerce.Platform.Core.DynamicProperties;

namespace VirtoCommerce.DemoCustomerSegmentsModule.Core.Models
{
    public class DemoConditionPropertyValues : ConditionTree
    {
        public DemoConditionPropertyValues()
        {
            Properties = new List<DynamicObjectProperty>();
        }

        public ICollection<DynamicObjectProperty> Properties { get; set; }
    }
}
