import { Component, OnDestroy, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { Router } from '@angular/router';
import { SubscriptionLike } from 'rxjs';

import { DodSubmissionDto } from 'app/models/dto/DodSubmissionDto';
import { DodSubmissionSubmissionDto } from 'app/models/dto/DodSubmissionSubmissionDto';
import { Conductor } from '../../../services/dertofderts-admin.conductor';
import { Mediator } from '../../../services/dertofderts-admin.mediator';
import { Tracker } from '../../../services/dertofderts-admin.tracker';
import { DodSubmissionCreateComponent } from './dod-submission-create/dod-submission-create.component';

@Component({
  selector: 'app-dod-admin-submissions',
  templateUrl: './submissions.component.html',
  styleUrls: ['./submissions.component.scss']
})
export class SubmissionsComponent implements OnInit, OnDestroy {

  private _subscriptions: SubscriptionLike[] = [];

  public editingBlocked = false;
  public dataSource = new MatTableDataSource<DodSubmissionDto>();
  public displayedColumns: string[] = ['groupName', 'embedOrigin', 'dertYearFrom', 'category', 'viewCards', 'delete'];

  constructor(
    private _tracker: Tracker,
    private _conductor: Conductor,
    private _mediator: Mediator,
    public composeDialog: MatDialog,
    public router: Router
  ) { }

  ngOnInit() {

    this._subscriptions.push(this._mediator.newSubmission$.subscribe((newSubmission: DodSubmissionSubmissionDto) => {
      this._conductor.addSubmission(newSubmission);
      this.composeDialog.closeAll();
    }));

    this._subscriptions.push(this._mediator.closeModal$.subscribe((newSubmission: DodSubmissionSubmissionDto) => {
      this.composeDialog.closeAll();
    }));

    this._subscriptions.push(this._tracker.dodSubmissions$.subscribe((submissions: DodSubmissionDto[]) => {
      this.dataSource.data = submissions;
    }));

    this._conductor.initSubmissions();
  }

  ngOnDestroy() {
    this._subscriptions.map((sub) => { sub.unsubscribe(); });
    this._subscriptions = [];
  }

  deleteSubmission(dodSubmission: DodSubmissionDto) {
    this._conductor.deleteSubmission(dodSubmission);
  }

  openAddSubmissionDialog() {
    const dialogRef = this.composeDialog.open(DodSubmissionCreateComponent);
    const dialogueSubscription = dialogRef.afterClosed().subscribe(result => {
      dialogueSubscription.unsubscribe();
    });
  }

  public onViewCardsClick(submission: DodSubmissionDto) {
    this.router.navigate([`/dodadmin/scorecards/s/${submission.id}`]);
  }

  public getEditRouteLink(submission: DodSubmissionDto) {
    return ['./' + submission.id];
  }
}
