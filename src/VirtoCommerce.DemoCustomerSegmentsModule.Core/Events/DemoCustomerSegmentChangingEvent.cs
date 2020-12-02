using System.Collections.Generic;
using VirtoCommerce.DemoSolutionFeaturesModule.Core.Models;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.DemoSolutionFeaturesModule.Core.Events
{
    public class DemoCustomerSegmentChangingEvent : GenericChangedEntryEvent<DemoCustomerSegment>
    {
        public DemoCustomerSegmentChangingEvent(IEnumerable<GenericChangedEntry<DemoCustomerSegment>> changedEntries)
            : base(changedEntries)
        {
        }
    }
}
