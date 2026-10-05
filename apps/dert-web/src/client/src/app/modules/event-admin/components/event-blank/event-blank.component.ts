import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';

@Component({
    selector: 'app-event-blank',
    templateUrl: './event-blank.component.html',
    styleUrls: ['./event-blank.component.css'],
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class EventBlankComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  }

}
