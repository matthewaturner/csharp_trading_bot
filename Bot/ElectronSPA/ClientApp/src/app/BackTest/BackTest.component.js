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
        this.currentCount = 0;
        this.portfolioValueDates = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];
        this.numberOfShares = [100, 120, 170, 90, 80, 100, 120, 200, 420.69, 694.20];
        this.chart = [];
        this.orders = [];
        this.httpClient = http;
        this.baseUrl = baseUrl;
    }
    BackTestComponent.prototype.buildChart = function (_labels, _data) {
        this.chart.push(new chart_js_1.Chart('canvas', {
            type: 'line',
            data: {
                labels: _labels,
                datasets: [
                    {
                        data: _data,
                        borderColor: '#3cba9f',
                        fill: false
                    }
                ]
            },
            options: {
                legend: {
                    display: false
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
    BackTestComponent.prototype.getOrders = function () {
        return this.orders;
    };
    BackTestComponent.prototype.runBackTest = function () {
        var _this = this;
        this.httpClient.get(this.baseUrl + 'backtest').subscribe(function (result) {
            _this.orders = result;
        }, function (error) { return console.error(error); });
        //this.buildChart()
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