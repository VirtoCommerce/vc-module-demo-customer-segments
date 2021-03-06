using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.CustomerModule.Data.Search.Indexing;
using VirtoCommerce.DemoCustomerSegmentsModule.Core.Models;
using VirtoCommerce.DemoCustomerSegmentsModule.Core.Services;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.SearchModule.Core.Extenstions;
using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.DemoCustomerSegmentsModule.Data.Search.Indexing
{
    public class DemoMemberDocumentBuilder : MemberDocumentBuilder
    {
        private readonly IUserGroupEvaluator _userGroupEvaluator;

        public DemoMemberDocumentBuilder(
            IMemberService memberService,
            IDynamicPropertySearchService dynamicPropertySearchService,
            IUserGroupEvaluator userGroupEvaluator
            )
            : base(memberService, dynamicPropertySearchService)
        {
            _userGroupEvaluator = userGroupEvaluator;
        }

        protected override async Task<IndexDocument> CreateDocument(Member member)
        {
            var document = await base.CreateDocument(member);

            if (member is IHasSecurityAccounts hasSecurityAccounts)
            {
                document.AddFilterableAndSearchableValues("Stores",
                    hasSecurityAccounts.SecurityAccounts.Select(x => x.StoreId).ToArray());
            }

            if (member is Contact customer)
            {
                var evaluationContext = new DemoUserGroupEvaluationContext { Customer = customer };
                document.AddFilterableValues("Groups", await _userGroupEvaluator.EvaluateUserGroupsAsync(evaluationContext));
            }

            return document;
        }
    }
}
