import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';

@Component({
    selector: 'app-admin-judges',
    templateUrl: './admin-judges.component.html',
    styleUrls: ['./admin-judges.component.css'],
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class AdminJudgesComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  }

}
