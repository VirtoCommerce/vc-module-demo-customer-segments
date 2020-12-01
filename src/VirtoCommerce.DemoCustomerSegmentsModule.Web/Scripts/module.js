// Call this to register your module to main application
var moduleName = "virtoCommerce.demoCustomerSegmentsModule";

if (AppDependencies !== undefined) {
    AppDependencies.push(moduleName);
}

angular.module(moduleName, [])
    .config(['$stateProvider', '$urlRouterProvider',
        function ($stateProvider, $urlRouterProvider) {
            $stateProvider
                .state('workspace.virtoCommerceDemoCustomerSegmentsModuleState', {
                    url: '/virtoCommerce.demoCustomerSegmentsModule',
                    templateUrl: '$(Platform)/Scripts/common/templates/home.tpl.html',
                    controller: [
                        '$scope', 'platformWebApp.bladeNavigationService', function ($scope, bladeNavigationService) {
                            var newBlade = {
                                id: 'blade1',
                                controller: 'virtoCommerce.demoCustomerSegmentsModule.helloWorldController',
                                template: 'Modules/$(virtoCommerce.demoCustomerSegmentsModule)/Scripts/blades/hello-world.html',
                                isClosingDisabled: true
                            };
                            bladeNavigationService.showBlade(newBlade);
                        }
                    ]
                });
        }
    ])
    .run(['platformWebApp.mainMenuService', 'platformWebApp.widgetService', '$state',
        function (mainMenuService, widgetService, $state) {
            //Register module in main menu
            var menuItem = {
                path: 'browse/virtoCommerce.demoCustomerSegmentsModule',
                icon: 'fa fa-cube',
                title: 'VirtoCommerce.DemoCustomerSegmentsModule',
                priority: 100,
                action: function () { $state.go('workspace.virtoCommerceDemoCustomerSegmentsModuleState'); },
                permission: 'virtoCommerceDemoCustomerSegmentsModule:access'
            };
            mainMenuService.addMenuItem(menuItem);
        }
    ]);
