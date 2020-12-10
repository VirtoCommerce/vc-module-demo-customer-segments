using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CustomerModule.Core.Model;

namespace VirtoCommerce.DemoCustomerSegmentsModule.Core.Models
{
    public class DemoUserGroupEvaluationContext: IEvaluationContext
    {
        public Contact Customer { get; set; }
    }
}
