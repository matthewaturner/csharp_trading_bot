import { Component, AfterViewInit } from '@angular/core';
import * as Chart from 'chart.js';
@Component({
  selector: 'app-exampleCharts-component',
  templateUrl: './ExampleCharts.component.html'
})
export class ExampleChartsComponent {
  title = 'chartjsangular';
  canvas: any;
  ctx: any;
  canvas1: any;
  ctx1: any;
  canvas2: any;
  ctx2: any;
  canvas3: any;
  ctx3: any;

  ngOnInit() {
    this.canvas = document.getElementById('myChart');
    this.ctx = this.canvas.getContext('2d');
    let myChart = new Chart(this.ctx, {
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
    let myChart1 = new Chart(this.ctx1, {
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
    let myChart2 = new Chart(this.ctx2, {
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
    let myChart3 = new Chart(this.ctx3, {
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
  }
}
