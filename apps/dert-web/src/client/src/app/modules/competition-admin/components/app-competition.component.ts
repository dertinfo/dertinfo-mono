import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';
import { CompetitionAdminConductor } from '../services/competition-admin.conductor';
import { CompetitionAdminTracker } from '../services/competition-admin.tracker';

@Component({
  selector: 'app-competition',
  templateUrl: './app-competition.component.html',
  styleUrls: ['./app-competition.component.css']
})
export class CompetitionComponent implements OnInit, OnDestroy {

  private _subscriptions: Subscription[] = [];

  public competitionName;

  constructor(
    private _activatedRoute: ActivatedRoute,
    private _conductor: CompetitionAdminConductor,
    private _tracker: CompetitionAdminTracker
  ) { }

  ngOnInit() {

    // Listen for changes to Route Paramaters
    this._subscriptions.push(this._activatedRoute.params.subscribe(params => {
      this._tracker.reset();
      this._conductor.setOverview(this._activatedRoute.snapshot.data.competitionOverview);
    }));

    // Listen for changes to Overview
    this._subscriptions.push(this._tracker.overview$.subscribe((overview) => {
      if (overview) {
        this.competitionName = overview.name;
      }
    }));
  }

  ngOnDestroy() {
    this._subscriptions.map((sub) => { sub.unsubscribe(); });
    this._subscriptions = []; // For good measure
  }

}
