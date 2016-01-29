/// <reference path="../../scripts/typings/toastr/toastr.d.ts" />
/// <reference path="../../scripts/typings/angularjs/angular.d.ts" />
(function () {
    "use strict";
    // replicated in app
    var settings = {
        apiServiceBaseUri: window.location.toString().substring(0, window.location.toString().indexOf(window.location.pathname)) + "/",
        authClientId: "openiddicttest",
        apiPrefix: "api/"
    };
    angular
        .module("openiddicttest", [
            "ui.router",
            "angular-jwt",
            "angular-storage"
        ])
        .config(config)
        .constant("appSettings", settings);

    config.$inject = ["$urlRouterProvider", "$stateProvider", "$locationProvider"];
    function config($urlRouterProvider, $stateProvider, $locationProvider) {
        // configure routes
        setupRoutes($urlRouterProvider, $stateProvider, $locationProvider);
    }

    function setupRoutes($urlRouterProvider, $stateProvider, $locationProvider) {
        $locationProvider.html5Mode(true);
        $urlRouterProvider.otherwise("/login");
        $stateProvider
            .state("login", {
            url: "/login?url",
            templateUrl: "/loginpartial.html",
            controller: "login",
            controllerAs: "vm"
        });
    }
}());
