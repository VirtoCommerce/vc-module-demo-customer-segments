angular.module('virtoCommerce.demoCustomerSegmentsModule')
    .controller('virtoCommerce.demoCustomerSegmentsModule.helloWorldController', ['$scope', 'virtoCommerce.demoCustomerSegmentsModule.webApi', function ($scope, api) {
        var blade = $scope.blade;
        blade.title = 'VirtoCommerce.DemoCustomerSegmentsModule';

        blade.refresh = function () {
            api.get(function (data) {
                blade.title = 'virtoCommerce.demoCustomerSegmentsModule.blades.hello-world.title';
                blade.data = data.result;
                blade.isLoading = false;
            });
        };

        blade.refresh();
    }]);
