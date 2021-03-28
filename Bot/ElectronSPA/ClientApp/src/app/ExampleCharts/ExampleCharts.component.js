"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.ExampleChartsComponent = void 0;
var core_1 = require("@angular/core");
var Chart = require("chart.js");
var ExampleChartsComponent = /** @class */ (function () {
    function ExampleChartsComponent() {
        this.title = 'chartjsangular';
    }
    ExampleChartsComponent.prototype.ngOnInit = function () {
        this.canvas = document.getElementById('myChart');
        this.ctx = this.canvas.getContext('2d');
        var myChart = new Chart(this.ctx, {
            type: 'pie',
            data: {
                labels: ["New", "In Progress", "On Hold"],
                datasets: [{
                        label: '# of Votes',
                        data: [1, 2, 3],
                        backgroundColor: [
                            'rgba(255, 99, 132, 1)',
                            'rgba(54, 162, 235, 1)',
                            'rgba(255, 206, 86, 1)'
                        ],
                        borderWidth: 1
                    }]
            }
        });
        this.canvas1 = document.getElementById('myChart1');
        this.ctx1 = this.canvas1.getContext('2d');
        var myChart1 = new Chart(this.ctx1, {
            type: 'line',
            data: {
                labels: ['January', 'February', 'March', 'April'],
                datasets: [{
                        label: '# of Votes',
                        fill: false,
                        data: [5, 3, 4, 2],
                        backgroundColor: [
                            'rgba(255, 99, 132, 1)',
                            'rgba(54, 162, 235, 1)',
                            'rgba(255, 206, 86, 1)'
                        ],
                        borderColor: "#ffbd35",
                        borderWidth: 1
                    }]
            },
            options: {
                responsive: false,
                scales: {
                    yAxes: [{ display: false }], xAxes: [{
                            display: false //this will remove all the x-axis grid lines
                        }]
                }
            },
        });
        this.canvas2 = document.getElementById('myChart2');
        this.ctx2 = this.canvas2.getContext('2d');
        var myChart2 = new Chart(this.ctx2, {
            type: 'line',
            data: {
                labels: ['January', 'February', 'March', 'April'],
                datasets: [{
                        label: '# of Votes',
                        fill: false,
                        data: [5, 3, 4, 2],
                        backgroundColor: [
                            'rgba(255, 99, 132, 1)',
                            'rgba(54, 162, 235, 1)',
                            'rgba(255, 206, 86, 1)'
                        ],
                        borderColor: "#ffbd35",
                        borderWidth: 1
                    }]
            },
            options: {
                responsive: false,
                scales: {
                    yAxes: [{ display: false }], xAxes: [{
                            display: false //this will remove all the x-axis grid lines
                        }]
                },
                elements: {
                    line: {
                        tension: 0.000001
                    }
                },
            },
        });
        this.canvas3 = document.getElementById('myChart3');
        this.ctx3 = this.canvas3.getContext('2d');
        var myChart3 = new Chart(this.ctx3, {
            type: 'bar',
            data: {
                labels: ["New", "In Progress", "On Hold"],
                datasets: [{
                        label: '# of Votes',
                        data: [3, 4, 3],
                        backgroundColor: [
                            'rgba(255, 99, 132, 1)',
                            'rgba(54, 162, 235, 1)',
                            'rgba(255, 206, 86, 1)'
                        ],
                        borderWidth: 1
                    }]
            },
            options: {
                responsive: false,
                scales: {
                    yAxes: [{
                            ticks: {
                                beginAtZero: true
                            }
                        }]
                }
            }
        });
    };
    ExampleChartsComponent = __decorate([
        core_1.Component({
            selector: 'app-exampleCharts-component',
            templateUrl: './ExampleCharts.component.html'
        })
    ], ExampleChartsComponent);
    return ExampleChartsComponent;
}());
exports.ExampleChartsComponent = ExampleChartsComponent;
//# sourceMappingURL=ExampleCharts.component.js.map