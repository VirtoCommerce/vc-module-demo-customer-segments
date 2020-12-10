using VirtoCommerce.CoreModule.Core.Conditions;

namespace VirtoCommerce.DemoCustomerSegmentsModule.Core.Models
{
    public class DemoCustomerSegmentTree : BlockConditionAndOr
    {
        public DemoCustomerSegmentTree()
        {
            All = true;
        }
    }
}
