using System.Collections.Generic;
using VirtoCommerce.DemoSolutionFeaturesModule.Core.Models;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.DemoSolutionFeaturesModule.Core.Events
{
    public class DemoCustomerSegmentChangedEvent : GenericChangedEntryEvent<DemoCustomerSegment>
    {
        public DemoCustomerSegmentChangedEvent(IEnumerable<GenericChangedEntry<DemoCustomerSegment>> changedEntries)
            : base(changedEntries)
        {
        }
    }
}
