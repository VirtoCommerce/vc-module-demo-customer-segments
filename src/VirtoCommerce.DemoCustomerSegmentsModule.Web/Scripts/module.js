// Call this to register your module to main application
var moduleName = "virtoCommerce.DemoCustomerSegmentsModule";

if (AppDependencies !== undefined) {
    AppDependencies.push(moduleName);
}

angular.module(moduleName, [])
    .run(['virtoCommerce.marketingModule.marketingMenuItemService',
        function (marketingMenuItemService) {
            marketingMenuItemService.register({
                id: 'customerSegments',
                name: 'Customer segments',
                entityName: 'customerSegment',
                icon: 'fa fa-pie-chart',
                controller: 'virtoCommerce.DemoCustomerSegmentsModule.customerSegmentDetailController',
                template: 'Modules/$(virtoCommerce.DemoCustomerSegmentsModule)/Scripts/blades/customerSegment-detail.tpl.html'
            });
        }
    ]);
