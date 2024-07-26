import { Component, OnDestroy, OnInit } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
import { ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';

import { CompetitionEntryAttributeDto, GroupTeamCompetitionEntryDto, ScoreCategoryDto, ScoreSetDto } from 'app/models/dto';

import { CompetitionAdminConductor } from '../../services/competition-admin.conductor';
import { CompetitionAdminTracker } from '../../services/competition-admin.tracker';
@Component({
  selector: 'app-competition-entrants',
  templateUrl: './entrants.component.html',
  styleUrls: ['./entrants.component.css']
})
export class EntrantsComponent implements OnInit, OnDestroy {

  private _subscriptions: Subscription[] = [];

  public displayedColumnsEntrants: string[] = ['avatar', 'name', 'attributes'];
  public dataSourceEntrants = new MatTableDataSource<GroupTeamCompetitionEntryDto>();

  public entryAttributes: Array<CompetitionEntryAttributeDto> = [];
  public isProcessingEntrants = false;

  public get canPopulate() {
    return this._tracker.overview.summary
      && this._tracker.overview.summary.allowPopulation
      && !this._tracker.overview.summary.hasBeenPopulated
      && this._tracker.overview.summary.numberOfTicketsSold > 0;
  }

  public get canReset() {
    return this._tracker.overview.summary
      && this._tracker.overview.summary.allowPopulation
      && this._tracker.overview.summary.allowDanceGeneration
      && this._tracker.overview.summary.hasBeenPopulated
      && this._tracker.settings.inTestingMode;
  }

  public get editingBlocked() {
    return this._tracker.overview.summary.resultsPublished;
  }

  constructor(
    private _activatedRoute: ActivatedRoute,
    private _conductor: CompetitionAdminConductor,
    private _tracker: CompetitionAdminTracker,

  ) { }

  ngOnInit() {

    // Start to listen for changes on the tracker
    this._subscriptions.push(this._tracker.entrants$.subscribe((entrants) => {
      this.loadOrRefreshComponent();
    }));

    // apply the settings through the conductor to the tracker from the resolver
    this._conductor.applyEntrants(this._activatedRoute.snapshot.data.entrants);
    this._conductor.applyEntryAttributes(this._activatedRoute.snapshot.data.entryAttributes);
    this._conductor.applySettings(this._activatedRoute.snapshot.data.settings);

    this.loadOrRefreshComponent();
  }

  ngOnDestroy() {
    this._subscriptions.map((sub) => { sub.unsubscribe(); });
    this._subscriptions = []; // For good measure
  }

  public onAttributeSelectionChanged(entrant: GroupTeamCompetitionEntryDto, $event: any) {
    if ($event.toggle === 'on') {
      this._conductor.applyEntryAttributeToEntrant($event.entryAttribute.id, entrant.competitionEntryId);
    } else {
      this._conductor.removeEntryAttributeFromEntrant($event.entryAttribute.id, entrant.competitionEntryId);
    }
  }

  public onPopulateClick() {

    this._conductor.populateCompetition(this._tracker.competitionId);
    this.isProcessingEntrants = true;
  }

  public onRePopulateClick() {

    this.isProcessingEntrants = true;
    this._conductor.resetCompetition(this._tracker.competitionId);

  }

  private loadOrRefreshComponent() {
    this.dataSourceEntrants = new MatTableDataSource<GroupTeamCompetitionEntryDto>(this._tracker.entrants);
    this.entryAttributes = this._tracker.entryAttributes;
    this.isProcessingEntrants = false;
  }
}
