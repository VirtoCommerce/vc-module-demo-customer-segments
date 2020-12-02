angular.module('virtoCommerce.DemoCustomerSegmentsModule')
.factory('virtoCommerce.DemoCustomerSegmentsModule.customerSearchCriteriaBuilder', function() {
    return {
        build: (keyword, properties, storeIds) => {
            let searchPhrase = [];
            if (keyword) {
                searchPhrase.push(keyword);
            }
            if (properties) {
                properties.forEach(property => {
                    searchPhrase.push(property.name + ':' + property.values.map(value => value.value.name || value.value).join(','));
                });
            }
            if (storeIds) {
                searchPhrase.push('stores:' + storeIds.join(','));
            }

            return {
                searchPhrase: searchPhrase.join(' '),
                deepSearch: true,
                objectType: 'Member'
            };
        }
    }
});
