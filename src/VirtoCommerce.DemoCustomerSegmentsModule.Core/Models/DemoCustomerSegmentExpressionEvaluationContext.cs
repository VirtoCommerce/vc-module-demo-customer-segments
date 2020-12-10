using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;

namespace VirtoCommerce.DemoCustomerSegmentsModule.Core.Models
{
    public class DemoCustomerSegmentExpressionEvaluationContext: IEvaluationContext
    {
        public Contact Customer { get; set; }

        public virtual bool CustomerRegisteredInStores(IEnumerable<string> storeIds)
        {
            return Customer.SecurityAccounts.Any() && !Customer.SecurityAccounts.Select(x => x.StoreId).Except(storeIds).Any();
        }

        public virtual bool CustomerHasPropertyValues(IEnumerable<DynamicObjectProperty> properties)
        {
            var result = properties.All(property =>
            {
                var hasPropertyValues = false;
                var dynamicProperty = Customer.DynamicProperties
                    .FirstOrDefault(dp => dp.Name.EqualsInvariant(property.Name));
                var propertyValues = property.Values.Where(v => v.Value != null).ToArray();

                if (dynamicProperty != null && !propertyValues.IsNullOrEmpty())
                {
                    var dynamicPropertyValues = dynamicProperty.Values.Where(v => v.Value != null).ToArray();
                    hasPropertyValues = propertyValues.Aggregate(true, (current, propertyValue) =>
                        current || dynamicPropertyValues.Any(dpv =>
                            dpv.Locale.EqualsInvariant(propertyValue.Locale) &&
                            dpv.Value.ToString().EqualsInvariant(propertyValue.Value.ToString())));
                }

                return hasPropertyValues;
            });

            return result;
        }
    }
}
