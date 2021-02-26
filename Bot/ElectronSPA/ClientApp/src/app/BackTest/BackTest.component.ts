import { Component, Inject } from '@angular/core';
import { Chart } from 'chart.js';
import { HttpClient } from '@angular/common/http'
import { inject } from '@angular/core/testing';

@Component({
  selector: 'app-BackTest-component',
  templateUrl: './BackTest.component.html'
})
export class BackTestComponent {
  public currentCount = 0;
  public portfolioValueDates = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];
  public portfolioValues = [100, 120, 170, 90, 80, 100, 120, 200, 420.69, 694.20];

  public chart = [];

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) { }

  public buildChart() {
    this.chart.push(new Chart('canvas', {
      type: 'line',
      data: {
        labels: this.portfolioValueDates,
        datasets: [
          {
            data: this.portfolioValues,
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
  }

  public incrementCounter() {
    this.currentCount++;
  }

  public getPortfolioValues() {
    return this.portfolioValues;
  }

  public runBackTest() {
    for (var i = 0; i < this.portfolioValues.length; i++) {
      this.portfolioValues[i] = Math.random() * 100;
    }
    this.buildChart()
  }

  public getChart() {
    return this.chart;
  }
}
