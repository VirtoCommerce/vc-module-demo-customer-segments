using System.Collections.Generic;
using VirtoCommerce.CoreModule.Core.Conditions;
using VirtoCommerce.Platform.Core.DynamicProperties;

namespace VirtoCommerce.DemoCustomerSegmentsModule.Core.Models
{
    public class DemoConditionPropertyValues : ConditionTree
    {
        public ICollection<DynamicObjectProperty> Properties { get; set; } = new List<DynamicObjectProperty>();
    }
}
