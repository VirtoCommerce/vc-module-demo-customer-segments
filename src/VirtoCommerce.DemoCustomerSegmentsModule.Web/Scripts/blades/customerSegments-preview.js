angular.module('virtoCommerce.DemoCustomerSegmentsModule')
.controller('virtoCommerce.DemoCustomerSegmentsModule.customerSegmentsPreview', [
    '$scope', 'virtoCommerce.DemoCustomerSegmentsModule.customerSegmentsApi', 'virtoCommerce.customerModule.members',
    'platformWebApp.bladeUtils', 'platformWebApp.uiGridHelper', 'platformWebApp.ui-grid.extension',
    'virtoCommerce.customerModule.memberTypesResolverService',
function($scope, customerSegmentsApi, membersApi, bladeUtils, uiGridHelper, gridOptionExtension, memberTypesResolverService) {
    $scope.uiGridConstants = uiGridHelper.uiGridConstants;

    var blade = $scope.blade;
    blade.title = 'demoCustomerSegmentsModule.blades.customer-segment-preview.title';
    blade.toolbarCommands = [
        {
            name: "platform.commands.refresh", icon: 'fa fa-refresh',
            executeMethod: () => blade.refresh(),
            canExecuteMethod: () => true
        }
    ];

    blade.refresh = function() {
        blade.isLoading = true;

        let refresh = searchResult => {
            $scope.pageSettings.totalItems = searchResult.totalCount;
            let memberTypeDefinition;
            _.each(searchResult.results,
                function(x) {
                    if (memberTypeDefinition = memberTypesResolverService.resolve(x.memberType)) {
                        x._memberTypeIcon = memberTypeDefinition.icon;
                    }
                });
            $scope.customers = searchResult.results ? searchResult.results : [];
            blade.isLoading = false;
        };

        let conditionEvaluationRequest = getConditionEvaluationRequest();
        customerSegmentsApi.preview(conditionEvaluationRequest, customerIds => {
            if (customerIds.length) {
                let searchCriteria = getSearchCriteria(customerIds);
                membersApi.search(searchCriteria, searchResult => refresh(searchResult));
            } else {
                refresh({ totalCount: 0 });
            }
        });
    }

    var filter = $scope.filter = { };

    filter.criteriaChanged = function () {
        if ($scope.pageSettings.currentPage > 1) {
            $scope.pageSettings.currentPage = 1;
        } else {
            blade.refresh();
        }
    };

    // ui-grid
    $scope.setGridOptions = function (gridId, gridOptions) {
        $scope.gridOptions = gridOptions;
        gridOptionExtension.tryExtendGridOptions(gridId, gridOptions);

        gridOptions.onRegisterApi = function (gridApi) {
            $scope.gridApi = gridApi;
            gridApi.core.on.sortChanged($scope, function () {
                if (!blade.isLoading) blade.refresh();
            });
        };

        bladeUtils.initializePagination($scope);
    };

    function getConditionEvaluationRequest() {
        let conditionEvaluationRequest = {
            keyword: filter.keyword,
            properties: blade.properties,
            storeIds: blade.currentEntity.storeIds,
            sort: uiGridHelper.getSortExpression($scope),
            skip: 0,
            take: 1000
        };
        return conditionEvaluationRequest;
    }

    function getSearchCriteria(customerIds) {
        let searchCriteria = {
            objectIds: customerIds,
            deepSearch: true,
            objectType: 'Member',
            skip: 0,
            take: 1000
        };
        return searchCriteria;
    }
}]);
