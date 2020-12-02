angular.module('virtoCommerce.DemoCustomerSegmentsModule')
.controller('virtoCommerce.DemoCustomerSegmentsModule.customerSegmentPropertyValuesController', ['$scope', 'platformWebApp.bladeNavigationService', 'platformWebApp.dynamicProperties.dictionaryItemsApi', function ($scope, bladeNavigationService, dictionaryItemsApi) {
    var blade = $scope.blade;
    blade.toolbarCommands = [
        {
            name: "platform.commands.preview", icon: 'fa fa-eye',
            executeMethod: (currentBlade) => {
                // We need to convert dynamic property object to key-value pair to pass into preview API
                let properties = currentBlade.currentEntities
                    .filter(property => property.valueType === 'Boolean' || property.values.length && property.values.every(value => !!value.value))
                    .reduce((propertiesAggregate, property) => {
                        propertiesAggregate[property.name] = property.values && property.values.length ? property.values.map(propertyValue => propertyValue.value.name || propertyValue.value) : [false];
                        return propertiesAggregate;
                    }, {});
                let previewBlade = {
                    id: 'customerSegmentsPreview',
                    controller: 'virtoCommerce.DemoCustomerSegmentsModule.customerSegmentsPreview',
                    template: 'Modules/$(virtoCommerce.DemoCustomerSegmentsModule)/Scripts/blades/customerSegments-preview.tpl.html',
                    currentEntity: blade.currentEntity,
                    properties: properties
                };
                bladeNavigationService.showBlade(previewBlade, currentBlade);
            },
            canExecuteMethod: () => true
        }
    ];

    blade.refresh = function () {
        blade.currentEntities = angular.copy(blade.entities);
        blade.isLoading = false;
    };

    $scope.cancelChanges = function () {
        $scope.bladeClose();
    };

    $scope.saveChanges = function () {
        if (blade.onSelected) {
            blade.onSelected(blade.currentEntities);
        }
        $scope.bladeClose();
    };

    $scope.getDictionaryValues = function (property, callback) {
        dictionaryItemsApi.query({ id: property.objectType, propertyId: property.id }, callback);
    };

    $scope.bladeClose = function() {
        blade.parentBlade.activeBladeId = null;
        bladeNavigationService.closeBlade(blade);
    };

    blade.refresh();
}]);
