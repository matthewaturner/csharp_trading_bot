"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __param = (this && this.__param) || function (paramIndex, decorator) {
    return function (target, key) { decorator(target, key, paramIndex); }
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.BackTestComponent = void 0;
var core_1 = require("@angular/core");
var chart_js_1 = require("chart.js");
var BackTestComponent = /** @class */ (function () {
    function BackTestComponent(http, baseUrl) {
        this.http = http;
        this.currentCount = 0;
        this.chart = [];
        this.httpClient = http;
        this.baseUrl = baseUrl;
    }
    BackTestComponent.prototype.buildChart = function (_labels, _dataSet1, _dataSet1Name, _dataSet1ChartType, _dataSet2, _dataSet2Name, _dataSet2ChartType) {
        this.chart.push(new chart_js_1.Chart('canvas', {
            data: {
                labels: _labels,
                datasets: [
                    {
                        label: _dataSet1Name,
                        data: _dataSet1,
                        borderColor: '#3cba9f',
                        fill: false,
                        type: _dataSet1ChartType
                    },
                    {
                        label: _dataSet2Name,
                        data: _dataSet2,
                        borderColor: '#3cba9f',
                        fill: false,
                        type: _dataSet2ChartType
                    }
                ]
            },
            options: {
                legend: {
                    display: true
                },
                scales: {
                    xAxes: [{
                            display: true
                        }],
                    yAxes: [{
                            display: true
                        }]
                }
            }
        }));
    };
    BackTestComponent.prototype.incrementCounter = function () {
        this.currentCount++;
    };
    BackTestComponent.prototype.getOrderDates = function () {
        return this.orderHistory.dates;
    };
    BackTestComponent.prototype.getOrderValues = function () {
        return this.orderHistory.portfolioValue;
    };
    BackTestComponent.prototype.getOrderQuantities = function () {
        return this.orderHistory.quantity;
    };
    BackTestComponent.prototype.runBackTest = function () {
        var _this = this;
        this.http.get(this.baseUrl + 'backtest').subscribe(function (result) {
            _this.orderHistory = result;
            _this.buildChart(_this.orderHistory.dates, _this.orderHistory.quantity, 'Order Quantity', 'bar', _this.orderHistory.portfolioValue, 'Portfolio Value', 'line');
        }, function (error) { return console.error(error); });
    };
    BackTestComponent.prototype.getChart = function () {
        return this.chart;
    };
    BackTestComponent = __decorate([
        core_1.Component({
            selector: 'app-BackTest-component',
            templateUrl: './BackTest.component.html'
        }),
        __param(1, core_1.Inject('BASE_URL'))
    ], BackTestComponent);
    return BackTestComponent;
}());
exports.BackTestComponent = BackTestComponent;
//# sourceMappingURL=BackTest.component.js.map