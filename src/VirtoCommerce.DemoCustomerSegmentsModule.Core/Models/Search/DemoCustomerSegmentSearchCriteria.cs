using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.DemoSolutionFeaturesModule.Core.Models.Search
{
    public class DemoCustomerSegmentSearchCriteria: SearchCriteriaBase
    {
        public DemoCustomerSegmentSearchCriteria()
        {
            StoreIds = new List<string>();
        }

        public IList<string> StoreIds { get; set; }

        public bool? IsActive { get; set; }
    }
}
