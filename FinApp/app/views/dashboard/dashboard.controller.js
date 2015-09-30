(function () {
    'use strict';

    angular.module('finapp').controller('DashboardCtrl', DashboardCtrl);

    DashboardCtrl.$inject = ['$q', '$log', 'DataSvc'];

    function DashboardCtrl($q, $log, DataSvc) {
        var ctrl = this;
        ctrl.industries = '';
        ctrl.sectors = '';
        ctrl.getHistoricalData = getHistoricalData;
        ctrl.getIndustries = getIndustries;
        ctrl.getStocks = getStocks;

        activate();

        function activate() {
            DataSvc.checkSiteStatus().then(function () {
                $log.debug("success");
                DataSvc.getSectors().then(function (response) {
                    ctrl.sectors = response.data;
                }).catch(function (error) {
                    $log.error(error);
                });

            }).catch(function (error) {
                $log.error(error);
            });
        }

        function getIndustries(sectorId) {
            DataSvc.getIndustries(ctrl.sector.sectorId).then(function (response) {
                ctrl.industries = response.data;
            }).catch(function (error) {

            });
        }

        function getHistoricalData(symbol) {
            ctrl.selectedHistoricalSymbol = symbol;
            DataSvc.getHistoricalData(symbol).then(function (response) {
                ctrl.historyData = response.data;
            }).catch(function (error) {
                toastr.error(error.message);
            });
        }

        function getStocks(id) {
            DataSvc.getStocks(id).then(function (response) {
                ctrl.companies = response.data;
            }).catch(function (error) {
            });
        }
    }
})();