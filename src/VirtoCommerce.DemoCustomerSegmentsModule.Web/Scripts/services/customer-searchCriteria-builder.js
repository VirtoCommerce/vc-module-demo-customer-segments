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
                    const values = property.values.map(value => value.value !== undefined && value.value !== null ? value.value.name || value.value : '').join('","');
                    searchPhrase.push(`"${property.name}":"${values}"`);
                });
            }
            if (storeIds) {
                searchPhrase.push(`stores:"${storeIds.join('","')}"`);
            }

            return {
                searchPhrase: searchPhrase.join(' '),
                deepSearch: true,
                objectType: 'Member'
            };
        }
    }
});
