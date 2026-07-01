import { Component, OnDestroy, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { SubscriptionLike } from 'rxjs';

import { DodSubmissionDto } from 'app/models/dto/DodSubmissionDto';
import { DodSubmissionSubmissionDto } from 'app/models/dto/DodSubmissionSubmissionDto';
import { Conductor } from '../../services/dertofderts-admin.conductor';
import { Mediator } from '../../services/dertofderts-admin.mediator';
import { Tracker } from '../../services/dertofderts-admin.tracker';

@Component({
  selector: 'app-dod-admin-home',
  templateUrl: './admin-home.component.html',
  styleUrls: ['./admin-home.component.scss']
})
export class AdminHomeComponent implements OnInit, OnDestroy {

  private _subscriptions: SubscriptionLike[] = [];

  constructor(
    private _tracker: Tracker,
    private _conductor: Conductor,
    private _mediator: Mediator,
    public composeDialog: MatDialog,
  ) { }

  ngOnInit() {
  }

  ngOnDestroy() {
    this._subscriptions.map((sub) => { sub.unsubscribe(); });
    this._subscriptions = [];
  }

}
