using System;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.DemoCustomerSegmentsModule.Core.Events;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Data.BackgroundJobs;

namespace VirtoCommerce.DemoCustomerSegmentsModule.Data.Handlers
{
    public class IndexMemberCustomerSegmentChangedEventHandler: IEventHandler<DemoCustomerSegmentChangedEvent>
    {
        public Task Handle(DemoCustomerSegmentChangedEvent message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            var userName = message.ChangedEntries.Select(x => x.NewEntry.ModifiedBy ?? x.OldEntry.ModifiedBy).FirstOrDefault(x => !x.IsNullOrEmpty());

            IndexingJobs.Enqueue(userName, new[] { new IndexingOptions { DocumentType = KnownDocumentTypes.Member } });

            return Task.CompletedTask;
        }
    }
}
