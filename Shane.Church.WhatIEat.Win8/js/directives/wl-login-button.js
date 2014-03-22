angular.module('whatieat.directives')
    .directive('wlLoginButton', function () {
        return {
            restrict: 'E',
            templateUrl: 'template/inlineEdit.html',
            scope: {
                field: '='
            },
            link: function postLink(scope, element, attrs) {
            }
        };
    });