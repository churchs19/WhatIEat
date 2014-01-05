angular.module('whatieat.services')
  .factory('AzureMobileService', ['$q', function ($q) {
      var client = new WindowsAzure.MobileServiceClient("https://whatieat.azure-mobile.net/", "stXIOZDpAbYsxvASsmSXyRVTgXsYXU62");
      var userId = null;

      // Request authentication from Mobile Services using a Microsoft login.
      var login = function () {
          return new WinJS.Promise(function (complete) {
              if (userId === null) {
                  client.login("microsoftaccount", null, false).done(function (results) {
                      userId = results.userId;
//                      FlurryAgent.setUserId(userId);
                      //var message = "You are now logged in as: " + userId;
                      //var dialog = new Windows.UI.Popups.MessageDialog(message);
                      //dialog.showAsync().done(complete);
                      complete(userId);
                  }, function (error) {
                      userId = null;
                      var dialog = new Windows.UI.Popups
                          .MessageDialog("An error occurred during login", "Login Required");
                      dialog.showAsync().done(complete);
                  });
              }
              else {
                  complete(userId);
              }
          });
      }

      var logout = function () {
          return new WinJS.Promise(function (complete) {
              client.logout();
              userId = null;
              complete();
          });
      }

      // Public API here
      return {
          isAuthenticated: function () {
              return userId !== null;
          },
          authenticate: function () {
              return login();
          },
          logout: function () {
              return logout();
          },
          getTable: function (tableName) {
              return login().then(function () {
                  return new WinJS.Promise(function (complete) {
                      var table = client.getTable(tableName);
                      complete(table);
                  });
              });
          }
      };
  }]);