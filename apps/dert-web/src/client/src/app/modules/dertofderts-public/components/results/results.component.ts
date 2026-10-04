import { Component, OnDestroy, OnInit, ViewEncapsulation } from '@angular/core';
import { MatTabChangeEvent } from '@angular/material/tabs';
import { CompetitionResultDto } from 'app/models/dto';
import { DodTeamCollatedResultPairDto } from 'app/models/dto/DodCollatedResultPairDto';
import { DodTeamCollatedResultDto } from 'app/models/dto/DodTeamCollatedResultDto';
import { ClientSettingsService } from 'app/core/services/clientsettings.service';
import { Subscription } from 'rxjs';
import { Conductor } from '../../services/dertofderts-public.conductor';
import { Tracker } from '../../services/dertofderts-public.tracker';

@Component({
  selector: 'app-results',
  templateUrl: './results.component.html',
  styleUrls: ['./results.component.scss']
})
export class ResultsComponent implements OnInit, OnDestroy {

  private _selectedOfficialTab = 'all';
  private _selectedPublicTab = 'all';
  private _rawResults: DodTeamCollatedResultPairDto = null;
  private _subscriptions: Array<Subscription> = [];

  public resultsPublished: boolean = false;

  public officalResults = [];
  public publicResults = [];

  public showOfficialAveragesChecked = false;
  public officialJudgeDiscrepency = false;

  public resultTypes = [
    {
      resultTypeKey: 'all',
      resultTypeName: 'Steve Marris'
    },
    {
      resultTypeKey: 'main',
      resultTypeName: 'Main'
    },
    {
      resultTypeKey: 'buzz',
      resultTypeName: 'Buzz'
    },
    {
      resultTypeKey: 'characters',
      resultTypeName: 'Characters'
    },
    {
      resultTypeKey: 'music',
      resultTypeName: 'Music'
    }
  ];

  constructor(
    private _conductor: Conductor,
    private _tracker: Tracker,
    private _clientSettings: ClientSettingsService,
  ) { }

  ngOnInit() {
    this._subscriptions.push(this._tracker.dodResults$.subscribe((results: DodTeamCollatedResultPairDto) => {
      if (results != null) {

        this.reset();

        this._rawResults = results;
        this.officalResults = this.processResults(
          this._rawResults.collatedOfficialResults,
          this._selectedOfficialTab,
          this.showOfficialAveragesChecked
        );
        this.publicResults = this.processResults(
          this._rawResults.collatedPublicResults,
          this._selectedPublicTab,
          true
        );
      }
    }));

    if (this._clientSettings.dodResultsPublished) {
      this._conductor.initResults();
      this.resultsPublished = true;
    }
  }

  ngOnDestroy(): void {
    this._subscriptions.map((sub) => { sub.unsubscribe(); });
    this._subscriptions = [];
  }

  public onShowAveragesChange() {
    this.officalResults = this.processResults(
      this._rawResults.collatedOfficialResults,
      this._selectedOfficialTab,
      this.showOfficialAveragesChecked
    );
  }

  public onOfficialSelectedTabChange(event: MatTabChangeEvent) {
    const selectedTabType = this.resultTypes.find(rt => rt.resultTypeName === event.tab.textLabel);

    this._selectedOfficialTab = selectedTabType.resultTypeKey;
    this.officalResults = this.processResults(
      this._rawResults.collatedOfficialResults,
      this._selectedOfficialTab,
      this.showOfficialAveragesChecked
    );
  }

  public onPublicSelectedTabChange(event: MatTabChangeEvent) {
    const selectedTabType = this.resultTypes.find(rt => rt.resultTypeName === event.tab.textLabel);

    this._selectedPublicTab = selectedTabType.resultTypeKey;
    this.publicResults = this.processResults(this._rawResults.collatedPublicResults, this._selectedPublicTab, true);
  }

  private processResults(results: Array<DodTeamCollatedResultDto>, resultCategory: string, doAverage: boolean) {
    // Split and process the results so that we can display the collated results from the official judges.

    let selectCollatedFunction = (x) => x.steveMarrisCollatedScore;
    let selectAverageFunction = (x) => x.steveMarrisCollatedScore / x.numberOfResults;

    switch (resultCategory) {
      case 'all': selectCollatedFunction = (a) => a.steveMarrisCollatedScore; break;
      case 'main': selectCollatedFunction = (a) => a.mainCollatedScore; break;
      case 'buzz': selectCollatedFunction = (a) => a.buzzCollatedScore; break;
      case 'characters': selectCollatedFunction = (a) => a.charactersCollatedScore; break;
      case 'music': selectCollatedFunction = (a) => a.musicCollatedScore; break;
    }

    switch (resultCategory) {
      case 'all': selectAverageFunction = (a) => a.numberOfResults > 0 ? a.steveMarrisCollatedScore / a.numberOfResults : 0; break;
      case 'main': selectAverageFunction = (a) => a.numberOfResults > 0 ? a.mainCollatedScore / a.numberOfResults : 0; break;
      case 'buzz': selectAverageFunction = (a) => a.numberOfResults > 0 ? a.buzzCollatedScore / a.numberOfResults : 0; break;
      case 'characters': selectAverageFunction = (a) => a.numberOfResults > 0 ? a.charactersCollatedScore / a.numberOfResults : 0; break;
      case 'music': selectAverageFunction = (a) => a.numberOfResults > 0 ? a.musicCollatedScore / a.numberOfResults : 0; break;
    }

    const unorderedResults = results.map((resultEntry: DodTeamCollatedResultDto) => {
      return {
        teamName: resultEntry.teamName,
        numberOfResults: resultEntry.numberOfResults,
        score: doAverage ? selectAverageFunction(resultEntry).toFixed(2) : selectCollatedFunction(resultEntry),
        entryAttribute: resultEntry.entryAttribute
      };
    });

    const sortFunction = (a, b) => b.score - a.score;
    return unorderedResults.sort(sortFunction);
  }

  private reset() {
    this.officalResults = [];
    this.publicResults = [];
    this.officialJudgeDiscrepency = false;
    this.showOfficialAveragesChecked = false;
  }

}
