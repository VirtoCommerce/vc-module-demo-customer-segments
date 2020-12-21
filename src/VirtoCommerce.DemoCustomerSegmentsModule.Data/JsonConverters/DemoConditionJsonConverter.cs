using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using VirtoCommerce.CoreModule.Core.Conditions;
using VirtoCommerce.CustomerModule.Data.Model;
using VirtoCommerce.DemoCustomerSegmentsModule.Core.Models;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.Platform.Data.Model;

namespace VirtoCommerce.DemoCustomerSegmentsModule.Data.JsonConverters
{
    public class DemoConditionJsonConverter: ConditionJsonConverter
    {
        public DemoConditionJsonConverter(bool doNotSerializeAvailCondition = false)
            : base(doNotSerializeAvailCondition)
        {
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var result = base.ReadJson(reader, objectType, existingValue, serializer);
            if (result is DemoConditionPropertyValues demoConditionPropertyValues)
            {
                demoConditionPropertyValues.Properties =
                    NormalizeDynamicProperties(demoConditionPropertyValues.Properties);
            }
            return result;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is DemoConditionPropertyValues demoConditionPropertyValues)
            {
                demoConditionPropertyValues.Properties =
                    NormalizeDynamicProperties(demoConditionPropertyValues.Properties);
            }
            base.WriteJson(writer, value, serializer);
        }

        /// <summary>
        /// This method is workaround for dynamic property value data normalization.
        /// It should be separate service, but placed in data model conversion methods
        /// </summary>
        /// <param name="properties">Dynamic properties to normalize</param>
        /// <returns>Normalized dynamic properties</returns>
        public virtual ICollection<DynamicObjectProperty> NormalizeDynamicProperties(ICollection<DynamicObjectProperty> properties)
        {
            var owner = new DynamicPropertyOwnerStub();
            var primaryKeyResolvingMap = new PrimaryKeyResolvingMap();
            
            var propertyDataModels = properties.Select(property =>
                AbstractTypeFactory<DynamicPropertyEntity>.TryCreateInstance()
                    .FromModel(property, primaryKeyResolvingMap)).ToList();
            var valueDataModels = properties
                .SelectMany(property => property.Values.Select(value =>
                    AbstractTypeFactory<MemberDynamicPropertyObjectValueEntity>.TryCreateInstance()
                        .FromModel(value, owner, property)))
                .GroupBy(valueDataModel => valueDataModel.PropertyName, valueDataModel => valueDataModel)
                .ToDictionary(group => group.Key, group => group.ToArray());
            primaryKeyResolvingMap.ResolvePrimaryKeys();
            
            return propertyDataModels.Select((propertyDataModel, index) =>
            {
                var property = (DynamicObjectProperty) propertyDataModel.ToModel(
                    AbstractTypeFactory<DynamicObjectProperty>.TryCreateInstance());
                property.Values = valueDataModels[property.Name].Select(valueDataModel =>
                        valueDataModel.ToModel(AbstractTypeFactory<DynamicPropertyObjectValue>.TryCreateInstance()))
                    .ToArray();
                return property;
            }).ToArray();
        }

        private class DynamicPropertyOwnerStub : IHasDynamicProperties
        {
            public string Id { get; set; }
            public string ObjectType { get; }
            public ICollection<DynamicObjectProperty> DynamicProperties { get; set; }
        }
    }
}
