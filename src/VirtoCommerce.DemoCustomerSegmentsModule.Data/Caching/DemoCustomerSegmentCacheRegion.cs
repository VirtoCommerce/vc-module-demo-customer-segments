using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Primitives;
using VirtoCommerce.DemoSolutionFeaturesModule.Core.Models;
using VirtoCommerce.Platform.Core.Caching;

namespace VirtoCommerce.DemoSolutionFeaturesModule.Data.Caching
{
    public class DemoCustomerSegmentCacheRegion : CancellableCacheRegion<DemoCustomerSegmentCacheRegion>
    {
        public static IChangeToken CreateChangeToken(string[] customerSegmentIds)
        {
            if (customerSegmentIds == null)
            {
                throw new ArgumentNullException(nameof(customerSegmentIds));
            }

            var changeTokens = new List<IChangeToken> { CreateChangeToken() };

            changeTokens.AddRange(
                customerSegmentIds
                    .Select(associationId => CreateChangeTokenForKey(associationId))
            );

            return new CompositeChangeToken(changeTokens);
        }

        public static IChangeToken CreateChangeToken(DemoCustomerSegment[] customerSegments)
        {
            if (customerSegments == null)
            {
                throw new ArgumentNullException(nameof(customerSegments));
            }

            return CreateChangeToken(customerSegments.Select(x => x.Id).ToArray());
        }

        public static void ExpireEntity(DemoCustomerSegment customerSegment)
        {
            if (customerSegment == null)
            {
                throw new ArgumentNullException(nameof(customerSegment));
            }

            ExpireTokenForKey(customerSegment.Id);
        }
    }
}
