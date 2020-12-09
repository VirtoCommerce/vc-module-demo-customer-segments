angular.module('virtoCommerce.DemoCustomerSegmentsModule')
    .factory('virtoCommerce.DemoCustomerSegmentsModule.customerHelper', ['$q', 'virtoCommerce.DemoCustomerSegmentsModule.customerSearchCriteriaBuilder', 'virtoCommerce.customerModule.members',
        function ($q, customerSearchCriteriaBuilder, membersApi) {
            return {
                getCustomersCount: (keyword, properties, storeIds) => {
                    var deferred = $q.defer();
                    let searchCriteria = customerSearchCriteriaBuilder.build(keyword, properties, storeIds);
                    searchCriteria.skip = 0;
                    searchCriteria.take = 0;
                    membersApi.search(searchCriteria, searchResult => {
                        deferred.resolve(searchResult.totalCount);
                    });

                    return deferred.promise;
                }
            }
        }]);
