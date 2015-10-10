(function () {
    'use strict';

    var app = angular.module('finapp')
        .config(configureApp);

    configureApp.$inject =  ['$stateProvider', '$urlRouterProvider'];

    function configureApp($stateProvider, $urlRouteProvider) { 
        $urlRouteProvider.when('', 'dashboard');

        $stateProvider.state('dashboard', {
            url: '/dashboard/{sector}/{industry}',
            controller: 'DashboardCtrl',
            controllerAs: 'ctrl',
            templateUrl: 'app/views/dashboard/dashboard.html',
        }).state('company', {
            url: '/company/{symbol}/{sector}/{industry}',
            controller: 'CompanyCtrl',
            controllerAs: 'ctrl',
            templateUrl: 'app/views/company/company.html'
        })
    }
})();