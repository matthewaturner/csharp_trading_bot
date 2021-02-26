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
  public numberOfShares = [100, 120, 170, 90, 80, 100, 120, 200, 420.69, 694.20];

  public chart = [];
  public orderHistory: OrderHistory;

  private httpClient: HttpClient;
  private baseUrl: string;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.httpClient = http;
    this.baseUrl = baseUrl;
  }

  public buildChart(_labels, _data, _chartType) {
    this.chart.push(new Chart('canvas', {
      type: _chartType,
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
  }

  public incrementCounter() {
    this.currentCount++;
  }

  public getOrderDates() {
    return this.orderHistory.dates;
  }

  public getOrderValues() {
    return this.orderHistory.values;
  }

  public getOrderQuantities() {
    return this.orderHistory.quantity;
  }

  public runBackTest() {
    this.httpClient.get<OrderHistory>(this.baseUrl + 'backtest').subscribe(result => {
      this.orderHistory = result;
    }, error => console.error(error));

    this.buildChart(this.orderHistory.dates, this.orderHistory.values, "line");
    //this.buildChart(this.orderHistory.dates, this.orderHistory.quantity, "bar");
  }

  public getChart() {
    return this.chart;
  }
}

interface OrderHistory {
  symbol: string;
  dates: Date[];
  values: number[];
  quantity: number[];
}
