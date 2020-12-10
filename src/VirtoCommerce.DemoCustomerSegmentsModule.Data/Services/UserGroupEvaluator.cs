using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.DemoCustomerSegmentsModule.Core.Models;
using VirtoCommerce.DemoCustomerSegmentsModule.Core.Models.Search;
using VirtoCommerce.DemoCustomerSegmentsModule.Core.Services;

namespace VirtoCommerce.DemoCustomerSegmentsModule.Data.Services
{
    public class UserGroupEvaluator: IUserGroupEvaluator
    {
        private readonly IDemoCustomerSegmentSearchService _customerSegmentSearchService;

        public UserGroupEvaluator(IDemoCustomerSegmentSearchService customerSegmentSearchService)
        {
            _customerSegmentSearchService = customerSegmentSearchService;
        }

        public async Task<ICollection<string>> EvaluateUserGroupsAsync(IEvaluationContext context)
        {
            var result = Array.Empty<string>();
            if (context is DemoUserGroupEvaluationContext evaluationContext)
            {
                var customerSegments =
                    (await _customerSegmentSearchService.SearchCustomerSegmentsAsync(
                        new DemoCustomerSegmentSearchCriteria { IsActive = true })).Results;
                result = customerSegments
                    .Where(customerSegment => customerSegment.ExpressionTree.IsSatisfiedBy(
                        new DemoCustomerSegmentExpressionEvaluationContext { Customer = evaluationContext.Customer }))
                    .Select(customerSegment => customerSegment.UserGroup)
                    .ToArray();
            }

            return result;
        }

        public ICollection<string> EvaluateUserGroups(IEvaluationContext context)
        {
            return EvaluateUserGroupsAsync(context).GetAwaiter().GetResult();
        }
    }
}
