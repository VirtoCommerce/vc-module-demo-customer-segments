using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.DemoCustomerSegmentsModule.Core.Models;
using VirtoCommerce.DemoCustomerSegmentsModule.Core.Models.Search;
using VirtoCommerce.DemoCustomerSegmentsModule.Core.Services;
using VirtoCommerce.DemoCustomerSegmentsModule.Data.Search.Indexing;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.SearchModule.Core.Model;
using Xunit;

namespace VirtoCommerce.DemoCustomerSegmentsModule.Tests
{
    [Trait("Category", "CI")]
    public class CustomerSegmentExpressionTest
    {
        private DynamicObjectProperty UsualDynamicProperty { get; }

        private DynamicObjectProperty MultiValueDynamicPropertyWithSingleValue { get; }

        public DynamicObjectProperty MultiValueDynamicPropertyWithMultipleValues { get; }

        public DynamicObjectProperty MultiLanguageDynamicProperty { get; }

        public DynamicObjectProperty MultiLanguageDynamicPropertyWithMultipleValues { get; }

        public CustomerSegmentExpressionTest()
        {
            UsualDynamicProperty = new DynamicObjectProperty
            {
                Name = "Usual Property",
                ValueType = DynamicPropertyValueType.ShortText,
                Values = new[]
                {
                    new DynamicPropertyObjectValue
                    {
                        Value = "Usual", ValueType = DynamicPropertyValueType.ShortText
                    }
                }
            };
            MultiValueDynamicPropertyWithSingleValue = new DynamicObjectProperty
            {
                Name = "Multivalue property",
                IsArray = true,
                ValueType = DynamicPropertyValueType.ShortText,
                Values = new []
                {
                    new DynamicPropertyObjectValue
                    {
                        Value = "Value1", ValueType = DynamicPropertyValueType.ShortText
                    }
                }
            };
            MultiValueDynamicPropertyWithMultipleValues =
                (DynamicObjectProperty) MultiValueDynamicPropertyWithSingleValue.Clone();
            MultiValueDynamicPropertyWithMultipleValues.Values = MultiValueDynamicPropertyWithSingleValue.Values.Concat(new[]
            {
                new DynamicPropertyObjectValue { Value = "Value2", ValueType = DynamicPropertyValueType.ShortText }
            }).ToArray();
            MultiLanguageDynamicProperty = new DynamicObjectProperty
            {
                Name = "Multilingual property",
                IsMultilingual = true,
                ValueType = DynamicPropertyValueType.ShortText,
                Values = new[]
                {
                    new DynamicPropertyObjectValue
                    {
                        Locale = "en-US", Value = "Value1", ValueType = DynamicPropertyValueType.ShortText
                    },
                    new DynamicPropertyObjectValue
                    {
                        Locale = "fr-FR", Value = "Value1", ValueType = DynamicPropertyValueType.ShortText
                    },
                    new DynamicPropertyObjectValue
                    {
                        Locale = "de-DE", Value = "Value2", ValueType = DynamicPropertyValueType.ShortText
                    }
                }
            };
            MultiLanguageDynamicPropertyWithMultipleValues =
                (DynamicObjectProperty) MultiLanguageDynamicProperty.Clone();
            MultiLanguageDynamicPropertyWithMultipleValues.Values = MultiLanguageDynamicProperty.Values.Concat(new[]
            {
                new DynamicPropertyObjectValue
                {
                    Locale = "en-US", Value = "Value3", ValueType = DynamicPropertyValueType.ShortText
                },
                new DynamicPropertyObjectValue
                {
                    Locale = "fr-FR", Value = "Value4", ValueType = DynamicPropertyValueType.ShortText
                },
                new DynamicPropertyObjectValue
                {
                    Locale = "de-DE", Value = "Value5", ValueType = DynamicPropertyValueType.ShortText
                }
            }).ToArray();
        }

        [Fact]
        public async Task Store_And_Usual_Property()
        {
            var secondUsualProperty = (DynamicObjectProperty) UsualDynamicProperty.Clone();
            secondUsualProperty.Name = "Test Property";
            secondUsualProperty.Values.First().Value = "Test";
            var properties = new[] { UsualDynamicProperty, secondUsualProperty };

            var store1Customer = new Contact
            {
                Id = "Customer1",
                SecurityAccounts = new[] { new ApplicationUser { StoreId = "Store1" } },
                DynamicProperties = properties
            };
            var store2Customer = new Contact
            {
                Id = "Customer2",
                SecurityAccounts = new[] { new ApplicationUser { StoreId = "Store2" } },
                DynamicProperties = properties
            };

            var segment = GetCustomerSegment(new[] { UsualDynamicProperty }, new[] { "Store1" }, "Test");

            var documents = await GetDocuments(segment, store1Customer, store2Customer);

            var firstCustomerUserGroups = GetUserGroups(documents[0]);
            Assert.Equal("Test", firstCustomerUserGroups.ToString());

            var secondCustomerUserGroups = GetUserGroups(documents[1]);
            Assert.Null(secondCustomerUserGroups);
        }

