using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.CoreModule.Core.Common;

namespace VirtoCommerce.DemoCustomerSegmentsModule.Core.Services
{
    public interface IUserGroupEvaluator
    {
        Task<ICollection<string>> EvaluateUserGroupsAsync(IEvaluationContext context);
    }
}
