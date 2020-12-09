angular.module('virtoCommerce.DemoCustomerSegmentsModule')
.factory('virtoCommerce.DemoCustomerSegmentsModule.customerSegmentsApi', ['$resource', function ($resource) {
    return $resource('api/demo/customersegments/:id', {}, {
        new: { method: 'GET', url: 'api/demo/customersegments/new' },
        save: { method: 'POST', isArray: true },
        delete: { method: 'DELETE' },
        search: { method: 'POST', url: 'api/demo/customersegments/search' }
    });
}]);
