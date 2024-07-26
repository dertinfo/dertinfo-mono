import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';
import { EventAdminConductor } from '../../services/event-admin.conductor';
import { EventAdminTracker } from '../../services/event-admin.tracker';

@Component({
  selector: 'app-event-registrations',
  templateUrl: './event-registrations.component.html',
  styleUrls: ['./event-registrations.component.css']
})
export class EventRegistrationsComponent implements OnInit, OnDestroy {

  private _subscriptions: Subscription[] = [];

  public eventRegistrations$;

  constructor(
    private router: ActivatedRoute,
    private _eventConductor: EventAdminConductor,
    private _eventTracker: EventAdminTracker,
  ) { }

  ngOnInit() {

    this.eventRegistrations$ = this._eventTracker.eventRegistrations$; // uses | async

    this._subscriptions.push(this.router.parent.params.subscribe(params => {
      this._eventConductor.initRegistrations(params['id']);
    }));
  }

  ngOnDestroy() {
    this._subscriptions.map((sub) => { sub.unsubscribe(); });
    this._subscriptions = []; // For good measure
  }

}
