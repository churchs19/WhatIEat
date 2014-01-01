angular.module('app')
    .controller('CalendarCtrl', function ($scope) {
        /* config object */
        $scope.uiConfig = {
            calendar: {
                //height: 450,
                //editable: true,
                //header: {
                //    left: 'month basicWeek basicDay agendaWeek agendaDay',
                //    center: 'title',
                //    right: 'today prev,next'
                //},
                //dayClick: $scope.alertEventOnClick,
                //eventDrop: $scope.alertOnDrop,
                //eventResize: $scope.alertOnResize
                aspectRatio: 1
            }
        };
        $scope.eventSources = [];
        //FlurryAgent.logEvent("CalendarView");
});
