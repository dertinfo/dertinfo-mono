import { AfterViewInit, Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';

import { DanceDetailDto } from 'app/models/dto';

import { CompetitionAdminConductor } from '../../services/competition-admin.conductor';
import { CompetitionAdminTracker } from '../../services/competition-admin.tracker';
import { DanceCreateComponent } from './dialogs/dance-create/dance-create.component';

@Component({
  selector: 'app-competition-dances',
  templateUrl: './dances.component.html',
  styleUrls: ['./dances.component.css']
})
export class DancesComponent implements OnInit, OnDestroy, AfterViewInit {

  public get allowDanceGeneration() {
    return this._tracker.overview.summary
      && this._tracker.overview.summary.hasBeenPopulated
      && !this._tracker.overview.summary.hasDancesGenerated
      && this._tracker.overview.summary.allowDanceGeneration;
  }

  public get resultsPublished() {
    return this._tracker.overview.summary.resultsPublished;
  }

  private _subscriptions: Subscription[] = [];

  @ViewChild(MatSort, { static: true }) sort: MatSort;

  public displayedColumns: string[] = ['teamName', 'venueName', 'hasScoresEntered', 'hasScoresChecked', 'overrun', 'check'];
  public dataSource = new MatTableDataSource<DanceDetailDto>();
  public isProcessingDances = false;
  public allowDanceAddition = false;
  public disableDanceAddition = false;

  public teamFilter = null;
  public venueFilter = null;

  public teamFilterItems = [];
  public venueFilterItems = [];

  constructor(
    public composeDialog: MatDialog,
    private _activatedRoute: ActivatedRoute,
    private _conductor: CompetitionAdminConductor,
    private _tracker: CompetitionAdminTracker,

  ) { }

  ngOnInit() {

    // Start to listen for changes on the tracker
    this._subscriptions.push(this._tracker.dances$.subscribe((overview) => {

      this.loadOrRefreshComponent();
    }));

    // apply the settings through the conductor to the tracker from the resolver
    this._conductor.applyDances(this._activatedRoute.snapshot.data.dances);

    this.loadOrRefreshComponent();

    // apply the datasource filter predicate
    this.dataSource.filterPredicate = (data: DanceDetailDto, filter: string) => {

      return data.teamName === filter || data.venueName === filter || (data.teamName + data.venueName) === filter;
    };

  }

  ngAfterViewInit() {

    this.dataSource.sort = this.sort;
  }

  ngOnDestroy() {
    this._subscriptions.map((sub) => { sub.unsubscribe(); });
    this._subscriptions = []; // For good measure
  }

  public onTeamFilterChanged() {
    this.applyFilter();
  }

  public onVenueFilterChanged() {
    this.applyFilter();
  }

  public refreshData() {
    this._conductor.initDances(true);
  }

  public applyFilter() {
    if (this.teamFilter || this.venueFilter) {
      const teamFilterStr = this.teamFilter ? this.teamFilter : '';
      const venueFilterStr = this.venueFilter ? this.venueFilter : '';
      this.dataSource.filter = teamFilterStr + venueFilterStr;

    } else {
      this.dataSource.filter = null;
    }
  }

  public onGenerateDancesClick() {

    this.isProcessingDances = true;
    this._conductor.generateCompetitionDances(this._tracker.competitionId);

  }

  public openAddDanceDialog() {

    const dialogRef = this.composeDialog.open(DanceCreateComponent);
    const dialogueSubscription = dialogRef.afterClosed().subscribe(result => {
      dialogueSubscription.unsubscribe();
    });
  }

  private loadOrRefreshComponent() {

    this.allowDanceAddition = this._tracker.overview.summary.allowAdHocDanceAddition;
    const dances = this._tracker.dances;
    if (dances) {

      this.disableDanceAddition = dances.length === 0 || !this._tracker.overview.summary.allowDanceGeneration;
      dances.map((dance) => {
        // Build The Team & Venue Filter Items
        if (!this.teamFilterItems.some(tfi => tfi.teamId === dance.teamId)) {
          this.teamFilterItems.push({ teamId: dance.teamId, teamName: dance.teamName });
        }

        if (!this.venueFilterItems.some(vfi => vfi.venueId === dance.venueId)) {
          this.venueFilterItems.push({ venueId: dance.venueId, venueName: dance.venueName });
        }
      });

      this.dataSource = new MatTableDataSource<DanceDetailDto>(dances);
      this.isProcessingDances = false;
      this.dataSource.sort = this.sort;
    }

  }
}
