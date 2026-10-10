import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';

@Component({
    selector: 'app-admin-complaints',
    templateUrl: './admin-complaints.component.html',
    styleUrls: ['./admin-complaints.component.scss'],
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class AdminComplaintsComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  }

}
