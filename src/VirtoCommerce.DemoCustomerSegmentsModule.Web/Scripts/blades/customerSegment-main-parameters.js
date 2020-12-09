angular.module('virtoCommerce.DemoCustomerSegmentsModule')
.controller('virtoCommerce.DemoCustomerSegmentsModule.customerSegmentMainParametersController',
    ['$scope', 'platformWebApp.bladeNavigationService', 'virtoCommerce.storeModule.stores',
    function ($scope, bladeNavigationService, storeService) {
        var blade = $scope.blade;
        blade.isLoading = true;
        blade.currentEntity = {};

        var formScope;
        $scope.setForm = (form) => { formScope = form; };

        $scope.isValid = () => {
            return formScope && formScope.$valid;
        };

        blade.refresh = () => {
            storeService.query({}, response => {
                $scope.stores = response;
                blade.isLoading = false;
            });

            blade.currentEntity = angular.copy(blade.originalEntity);
        };

        // datepicker
        $scope.datepickers = {
            str: false,
            end: false
        };

        $scope.open = ($event, which) => {
            $event.preventDefault();
            $event.stopPropagation();
            $scope.datepickers[which] = true;
        };

        $scope.cancelChanges = () => {
            $scope.bladeClose();
        };

        $scope.saveChanges = () => {
            if (blade.currentEntity.isActive === undefined) {
                blade.currentEntity.isActive = false;
            }

            if (blade.onSelected) {
                blade.onSelected(blade.currentEntity);
            }

            $scope.bladeClose();
        };

        $scope.bladeClose = () => {
            blade.parentBlade.activeBladeId = null;
            bladeNavigationService.closeBlade(blade);
        };

        blade.refresh();
    }]);
