import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';

@Component({
    selector: 'app-competition-reports',
    templateUrl: './reports.component.html',
    styleUrls: ['./reports.component.css'],
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class ReportsComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  }

}
