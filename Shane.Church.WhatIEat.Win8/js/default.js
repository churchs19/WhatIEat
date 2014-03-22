// For an introduction to the Blank template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkId=232509

var flurryApiKey = "MYS2KYZW5SNXJ8QQR2H4";
var appVersion = "1.0.0.0";

(function () {
    "use strict";

    WinJS.Binding.optimizeBindingReferences = true;

    var app = WinJS.Application;
    var activation = Windows.ApplicationModel.Activation;
    var nav = WinJS.Navigation;

    app.onsettings = function (e) {
        e.detail.applicationcommands = { "app": { title: "Settings", href: "#/settings" } };
        WinJS.UI.SettingsFlyout.populateSettings(e);
    };

    app.onactivated = function (args) {
        if (args.detail.kind === activation.ActivationKind.launch) {
            if (args.detail.previousExecutionState !== activation.ApplicationExecutionState.terminated) {
                // TODO: This application has been newly launched. Initialize
                // your application here.
            } else {
                // TODO: This application has been reactivated from suspension.
                // Restore application state here.
            }

            //FlurryAgent.startSession(flurryApiKey);
            //FlurryAgent.setAppVersion(appVersion);
            args.setPromise(WinJS.UI.processAll().then(function () {
                //authenticate();
            }));
        }
    };

    app.oncheckpoint = function (args) {
        // TODO: This application is about to be suspended. Save any state
        // that needs to persist across suspensions here. You might use the
        // WinJS.Application.sessionState object, which is automatically
        // saved and restored across suspension. If you need to complete an
        // asynchronous operation before your application is suspended, call
        // args.setPromise().
        //FlurryAgent.endSession();
    };

    var myApp = angular.module('app', [
        'ngRoute',
        'ui.bootstrap',
        'ui.directives',
        'winjs',
        'whatieat.services',
        'whatieat.models',
        'whatieat.directives'
    ]).config(function ($routeProvider) {
        $routeProvider
            .when('/login', {
                templateUrl: 'views/login.html',
                controller: 'LoginCtrl'
            })
            .when('/calendar', {
                templateUrl: 'views/calendar.html',
                controller: 'CalendarCtrl'
            })
            .when('/settings', {
                templateUrl: 'views/settings.html',
                controller: 'SettingsCtrl'
            })
            .otherwise({
                redirectTo: '/login'
            });
    });

    app.start();
})();