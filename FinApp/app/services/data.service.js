﻿(function () {
    'use strict';

    angular.module('finapp').service('DataSvc', data);

    data.$inject = ['$http'];


    function data($http) {
        this.checkSiteStatus = checkSiteStatus;
        this.getSectors = getSectors;
        this.getIndustries = getIndustries;
        this.getHistoricalData = getHistoricalData;
        this.getStocks = getStocks;
        this.getQuote = getQuote;
        this.getTrend = getTrend;
        this.getSma = getSma;

        this.baseUrl = 'api/data';

        this.broadCast = broadCast;
        this.callBacks = [];
        this.registerCallBack = registerCallBack;

        function registerCallBack(callBack) {
            this.callBacks.push(callBack);
        }

        function broadCast() {

        }

        function checkSiteStatus() {
            var promise = $http({ method: 'GET', url: this.baseUrl + '/checkSiteStatus' });
            return promise;
        }

        function getSectors() {
            var promise = $http({ method: 'GET', url: this.baseUrl + '/sectors' });
            return promise;
        }

        function getIndustries(sectorId) {
            var promise = $http({ method: 'GET', url: this.baseUrl + '/industry', params: { sectorID: sectorId } });
            return promise;
        }

        function getHistoricalData(symbol) {
            var promise = $http({ method: 'GET', url: this.baseUrl + '/history', params: { symbol: symbol } });
            return promise;
        }

        function getStocks(id) {
            var promise = $http({ method: 'GET', url: this.baseUrl + '/stocks', params: { id: id } });
            return promise;
        }

        function getQuote(symbol) {
            var promise = $http({ method: 'GET', url: this.baseUrl + '/quote', params: { symbol: symbol } });
            return promise;
        }

        function getTrend(symbol) {
            var promise = $http({ method: 'GET', url: this.baseUrl + '/trend', params: { symbol: symbol } });
            return promise;
        }

        function getSma(date, days, symbol) {
            var promise = $http({ method: 'GET', url: this.baseUrl + '/sma', params: { startDate: date, avergaeOnDays: days, symbol: symbol } });
            return promise;
        }
    }
})();