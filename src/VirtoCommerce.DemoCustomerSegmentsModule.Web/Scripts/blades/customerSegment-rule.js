angular.module('virtoCommerce.DemoCustomerSegmentsModule')
.controller('virtoCommerce.DemoCustomerSegmentsModule.customerSegmentRuleController',
    ['$scope', 'platformWebApp.bladeNavigationService', 'virtoCommerce.DemoCustomerSegmentsModule.customerSearchCriteriaBuilder',
    'platformWebApp.dynamicProperties.api', 'virtoCommerce.customerModule.members',
    function ($scope, bladeNavigationService, customerSearchCriteriaBuilder, dynamicPropertiesApi, membersApi) {
        var blade = $scope.blade;
        blade.isLoading = true;
        blade.activeBladeId = null;

        const expressionTreeDemoBlockCustomerSegmentRuleId = "DemoBlockCustomerSegmentRule"
        const expressionTreeDemoConditionPropertyValuesId = "DemoConditionPropertyValues";

        blade.propertiesCount = 0;
        blade.selectedPropertiesCount = 0;
        blade.customersCount = 0;
               

        var properties = [];

        function initializeBlade() {
            dynamicPropertiesApi.search({
                    "objectType": 'VirtoCommerce.CustomerModule.Core.Model.Contact',
                    "take": 100
                },
                response => {
                    _.each(response.results,
                        function(property) {
                            property.isRequired = true;
                            property.values = property.valueType === 'Boolean' ? [{ value: false }] : [];
                        });
                    properties = response.results;
                    blade.propertiesCount = response.totalCount;
                    blade.isLoading = false;
                });

            blade.currentEntity = angular.copy(blade.originalEntity);

            const customerSegmentRuleBlock = blade.currentEntity.expressionTree.children.find(x => x.id === expressionTreeDemoBlockCustomerSegmentRuleId);

            if (!customerSegmentRuleBlock) {
                throw new Error(expressionTreeDemoBlockCustomerSegmentRuleId + " block is missed in expression tree");
            }

            if (customerSegmentRuleBlock.children[0]) {
                blade.originalProperties = customerSegmentRuleBlock.children[0].properties;
                blade.selectedPropertiesCount = blade.originalProperties.length;
            } else {
                blade.originalProperties = [];
            }

            blade.selectedProperties = angular.copy(blade.originalProperties);
        }

        function isDirty() {
            return !angular.equals(blade.selectedProperties, blade.originalProperties);
        }

        $scope.selectProperties = function () {
            var newBlade = {
                id: 'propertiesSelector',
                title: 'demoCustomerSegmentsModule.blades.customer-segment-properties.title',
                controller: 'virtoCommerce.DemoCustomerSegmentsModule.customerSegmentPropertiesController',
                template: 'Modules/$(virtoCommerce.DemoCustomerSegmentsModule)/Scripts/blades/customerSegment-properties.tpl.html',
                originalEntity: blade.currentEntity,
                properties: properties,
                selectedProperties: blade.selectedProperties,
                onSelected: function (entity, selectedProperties) {
                    blade.currentEntity = entity;
                    blade.selectedProperties = selectedProperties;
                    blade.selectedPropertiesCount = blade.selectedProperties.length;
                    $scope.editProperties();
                }
            };
            blade.activeBladeId = newBlade.id;
            bladeNavigationService.showBlade(newBlade, blade);
        };

        $scope.editProperties = function () {
            var newBlade = {
                id: 'propertiesEditor',
                title: 'demoCustomerSegmentsModule.blades.customer-segment-property-values.title',
                headIcon: 'fa-pie-chart',
                controller: 'virtoCommerce.DemoCustomerSegmentsModule.customerSegmentPropertyValuesController',
                template: 'Modules/$(virtoCommerce.DemoCustomerSegmentsModule)/Scripts/blades/customerSegment-property-values.tpl.html',
                originalEntity: blade.currentEntity,
                selectedProperties: blade.selectedProperties,
                onSelected: function (entity, selectedProperties) {
                    blade.currentEntity = entity;
                    blade.selectedProperties = selectedProperties;
                    getCustomersCount();
                }
            };
            blade.activeBladeId = newBlade.id;
            bladeNavigationService.showBlade(newBlade, blade);
        };

        function getCustomersCount() {
            let searchCriteria = customerSearchCriteriaBuilder.build('', blade.selectedProperties, blade.currentEntity.storeIds);
            searchCriteria.skip = 0;
            searchCriteria.take = 0;
            membersApi.search(searchCriteria, searchResult => {
                blade.customersCount = searchResult.totalCount;
            });
        }

        $scope.canSave = () => {
            return isDirty() && blade.selectedProperties && blade.selectedProperties.length && blade.selectedProperties.every(x => x.values && x.values.length);
        };

        $scope.saveChanges = function() {
            if (blade.onSelected) {
                const customerSegmentRuleBlock = blade.currentEntity.expressionTree.children.find(x => x.id === expressionTreeDemoBlockCustomerSegmentRuleId);
                customerSegmentRuleBlock.children = [];

                customerSegmentRuleBlock.children.push({
                    id: expressionTreeDemoConditionPropertyValuesId,
                    properties: blade.selectedProperties
                });

                blade.onSelected(blade.currentEntity);
            }

            $scope.bladeClose();
        };

        $scope.bladeClose = function() {
            blade.parentBlade.activeBladeId = null;
            bladeNavigationService.closeBlade(blade);
        };

        initializeBlade();
    }]);
