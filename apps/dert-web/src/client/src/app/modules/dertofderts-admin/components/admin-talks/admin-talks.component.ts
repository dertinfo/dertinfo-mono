import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';

@Component({
    selector: 'app-admin-talks',
    templateUrl: './admin-talks.component.html',
    styleUrls: ['./admin-talks.component.scss'],
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class AdminTalksComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  }

}
