using System.Collections.Generic;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CoreModule.Core.Conditions;
using VirtoCommerce.Platform.Core.DynamicProperties;

namespace VirtoCommerce.DemoCustomerSegmentsModule.Core.Models
{
    public class DemoConditionPropertyValues : ConditionTree
    {
        public IList<string> StoreIds { get; set; }

        public ICollection<DynamicObjectProperty> Properties { get; set; } = new List<DynamicObjectProperty>();

        public override bool IsSatisfiedBy(IEvaluationContext context)
        {
            var result = false;
            if (context is DemoCustomerSegmentExpressionEvaluationContext evaluationContext)
            {
                result = evaluationContext.CustomerRegisteredInStores(StoreIds) &&
                         evaluationContext.CustomerHasPropertyValues(Properties);
            }

            return result;
        }
    }
}
