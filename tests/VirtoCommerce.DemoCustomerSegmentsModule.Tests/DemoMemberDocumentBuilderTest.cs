using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using VirtoCommerce.CoreModule.Core.Conditions;
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
    public class DemoMemberDocumentBuilderTest
    {
        private readonly DynamicObjectProperty _usualDynamicProperty = new DynamicObjectProperty
        {
            Name = "Usual Property",
            Values = new[]
            {
                new DynamicPropertyObjectValue
                {
                    Value = "Usual", ValueType = DynamicPropertyValueType.ShortText
                }
            }
        };

        private readonly DynamicObjectProperty _multiValueDynamicProperty = new DynamicObjectProperty
        {
            Name = "Multivalue property",
            Values = new[]
            {
                new DynamicPropertyObjectValue
                {
                    Value = "Value1", ValueType = DynamicPropertyValueType.ShortText
                },
                new DynamicPropertyObjectValue
                {
                    Value = "Value2", ValueType = DynamicPropertyValueType.ShortText
                }
            }
        };

        private readonly DynamicObjectProperty _multiLanguageDynamicProperty = new DynamicObjectProperty
        {
            Name = "Multilingual property",
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

        private readonly DynamicObjectProperty _multiLanguageMultiValueDynamicProperty = new DynamicObjectProperty
        {
            Name = "Multilingual property",
            Values = new[]
            {
                new DynamicPropertyObjectValue
                {
                    Locale = "en-US", Value = "Value1", ValueType = DynamicPropertyValueType.ShortText
                },
                new DynamicPropertyObjectValue
                {
                    Locale = "en-US", Value = "Value2", ValueType = DynamicPropertyValueType.ShortText
                },
                new DynamicPropertyObjectValue
                {
                    Locale = "fr-FR", Value = "Value1", ValueType = DynamicPropertyValueType.ShortText
                },
                new DynamicPropertyObjectValue
                {
                    Locale = "fr-FR", Value = "Value3", ValueType = DynamicPropertyValueType.ShortText
                },
                new DynamicPropertyObjectValue
                {
                    Locale = "de-DE", Value = "Value4", ValueType = DynamicPropertyValueType.ShortText
                },
                new DynamicPropertyObjectValue
                {
                    Locale = "de-DE", Value = "Value5", ValueType = DynamicPropertyValueType.ShortText
                }
            }
        };

        [Fact]
        public async Task Should_Filter_By_Store_And_Usual_Property()
        {
            var properties = new[] { _usualDynamicProperty };
            var storeIds = new[] { "Store1" };

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

            var segment = new DemoCustomerSegment
            {
                UserGroup = "Test", StoreIds = storeIds, ExpressionTree = new DemoCustomerSegmentTree()
            };
            segment.ExpressionTree.MergeFromPrototype(new DemoCustomerSegmentTreePrototype());
            var block = segment.ExpressionTree.Children
                .First(child => child.Id == nameof(DemoBlockCustomerSegmentRule));
            var condition = (DemoConditionPropertyValues)block.AvailableChildren
                .First(child => child.Id == nameof(DemoConditionPropertyValues));
            block.Children.Add(condition);
            condition.Properties = properties;
            condition.StoreIds = storeIds;

            var memberServiceMock = new Mock<IMemberService>();
            memberServiceMock
                .Setup(memberService =>
                    memberService.GetByIdsAsync(It.IsAny<string[]>(), It.IsAny<string>(), It.IsAny<string[]>()))
                .Returns(() => Task.FromResult(new Member[] { store1Customer, store2Customer }));
            var segmentSearchServiceMock = new Mock<IDemoCustomerSegmentSearchService>();
            segmentSearchServiceMock.Setup(segmentSearchService =>
                    segmentSearchService.SearchCustomerSegmentsAsync(It.IsAny<DemoCustomerSegmentSearchCriteria>()))
                .Returns(() => Task.FromResult(new DemoCustomerSegmentSearchResult { Results = new[] { segment } }));

            var documentBuilder = new DemoMemberDocumentBuilder(memberServiceMock.Object, segmentSearchServiceMock.Object);
            var documents = await documentBuilder.GetDocumentsAsync(new List<string> { "Customer1", "Customer2" });

            var firstCustomerUserGroups = GetUserGroups(documents[0]);
            Assert.Equal("Test", firstCustomerUserGroups.ToString());

            var secondCustomerUserGroups = GetUserGroups(documents[1]);
            Assert.Null(secondCustomerUserGroups);
        }

        private object GetUserGroups(IndexDocument document)
        {
            return document.Fields.FirstOrDefault(field => field.Name == "Groups")?.Value;
        }
    }
}
