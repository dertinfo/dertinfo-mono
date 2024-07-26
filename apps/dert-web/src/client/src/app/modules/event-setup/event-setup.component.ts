import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';
import { EventSetupConductor } from './services/event-setup.conductor';
import { EventSetupTracker } from './services/event-setup.tracker';

@Component({
  selector: 'app-event-setup',
  templateUrl: './event-setup.component.html'
})
export class EventSetupComponent implements OnInit, OnDestroy {

  private _subscriptions: Subscription[] = [];
  private activeView = 'start';

  public informationHeader = '';
  public informationBody = '';

  constructor(
    private _activatedRoute: ActivatedRoute,
    private _conductor: EventSetupConductor,
    private _tracker: EventSetupTracker
  ) { }

  ngOnInit() {

    // Listen for changes to Route Paramaters
    this._subscriptions.push(this._activatedRoute.params.subscribe(params => {
      this._tracker.reset();
      this._conductor.setOverview(this._activatedRoute.snapshot.data.eventConfigureOverview);
      this._conductor.initUserData();
      this.activeView = this._activatedRoute.snapshot.params['view'];
    }));

    this._subscriptions.push(this._tracker.configurationHelp$.subscribe(help => {
      if (help != null) {
        this.informationHeader = help.header;
        this.informationBody = help.body;
      }
    }));
  }

  ngOnDestroy() {
    this._subscriptions.map((sub) => { sub.unsubscribe(); });
    this._subscriptions = []; // For good measure
  }
}
