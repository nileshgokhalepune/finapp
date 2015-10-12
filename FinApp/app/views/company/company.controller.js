(function () {
    'use strict';

    angular.module('finapp').controller('CompanyCtrl', DashboardCtrl);

    DashboardCtrl.$inject = ['$scope', '$q', '$log', '$stateParams', 'DataSvc', '$state'];

    function DashboardCtrl($scope, $q, $log, $stateParams, DataSvc, $state) {
        var ctrl = this;
        ctrl.symbol = $stateParams.symbol;
        ctrl.sector = $stateParams.sector;
        ctrl.industry = $stateParams.industry;

        ctrl.quote = {};
        ctrl.enlargeChart = enlargeChart;
        ctrl.drawYearlyTrendChart = drawYearlyTrendChart;
        ctrl.drawDailyTrendChart = drawDailyTrendChart;
        ctrl.drawSma = drawSma;
        ctrl.navigateToDashboard = navigateToDashboard;
        ctrl.zoomChart = zoomChart;
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
            //DatSvc.getDailyTrend(ctrl.symbol, )
        }

        function drawYearlyTrendChart(dim, symbol) {
            if (!symbol) symbol = ctrl.symbol;
            var mybar = document.getElementById("myBar");
            var context = mybar.getContext("2d");

            context.clearRect(0, 0, mybar.width, mybar.height);
            DataSvc.getTrend(symbol).then(function (response) {
                ctrl.trendData = response.data;
                var labels = [];
                var dataPoints = []
                ctrl.trendData.forEach(function (data) {
                    labels.push(data.monthWord);
                    dataPoints.push(data[dim.toLowerCase()]);
                });
                var data = {
                    labels: labels,
                    datasets: [{
                        fillColor: "rgba(151,187,205,0.5)",
                        strokeColor: "rgba(151,187,205,0.8)",
                        highlightFill: "rgba(151,187,205,0.75)",
                        highlightStroke: "rgba(151,187,205,1)",
                        data: dataPoints
                    }]
                }
                var myBarChart = new Chart(context).Bar(data);
                //drawBarChart(context, ctrl.trendData, 50, 100, 1000, 50);
                ////drawLine(context, 30, 10, 30, 140);
                ////drawLine(context, 30, 140, 350, 140);
                ////var maxValue = 0;
                ////for (var i = 0; i < ctrl.trendData.length; i++) {
                ////    var name = ctrl.trendData[i].monthWord;
                ////    var dimension = (dim == 1 ? ctrl.trendData[i].high : (dim == 2 ? ctrl.trendData[i].low : (dim == 3 ? ctrl.trendData[i].open : (dim == 4 ? ctrl.trendData[i].close : 0))));
                ////    var height = dimension < mybar.height ? parseInt(dimension) : parseInt(dimension) / mybar.height;
                ////    if (parseInt(height) > parseInt(maxValue)) maxValue = height;
                ////    context.fillStyle = getRandomColor();// "#b90000";
                ////    drawRectangle(context, 30 + (i * 20) + i, 140 - height, 20, height, true);
                ////    context.textAlign = "left";
                ////    context.fillStyle = "#000";
                ////    context.fillText(name, 30 + (i * 20) + i, 140 + 10, 200);
                ////}

                ////var numMarkers = Math.ceil(maxValue / 50);
                ////context.textAlign = "right";
                ////context.fillStyle = "#000";
                ////var markerValue = 0;
                ////for (var i = 0; i < numMarkers; i++) {
                ////    context.fillText(markerValue, 30 - 5, 140 - markerValue, 50);
                ////    markerValue += 50
                ////}
            }).catch(function (error) {

            });
        }

        function drawSma() {
            DataSvc.getSma(ctrl.dt1, parseInt(ctrl.averageOnDays), ctrl.symbol).then(function (response) {
                ctrl.smaData = JSON.parse(response.data);
                drawSmaLineGraph();
            }).catch(function (error) {

            })
        }

        function drawSmaLineGraph() {
            var sma = document.getElementById("smaChart");
            var context = sma.getContext("2d");
            var labels = [];
            var dataPoints = [];
            ctrl.smaData.forEach(function (data) {
                labels.push(data.Date);
                dataPoints.push(data.Average);
            })
            var data = {
                labels: labels,
                datasets: [{
                    fillColor: "rgba(110,200,205,0.2)",
                    strokeColor: "rgba(151,187,205,1)",
                    pointColor: "rgba(151,187,205,1)",
                    pointStrokeColor: "#000",
                    pointHighlightFill: "#fff",
                    pointHighlightStroke: "rgba(151,187,205,1)",
                    data: dataPoints
                }]
            }
            var myLineChart = new Chart(context).Line(data, {
                bezierCurve: false
            });
        }

        function getMinMaxValue(list, prop, findmax) {
            var max = 0;
            var min = list[0][prop];

            list.forEach(function (data) {
                if (findmax) {
                    if (data[prop] > max) {
                        max = data[prop];
                    }
                } else if (!findmax) {
                    if (data[prop] < min) {
                        min = data[prop];
                    }
                }
            });
            return max;
        }

        function navigateToDashboard() {
            $state.go("dashboard", { sector: ctrl.sector, industry: ctrl.industry });
        }

        function zoomChart(id) {
            angular.element("#" + id).addClass("chartZoom");
        }
    }
})();


