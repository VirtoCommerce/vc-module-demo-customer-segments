using System.Collections.Generic;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CustomerModule.Core.Model;

namespace VirtoCommerce.DemoCustomerSegmentsModule.Core.Models
{
    public class DemoUserGroupEvaluationContext: IEvaluationContext
    {
        public ICollection<DemoCustomerSegment> CustomerSegments { get; set; }

        public Contact Customer { get; set; }
    }
}
