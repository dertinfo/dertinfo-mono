import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { MatTabChangeEvent } from '@angular/material/tabs';
import { CompetitionLookupCompetitionDto, CompetitionLookupDto, CompetitionResultDto } from 'app/models/dto';
import { Subscription } from 'rxjs';
import { PublicContentConductor } from '../services/public-content.conductor';
import { PublicContentTracker } from '../services/public-content.tracker';

@Component({
  selector: 'app-public-results',
  templateUrl: './public-results.component.html',
  styleUrls: ['./public-results.component.css']
})
export class PublicResultsComponent implements OnInit, OnDestroy {

  private _subscriptions: Subscription[] = [];

  public eventLookupsReady = false;
  public eventLookups;

  public selectedCompetition: CompetitionLookupCompetitionDto = null;
  public selectedTab = 'all';

  public results: { [key: string]: CompetitionResultDto };

  constructor(

    private _publicContentConductor: PublicContentConductor,
    private _publicContentTracker: PublicContentTracker
  ) { }

  ngOnInit() {
    const subsLookup = this._publicContentTracker.resultLookup$.subscribe((resultsLookup: CompetitionLookupDto) => {

      this.eventLookups = resultsLookup;
      this.eventLookupsReady = true;
    });

    const subsCachedResults = this._publicContentTracker.cachedResults$.subscribe((cachedResults) => {

      if (cachedResults && this.selectedCompetition) {
        const key = `${this.selectedCompetition.competitionId}_${this.selectedTab}`;
        this.results[key] = cachedResults[key];
      }
    });

    this._subscriptions.push(subsLookup);
    this._subscriptions.push(subsCachedResults);

    this._publicContentConductor.initResultLookup();
  }

  ngOnDestroy() {
    this._subscriptions.map((sub) => { sub.unsubscribe(); });
    this._subscriptions = []; // For good measure
  }

  getTabResults(resultType) {
    //

    const competitionId = this.selectedCompetition.competitionId;
    const key = `${competitionId}_${resultType.resultTypeKey}`;

    if (this.results[key]) {
      return this.results[key].teamCollatedResults;
    } else {
      return [];
    }
  }

  public onSelectionChange(competition: CompetitionLookupCompetitionDto) {
    if (competition != null) {

      this.selectedCompetition = competition;
      this.results = {};
      this.loadCompetitionResults(competition, 'all');
    } else {
      this.selectedCompetition = null;
    }
  }

  public onSelectedTabChange(event: MatTabChangeEvent) {

    const selectedTabType = this.selectedCompetition.resultTypes.find(rt => rt.resultTypeName === event.tab.textLabel);

    this.selectedTab = selectedTabType.resultTypeKey;
    this._publicContentConductor.initResults(this.selectedCompetition.competitionId, selectedTabType.resultTypeKey);
  }

  private loadCompetitionResults(competition: CompetitionLookupCompetitionDto, resultType: string) {

    this._publicContentConductor.initResults(competition.competitionId, resultType);
  }

}
