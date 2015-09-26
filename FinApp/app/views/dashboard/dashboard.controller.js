(function () {
    'use strict';

    angular.module('finapp').controller('DashboardCtrl', DashboardCtrl);

    DashboardCtrl.$inject = ['$q', '$log', 'DataSvc'];

    function DashboardCtrl($q, $log, DataSvc) {
        var ctrl = this;
        ctrl.industries = '';
        activate();

        function activate() {
            var q = [];

            DataSvc.checkSiteStatus().then(function () {
                $log.debug("success");
                q.push(DataSvc.getSectors());

                $q.all(q).then(function (response) {
                    ctrl.industries = response[0].data;
                }).catch(function (error) {
                    $log.error(error);
                });

            }).catch(function (error) {

                $log.error(error);
            });
        }

        ctrl.getHistoricalData = function(symbol) {
            DataSvc
        }
    }
})();