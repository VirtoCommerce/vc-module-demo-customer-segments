angular.module('virtoCommerce.DemoCustomerSegmentsModule')
.factory('virtoCommerce.DemoCustomerSegmentsModule.customerSegmentsApi', ['$resource', function ($resource) {
    return $resource('api/demo/customersegments/:id', {}, {
        new: { method: 'GET', url: 'api/demo/customersegments/new' },
        update: { method: 'PUT' },
        delete: { method: 'DELETE' },
        search: { method: 'POST', url: 'api/demo/customersegments/search' },
        preview: { method: 'POST', url: 'api/demo/customersegments/preview', isArray: true }
    });
}]);
