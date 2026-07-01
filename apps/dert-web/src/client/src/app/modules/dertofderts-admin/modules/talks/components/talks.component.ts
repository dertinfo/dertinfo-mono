import { Component, OnDestroy, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { SubscriptionLike } from 'rxjs';

import { DodTalkDto } from 'app/models/dto/DodTalkDto';
import { DodTalkSubmissionDto } from 'app/models/dto/DodTalkSubmissionDto';
import { Conductor } from '../../../services/dertofderts-admin.conductor';
import { Mediator } from '../../../services/dertofderts-admin.mediator';
import { Tracker } from '../../../services/dertofderts-admin.tracker';
import { CreateTalkComponent } from './create-talk/create-talk.component';

@Component({
  selector: 'app-dod-admin-talks',
  templateUrl: './talks.component.html',
  styleUrls: ['./talks.component.scss']
})
export class TalksComponent implements OnInit, OnDestroy {

  private _subscriptions: SubscriptionLike[] = [];

  public editingBlocked = false;
  public dataSource = new MatTableDataSource<DodTalkDto>();
  public displayedColumns: string[] = ['title', 'broadcastDateTime', 'buttons'];

  constructor(
    private _tracker: Tracker,
    private _conductor: Conductor,
    private _mediator: Mediator,
    public _dialog: MatDialog
  ) { }

  ngOnInit() {

    this._subscriptions.push(this._mediator.newTalk$.subscribe((newTalk: DodTalkSubmissionDto) => {
      this._conductor.addTalk(newTalk);
      this._dialog.closeAll();
    }));

    this._subscriptions.push(this._mediator.closeModal$.subscribe(() => {
      this._dialog.closeAll();
    }));

    this._subscriptions.push(this._tracker.dodTalks$.subscribe((talks: DodTalkDto[]) => {
      this.dataSource.data = talks;
    }));

    this._conductor.initTalks();
  }

  ngOnDestroy() {
    this._subscriptions.map((sub) => { sub.unsubscribe(); });
    this._subscriptions = [];
  }

  openCreateTalkDialog() {
    const dialogRef = this._dialog.open(CreateTalkComponent);
    const dialogueSubscription = dialogRef.afterClosed().subscribe(result => {
      dialogueSubscription.unsubscribe();
    });
  }

  onDeleteTalkClick(talk: DodTalkDto) {
    this._conductor.deleteTalk(talk);
  }

  public getEditRouteLink(talk: DodTalkDto) {
    return ['./' + talk.id];
  }
}
