import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
import { ActivatedRoute } from '@angular/router';
import { ScoreCategoryDto, ScoreSetDto } from 'app/models/dto';
import { Observable ,  Subscription } from 'rxjs';
import { CompetitionAdminConductor } from '../../services/competition-admin.conductor';
import { CompetitionAdminTracker } from '../../services/competition-admin.tracker';

import { ScoreCategoryEditComponent } from './dialogs/scorecategory-edit/scorecategory-edit.component';
import { ScoreSetEditComponent } from './dialogs/scoreset-edit/scoreset-edit.component';

@Component({
  selector: 'app-competition-scoring',
  templateUrl: './scoring.component.html',
  styleUrls: ['./scoring.component.css']
})
export class ScoringComponent implements OnInit, OnDestroy {

  private _subscriptions: Subscription[] = [];

  displayedColumnsScoreSets: string[] = ['name', 'categoryTags', 'edit'];
  dataSourceScoreSets = new MatTableDataSource<ScoreSetDto>();
  displayedColumnsScoreCategories: string[] = ['name', 'maxmarks', 'tag', 'sortOrder', 'edit'];
  dataSourceScoreCategories = new MatTableDataSource<ScoreCategoryDto>();

  public columnsToDisplay = ['name', 'description'];

  constructor(
    public composeDialog: MatDialog,
    private _activatedRoute: ActivatedRoute,
    private _conductor: CompetitionAdminConductor,
    private _tracker: CompetitionAdminTracker,

  ) { }

  ngOnInit() {

    // Start to listen for changes on the tracker
    this._subscriptions.push(this._tracker.scoreSets$.subscribe((scoreSets) => {
      // Continue here after adding the scores to the tracker
    }));

    this._subscriptions.push(this._tracker.scoreCategories$.subscribe((scoreCategories) => {
      // Continue here after adding the scores to the tracker
    }));

    // apply the settings through the conductor to the tracker from the resolver
    this._conductor.applyScoreSets(this._activatedRoute.snapshot.data.scoring.scoreSets);
    this._conductor.applyScoreCategories(this._activatedRoute.snapshot.data.scoring.scoreCategories);

    this.dataSourceScoreSets = new MatTableDataSource<ScoreSetDto>(this._tracker.scoreSets);
    this.dataSourceScoreCategories = new MatTableDataSource<ScoreCategoryDto>(this._tracker.scoreCategories);

  }

  ngOnDestroy() {
    this._subscriptions.map((sub) => { sub.unsubscribe(); });
    this._subscriptions = []; // For good measure
  }

  public editScoreSet(scoreSet: ScoreSetDto) {

    const editScoreSetRef = this.composeDialog.open(ScoreSetEditComponent, { data: { scoreSet: scoreSet } });

    // This approach has limited lifespan on updating of angular material. It is implemented different in later versions
    editScoreSetRef.componentInstance.scoreSet = scoreSet;

    const dialogueSubscription = editScoreSetRef.afterClosed().subscribe(result => {
      dialogueSubscription.unsubscribe();
    });
  }

  public editScoreCategory(scoreCategory: ScoreCategoryDto) {

    const editScoreCategoryRef = this.composeDialog.open(ScoreCategoryEditComponent, { data: { scoreCategory: scoreCategory } });

    // This approach has limited lifespan on updating of angular material. It is implemented different in later versions
    editScoreCategoryRef.componentInstance.scoreCategory = scoreCategory;

    const dialogueSubscription = editScoreCategoryRef.afterClosed().subscribe(result => {
      dialogueSubscription.unsubscribe();
    });
  }

}
