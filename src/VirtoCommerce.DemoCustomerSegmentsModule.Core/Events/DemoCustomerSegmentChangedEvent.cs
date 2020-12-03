using System.Collections.Generic;
using VirtoCommerce.DemoCustomerSegmentsModule.Core.Models;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.DemoCustomerSegmentsModule.Core.Events
{
    public class DemoCustomerSegmentChangedEvent : GenericChangedEntryEvent<DemoCustomerSegment>
    {
        public DemoCustomerSegmentChangedEvent(IEnumerable<GenericChangedEntry<DemoCustomerSegment>> changedEntries)
            : base(changedEntries)
        {
        }
    }
}
