using System.Collections.Generic;
using VirtoCommerce.CoreModule.Core.Common;

namespace VirtoCommerce.DemoCustomerSegmentsModule.Core.Services
{
    public interface IUserGroupEvaluator
    {
        ICollection<string> EvaluateUserGroups(IEvaluationContext context);
    }
}
