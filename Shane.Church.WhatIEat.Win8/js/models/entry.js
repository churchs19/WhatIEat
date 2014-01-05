angular.module('whatieat.models').factory('Entry', ['AzureMobileService', function (AzureMobileService) {
    return {
        get: function (id) {
        },
        getList: function () {
            return AzureMobileService.getTable("AzureEntry").then(function (table) {
                return table.read();
            })
            .then(function (results) {
                return WinJS.Promise.wrap(results);
            });
        },
        getFilteredList: function (startDate, endDate) {
            return AzureMobileService.getTable("AzureEntry").then(function (table) {
                return table.where(function (start, end) {
                    return this.EntryDate >= start && this.EntryDate <= end;
                }, startDate, endDate).read();
            })
            .then(function (results) {
                return WinJS.Promise.wrap(results);
            });
        },
        update: function (entry) {
        },
        add: function (entry) {
        },
        search: function (query) {
        }
    };
}]);