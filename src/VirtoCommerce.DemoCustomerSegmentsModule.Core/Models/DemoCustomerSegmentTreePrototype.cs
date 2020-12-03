using VirtoCommerce.CoreModule.Core.Conditions;

namespace VirtoCommerce.DemoCustomerSegmentsModule.Core.Models
{
    public class DemoCustomerSegmentTreePrototype : ConditionTree
    {
        public DemoCustomerSegmentTreePrototype()
        {
            var rule = new DemoBlockCustomerSegmentRule()
                .WithAvailConditions(new DemoConditionPropertyValues());

            WithAvailConditions(rule);
            WithChildrens(rule);
        }
    }
}
