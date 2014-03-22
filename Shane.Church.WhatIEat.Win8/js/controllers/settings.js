angular.module('app')
    .controller('LoginCtrl', ['$scope', '$rootScope', 'Settings', function ($scope, $rootScope, Settings) {
        $scope.handleKeys = function (evt) {
            // Handles Alt+Left and backspace key in the control and dismisses it 
            if ((evt.altKey && evt.key === 'Left') || (evt.key === 'Backspace')) {
                WinJS.UI.SettingsFlyout.show();
            }
        }

        WinJS.UI.SettingsFlyout.show();
    }]);