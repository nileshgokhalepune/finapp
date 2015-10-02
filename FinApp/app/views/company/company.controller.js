(function () {
    'use strict';

    angular.module('finapp').controller('CompanyCtrl', DashboardCtrl);

    DashboardCtrl.$inject = ['$scope', '$q', '$log', '$stateParams', 'DataSvc'];

    function DashboardCtrl($scope, $q, $log, $stateParams, DataSvc) {
        var ctrl = this;
        ctrl.symbol = $stateParams.symbol;
        ctrl.quote = {};
        ctrl.enlargeChart = enlargeChart;
        ctrl.drawYearlyTrendChart = drawYearlyTrendChart;
        activate();

        $scope.$on("$viewContentLoaded", function () {

        });

        function activate() {
            if ($stateParams.symbol && $stateParams.symbol !== '') {
                var symbol = $stateParams.symbol;
                var q = [];
                q.push(DataSvc.getQuote(symbol));
                q.push(DataSvc.getHistoricalData(symbol));

                $q.all(q).then(function (response) {
                    ctrl.quote = response[0].data;
                    ctrl.historyData = response[1].data;

                }).catch(function (error) {

                });

            }
        }

        function enlargeChart(chartId) {
            document.getElementById(chartId).width = "500px";
            document.getElementById(chartId).zindex = "999";
        }

        function drawBarChart(context, data, startx, barwidht, chartHeight, makrDataIncrementsIn) {
            context.lineWidth = "1.0";
            var startY = 380;
            drawLine(context, startx, startY, startx, 30);
            drawLine(context, startx, startY, 370, startY);
            context.lineWidth = "0.0";
            var maxValue = 0;
            for (var i = 0; i < data.length; i++) {
                // Extract the data
                //var values = data[i].split(",");
                var name = data[i].date;//values[0];
                var height = parseInt(data[i].t1);
                if (parseInt(height) > parseInt(maxValue)) maxValue = height;

                // Write the data to the chart
                context.fillStyle = "#b90000";
                drawRectangle(context, startX + (i * barWidth) + i, (chartHeight - height), barWidth, height, true);

                // Add the column title to the x-axis
                context.textAlign = "left";
                context.fillStyle = "#000";
                context.fillText(name, startX + (i * barWidth) + i, chartHeight + 10, 200);
            }
            // Add some data markers to the y-axis
            var numMarkers = Math.ceil(maxValue / markDataIncrementsIn);
            context.textAlign = "right";
            context.fillStyle = "#000";
            var markerValue = 0;
            for (var i = 0; i < numMarkers; i++) {
                context.fillText(markerValue, (startX - 5), (chartHeight - markerValue), 50);
                markerValue += markDataIncrementsIn;
            }
        }

        function drawLine(contextO, startx, starty, endx, endy) {
            contextO.beginPath();
            contextO.moveTo(startx, starty);
            contextO.lineTo(endx, endy);
            contextO.closePath();
            contextO.stroke();
        }

        function drawRectangle(contextO, x, y, w, h, fill) {
            contextO.beginPath();
            contextO.rect(x, y, w, h);
            contextO.closePath();
            contextO.stroke();
            if (fill) contextO.fill();
        }

        function drawDailyTrendChart() {

        }

        function drawYearlyTrendChart(dim, symbol) {
            if (!symbol) symbol = ctrl.symbol;
            var mybar = document.getElementById("myBar");
            var context = mybar.getContext("2d");
            context.clearRect(0, 0, mybar.width, mybar.height);
            DataSvc.getTrend(symbol).then(function (response) {
                ctrl.trendData = response.data;
                //drawBarChart(context, ctrl.trendData, 50, 100, 1000, 50);
                drawLine(context, 30, 10, 30, 140);
                drawLine(context, 30, 140, 350, 140);
                var maxValue = 0;
                for (var i = 0; i < ctrl.trendData.length; i++) {
                    var name = ctrl.trendData[i].monthWord;
                    var dimension = (dim == 1 ? ctrl.trendData[i].high : (dim == 2 ? ctrl.trendData[i].low : (dim == 3 ? ctrl.trendData[i].open : (dim == 4 ? ctrl.trendData[i].close : 0))));
                    var height = dimension < mybar.height ? parseInt(dimension) : parseInt(dimension) / mybar.height;
                    if (parseInt(height) > parseInt(maxValue)) maxValue = height;
                    context.fillStyle = "#b90000";
                    drawRectangle(context, 30 + (i * 20) + i, 140 - height, 20, height, true);
                    context.textAlign = "left";
                    context.fillStyle = "#000";
                    context.fillText(name, 30 + (i * 20) + i, 140 + 10, 200);
                }

                var numMarkers = Math.ceil(maxValue / 50);
                context.textAlign = "right";
                context.fillStyle = "#000";
                var markerValue = 0;
                for (var i = 0; i < numMarkers; i++) {
                    context.fillText(markerValue, 30 - 5, 140 - markerValue, 50);
                    markerValue += 50
                }
            }).catch(function (error) {

            });
        }

    }
})();