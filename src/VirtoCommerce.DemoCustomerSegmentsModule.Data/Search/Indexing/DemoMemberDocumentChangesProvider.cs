using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.CustomerModule.Data.Repositories;
using VirtoCommerce.CustomerModule.Data.Search.Indexing;
using VirtoCommerce.DemoCustomerSegmentsModule.Core.Models;
using VirtoCommerce.Platform.Core.ChangeLog;
using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.DemoCustomerSegmentsModule.Data.Search.Indexing
{
    public class DemoMemberDocumentChangesProvider: MemberDocumentChangesProvider 
    {
        private readonly Func<IMemberRepository> _memberRepositoryFactory;
        private readonly IChangeLogSearchService _changeLogSearchService;

        public DemoMemberDocumentChangesProvider(Func<IMemberRepository> memberRepositoryFactory,
            IChangeLogSearchService changeLogSearchService) : base(memberRepositoryFactory, changeLogSearchService)
        {
            _memberRepositoryFactory = memberRepositoryFactory;
            _changeLogSearchService = changeLogSearchService;
        }

        public override async Task<long> GetTotalChangesCountAsync(DateTime? startDate, DateTime? endDate)
        {
            long result;

            var customerSegmentsCount = await GetCustomerSegmentsChangesCount(startDate, endDate);
            // If any customer segments changed, then all members should be indexed
            if (customerSegmentsCount > 0)
            {
                // Get total members count
                using var memberRepository = _memberRepositoryFactory();
                result = memberRepository.Members.Count();
            }
            else
            {
                result = await base.GetTotalChangesCountAsync(startDate, endDate);
            }

            return result;
        }

        public override async Task<IList<IndexDocumentChange>> GetChangesAsync(DateTime? startDate, DateTime? endDate, long skip, long take)
        {
            IList<IndexDocumentChange> result;

            var customerSegmentsCount = await GetCustomerSegmentsChangesCount(startDate, endDate);
            // If any customer segments changed, then all members should be indexed
            if (customerSegmentsCount > 0)
            {
                // Get members from repository and return them as changes
                using var repository = _memberRepositoryFactory();
                var memberIds = repository.Members
                    .OrderBy(i => i.CreatedDate)
                    .Select(i => i.Id)
                    .Skip((int)skip)
                    .Take((int)take)
                    .ToArray();

                result = memberIds.Select(id =>
                    new IndexDocumentChange
                    {
                        DocumentId = id,
                        ChangeType = IndexDocumentChangeType.Modified,
                        ChangeDate = DateTime.UtcNow
                    }
                ).ToArray();
            }
            else
            {
                result = await base.GetChangesAsync(startDate, endDate, skip, take);
            }

            return result;
        }

        private async Task<int> GetCustomerSegmentsChangesCount(DateTime? startDate, DateTime? endDate)
        {
            var customerSegmentsChangesCount = 0;
            if (startDate != null || endDate != null)
            {
                var criteria = new ChangeLogSearchCriteria
                {
                    ObjectType = nameof(DemoCustomerSegment), StartDate = startDate, EndDate = endDate, Take = 0
                };
                // Get customer segments count from operation log
                customerSegmentsChangesCount = (await _changeLogSearchService.SearchAsync(criteria)).TotalCount;
                
            }

            return customerSegmentsChangesCount;
        }
    }
}
