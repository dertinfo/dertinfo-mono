import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
import { ActivatedRoute, Router } from '@angular/router';
import { CompetitionEntryAttributeDto, DanceDetailDto, GroupTeamCompetitionEntryDto, ScoreCategoryDto, ScoreSetDto } from 'app/models/dto';
import { Observable ,  Subscription } from 'rxjs';
import { CompetitionAdminConductor } from '../../services/competition-admin.conductor';
import { CompetitionAdminTracker } from '../../services/competition-admin.tracker';

@Component({
  selector: 'app-competition-paperwork',
  templateUrl: './paperwork.component.html',
  styleUrls: ['./paperwork.component.css']
})
export class PaperworkComponent implements OnInit, OnDestroy {

  private _subscriptions: Subscription[] = [];

  constructor(
    private _tracker: CompetitionAdminTracker
  ) { }

  ngOnInit() {
  }

  ngOnDestroy() {
    this._subscriptions.map((sub) => { sub.unsubscribe(); });
    this._subscriptions = []; // For good measure
  }

  public onSparesClick() {

    const window = this.getNativeWindow();

    window.open(`./paperwork/scoresheets/${this._tracker.competitionId}/spares`, '_blank');

  }

  public onPopulatedClick() {

    const window = this.getNativeWindow();

    window.open(`./paperwork/scoresheets/${this._tracker.competitionId}/populated`, '_blank');

  }

  private getNativeWindow(): Window {
    return window;
  }
}
