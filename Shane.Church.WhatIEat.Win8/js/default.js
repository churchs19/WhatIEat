// For an introduction to the Blank template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkId=232509

var flurryApiKey = "MYS2KYZW5SNXJ8QQR2H4";
var client = new WindowsAzure.MobileServiceClient("https://whatieat.azure-mobile.net/", "stXIOZDpAbYsxvASsmSXyRVTgXsYXU62");
var appVersion = "1.0.0.0";

(function () {
    "use strict";

    WinJS.Binding.optimizeBindingReferences = true;

    var app = WinJS.Application;
    var activation = Windows.ApplicationModel.Activation;

    var userId = null;

    // Request authentication from Mobile Services using a Facebook login.
    var login = function () {
        return new WinJS.Promise(function (complete) {
            client.login("microsoftaccount", null, true).done(function (results) {
                userId = results.userId;
                FlurryAgent.setUserId(userId);
                var message = "You are now logged in as: " + userId;
                var dialog = new Windows.UI.Popups.MessageDialog(message);
                dialog.showAsync().done(complete);
            }, function (error) {
                userId = null;
                var dialog = new Windows.UI.Popups
                    .MessageDialog("An error occurred during login", "Login Required");
                dialog.showAsync().done(complete);
            });
        });
    }

    var authenticate = function () {
        login().then(function () {
            if (userId === null) {
                // Authentication failed, try again.
                authenticate();
            }
        });
    }

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
                //WinJS.xhr({ url: "ms-appx-web:///InneractiveAd.html", responseType: "text" }).done(function (request) {
//                    var webviewControl = document.getElementById("webview");
//                    webviewControl.addEventListener("MSWebViewNavigationStarting", navigationStarting);
//                    webviewControl.addEventListener("MSWebViewContentLoading", contentLoading);
//                    webviewControl.addEventListener("MSWebViewDOMContentLoaded", domContentLoaded);
//                    webviewControl.addEventListener("MSWebViewNavigationCompleted", navigationCompleted);
//                    webviewControl.addEventListener("MSWebViewUnviewableContentIdentified", unviewableContentIdentified);
////                    webviewControl.navigateToString(request.responseText);
//                });
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
        FlurryAgent.endSession();
    };

    var myApp = angular.module('app', [
        'ngRoute',
        'ui.bootstrap',
        'ui.directives'
    ]).config(function ($routeProvider) {
        $routeProvider
            .when('/', {
                templateUrl: 'views/calendar/index.html',
                controller: 'CalendarCtrl'
            })
            //.when('/wells/new', {
            //    templateUrl: 'views/well/new.html',
            //    controller: 'WellAddCtrl'
            //})
            //.when('/wells/wellPathConversion', {
            //    templateUrl: 'views/well/wellPathConversion.html',
            //    controller: 'WellPathConversionCtrl'
            //})
            //.when('/wells/formationColorPatternList', {
            //    templateUrl: 'views/well/formationColorPatternList.html',
            //    controller: 'FormationColorPatternListCtrl'
            //})
            //.when('/wells/:wellId', {
            //    templateUrl: 'views/well/item.html',
            //    controller: 'WellItemCtrl'
            //})
            //.when('/wells/edit/:wellId', {
            //    templateUrl: 'views/well/edit.html',
            //    controller: 'WellEditCtrl'
            //})
            .otherwise({
                redirectTo: '/'
            });
    });

    // NavigationStarting event is triggered when the WebView begins navigating to a new URL. 
    function navigationStarting(e) {
        appendLog && appendLog("Starting navigation to " + e.uri + ".\n");
    }

    function contentLoading(e) {
        appendLog && appendLog("Loading content for " + e.uri + ".\n");
    }

    function domContentLoaded(e) {
        appendLog && appendLog("Content for " + e.uri + " has finished loading.\n");
    }

    // NavigationCompleted event is triggered either after all the DOM content has been loaded 
    // successfully, or when loading failed.  The event arg for this is different from the other 
    // navigation events, and includes a isSuccess field to indicate the status. 
    function navigationCompleted(e) {
        if (e.isSuccess) {
            appendLog && appendLog("Navigation completed successfully.\n");
        } else {
            WinJS.log && WinJS.log("Navigation failed with error code " + e.webErrorStatus, "WhatIEat", "error");
        }
    }

    // UnviewableContentIdentified event is triggered when the URL being navigated to is not 
    // a type that can be displayed in WebView, for example an EXE file or a ZIP file. 
    function unviewableContentIdentified(e) {
        WinJS.log && WinJS.log(e.uri + " cannot be displayed in WebView", "WhatIEat", "error");
    }

    function appendLog(message) {
        WinJS.log && WinJS.log(message, "WhatIEat", "info");
    }

    app.start();
})();