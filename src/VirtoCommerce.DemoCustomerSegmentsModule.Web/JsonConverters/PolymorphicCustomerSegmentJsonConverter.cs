using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VirtoCommerce.DemoCustomerSegmentsModule.Core.Models;
using VirtoCommerce.DemoCustomerSegmentsModule.Core.Models.Search;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.DemoCustomerSegmentsModule.Web.JsonConverters
{
    public class PolymorphicCustomerSegmentJsonConverter : JsonConverter
    {
        private static Type[] _knownTypes =
        {
            typeof(DemoCustomerSegment),
            typeof(DemoCustomerSegmentSearchCriteria),
            typeof(DemoCustomerSegmentConditionEvaluationRequest)
        };

        public override bool CanWrite => false;
        public override bool CanRead => true;

        public override bool CanConvert(Type objectType)
        {
            return _knownTypes.Any(x => x.IsAssignableFrom(objectType));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var tryCreateInstance = typeof(AbstractTypeFactory<>).MakeGenericType(objectType).GetMethods().FirstOrDefault(x => x.Name.EqualsInvariant("TryCreateInstance") && x.GetParameters().Length == 0);
            var retVal = tryCreateInstance?.Invoke(null, null);

            serializer.Populate(JObject.Load(reader).CreateReader(), retVal);
            return retVal;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