        [Fact]
        public async Task Single_Value_Of_Multi_Value_Property()
        {
            var segment = GetCustomerSegment(
                new[] { MultiValueDynamicPropertyWithSingleValue },
                new[] { "Store" },
                "Test");

            var documents = await GetDocuments(segment,
                GetCustomer(MultiValueDynamicPropertyWithMultipleValues),
                GetCustomer(MultiValueDynamicPropertyWithSingleValue));

            var firstCustomerUserGroups = GetUserGroups(documents[0]);
            Assert.Equal("Test", firstCustomerUserGroups.ToString());

            var secondCustomerUserGroups = GetUserGroups(documents[1]);
            Assert.Equal("Test", secondCustomerUserGroups.ToString());
        }

        [Fact]
        public async Task Multiple_Values_Of_Multi_Value_Property()
        {
            var segment = GetCustomerSegment(
                new[] { MultiValueDynamicPropertyWithMultipleValues },
                new[] { "Store" },
                "Test");
            
            var documents = await GetDocuments(segment,
                GetCustomer(MultiValueDynamicPropertyWithMultipleValues),
                GetCustomer(MultiValueDynamicPropertyWithSingleValue));

            var firstCustomerUserGroups = GetUserGroups(documents[0]);
            Assert.Equal("Test", firstCustomerUserGroups.ToString());

            var secondCustomerUserGroups = GetUserGroups(documents[1]);
            Assert.Equal("Test", secondCustomerUserGroups.ToString());
        }

        [Fact]
        public async Task Single_Language_Of_Multi_Language_Property()
        {
            var multiLanguageDynamicPropertyWithSingleLanguage =
                (DynamicObjectProperty) MultiLanguageDynamicProperty.Clone();
            multiLanguageDynamicPropertyWithSingleLanguage.Values =
                MultiLanguageDynamicProperty.Values.Take(1).ToArray();
            var segment = GetCustomerSegment(
                new[] { multiLanguageDynamicPropertyWithSingleLanguage },
                new[] { "Store" },
                "Test");

            var documents = await GetDocuments(segment, GetCustomer(MultiLanguageDynamicProperty));

            var customerUserGroups = GetUserGroups(documents[0]);
            Assert.Equal("Test", customerUserGroups.ToString());
        }

        [Fact]
        public async Task Multiple_Languages_Of_Multi_Language_Property()
        {
            var multiLanguageDynamicPropertyWithMultipleLanguages =
                (DynamicObjectProperty) MultiLanguageDynamicProperty.Clone();
            multiLanguageDynamicPropertyWithMultipleLanguages.Values =
                MultiLanguageDynamicProperty.Values.Take(2).ToArray();
            var segment = GetCustomerSegment(
                new[] { multiLanguageDynamicPropertyWithMultipleLanguages },
                new[] { "Store" },
                "Test");

            var documents = await GetDocuments(segment, GetCustomer(MultiLanguageDynamicProperty));

            var customerUserGroups = GetUserGroups(documents[0]);
            Assert.Equal("Test", customerUserGroups.ToString());
        }

        [Fact]
        public async Task Single_Value_And_Language_Of_Multi_Language_Property_With_Multiple_Values()
        {
            var multiLanguageDynamicPropertyWithSingleLanguage =
                (DynamicObjectProperty) MultiLanguageDynamicPropertyWithMultipleValues.Clone();
            multiLanguageDynamicPropertyWithSingleLanguage.Values =
                MultiLanguageDynamicPropertyWithMultipleValues.Values.Take(1).ToArray();
            var segment = GetCustomerSegment(
                new[] { multiLanguageDynamicPropertyWithSingleLanguage },
                new[] { "Store" },
                "Test");

            var documents = await GetDocuments(segment, GetCustomer(MultiLanguageDynamicPropertyWithMultipleValues));

            var customerUserGroups = GetUserGroups(documents[0]);
            Assert.Equal("Test", customerUserGroups.ToString());
        }

