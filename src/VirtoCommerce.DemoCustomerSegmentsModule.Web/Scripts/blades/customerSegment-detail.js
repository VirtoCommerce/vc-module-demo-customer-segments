angular.module('virtoCommerce.DemoCustomerSegmentsModule')
.controller('virtoCommerce.DemoCustomerSegmentsModule.customerSegmentDetailController',
        ['$scope', 'platformWebApp.bladeNavigationService', 'platformWebApp.settings', 'platformWebApp.dialogService', 'virtoCommerce.DemoCustomerSegmentsModule.customerSegmentsApi', function ($scope, bladeNavigationService, settings, dialogService, customerSegmentsApi) {
        const blade = $scope.blade;
        blade.headIcon = 'fa-pie-chart';
        blade.activeBladeId = null;
        blade.currentEntity = {};

        $scope.groups = settings.getValues({ id: 'Customer.MemberGroups' });

        blade.refresh = function (parentRefresh) {
            if (blade.isNew) {
                customerSegmentsApi.new({},
                    data => {
                        blade.originalEntity = data;
                        blade.currentEntity = angular.copy(blade.originalEntity);
                        blade.isLoading = false;
                    });
            } else {              
                blade.currentEntity = angular.copy(blade.originalEntity);
                blade.mainParametersAreSet = true;
                blade.ruleIsSet = true;
                blade.isLoading = false;                
            }

            if (parentRefresh) {
                blade.parentBlade.refresh();
            }
        }

        blade.onClose = function (closeCallback) {
            bladeNavigationService.showConfirmationIfNeeded(isDirty() && !blade.isNew, $scope.isValid(), blade, $scope.saveChanges, closeCallback, "demoCustomerSegmentsModule.dialogs.customer-segment-save.title", "demoCustomerSegmentsModule.dialogs.customer-segment-save.message");
        };

        var formScope;
        $scope.setForm = (form) => { formScope = form; };

        $scope.isValid = function () {
            return formScope && formScope.$valid;
        };

        $scope.canSave = () => {
            return isDirty() && $scope.isValid() && blade.mainParametersAreSet && blade.ruleIsSet;
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

        function isDirty() {
            return !angular.equals(blade.currentEntity, blade.originalEntity);
        }


        $scope.saveChanges = function () {
            if (blade.isNew) {
                customerSegmentsApi.save({}, [blade.currentEntity], function (data) {
                    blade.isNew = undefined;
                    blade.originalEntity = data[0];
                    blade.refresh(true);
                    $scope.closeBlade();

                    const dialog = {
                        id: "customerSegmentCreatedDialog",
                        title: 'demoCustomerSegmentsModule.dialogs.customer-segment-created-successfully.title',
                        message: 'demoCustomerSegmentsModule.dialogs.customer-segment-created-successfully.message'
                    };

                    dialogService.showNotificationDialog(dialog);
                });
                }
            else {
                customerSegmentsApi.save({}, [blade.currentEntity], function (data) {                    
                    blade.originalEntity = data[0];
                    blade.refresh(true);
                    $scope.closeBlade();

                    const dialog = {
                        id: "customerSegmentUpdatedDialog",
                        title: 'demoCustomerSegmentsModule.dialogs.customer-segment-updated-successfully.title',
                        message: 'demoCustomerSegmentsModule.dialogs.customer-segment-updated-successfully.message'
                    };

                    dialogService.showNotificationDialog(dialog);
                });
            }                            
        }

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
                    blade.mainParametersAreSet = true;
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
                        blade.ruleIsSet = true;
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

        $scope.closeBlade = function () {
            blade.parentBlade.activeBladeId = null;
            bladeNavigationService.closeBlade(blade);
        };

        blade.refresh();
    }]);
