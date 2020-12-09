angular.module('virtoCommerce.DemoCustomerSegmentsModule')
.factory('virtoCommerce.DemoCustomerSegmentsModule.expressionTreeHelper', function () {
    const expressionTreeDemoBlockCustomerSegmentRuleId = "DemoBlockCustomerSegmentRule";
    const expressionTreeDemoConditionPropertyValuesId = "DemoConditionPropertyValues";

    return {
        extractSelectedProperties: (customerSegment) => {
            let result = [];

            const customerSegmentRuleBlock = customerSegment.expressionTree.children.find(x => x.id === expressionTreeDemoBlockCustomerSegmentRuleId);

            if (!customerSegmentRuleBlock) {
                throw new Error(expressionTreeDemoBlockCustomerSegmentRuleId + " block is missed in expression tree");
            }

            if (customerSegmentRuleBlock.children[0]) {
                result = customerSegmentRuleBlock.children[0].properties;
            }

            return result;
        },
        setSelectedProperties: (customerSegment, selectedProperties) => {
            const customerSegmentRuleBlock = customerSegment.expressionTree.children.find(x => x.id === expressionTreeDemoBlockCustomerSegmentRuleId);
            customerSegmentRuleBlock.children = [];

            customerSegmentRuleBlock.children.push({
                id: expressionTreeDemoConditionPropertyValuesId,
                properties: selectedProperties
            });
        }
    }
});
