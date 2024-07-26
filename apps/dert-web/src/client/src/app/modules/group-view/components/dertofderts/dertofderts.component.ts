import { Component, OnDestroy, OnInit } from '@angular/core';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { ScoreCard } from 'app/models/app/ScoreCard';
import { DodGroupResultsDto } from 'app/models/dto/DodGroupResultsDto';
import { DodGroupResultsScoreCardDto } from 'app/models/dto/DodGroupResultsScoreCardDto';
import { ClientSettingsService } from 'app/core/services/clientsettings.service';
import { SubscriptionLike } from 'rxjs';
import { GroupViewConductor } from '../../services/group-view.conductor';
import { GroupViewTracker } from '../../services/group-view.tracker';
import { DertOfDertsReportDialogComponent } from '../dertofderts-reportdialog/dertofderts-reportdialog.component';

@Component({
  selector: 'app-dertofderts',
  templateUrl: './dertofderts.component.html',
  styleUrls: ['./dertofderts.component.scss']
})
export class DertOfDertsComponent implements OnInit, OnDestroy {

  private _subscriptions: SubscriptionLike[] = [];
  private _reportDialogRef: MatDialogRef<DertOfDertsReportDialogComponent>;
  private _updateOnly = false;

  public embedOrigin: string = 'youtube';
  public embedLink: string = '';
  public scoreCards: Array<DodGroupResultsScoreCardDto> = [];
  public displayedScoreCards: Array<ScoreCard> = [];
  public resultsPublished: boolean = false;
  public publicResultsForwarded: boolean = false;
  public officialResultsForwarded: boolean = false;
  public showPublicOnly = false;
  public showVideo = false;

  public get showResults() {

    return this.officialResultsForwarded || this.publicResultsForwarded || this.resultsPublished;
  }

  public get showSwitcher() {

    return this.officialResultsForwarded && this.publicResultsForwarded;
  }

  public get showAwaitingResults() {

    return this.scoreCards.length === 0;
  }

  constructor(
    private _dialog: MatDialog,
    private _conductor: GroupViewConductor,
    private _tracker: GroupViewTracker,
    private _clientSettingsService: ClientSettingsService
  ) { }

  ngOnInit() {

    this.resultsPublished = this._clientSettingsService.dodResultsPublished;
    this.publicResultsForwarded = this._clientSettingsService.dodPublicResultsForwarded;
    this.officialResultsForwarded = this._clientSettingsService.dodOfficialResultsForwarded;

    this.subscribeToInformation();
    this._conductor.initDodResults(this._tracker.groupId);
    this.showPublicOnly = !this.officialResultsForwarded;
  }

  ngOnDestroy() {
    this._subscriptions.map((sub) => { sub.unsubscribe(); });
    this._subscriptions = []; // For good measure
  }

  public onTogglePublicOfficial() {
    this.displayedScoreCards = this.MapAndFilterScoreCards(this.scoreCards);
  }

  public onRefreshCards() {
    this._conductor.initDodResults(this._tracker.groupId, true);
    this.showPublicOnly = !this.officialResultsForwarded;
    this._updateOnly = true;
  }

  /** Score cards are reported by the user if there are offensive comments or distictly unfair results.*/
  public onScoreCardReported($event: ScoreCard) {

    const dialogData = {
      scoreCard: $event,
      groupId: this._tracker.groupId
    };

    this._reportDialogRef = this._dialog.open(
      DertOfDertsReportDialogComponent,
      {
        width: '600px',
        data: dialogData,
        panelClass: 'dod-dialog-panel'
      },
    );
  }

  private subscribeToInformation() {
    this._subscriptions.push(this._tracker.dodResults$.subscribe((dodGroupResults: DodGroupResultsDto) => {
      if (dodGroupResults != null) {

        if (!this._updateOnly) {
          this.embedOrigin = dodGroupResults.embedOrigin;
          this.embedLink = dodGroupResults.embedLink;
          this.showVideo = true;
        }

        this.scoreCards = dodGroupResults.scoreCards;
        this.displayedScoreCards = this.MapAndFilterScoreCards(this.scoreCards);
      }
    }));
  }

  private MapAndFilterScoreCards(scoreCards: Array<DodGroupResultsScoreCardDto>): Array<ScoreCard> {

    return scoreCards.map(sc => {
      return {
        dodResultId: sc.dodResultId,
        dodSubmissionId: sc.dodSubmissionId,
        scoreCategories: sc.scoreCategories,
        comments: sc.comments,
        isOfficial: sc.isOfficial
      };
    }).filter(sc => sc.isOfficial !== this.showPublicOnly);
  }
}
