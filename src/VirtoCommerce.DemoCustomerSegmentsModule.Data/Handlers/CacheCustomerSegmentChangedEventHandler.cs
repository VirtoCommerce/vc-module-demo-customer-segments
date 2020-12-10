using System;
using System.Threading.Tasks;
using VirtoCommerce.CustomerModule.Data.Caching;
using VirtoCommerce.DemoCustomerSegmentsModule.Core.Events;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.DemoCustomerSegmentsModule.Data.Handlers
{
    public class CacheCustomerSegmentChangedEventHandler: IEventHandler<DemoCustomerSegmentChangedEvent>
    {
        public Task Handle(DemoCustomerSegmentChangedEvent message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            // Expire cache for all members
            CustomerCacheRegion.ExpireRegion();

            return Task.CompletedTask;
        }
    }
}
