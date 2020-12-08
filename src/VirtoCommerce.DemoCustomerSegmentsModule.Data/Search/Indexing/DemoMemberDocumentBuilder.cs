using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.CustomerModule.Data.Search.Indexing;
using VirtoCommerce.DemoCustomerSegmentsModule.Core.Models;
using VirtoCommerce.DemoCustomerSegmentsModule.Core.Models.Search;
using VirtoCommerce.DemoCustomerSegmentsModule.Core.Services;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.SearchModule.Core.Extenstions;
using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.DemoCustomerSegmentsModule.Data.Search.Indexing
{
    public class DemoMemberDocumentBuilder: MemberDocumentBuilder 
    {
        private readonly IDemoCustomerSegmentSearchService _customerSegmentSearchService;
        private IList<DemoCustomerSegment> _customerSegments;

        public DemoMemberDocumentBuilder(IMemberService memberService, IDemoCustomerSegmentSearchService customerSegmentSearchService) : base(memberService)
        {
            _customerSegmentSearchService = customerSegmentSearchService;
        }

        public override async Task<IList<IndexDocument>> GetDocumentsAsync(IList<string> documentIds)
        {
            var customerSegments =
                await _customerSegmentSearchService.SearchCustomerSegmentsAsync(
                    new DemoCustomerSegmentSearchCriteria { IsActive = true });
            _customerSegments = customerSegments.Results;
            var documents = await base.GetDocumentsAsync(documentIds);
            return documents;
        }

        protected override IndexDocument CreateDocument(Member member)
        {
            var document = base.CreateDocument(member);

            if (member is IHasSecurityAccounts hasSecurityAccounts)
            {
                document.AddFilterableAndSearchableValues("Stores", hasSecurityAccounts.SecurityAccounts.Select(x => x.StoreId).ToArray());
            }

            if (member is Contact customer)
            {
                var evaluationContext = new DemoCustomerSegmentExpressionEvaluationContext { Customer = customer };
                var userGroups = _customerSegments
                    .Where(customerSegment => customerSegment.ExpressionTree.IsSatisfiedBy(evaluationContext))
                    .Select(customerSegment => customerSegment.UserGroup)
                    .ToArray();
                document.AddFilterableValues("Groups", userGroups);
            }

            return document;
        }
    }
}
