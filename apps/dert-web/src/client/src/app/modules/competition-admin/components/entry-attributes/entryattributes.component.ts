import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
import { ActivatedRoute } from '@angular/router';
import { CompetitionEntryAttributeDto, GroupTeamCompetitionEntryDto, ScoreCategoryDto, ScoreSetDto } from 'app/models/dto';
import { Observable ,  Subscription } from 'rxjs';
import { CompetitionAdminConductor } from '../../services/competition-admin.conductor';
import { CompetitionAdminTracker } from '../../services/competition-admin.tracker';

@Component({
  selector: 'app-competition-entryattributes',
  templateUrl: './entryattributes.component.html',
  styleUrls: ['./entryattributes.component.css']
})
export class EntryAttributesComponent implements OnInit, OnDestroy {

  private _subscriptions: Subscription[] = [];

  public displayedColumns: string[] = ['name', 'tag'];
  public dataSource = new MatTableDataSource<CompetitionEntryAttributeDto>();

  constructor(
    private _activatedRoute: ActivatedRoute,
    private _conductor: CompetitionAdminConductor,
    private _tracker: CompetitionAdminTracker,

  ) { }

  ngOnInit() {

    // Start to listen for changes on the tracker
    this._subscriptions.push(this._tracker.entryAttributes$.subscribe((overview) => {

    }));

    // apply the settings through the conductor to the tracker from the resolver
    this._conductor.applyEntryAttributes(this._activatedRoute.snapshot.data.entryAttributes);

    this.dataSource = new MatTableDataSource<CompetitionEntryAttributeDto>(this._tracker.entryAttributes);

  }

  ngOnDestroy() {
    this._subscriptions.map((sub) => { sub.unsubscribe(); });
    this._subscriptions = []; // For good measure
  }
}
