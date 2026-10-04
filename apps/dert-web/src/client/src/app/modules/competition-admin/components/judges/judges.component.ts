import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';

import { JudgeDto } from 'app/models/dto';
import { CompetitionAdminConductor } from '../../services/competition-admin.conductor';
import { CompetitionAdminTracker } from '../../services/competition-admin.tracker';
import { JudgeCreateComponent } from './dialogs/judge-create/judge-create.component';
import { JudgeEditComponent } from './dialogs/judge-edit/judge-edit.component';

@Component({
  selector: 'app-competition-judges',
  templateUrl: './judges.component.html',
  styleUrls: ['./judges.component.css']
})
export class JudgesComponent implements OnInit, OnDestroy {

  private _subscriptions: Subscription[] = [];

  public displayedColumns: string[] = ['name', 'edit'];
  public dataSourceJudges;

  public get editingBlocked() {
    return this._tracker.overview.summary.resultsPublished;
  }

  constructor(
    public composeDialog: MatDialog,
    private _activatedRoute: ActivatedRoute,
    private _conductor: CompetitionAdminConductor,
    private _tracker: CompetitionAdminTracker,

  ) { }

  ngOnInit() {

    // Start to listen for changes on the tracker
    this._subscriptions.push(this._tracker.judges$.subscribe((judges) => {

      this.dataSourceJudges = new MatTableDataSource<JudgeDto>(judges);
    }));

    // apply the settings through the conductor to the tracker from the resolver
    this._conductor.applyJudges(this._activatedRoute.snapshot.data.judges);
    this.dataSourceJudges = new MatTableDataSource<JudgeDto>(this._tracker.judges);
  }

  ngOnDestroy() {
    this._subscriptions.map((sub) => { sub.unsubscribe(); });
    this._subscriptions = []; // For good measure
  }

  public openCreateJudgeDialog() {

    const dialogRef = this.composeDialog.open(JudgeCreateComponent);
    const dialogueSubscription = dialogRef.afterClosed().subscribe(result => {
      dialogueSubscription.unsubscribe();
    });
  }

  public editJudge(judge: JudgeDto) {

    const editDialogRef = this.composeDialog.open(JudgeEditComponent, { data: { judge: judge } });

    // This approach has limited lifespan on updating of angular material. It is implemented different in later versions
    editDialogRef.componentInstance.judge = judge;

    const dialogueSubscription = editDialogRef.afterClosed().subscribe(result => {
      dialogueSubscription.unsubscribe();
    });
  }
}
