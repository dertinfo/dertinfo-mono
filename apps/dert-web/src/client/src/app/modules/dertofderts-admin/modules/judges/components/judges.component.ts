import { Component, OnInit } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { Observable, SubscriptionLike } from 'rxjs';

import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { DodJudgeInfoDto } from 'app/models/dto/DodJudgeInfoDto';
import { Conductor } from '../../../services/dertofderts-admin.conductor';
import { Tracker } from '../../../services/dertofderts-admin.tracker';

@Component({
  selector: 'app-dod-admin-judges',
  templateUrl: './judges.component.html',
  styleUrls: ['./judges.component.scss']
})
export class JudgesComponent implements OnInit {

  private _subscriptions: SubscriptionLike[] = [];
  private _snackBarDuration: number = 1200;

  public dataSourceOfficial = new MatTableDataSource<DodJudgeInfoDto>();
  public dataSourcePublic = new MatTableDataSource<DodJudgeInfoDto>();
  public displayedColumnsOfficial: string[] = ['judgeName', 'judgeEmail', 'stillToJudge', 'viewCards', 'toggleInclusion'];
  public displayedColumnsPublic: string[] = ['judgeName', 'judgeEmail', 'stillToJudge', 'interestedInJudging', 'viewCards', 'toggleInclusion'];

  constructor(
    private _tracker: Tracker,
    private _conductor: Conductor,
    public snackBar: MatSnackBar,
    public router: Router
  ) { }

  public ngOnInit() {

    this._subscriptions.push(this._tracker.dodJudgeInfos$.subscribe((judgeInfos: DodJudgeInfoDto[]) => {
      this.dataSourceOfficial.data = judgeInfos.filter(j => j.isOfficial);
      this.dataSourcePublic.data = judgeInfos.filter(j => !j.isOfficial);
    }));

    this._conductor.initJudges();
  }

  public toggleJudgeInclusion(judgeInfo: DodJudgeInfoDto) {

    if (judgeInfo.resultsBlocked) {
      const subs = this._conductor.unblockJudge(judgeInfo.dodUserId).subscribe(() => {
        subs.unsubscribe();
        this.snackBar.open('Judge Results Included', 'close', { duration: this._snackBarDuration });
      });
    }

    if (!judgeInfo.resultsBlocked) {
      const subs = this._conductor.blockJudge(judgeInfo.dodUserId).subscribe(() => {
        subs.unsubscribe();
        this.snackBar.open('Judge Results Removed', 'close', { duration: this._snackBarDuration });
      });
    }
  }

  public onViewCardsClick(judgeInfo: DodJudgeInfoDto) {
    this.router.navigate([`/dodadmin/scorecards/j/${judgeInfo.dodUserId}`]);
  }

}
