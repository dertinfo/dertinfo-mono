import { Component, OnDestroy, OnInit } from '@angular/core';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { SubscriptionLike } from 'rxjs';

import { DodSubmissionDto } from 'app/models/dto/DodSubmissionDto';
import { AppConfirmService } from 'app/services/app-confirm/app-confirm.service';
import { ClientSettingsService } from 'app/core/services/clientsettings.service';
import { Conductor } from '../../services/dertofderts-public.conductor';
import { SessionService } from '../../services/dertofderts-public.session';
import { Tracker } from '../../services/dertofderts-public.tracker';
import { ClosedToPublicDialogComponent } from '../closedtopublicdialog/closedtopublicdialog.component';
import { RecoverSessionDialogComponent } from '../recoversessiondialog/recoversessiondialog.component';

@Component({
  selector: 'app-judging',
  templateUrl: './judging.component.html',
  styleUrls: ['./judging.component.scss']
})
export class JudgingComponent implements OnInit, OnDestroy {

  private _defaultUserName: string = 'Anonymous';
  private _subscriptions: SubscriptionLike[] = [];
  private _recoverDialogRef: MatDialogRef<RecoverSessionDialogComponent>;
  private _closedToPublicDialogRef: MatDialogRef<ClosedToPublicDialogComponent>;

  public submissions_all: DodSubmissionDto[] = [];
  public submissions_premier: DodSubmissionDto[] = [];
  public submissions_championship: DodSubmissionDto[] = [];
  public submissions_open: DodSubmissionDto[] = [];
  public currentUserName: string = this._defaultUserName;
  public sessionPresent: boolean;
  public isOfficialJudge: boolean;
  public numberOfDancesJudged: number;
  public numberOfDancesTotal: number;
  public allDancesCompleted: boolean = false;
  public resultsPublished: boolean = false;

  constructor(
    private _dialog: MatDialog,
    private _conductor: Conductor,
    private _tracker: Tracker,
    private _sessionService: SessionService,
    private _clientSettings: ClientSettingsService,
    private _router: Router
  ) { }

  ngOnInit() {

    this.performSessionCheck();

    this._subscriptions.push(this._tracker.dodSubmissions$.subscribe((submissions: DodSubmissionDto[]) => {
      this.categoriseSubmissions(submissions);
    }));

    this._conductor.initSubmissions();

    this.resultsPublished = this._clientSettings.dodResultsPublished;
  }

  ngOnDestroy() {

  }

  // public confirmClearSession() {
  //   const obs$ = this._appConfirmService.confirm(
  //     `Forget that I've ever been here!`,
  //     `Are you sure that you want to clear all stored information from this device.
  //     We will no longer track the dances that you've scored and remove any user infomation.
  //     Once cleared you will not be able to recover this data.`,
  //     'dod-dialog-panel');

  //   const subs = obs$.subscribe((confirmed: boolean) => {
  //     subs.unsubscribe();

  //     if (confirmed) { this.clearSession() };
  //   });
  // }

  public onClearSessionClick() {
    const subs = this._conductor.confirmClearSession().subscribe((cleared) => {
      if (cleared) {
        this.currentUserName = this._defaultUserName;
        this.isOfficialJudge = false;
        this.sessionPresent = false;
        this.categoriseSubmissions(this.submissions_all);

        this.performSessionCheck();
      }
    });
  }

  public onSelectRandomClick() {
    const unjudgedSubmissions = this.submissions_all.filter(s => !s.alreadyJudged);
    const randomOne = unjudgedSubmissions[Math.floor(Math.random() * unjudgedSubmissions.length)];
    this._router.navigate(['dertofderts/score', randomOne.id]);
  }

  private applyUserInfoToSubmissions(submissions: Array<DodSubmissionDto>): Array<DodSubmissionDto> {
    const judgedDances = this._sessionService.getDancesJudged();
    this.numberOfDancesJudged = judgedDances.length;
    submissions.forEach((submission: DodSubmissionDto) => {

      if (judgedDances.some(x => submission.id === x)) {
        submission.tagText = 'done';
        submission.alreadyJudged = true;
      } else {
        submission.tagText = 'to judge';
        submission.alreadyJudged = false;
      }
    });

    this.allDancesCompleted = this.numberOfDancesJudged === submissions.length;
    return submissions;
  }

  private performSessionCheck() {

    const hasLocalState = this._sessionService.hasLocalState();
    const hasSessionState = this._sessionService.hasSessionState();
    const isOpenToPublic = this._clientSettings.dodOpenToPublic;

    if (!isOpenToPublic) {

      if (!hasLocalState) {
        // The user has not submitted any results at all.
        this.openClosedToPublicDialog();
      } else {
        // At some point this user has submitted results so they must be a judge
        if (hasLocalState && !hasSessionState) { this.openRecoverStateDialog(); }
      }

    } else {

      // Show recover state
      if (hasLocalState && !hasSessionState) {
        this.openRecoverStateDialog();
      }
    }

    // Set up with recovered state
    if (hasLocalState && hasSessionState) {

      const session = this._sessionService.getSessionInfo();
      this.sessionPresent = true;
      this.currentUserName = session.name;
      this.isOfficialJudge = session.officialJudge;
    }
  }

  private openClosedToPublicDialog() {
    this._closedToPublicDialogRef = this._dialog.open(
      ClosedToPublicDialogComponent,
      {
        disableClose: true,
        width: '600px',
        panelClass: 'dod-dialog-panel'
      },
    );

    const dialogSubscription = this._closedToPublicDialogRef.afterClosed().subscribe(() => {
      dialogSubscription.unsubscribe();

      // Now that the user has recovered thier session data reapply the tags.
      const session = this._sessionService.getSessionInfo();
      if (session) {
        // Now that the user has recovered thier session data reapply the tags.
        this.categoriseSubmissions(this.submissions_all);
        this.currentUserName = session.name;
        this.sessionPresent = true;
        this.isOfficialJudge = session.officialJudge;
      }
    });
  }

  private openRecoverStateDialog() {
    const scores = {};

    this._recoverDialogRef = this._dialog.open(
      RecoverSessionDialogComponent,
      {
        width: '600px',
        panelClass: 'dod-dialog-panel'
      },
    );

    const dialogSubscription = this._recoverDialogRef.afterClosed().subscribe(() => {
      dialogSubscription.unsubscribe();

      const session = this._sessionService.getSessionInfo();
      if (session) {
        // Now that the user has recovered thier session data reapply the tags.
        this.categoriseSubmissions(this.submissions_all);
        this.currentUserName = session.name;
        this.sessionPresent = true;
        this.isOfficialJudge = session.officialJudge;
      }
    });
  }

  private categoriseSubmissions(allSubmissions: Array<DodSubmissionDto>) {
    this.submissions_all = this.applyUserInfoToSubmissions(allSubmissions);
    this.submissions_premier = this.submissions_all.filter(s => s.isPremier === true);
    this.submissions_championship = this.submissions_all.filter(s => s.isChampionship === true);
    this.submissions_open = this.submissions_all.filter(s => s.isOpen === true);
  }
}
