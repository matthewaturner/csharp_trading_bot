import { Component, Inject } from '@angular/core';
import { Chart } from 'chart.js';
import { HttpClient } from '@angular/common/http';
import { inject } from '@angular/core/testing';

@Component({
  selector: 'app-BackTest-component',
  templateUrl: './BackTest.component.html'
})
export class BackTestComponent {
  public currentCount = 0;

  public chart = [];
  public orderHistory: OrderHistory;

  private httpClient: HttpClient;
  private baseUrl: string;

  constructor(private http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.httpClient = http;
    this.baseUrl = baseUrl;
  }

  public buildChart(_labels, _dataSet1, _dataSet1Name, _dataSet1ChartType, _dataSet2, _dataSet2Name, _dataSet2ChartType) {
    this.chart.push(new Chart('canvas', {      
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
  }

  public incrementCounter() {
    this.currentCount++;
  }

  public getOrderDates() {
    return this.orderHistory.dates;
  }

  public getOrderValues() {
    return this.orderHistory.portfolioValue;
  }

  public getOrderQuantities() {
    return this.orderHistory.quantity;
  }

  public runBackTest(): void {
     this.http.get<OrderHistory>(this.baseUrl + 'backtest').subscribe(result => {
       this.orderHistory = result;
       this.buildChart(this.orderHistory.dates, this.orderHistory.quantity, 'Order Quantity', 'bar', this.orderHistory.portfolioValue, 'Portfolio Value', 'line',);
     }, error => console.error(error));    
  }

  public getChart() {
    return this.chart;
  }
}

interface OrderHistory {
  symbol: string;
  dates: Date[];
  portfolioValue: number[];
  quantity: number[];
}
