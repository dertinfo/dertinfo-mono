import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';

@Component({
    selector: 'app-admin-submissions',
    templateUrl: './admin-submissions.component.html',
    styleUrls: ['./admin-submissions.component.css'],
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class AdminSubmissionsComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  }

}
