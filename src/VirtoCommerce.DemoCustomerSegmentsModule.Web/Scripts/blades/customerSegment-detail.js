angular.module('virtoCommerce.DemoCustomerSegmentsModule')
.controller('virtoCommerce.DemoCustomerSegmentsModule.customerSegmentDetailController',
    ['$scope', 'platformWebApp.bladeNavigationService', 'platformWebApp.settings', function ($scope, bladeNavigationService, settings) {
    ['$scope', 'platformWebApp.bladeNavigationService', 'virtoCommerce.DemoCustomerSegmentsModule.customerSegmentsApi', function ($scope, bladeNavigationService, customerSegmentsApi) {
        const blade = $scope.blade;
        blade.headIcon = 'fa-pie-chart';
        blade.activeBladeId = null;

        blade.currentEntity = {};

        $scope.groups = settings.getValues({ id: 'Customer.MemberGroups' });

        blade.refresh = function () {
            if (blade.isNew) {
                customerSegmentsApi.new({},
                    data => {
                        blade.currentEntity = data;
                        blade.isLoading = false;
                    });
            } else {
                customerSegmentsApi.get({ id: blade.currentEntityId }, data => {
                    blade.currentEntity = data;
                    blade.isLoading = false;
                });
            }
        }

        $scope.canSave = () => {
            return false;
        };

        $scope.openGroupsDictionarySettingManagement = function () {
            var newBlade = {
                id: 'settingDetailChild',
                isApiSave: true,
                currentEntityId: 'Customer.MemberGroups',
                parentRefresh: function (data) { $scope.groups = data; },
                controller: 'platformWebApp.settingDictionaryController',
                template: '$(Platform)/Scripts/app/settings/blades/setting-dictionary.tpl.html'
            };
            bladeNavigationService.showBlade(newBlade, blade);
        };

        $scope.mainParameters = function () {
            const parametersBlade = {
                id: "mainParameters",
                title: "demoCustomerSegmentsModule.blades.customer-segment-parameters.title",
                subtitle: 'demoCustomerSegmentsModule.blades.customer-segment-parameters.subtitle',
                controller: 'virtoCommerce.DemoCustomerSegmentsModule.customerSegmentMainParametersController',
                template: 'Modules/$(virtoCommerce.DemoCustomerSegmentsModule)/Scripts/blades/customerSegment-main-parameters.tpl.html',
                originalEntity: blade.currentEntity,
                onSelected: function (entity) {
                    blade.currentEntity = entity;
                }
            };
            blade.activeBladeId = parametersBlade.id;
            bladeNavigationService.showBlade(parametersBlade, blade);
        };

            $scope.createCustomerFilter = function () {
                var ruleCreationBlade = {
                    id: "createCustomerSegmentRule",
                    controller: 'virtoCommerce.DemoCustomerSegmentsModule.customerSegmentRuleController',
                    title: 'demoCustomerSegmentsModule.blades.customer-segment-rule-creation.title',
                    subtitle: 'demoCustomerSegmentsModule.blades.customer-segment-rule-creation.subtitle',
                    template: 'Modules/$(virtoCommerce.DemoCustomerSegmentsModule)/Scripts/blades/customerSegment-rule.tpl.html',
                    originalEntity: blade.currentEntity,
                    onSelected: function (entity) {
                        blade.currentEntity = entity;
                    }
                };
                blade.activeBladeId = ruleCreationBlade.id;
                bladeNavigationService.showBlade(ruleCreationBlade, blade);
            };

        $scope.$watch('blade.currentEntity', (data) => {
            if (data) {
                $scope.totalPropertiesCount = 4;
                $scope.filledPropertiesCount = 0;

                $scope.filledPropertiesCount += blade.currentEntity.isActive !== undefined ? 1 : 0;
                $scope.filledPropertiesCount += blade.currentEntity.startDate ? 1 : 0;
                $scope.filledPropertiesCount += blade.currentEntity.endDate ? 1 : 0;
                $scope.filledPropertiesCount += blade.currentEntity.storeIds && blade.currentEntity.storeIds.length ? 1 : 0;
            }
        }, true);

        blade.refresh();
    }]);
