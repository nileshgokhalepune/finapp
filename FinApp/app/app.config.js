(function () {
    'use strict';

    var app = angular.module('finapp')
        .config(configureApp);

    configureApp.$inject =  ['$stateProvider', '$urlRouterProvider'];

    function configureApp($stateProvider, $urlRouteProvider) { 
        $urlRouteProvider.when('', 'dashboard');

        $stateProvider.state('dasboard', {
            url: '/dashboard',
            controller: 'DashboardCtrl',
            controllerAs: 'ctrl',
            templateUrl: 'app/views/dashboard/dashboard.html',
        });
    }
})();