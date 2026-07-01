import { AfterViewInit, Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';
import { CompetitionAdminConductor } from '../../services/competition-admin.conductor';
import { CompetitionAdminTracker } from '../../services/competition-admin.tracker';

import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { CompetitionEntryAttributeDto, DanceDetailDto, TeamCollatedFullResultDto } from 'app/models/dto';

class DataTableDataElement {
  public teamName: string;
  public counts: any;
  public attributes: Array<CompetitionEntryAttributeDto>;

}
@Component({
  selector: 'app-competition-results',
  templateUrl: './results.component.html',
  styleUrls: ['./results.component.css']
})
export class ResultsComponent implements OnInit, OnDestroy {

  public get resultsPublished() {
    return this._tracker.overview.summary.resultsPublished;
  }

  private _subscriptions: Subscription[] = [];

  public collatedResults: Array<TeamCollatedFullResultDto> = [];
  public danceResults: Array<DanceDetailDto> = [];

  public entryAttributeFilterData: Array<CompetitionEntryAttributeDto> = [];

  public collatedColumns = [];
  public collatedDisplayedColumns: string[] = ['teamName'];
  public collatedDataSource = new MatTableDataSource<DataTableDataElement>();
  public competitionName: string;

  constructor(
    private _activatedRoute: ActivatedRoute,
    private _conductor: CompetitionAdminConductor,
    private _tracker: CompetitionAdminTracker,

  ) { }

  ngOnInit() {

    // Start to listen for changes on the tracker
    this._subscriptions.push(this._tracker.collatedResult$.subscribe((collatedResults) => {

      this.loadOrRefreshComponent();
    }));

    this._subscriptions.push(this._tracker.dances$.subscribe((dances) => {

      this.loadOrRefreshComponent();
    }));

    // apply the settings through the conductor to the tracker from the resolver
    this._conductor.applyDanceResultsCollated(this._activatedRoute.snapshot.data.collatedDances);
    this._conductor.applyDances(this._activatedRoute.snapshot.data.dances);

    this.loadOrRefreshComponent();

    this.competitionName = this._tracker.overview.name;

  }

  ngOnDestroy() {
    this._subscriptions.map((sub) => { sub.unsubscribe(); });
    this._subscriptions = []; // For good measure
  }

  public refreshData() {
    this._conductor.initDances(true);
    this._conductor.initDanceResultsCollated(true);
  }

  // private processCollatedResults(results: Array<TeamCollatedFullResultDto>) {
  //

  //   const collatedColumns = this.processCollatedColumns(results[0].scoreGroupResults);

  //   // Apply the dynamic columns
  //   this.collatedColumns = collatedColumns;

  //   const flattenedData = this.flattenCollatedData(results, collatedColumns);

  //   this.collatedDataSource = new MatTableDataSource<any>(flattenedData);
  // }

  // private flattenCollatedData(results: TeamCollatedFullResultDto[], collatedColumns): any {

  //   let distinctEntryAttributes = [];

  //   // Iterate over the results
  //   const flattenedData = results.map((result) => {

  //     // Get the attributes for the team to build up the filter
  //     distinctEntryAttributes = this.processEntryAttributes(result.teamEntryAttributes, distinctEntryAttributes);

  //     // Produce the flatened result
  //     const flatResult = {
  //       teamName: result.teamName,
  //       counts: {
  //         danceEnteredCount: result.danceEnteredCount,
  //         danceTotalCount: result.danceTotalCount
  //       },
  //       attributes: result.teamEntryAttributes
  //     };

  //     // Pin the dynamic categories to the flattened result
  //     collatedColumns.map((col) => {
  //       flatResult[col.header] = result.scoreGroupResults[col.header].collatedScore;
  //     });

  //     // Return the flattened result
  //     return flatResult;
  //   });

  //   // Apply the filters
  //   this.entryAttributeFilterData = [{
  //     id: 0,
  //     competitionAppliesToId: 0,
  //     name: 'All',
  //     tag: ''
  //   }, ...distinctEntryAttributes];

  //   return flattenedData;
  // }

  processEntryAttributes(
    teamEntryAttributes: CompetitionEntryAttributeDto[],
    currentAttributeSet: CompetitionEntryAttributeDto[]
    ): CompetitionEntryAttributeDto[] {

    teamEntryAttributes.map((att) => {
      const index = currentAttributeSet.findIndex(x => x.id === att.id);
      if (index === -1) {
        currentAttributeSet.push(att);
      }
    });

    return currentAttributeSet;
  }

  // private processCollatedColumns(resultEntry) {
  //
  //   const keys = Object.keys(resultEntry);
  //   const allEntryAttributes: Array<CompetitionEntryAttributeDto> = [];

  //   return keys.map((key) => {
  //     return {
  //       columnDef: key, header: key, cell: (element: any) => {
  //         return `${element[key]}`;
  //       }
  //     };
  //   });
  // }

  public onAttributeFilterChange($event) {

    const filterId = $event.value.toString();
    if (filterId > 0) {
      this.collatedDataSource.filter = filterId;
    } else {
      this.collatedDataSource.filter = null;
    }
  }

  private loadOrRefreshComponent() {

    if (this._tracker.hasLoadedCollatedResults()) {
      this.collatedResults = this._tracker.collatedResults;
    }

    if (this._tracker.hasLoadedDances()) {
      this.danceResults = this._tracker.dances;
    }
  }
}
