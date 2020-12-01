angular.module('virtoCommerce.demoCustomerSegmentsModule')
    .factory('virtoCommerce.demoCustomerSegmentsModule.webApi', ['$resource', function ($resource) {
        return $resource('api/VirtoCommerceDemoCustomerSegmentsModule');
}]);
