angular.module('virtoCommerce.DemoCustomerSegmentsModule')
.controller('virtoCommerce.DemoCustomerSegmentsModule.customerSegmentRuleController',
    ['$scope', 'platformWebApp.bladeNavigationService', 'platformWebApp.dynamicProperties.api', 'virtoCommerce.DemoCustomerSegmentsModule.expressionTreeHelper', 'virtoCommerce.DemoCustomerSegmentsModule.customerHelper',
    function ($scope, bladeNavigationService, dynamicPropertiesApi, expressionTreeHelper, customerHelper) {
        var blade = $scope.blade;
        blade.isLoading = true;
        blade.activeBladeId = null;
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
                        (property) => {
                            property.isRequired = true;
                            property.values = property.valueType === 'Boolean' ? [{ value: false }] : [];
                        });
                    properties = response.results;
                    blade.propertiesCount = response.totalCount;
                    blade.isLoading = false;
                });

            blade.currentEntity = angular.copy(blade.originalEntity);          

            blade.originalProperties = expressionTreeHelper.extractSelectedProperties(blade.currentEntity);

            blade.selectedPropertiesCount = blade.originalProperties.length;

            blade.selectedProperties = angular.copy(blade.originalProperties);

            if (blade.selectedProperties && blade.selectedProperties.length > 0) {
                refreshCustomersCount();
            }
        }

        function isDirty() {
            return !angular.equals(blade.selectedProperties, blade.originalProperties);
        }

        $scope.selectProperties = () => {
            var newBlade = {
                id: 'propertiesSelector',
                title: 'demoCustomerSegmentsModule.blades.customer-segment-properties.title',
                controller: 'virtoCommerce.DemoCustomerSegmentsModule.customerSegmentPropertiesController',
                template: 'Modules/$(virtoCommerce.DemoCustomerSegmentsModule)/Scripts/blades/customerSegment-properties.tpl.html',
                originalEntity: blade.currentEntity,
                properties: properties,
                selectedProperties: blade.selectedProperties,
                onSelected: (entity, selectedProperties) => {
                    blade.currentEntity = entity;
                    blade.selectedProperties = selectedProperties;
                    blade.selectedPropertiesCount = blade.selectedProperties.length;
                    $scope.editProperties();
                }
            };
            blade.activeBladeId = newBlade.id;
            bladeNavigationService.showBlade(newBlade, blade);
        };

        $scope.editProperties = () => {
            var newBlade = {
                id: 'propertiesEditor',
                title: 'demoCustomerSegmentsModule.blades.customer-segment-property-values.title',
                headIcon: 'fa-pie-chart',
                controller: 'virtoCommerce.DemoCustomerSegmentsModule.customerSegmentPropertyValuesController',
                template: 'Modules/$(virtoCommerce.DemoCustomerSegmentsModule)/Scripts/blades/customerSegment-property-values.tpl.html',
                originalEntity: blade.currentEntity,
                selectedProperties: blade.selectedProperties,
                onSelected: (entity, selectedProperties) => {
                    blade.currentEntity = entity;
                    blade.selectedProperties = selectedProperties;
                    refreshCustomersCount();
                }
            };
            blade.activeBladeId = newBlade.id;
            bladeNavigationService.showBlade(newBlade, blade);
        };

        function refreshCustomersCount() {            
            customerHelper.getCustomersCount('', blade.selectedProperties, blade.currentEntity.storeIds).then((x) => blade.customersCount = x);
        }

        $scope.canSave = () => {
            return isDirty() && blade.selectedProperties && blade.selectedProperties.length && blade.selectedProperties.every(x => x.values && x.values.length);
        };

        $scope.saveChanges = () => {
            if (blade.onSelected) {
                expressionTreeHelper.setSelectedProperties(blade.currentEntity, blade.selectedProperties);
                blade.onSelected(blade.currentEntity);
            }

            $scope.bladeClose();
        };

        $scope.bladeClose = () => {
            blade.parentBlade.activeBladeId = null;
            bladeNavigationService.closeBlade(blade);
        };

        initializeBlade();
    }]);
