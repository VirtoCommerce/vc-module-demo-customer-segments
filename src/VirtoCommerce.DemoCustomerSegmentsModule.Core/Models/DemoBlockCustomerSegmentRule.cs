using VirtoCommerce.CoreModule.Core.Conditions;

namespace VirtoCommerce.DemoCustomerSegmentsModule.Core.Models
{
    public class DemoBlockCustomerSegmentRule : BlockConditionAndOr
    {
        public DemoBlockCustomerSegmentRule()
        {
            All = true;
        }
    }
}
