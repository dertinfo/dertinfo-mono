import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CompetitionSummaryDto } from 'app/models/dto';
import { Subscription } from 'rxjs';
import { CompetitionAdminConductor } from '../../services/competition-admin.conductor';
import { CompetitionAdminTracker } from '../../services/competition-admin.tracker';

@Component({
  selector: 'app-competition-overview',
  templateUrl: './overview.component.html',
  styleUrls: ['./overview.component.css']
})
export class OverviewComponent implements OnInit, OnDestroy {

  private _subscriptions: Subscription[] = [];

  public summary: CompetitionSummaryDto;

  constructor(
    private _activatedRoute: ActivatedRoute,
    private _conductor: CompetitionAdminConductor,
    private _tracker: CompetitionAdminTracker
  ) { }

  ngOnInit() {
    this._subscriptions.push(this._tracker.overview$.subscribe((overview) => {
      this.summary = overview.summary;
    }));
  }

  ngOnDestroy() {
    this._subscriptions.map((sub) => { sub.unsubscribe(); });
    this._subscriptions = []; // For good measure
  }

}
