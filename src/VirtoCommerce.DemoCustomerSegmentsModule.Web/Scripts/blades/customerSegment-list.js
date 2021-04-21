angular.module('virtoCommerce.DemoCustomerSegmentsModule')
    .controller('virtoCommerce.DemoCustomerSegmentsModule.customerSegmentListController', ['$scope', 'platformWebApp.dialogService', 'platformWebApp.bladeUtils', 'platformWebApp.uiGridHelper', 'virtoCommerce.DemoCustomerSegmentsModule.customerSegmentsApi',
        function ($scope, dialogService, bladeUtils, uiGridHelper, customerSegmentsApi) {
            const blade = $scope.blade;
            blade.headIcon = 'fas fa-chart-pie';
            const bladeNavigationService = bladeUtils.bladeNavigationService;

            blade.refresh = () => {
                blade.isLoading = true;

                if ($scope.pageSettings.currentPage !== 1) {
                    $scope.pageSettings.currentPage = 1;
                }

                customerSegmentsApi.search(getSearchCriteria(), (data) => {
                    blade.isLoading = false;
                    $scope.pageSettings.totalItems = data.totalCount;
                    blade.currentEntities = data.results;
                    $scope.hasMore = data.results.length === $scope.pageSettings.itemsPerPageCount;

                    if ($scope.gridApi) {
                        $scope.gridApi.infiniteScroll.resetScroll(true, true);
                        $scope.gridApi.infiniteScroll.dataLoaded();
                    }
                });
            };

            blade.toolbarCommands = [
                {
                    name: "platform.commands.refresh", icon: 'fa fa-refresh',
                    executeMethod: blade.refresh,
                    canExecuteMethod: () => true
                },
                {
                    name: "platform.commands.add", icon: 'fa fa-plus',
                    executeMethod: () => {
                        bladeNavigationService.closeChildrenBlades(blade, function () {
                            var newBlade = {
                                id: 'customerSegmentDetail',
                                title: 'demoCustomerSegmentsModule.blades.customer-segment-detail.title',
                                subtitle: 'demoCustomerSegmentsModule.blades.customer-segment-detail.subtitle',
                                isNew: true,
                                controller: 'virtoCommerce.DemoCustomerSegmentsModule.customerSegmentDetailController',
                                template: 'Modules/$(virtoCommerce.DemoCustomerSegments)/Scripts/blades/customerSegment-detail.tpl.html'
                            };
                            bladeNavigationService.showBlade(newBlade, blade);
                        });
                    },
                    canExecuteMethod: () => true ,
                    permission: 'marketing:create'
                },
                {
                    name: "platform.commands.delete", icon: 'fa fa-trash-o',
                    executeMethod: () => {
                        $scope.deleteSegments($scope.gridApi.selection.getSelectedRows());
                    },
                    canExecuteMethod: () => {
                        return $scope.gridApi && _.any($scope.gridApi.selection.getSelectedRows());
                    },
                    permission: 'marketing:delete'
                }
            ];

            function showMore() {
                if ($scope.hasMore) {
                    ++$scope.pageSettings.currentPage;
                    $scope.gridApi.infiniteScroll.saveScrollPercentage();
                    blade.isLoading = true;

                    customerSegmentsApi.search(getSearchCriteria(), (data) => {
                        blade.isLoading = false;
                        $scope.pageSettings.totalItems = data.totalCount;
                        blade.currentEntities = blade.currentEntities.concat(data.results);
                        $scope.hasMore = data.results && data.results.length === $scope.pageSettings.itemsPerPageCount;
                        $scope.gridApi.infiniteScroll.dataLoaded();
                    });
                }
            }

            function getSearchCriteria() {
                return {
                    keyword: filter.keyword,
                    sort: uiGridHelper.getSortExpression($scope),
                    skip: ($scope.pageSettings.currentPage - 1) * $scope.pageSettings.itemsPerPageCount,
                    take: $scope.pageSettings.itemsPerPageCount
                };
            }

            $scope.selectNode = (node) => {
                $scope.selectedNodeId = node.id;

                var newBlade = {
                    id: 'customerSegmentDetail',
                    originalEntity: node,
                    title: node.name,
                    subtitle: 'demoCustomerSegmentsModule.blades.customer-segment-detail.subtitle',
                    controller: 'virtoCommerce.DemoCustomerSegmentsModule.customerSegmentDetailController',
                    template: 'Modules/$(virtoCommerce.DemoCustomerSegments)/Scripts/blades/customerSegment-detail.tpl.html'
                };

                bladeNavigationService.showBlade(newBlade, blade);
            };

            $scope.deleteSegments = (list) => {
                var dialog = {
                    id: "confirmDeleteItem",
                    title: "demoCustomerSegmentsModule.dialogs.customer-segment-delete.title",
                    message: list.length > 1 ? "demoCustomerSegmentsModule.dialogs.customer-segment-delete.many-segments-message" : "demoCustomerSegmentsModule.dialogs.customer-segment-delete.one-segment-message",
                    messageValues: list.length === 1 && {name: list[0].name},
                    callback: (remove) => {
                        if (remove) {
                            bladeNavigationService.closeChildrenBlades(blade, () => {
                                blade.isLoading = true;
                                var itemIds = _.pluck(list, 'id');
                                customerSegmentsApi.delete({ ids: itemIds }, () => {
                                    blade.refresh();
                                    const successDialog = {
                                        id: "customerSegmentDeletedDialog",
                                        title: list.length > 1 ? 'demoCustomerSegmentsModule.dialogs.customer-segment-deleted-successfully.plural-title' : 'demoCustomerSegmentsModule.dialogs.customer-segment-deleted-successfully.single-title',
                                        message: list.length > 1 ? 'demoCustomerSegmentsModule.dialogs.customer-segment-deleted-successfully.plural-message' : 'demoCustomerSegmentsModule.dialogs.customer-segment-deleted-successfully.single-message'
                                    };
                                    dialogService.showNotificationDialog(successDialog);
                                });
                            });
                        }
                    }
                };
                dialogService.showConfirmationDialog(dialog);
            };

            var filter = blade.filter = $scope.filter = {};
            filter.criteriaChanged = () => {
                if ($scope.pageSettings.currentPage > 1) {
                    $scope.pageSettings.currentPage = 1;
                } else {
                    blade.refresh();
                }
            };

            // ui-grid
            $scope.setGridOptions = (gridOptions) => {
                bladeUtils.initializePagination($scope, true);
                $scope.pageSettings.itemsPerPageCount = 20;

                uiGridHelper.initialize($scope, gridOptions, (gridApi) => {
                    //update gridApi for current grid
                    $scope.gridApi = gridApi;
                    uiGridHelper.bindRefreshOnSortChanged($scope);
                    $scope.gridApi.infiniteScroll.on.needLoadMoreData($scope, showMore);

                });

                blade.refresh();
            };
        }]);