var CanvasChart = function () {
    var ctx;
    var margin = { top: 40, left: 75, right: 0, bottom: 75 };
    var chartHeight, chartWidth, yMax, xMax, data;
    var maxYValue = 0;
    var ration = 0;
    var renderType = { lines: 'lines', points: 'points' };
    return {
        renderTyp: renderType,
        render: render
    }
}();

var render = function (canvasId, dataObj) {
    data = dataObj;
    getMasxDataYValues();
    var canvas = document.getElementById(canvasId);
    chartHeight = canvas.attr('height');
    chartWidth = canvas.attr('width');
    xMax = chartWidth - (margin.left + margin.right);
    yMax = chartHeight - (margin.top + margin.bottom);
    ration = yMax / maxYValue;
    ctx = canvas.getContext("2d");
    renderChart();
}

var renderChart = function () {
    renderBackground();
    renderText();
    renderLinesAndLabels();
    if (data.renderTypes == undefined || data.renderTypes == null) data.renderTypes = [renderTypes.lines];
    for (var i = 0; i < data.renderTypes.length; i++) {
        renderData(data.renderTypes[i]);
    }
}

var renderBackground = function () {
    var lingrad = ctx.createLinearGradient(margin.left, margin.top, xMax - margin.right, yMax);
    lingrad.addColorStop(0.0, "#d4d4d4");
    lingrad.addColorStop(0.2, "#fff");
    lingrad.addColorStop(0.8, "#fff");
    lingrad.addColorStop(1, "#d4d4d4");
    ctx.fillStyle = lingrad;
    ctx.fillRect(margin.left, margin.top, xMax - margin.left, yMax - margin.top);
    ctx.fillStyle = 'black';
}

var renderText = function () {
    var labelFont = (data.labelFont !== null) ? data.labelFont : '20pt Arial';
    ctx.font = labelFont;
    ctx.textAlign = "center";
    var txtSize = ctx.measureText(data.xLabel);
    ctx.fillText(data.xLabel, margin.left + (xMax / 2) - (txtSize.width / 2), yMax + (margin.bottom / 1.2));
    ctx.save();
    ctx.rotate(-Math.PI / 2);
    ctx.font = labelFont;
    ctx.fillText(data.yLabel, (yMax / 2) * -1, margin.left / 4);
    ctx.restore();
}

var renderLinesAndLabels = function () {
    var yInc = yMax / data.dataPoints.length;
    var yPos = 0;
    var yLabelInc = (maxYValue * ration) / data.dataPoints.length;
    var xInc = getXInc();
    var xPos = margin.left;
    for (var i = 0; i < data.dataPoints.length; i++) {
        yPos += (i == 0) ? margin.top : yInc;
        drawLine(margin.left, yPos, xMax, yPos, "#e8e8e8");
    }
}


function getRandomColor() {
    var letters = '0123456789ABCDEF'.split('');
    var color = '#';
    for (var i = 0; i < 6; i++) {
        color += letters[Math.floor(Math.random() * 16)];
    }
    return color;
}