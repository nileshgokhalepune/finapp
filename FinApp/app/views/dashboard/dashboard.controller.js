(function () {
    'use strict';

    angular.module('finapp').controller('DashboardCtrl', DashboardCtrl);

    DashboardCtrl.$inject = ['$q', '$log', 'DataSvc', '$stateParams', '$scope', '$timeout'];

    function DashboardCtrl($q, $log, DataSvc, $stateParams, $scope, $timeout) {
        var ctrl = this;
        ctrl.selectedSector = $stateParams.sector;
        ctrl.selectedIndustry = $stateParams.industry;

        ctrl.industries = '';
        ctrl.sectors = '';
        ctrl.getHistoricalData = getHistoricalData;
        ctrl.getIndustries = getIndustries;
        ctrl.getStocks = getStocks;
        ctrl.getQuote = getQuote;
        ctrl.currentIndustry;

        activate();

        function activate() {
            DataSvc.checkSiteStatus().then(function () {
                $log.debug("success");
                DataSvc.getSectors().then(function (response) {
                    ctrl.sectors = response.data;
                    if (ctrl.selectedSector) {
                        $timeout(function () {
                            $scope.$apply(function () {
                                ctrl.sector = ctrl.selectedSector;
                            });
                        }, 1000)

                    }
                }).catch(function (error) {
                    $log.error(error);
                });

            }).catch(function (error) {
                $log.error(error);
            });
        }

        function getIndustries(sectorId) {
            ctrl.companies = [];
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

        function getStocks(row) {
            ctrl.currentIndustry = row.sectorId;
            DataSvc.getStocks(row.sectorId).then(function (response) {
                ctrl.companies = response.data.company;
            }).catch(function (error) {
            });
        }

        function getQuote(symbol) {
            DataSvc.getQuote(symbol).then(function (response) {
                ctrl.quote = response.data;
            }).catch(function (error) {

            })
        }
    }
})();