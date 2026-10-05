import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';

@Component({
    selector: 'app-event-registration-tickets',
    templateUrl: './event-registration-tickets.component.html',
    styleUrls: ['./event-registration-tickets.component.css'],
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class EventRegistrationTicketsComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  }

}
