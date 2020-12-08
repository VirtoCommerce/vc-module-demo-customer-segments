angular.module('virtoCommerce.DemoCustomerSegmentsModule')
.controller('virtoCommerce.DemoCustomerSegmentsModule.customerSegmentPropertyValuesController',
    ['$scope', 'platformWebApp.bladeNavigationService', 'platformWebApp.dynamicProperties.dictionaryItemsApi',
    function ($scope, bladeNavigationService, dictionaryItemsApi) {
        var blade = $scope.blade;
        blade.currentEntity = {};
        blade.toolbarCommands = [
            {
                name: "platform.commands.preview", icon: 'fa fa-eye',
                executeMethod: (currentBlade) => {
                    let previewBlade = {
                        id: 'customerSegmentsPreview',
                        controller: 'virtoCommerce.DemoCustomerSegmentsModule.customerSegmentsPreview',
                        template: 'Modules/$(virtoCommerce.DemoCustomerSegmentsModule)/Scripts/blades/customerSegments-preview.tpl.html',
                        originalEntity: blade.currentEntity,
                        properties: blade.setProperties
                    };
                    bladeNavigationService.showBlade(previewBlade, currentBlade);
                },
                canExecuteMethod: () => true
            }
        ];

        var formScope;
        $scope.setForm = (form) => { formScope = form; };

        function initializeBlade () {
            blade.setProperties = angular.copy(blade.selectedProperties);
            blade.currentEntity = angular.copy(blade.originalEntity);
            blade.isLoading = false;
        }

        $scope.getDictionaryValues = function (property, callback) {
            dictionaryItemsApi.query({ id: property.objectType, propertyId: property.id }, callback);
        };

        $scope.isValid = function () {
            return formScope &&
                formScope.$valid &&
                _.every(blade.setProperties,
                    property => property.values.length &&
                    _.every(property.values, value => typeof value.value !== 'undefined' && value.value !== null));
        }

        $scope.cancelChanges = function () {
            $scope.bladeClose();
        };

        $scope.saveChanges = function () {
            if (blade.onSelected) {
                blade.onSelected(blade.currentEntity, blade.setProperties);
            }
            $scope.bladeClose();
        };

        $scope.bladeClose = function() {
            blade.parentBlade.activeBladeId = null;
            bladeNavigationService.closeBlade(blade);
        };

        initializeBlade();
    }]);
