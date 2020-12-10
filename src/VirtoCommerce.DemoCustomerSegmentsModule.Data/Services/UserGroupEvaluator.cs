using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.DemoCustomerSegmentsModule.Core.Models;
using VirtoCommerce.DemoCustomerSegmentsModule.Core.Services;

namespace VirtoCommerce.DemoCustomerSegmentsModule.Data.Services
{
    public class UserGroupEvaluator: IUserGroupEvaluator
    {
        public ICollection<string> EvaluateUserGroups(IEvaluationContext context)
        {
            var result = Array.Empty<string>();
            if (context is DemoUserGroupEvaluationContext evaluationContext)
            {
                result = evaluationContext.CustomerSegments
                    .Where(customerSegment => customerSegment.ExpressionTree.IsSatisfiedBy(
                        new DemoCustomerSegmentExpressionEvaluationContext { Customer = evaluationContext.Customer }))
                    .Select(customerSegment => customerSegment.UserGroup)
                    .ToArray();
            }

            return result;
        }
    }
}
