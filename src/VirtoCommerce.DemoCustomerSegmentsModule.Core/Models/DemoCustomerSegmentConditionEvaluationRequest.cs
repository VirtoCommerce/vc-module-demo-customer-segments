using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.DemoSolutionFeaturesModule.Core.Models
{
    public class DemoCustomerSegmentConditionEvaluationRequest
    {
        public DemoCustomerSegmentConditionEvaluationRequest()
        {
            Properties = new Dictionary<string, string[]>();
            StoreIds = new List<string>();
            SortInfos = new List<SortInfo>();
        }

        public string Keyword { get; set; }

        public IDictionary<string, string[]> Properties { get; set; }

        public IList<string> StoreIds { get; set; }

        public string Sort { get; set; }

        public IList<SortInfo> SortInfos { get; set; }

        public int Skip { get; set; } = 0;

        public int Take { get; set; } = 20;
    }
}
