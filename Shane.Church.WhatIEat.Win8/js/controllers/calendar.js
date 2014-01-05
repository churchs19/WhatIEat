angular.module('app')
    .controller('CalendarCtrl',  ['$scope', '$rootScope', 'Entry', function ($scope, $rootScope, Entry) {
        /* config object */
        $scope.uiConfig = {
            calendar: {
                //height: 450,
                editable: false,
                //header: {
                //    left: 'month basicWeek basicDay agendaWeek agendaDay',
                //    center: 'title',
                //    right: 'today prev,next'
                //},
                dayClick: $scope.alertEventOnClick,
                //eventDrop: $scope.alertOnDrop,
                //eventResize: $scope.alertOnResize
                aspectRatio: 1
            }
        };

        $scope.alertEventOnClick = function( date, allDay, jsEvent, view ) { 

        };

        $scope.items = [];
        $scope.groups = [];

        $scope.$watch('items', function () {
            var query = Enumerable.from($scope.items);

            var groups = query.groupBy(function (it) { return new Date(it.EntryDate.getUTCFullYear(), it.EntryDate.getUTCMonth(), it.EntryDate.getUTCDate(), 0, 0, 0, 0).toLocaleDateString() });
            var groupItems = [];
            groups.forEach(function (item) {
                var groupItem = {title: item.key(), entries: item.toArray() };
                groupItems.push(groupItem);
            });
            $scope.groups = groupItems;
        });

        $scope.$watch('groups', function () {
            var query = Enumerable.from($scope.groups)
                            .select(function (g) { return { title: g.entries.length + " entries", start: new Date(g.entries[0].EntryDate.getUTCFullYear(), g.entries[0].EntryDate.getUTCMonth(), g.entries[0].EntryDate.getUTCDate(), 0, 0, 0, 0), allDay: true }; });
            $scope.events.splice(0, $scope.events.length);
            query.forEach(function (item) {
                $scope.events.push(item);
            });
        });

//        $scope.events = function (start, end, callback) {
        //    Entry.getFilteredList(start, end)
        //        .then(function (results) {
        //            var items = [];
        //            angular.forEach(results, function (result) {
        //                items.push({
        //                    title: result.EntryText,
        //                    start: result.EntryDate,
        //                    allDay: true
        //                });
            //            });
            //var items = [];
            //callback(items);
        //        });
//        };

        $scope.events = [];

        $scope.entries = [$scope.events];
        //FlurryAgent.logEvent("CalendarView");

        Entry.getList().then(function (results) {
            $scope.$apply(function () {
                $scope.items = results;
            });
        });
}]);
