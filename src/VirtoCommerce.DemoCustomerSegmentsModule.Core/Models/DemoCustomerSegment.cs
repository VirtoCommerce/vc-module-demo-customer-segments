using System;
using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.DemoSolutionFeaturesModule.Core.Models
{
    public class DemoCustomerSegment: AuditableEntity
    {
        public DemoCustomerSegment()
        {
            StoreIds = new List<string>();
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public ICollection<string> StoreIds { get; set; }

        public DemoCustomerSegmentTree ExpressionTree { get; set; }
    }
}
