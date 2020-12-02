angular.module('virtoCommerce.DemoCustomerSegmentsModule')
    .controller('virtoCommerce.DemoCustomerSegmentsModule.customerSegmentRuleController', [
        '$scope',
        'platformWebApp.bladeNavigationService',
        'platformWebApp.dynamicProperties.api',
        function ($scope, bladeNavigationService, dynamicPropertiesApi) {
            var blade = $scope.blade;
            blade.isLoading = true;
            blade.activeBladeId = null;
            var allProperties = [];
            blade.propertiesCount = 0;
            blade.editedPropertiesCount = 0;
            blade.customersCount = 0;

            function initializeBlade() {
                dynamicPropertiesApi.search({
                    "objectType":"VirtoCommerce.CustomerModule.Core.Model.Contact",
                    "take":100}, response => {
                        _.each(response.results, function(prop) {
                            prop.group = 'All properties';
                            prop.values = [];
                        });
                        allProperties = response.results;
                        blade.propertiesCount = response.totalCount;
                    });

                blade.isLoading = false;
            }

            $scope.selectProperties = function () {
                var newBlade = {
                    id: 'propertiesSelector',
                    title: 'demoCustomerSegmentsModule.blades.customer-segment-rule-creation.select-properties-blade-title',
                    controller: 'virtoCommerce.DemoCustomerSegmentsModule.customerSegmentPropertiesController',
                    template: 'Modules/$(virtoCommerce.DemoCustomerSegmentsModule)/Scripts/blades/customerSegment-properties.tpl.html',
                    currentEntity: blade.currentEntity,
                    properties: allProperties,
                    includedProperties: blade.editedProperties,
                    onSelected: function (includedProperties) {
                        blade.editedProperties = includedProperties;
                        blade.editedPropertiesCount = blade.editedProperties.length;
                        $scope.editProperties();
                    }
                };
                blade.activeBladeId = newBlade.id;
                bladeNavigationService.showBlade(newBlade, blade);
            };

            $scope.editProperties = function () {
                var newBlade = {
                    id: 'propertiesEditor',
                    title: 'demoCustomerSegmentsModule.blades.customer-segment-rule-creation.select-properties-blade-title',
                    headIcon: 'fa-pie-chart',
                    controller: 'virtoCommerce.DemoCustomerSegmentsModule.customerSegmentPropertyValuesController',
                    template: 'Modules/$(virtoCommerce.DemoCustomerSegmentsModule)/Scripts/blades/customerSegment-property-values.tpl.html',
                    currentEntity: blade.currentEntity,
                    entities: blade.editedProperties,
                    onSelected: function (editedProps) {
                        blade.editedProperties = editedProps;
                    }
                };
                blade.activeBladeId = newBlade.id;
                bladeNavigationService.showBlade(newBlade, blade);
            };

            $scope.canSave = () => {
                return blade.editedProperties && blade.editedProperties.length && blade.editedProperties.every(x => x.values && x.values.length);
            };

            $scope.saveRule = function() {
                if (blade.onSelected) {
                    blade.onSelected(blade.editedProperties);
                }
                $scope.bladeClose();
            };

            $scope.bladeClose = function() {
                blade.parentBlade.activeBladeId = null;
                bladeNavigationService.closeBlade(blade);
            };

            initializeBlade();

        }]);
