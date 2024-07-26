import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
import { ActivatedRoute } from '@angular/router';
import { VenueAllocationDto } from 'app/models/dto';
import { Observable ,  Subscription } from 'rxjs';
import { CompetitionAdminConductor } from '../../services/competition-admin.conductor';
import { CompetitionAdminTracker } from '../../services/competition-admin.tracker';

import { VenueCreateComponent } from './dialogs/venue-create/venue-create.component';
import { VenueEditComponent } from './dialogs/venue-edit/venue-edit.component';

@Component({
  selector: 'app-competition-venues',
  templateUrl: './venues.component.html',
  styleUrls: ['./venues.component.css']
})
export class VenuesComponent implements OnInit, OnDestroy {

  private _subscriptions: Subscription[] = [];

  public displayedColumns: string[] = ['name', 'judgesAllocated', 'edit'];
  public dataSource;

  public get editingBlocked() {
    return this._tracker.overview.summary.resultsPublished;
  }

  constructor(
    public composeDialog: MatDialog,
    private _activatedRoute: ActivatedRoute,
    private _conductor: CompetitionAdminConductor,
    private _tracker: CompetitionAdminTracker
  ) { }

  ngOnInit() {

    // Start to listen for changes on the tracker
    this._subscriptions.push(this._tracker.venues$.subscribe((venues) => {

      this.dataSource = new MatTableDataSource<VenueAllocationDto>(venues);
    }));

    // apply the settings through the conductor to the tracker from the resolver
    this._conductor.applyVenues(this._activatedRoute.snapshot.data.venues);
    this.dataSource = new MatTableDataSource<VenueAllocationDto>(this._tracker.venues);
  }

  ngOnDestroy() {
    this._subscriptions.map((sub) => { sub.unsubscribe(); });
    this._subscriptions = []; // For good measure
  }

  public editVenue(venue: VenueAllocationDto) {

    if (this._tracker.hasLoadedJudges()) {
        this.LaunchEditDialog(venue);
    } else {
      const subs = this._conductor.initJudges(this._tracker.competitionId).subscribe((judges) => {
        subs.unsubscribe();
        this.LaunchEditDialog(venue);
      });
    }
  }

  public openCreateVenueDialog() {

    const dialogRef = this.composeDialog.open(VenueCreateComponent);
    const dialogueSubscription = dialogRef.afterClosed().subscribe(result => {
      dialogueSubscription.unsubscribe();
    });
  }

  private LaunchEditDialog(venue: VenueAllocationDto) {
    const editDialogRef = this.composeDialog.open(VenueEditComponent, { data: { venue: venue } });

    // This approach has limited lifespan on updating of angular material. It is implemented different in later versions
    editDialogRef.componentInstance.venue = venue;
    editDialogRef.componentInstance.allJudges = this._tracker.judges;

    const dialogueSubscription = editDialogRef.afterClosed().subscribe(result => {
      dialogueSubscription.unsubscribe();
    });
  }

}
