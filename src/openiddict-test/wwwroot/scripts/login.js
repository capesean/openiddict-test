/// <reference path="../../scripts/typings/angularjs/angular.d.ts" />
(function () {
    "use strict";
    angular.module("openiddicttest").controller("login", loginController);
    loginController.$inject = ["authService"];
    function loginController(authService) {
        var vm = this;
        vm.email = "user@test.com";
        vm.password = "P2ssw0rd!";
        vm.login = login;

        function login() {
            var user = { userName: vm.email, password: vm.password };
            authService.login(user).then(function () {
                authService.getIdentity().then(function (identity) {
                    alert("Hello " + identity.email + ". You can now redirect!");
                    vm.identity = identity;
                });
                // redirect here...
                //if ($stateParams.url)
                //    $window.location.assign($stateParams.url);
                //else
                //    $window.location.assign("/");
            }, function (err) {
                if (!err)
                    alert("No response was received from the server");
                else
                    alert(err.error_description);
            });
        }
        ;
    }
    ;
}());
