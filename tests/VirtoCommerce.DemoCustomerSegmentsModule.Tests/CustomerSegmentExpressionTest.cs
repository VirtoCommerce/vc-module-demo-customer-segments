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
using VirtoCommerce.DemoCustomerSegmentsModule.Data.Services;
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
        public async Task FilterBy_StoreAndUsualProperty_SetUserGroup()
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
            var segment = CreateCustomerSegment(new[] { UsualDynamicProperty }, new[] { "Store1" }, "Test");

            var documents = await BuildIndexDocuments(segment, store1Customer, store2Customer);

            Assert.Equal("Test", GetUserGroups(documents[0]).ToString());
            Assert.Null(GetUserGroups(documents[1]));
        }

        [Fact]
        public async Task FilterBy_MultipleStores_SetUserGroup()
        {
            var properties = new[] { UsualDynamicProperty };
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
            var segment = CreateCustomerSegment(new[] { UsualDynamicProperty }, new[] { "Store1", "Store2" }, "Test");

            var documents = await BuildIndexDocuments(segment, store1Customer, store2Customer);

            Assert.Equal("Test", GetUserGroups(documents[0]).ToString());
            Assert.Equal("Test", GetUserGroups(documents[1]).ToString());
        }

        [Fact]
        public async Task FilterBy_MultiValuePropertyWithSingleValue_SetUserGroup()
        {
            var segment = CreateCustomerSegment(
                new[] { MultiValueDynamicPropertyWithSingleValue },
                new[] { "Store" },
                "Test");
            var customer1 = GetCustomer(MultiValueDynamicPropertyWithMultipleValues);
            var customer2 = GetCustomer(MultiValueDynamicPropertyWithSingleValue);

            var documents = await BuildIndexDocuments(segment,customer1, customer2);

            Assert.Equal("Test", GetUserGroups(documents[0]).ToString());
            Assert.Equal("Test", GetUserGroups(documents[1]).ToString());
        }

        [Fact]
        public async Task FilterBy_MultiValuePropertyWithMultipleValues_SetUserGroup()
        {
            var segment = CreateCustomerSegment(
                new[] { MultiValueDynamicPropertyWithMultipleValues },
                new[] { "Store" },
                "Test");
            var customer1 = GetCustomer(MultiValueDynamicPropertyWithMultipleValues);
            var customer2 = GetCustomer(MultiValueDynamicPropertyWithSingleValue);
            
            var documents = await BuildIndexDocuments(segment, customer1, customer2);

            Assert.Equal("Test", GetUserGroups(documents[0]).ToString());
            Assert.Equal("Test", GetUserGroups(documents[1]).ToString());
        }

        [Fact]
        public async Task FilterBy_MultiLanguagePropertyWithSingleLanguage_SetUserGroup()
        {
            var multiLanguageDynamicPropertyWithSingleLanguage =
                (DynamicObjectProperty) MultiLanguageDynamicProperty.Clone();
            multiLanguageDynamicPropertyWithSingleLanguage.Values =
                MultiLanguageDynamicProperty.Values.Take(1).ToArray();
            var segment = CreateCustomerSegment(
                new[] { multiLanguageDynamicPropertyWithSingleLanguage },
                new[] { "Store" },
                "Test");
            var customer = GetCustomer(MultiLanguageDynamicProperty);

            var documents = await BuildIndexDocuments(segment, customer);

            Assert.Equal("Test", GetUserGroups(documents[0]).ToString());
        }

        [Fact]
        public async Task FilterBy_MultiLanguagePropertyWithMultipleLanguages_SetUserGroup()
        {
            var multiLanguageDynamicPropertyWithMultipleLanguages =
                (DynamicObjectProperty) MultiLanguageDynamicProperty.Clone();
            multiLanguageDynamicPropertyWithMultipleLanguages.Values =
                MultiLanguageDynamicProperty.Values.Take(2).ToArray();
            var segment = CreateCustomerSegment(
                new[] { multiLanguageDynamicPropertyWithMultipleLanguages },
                new[] { "Store" },
                "Test");
            var customer = GetCustomer(MultiLanguageDynamicProperty);

            var documents = await BuildIndexDocuments(segment, customer);

            Assert.Equal("Test", GetUserGroups(documents[0]).ToString());
        }

        [Fact]
        public async Task FilterBy_MultiLanguageMultiValuePropertyWithSingleLanguageAndValue_SetUserGroup()
        {
            var multiLanguageDynamicPropertyWithSingleLanguage =
                (DynamicObjectProperty) MultiLanguageDynamicPropertyWithMultipleValues.Clone();
            multiLanguageDynamicPropertyWithSingleLanguage.Values =
                MultiLanguageDynamicPropertyWithMultipleValues.Values.Take(1).ToArray();
            var segment = CreateCustomerSegment(
                new[] { multiLanguageDynamicPropertyWithSingleLanguage },
                new[] { "Store" },
                "Test");
            var customer = GetCustomer(MultiLanguageDynamicPropertyWithMultipleValues);

            var documents = await BuildIndexDocuments(segment, customer);

            Assert.Equal("Test", GetUserGroups(documents[0]).ToString());
        }

        [Fact]
        public async Task FilterBy_MultiLanguageMultiValuePropertyWithMultipleValuesOfSameLanguage_SetUserGroup()
        {
            var multiLanguageDynamicPropertyWithSingleLanguage =
                (DynamicObjectProperty) MultiLanguageDynamicPropertyWithMultipleValues.Clone();
            multiLanguageDynamicPropertyWithSingleLanguage.Values =
                MultiLanguageDynamicPropertyWithMultipleValues.Values
                    .Where(value => value.Locale == "en-US")
                    .ToArray();
            var segment = CreateCustomerSegment(
                new[] { multiLanguageDynamicPropertyWithSingleLanguage },
                new[] { "Store" },
                "Test");
            var customer = GetCustomer(MultiLanguageDynamicPropertyWithMultipleValues);

            var documents = await BuildIndexDocuments(segment, customer);

            Assert.Equal("Test", GetUserGroups(documents[0]).ToString());
        }

        [Fact]
        public async Task FilterBy_MultiLanguageMultiValuePropertyWithMultipleValuesOfDifferentLanguage_SetUserGroup()
        {
            var multiLanguageDynamicPropertyWithSingleLanguage =
                (DynamicObjectProperty) MultiLanguageDynamicPropertyWithMultipleValues.Clone();
            multiLanguageDynamicPropertyWithSingleLanguage.Values = new[]
            {
                MultiLanguageDynamicPropertyWithMultipleValues.Values.First(value => value.Locale == "en-US"),
                MultiLanguageDynamicPropertyWithMultipleValues.Values.First(value => value.Locale == "de-DE")
            };
            var segment = CreateCustomerSegment(
                new[] { multiLanguageDynamicPropertyWithSingleLanguage },
                new[] { "Store" },
                "Test");
            var customer = GetCustomer(MultiLanguageDynamicPropertyWithMultipleValues);

            var documents = await BuildIndexDocuments(segment, customer);

            Assert.Equal("Test", GetUserGroups(documents[0]).ToString());
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

        private DemoCustomerSegment CreateCustomerSegment(DynamicObjectProperty[] properties, string[] storeIds, string userGroup)
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

        private async Task<IList<IndexDocument>> BuildIndexDocuments(DemoCustomerSegment segment, params Member[] customers)
        {
            var documentBuilder = new DemoMemberDocumentBuilder(
                GetMemberService(customers),
                new UserGroupEvaluator(GetCustomerSegmentSearchService(segment)));
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
