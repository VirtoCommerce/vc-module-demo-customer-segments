angular.module('virtoCommerce.DemoCustomerSegmentsModule')
.controller('virtoCommerce.DemoCustomerSegmentsModule.customerSegmentRuleController',
    ['$scope', 'platformWebApp.bladeNavigationService', 'virtoCommerce.DemoCustomerSegmentsModule.customerSearchCriteriaBuilder',
    'platformWebApp.dynamicProperties.api', 'virtoCommerce.customerModule.members',
    function ($scope, bladeNavigationService, customerSearchCriteriaBuilder, dynamicPropertiesApi, membersApi) {
        var blade = $scope.blade;
        blade.isLoading = true;
        blade.activeBladeId = null;

        blade.propertiesCount = 0;
        blade.selectedPropertiesCount = 0;
        blade.customersCount = 0;

        blade.currentEntity = {};
        blade.selectedProperties = [];

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
                title: 'demoSolutionFeaturesModule.blades.customer-segment-property-values.title',
                headIcon: 'fa-pie-chart',
                controller: 'virtoCommerce.DemoCustomerSegmentsModule.customerSegmentPropertyValuesController',
                template: 'Modules/$(virtoCommerce.DemoCustomerSegmentsModule)/Scripts/blades/customerSegment-property-values.tpl.html',
                originalEntity: blade.currentEntity,
                selectedProperties: blade.selectedProperties,
                onSelected: function (entity, selectedProperties) {
                    blade.currentEntity = entity;
                    blade.selectedProperties = selectedProperties;
                    let searchCriteria = customerSearchCriteriaBuilder.build('', blade.selectedProperties, blade.currentEntity.storeIds);
                    searchCriteria.skip = 0;
                    searchCriteria.take = 0;
                    membersApi.search(searchCriteria, searchResult => {
                        blade.customersCount = searchResult.totalCount;
                    });
                }
            };
            blade.activeBladeId = newBlade.id;
            bladeNavigationService.showBlade(newBlade, blade);
        };

        $scope.canSave = () => {
            return blade.selectedProperties && blade.selectedProperties.length && blade.selectedProperties.every(x => x.values && x.values.length);
        };

        $scope.saveChanges = function() {
            if (blade.onSelected) {
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
