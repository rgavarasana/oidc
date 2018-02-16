﻿(function () {
    "use strict";

    angular
        .module("common.services")
        .factory("tokenContainer",
                  [tokenContainer])

    function tokenContainer() {
   
        var container = {
            token: ""
        };
         
        var setToken = function (token) {
            container.token = token;
        };

        var getToken = function () {

			debugger;
            if (container.token === "") {
                if (localStorage.getItem("access_token") === null) {

                    var url = "https://localhost:44356/connect/authorize?" +
                        "client_id=imagegalleryimplicit&" +
                        "redirect_uri=" + encodeURI(window.location.protocol + "//" + window.location.host + "/callback.html") + "&" +
                        "response_type=token&" +
                        "scope=imagegalleryapi";

                    window.location = url;
                }
                else {
                    // set the token in localstorage
                    setToken(localStorage["access_token"]);
                }
            }
            return container;
        };

        // return value to call when calling the API 
        return {
            getToken: getToken
        };
        

    };

})();