        [Fact]
        public async Task Multiple_Values_Of_Same_Language_Of_Multi_Language_Property_With_Multiple_Values()
        {
            var multiLanguageDynamicPropertyWithSingleLanguage =
                (DynamicObjectProperty) MultiLanguageDynamicPropertyWithMultipleValues.Clone();
            multiLanguageDynamicPropertyWithSingleLanguage.Values =
                MultiLanguageDynamicPropertyWithMultipleValues.Values
                    .Where(value => value.Locale == "en-US")
                    .ToArray();
            var segment = GetCustomerSegment(
                new[] { multiLanguageDynamicPropertyWithSingleLanguage },
                new[] { "Store" },
                "Test");

            var documents = await GetDocuments(segment, GetCustomer(MultiLanguageDynamicPropertyWithMultipleValues));

            var customerUserGroups = GetUserGroups(documents[0]);
            Assert.Equal("Test", customerUserGroups.ToString());
        }

        [Fact]
        public async Task Multiple_Values_Of_Different_Languages_Of_Multi_Language_Property_With_Multiple_Values()
        {
            var multiLanguageDynamicPropertyWithSingleLanguage =
                (DynamicObjectProperty) MultiLanguageDynamicPropertyWithMultipleValues.Clone();
            multiLanguageDynamicPropertyWithSingleLanguage.Values = new[]
            {
                MultiLanguageDynamicPropertyWithMultipleValues.Values.First(value => value.Locale == "en-US"),
                MultiLanguageDynamicPropertyWithMultipleValues.Values.First(value => value.Locale == "de-DE")
            };
            var segment = GetCustomerSegment(
                new[] { multiLanguageDynamicPropertyWithSingleLanguage },
                new[] { "Store" },
                "Test");

            var documents = await GetDocuments(segment, GetCustomer(MultiLanguageDynamicPropertyWithMultipleValues));

            var customerUserGroups = GetUserGroups(documents[0]);
            Assert.Equal("Test", customerUserGroups.ToString());
        }

        private IMemberService GetMemberService(params Member[] customers)
        {
            var memberServiceMock = new Mock<IMemberService>();
            memberServiceMock
                .Setup(memberService =>
                    memberService.GetByIdsAsync(It.IsAny<string[]>(), It.IsAny<string>(), It.IsAny<string[]>()))
                .Returns(() => Task.FromResult(customers));
            return memberServiceMock.Object;
        }

        private IDemoCustomerSegmentSearchService GetCustomerSegmentSearchService(params DemoCustomerSegment[] segments)
        {
            var segmentSearchServiceMock = new Mock<IDemoCustomerSegmentSearchService>();
            segmentSearchServiceMock.Setup(segmentSearchService =>
                    segmentSearchService.SearchCustomerSegmentsAsync(It.IsAny<DemoCustomerSegmentSearchCriteria>()))
                .Returns(() => Task.FromResult(new DemoCustomerSegmentSearchResult { Results = segments }));
            return segmentSearchServiceMock.Object;
        }

        private DemoCustomerSegment GetCustomerSegment(DynamicObjectProperty[] properties, string[] storeIds, string userGroup)
        {
            var segment = new DemoCustomerSegment
            {
                UserGroup = userGroup, StoreIds = storeIds, ExpressionTree = new DemoCustomerSegmentTree()
            };
            segment.ExpressionTree.MergeFromPrototype(new DemoCustomerSegmentTreePrototype());
            var block = segment.ExpressionTree.Children
                .First(child => child.Id == nameof(DemoBlockCustomerSegmentRule));
            var condition = (DemoConditionPropertyValues)block.AvailableChildren
                .First(child => child.Id == nameof(DemoConditionPropertyValues));
            block.Children.Add(condition);
            condition.Properties = properties;
            condition.StoreIds = storeIds;

            return segment;
        }

        private async Task<IList<IndexDocument>> GetDocuments(DemoCustomerSegment segment, params Member[] customers)
        {
            var documentBuilder = new DemoMemberDocumentBuilder(
                GetMemberService(customers),
                GetCustomerSegmentSearchService(segment));
            return await documentBuilder.GetDocumentsAsync(customers.Select(customer => customer.Id).ToList());
        }

        private object GetUserGroups(IndexDocument document)
        {
            return document.Fields.FirstOrDefault(field => field.Name == "Groups")?.Value;
        }

        private Member GetCustomer(params DynamicObjectProperty[] properties)
        {
            return new Contact
            {
                Id = "Customer",
                SecurityAccounts = new[] { new ApplicationUser { StoreId = "Store" } },
                DynamicProperties =  properties
            };
        }
    }
}
