import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { DanceDetailDto, DanceMarkingSheetDto, GroupDto, GroupRegistrationDto, GroupTeamDto, TeamAttendanceDto } from 'app/models/dto';
import { Observable, Subscription } from 'rxjs';
import { GroupViewConductor } from '../../services/group-view.conductor';

import { MatDialog } from '@angular/material/dialog';
import { MatTabChangeEvent, MatTabGroup } from '@angular/material/tabs';
import { GroupAccessContext } from 'app/models/app/Enumerations/GroupAccessContext';
import { GroupViewTracker } from '../../services/group-view.tracker';
import { MarkingSheetZoomComponent } from './dialogs/marking-sheet-zoom/marking-sheet-zoom.component';

@Component({
  selector: 'app-groupview-registration',
  templateUrl: './registration.component.html',
  styleUrls: ['./registration.component.css']
})
export class GroupViewRegistrationComponent implements OnInit, OnDestroy {

  private _subscriptions: Subscription[] = [];

  public isLoadingData: boolean = false;
  public registration: GroupRegistrationDto;
  public danceResults: Array<DanceDetailDto>;

  public teamAttendanceFilter: TeamAttendanceDto;
  public teamAttendanceFilterData: TeamAttendanceDto[];

  @ViewChild('activityTabGroup', { static: true }) tabGroup: MatTabGroup;

  constructor(

    private _activatedRoute: ActivatedRoute,
    private _router: Router,
    private _conductor: GroupViewConductor,
    private _tracker: GroupViewTracker,
    public composeDialog: MatDialog,
  ) { }

  ngOnInit() {

    this._subscriptions.push(this._activatedRoute.params.subscribe(params => {

      const registrationIdStr: string = this._activatedRoute.snapshot.params['registrationId'];
      const registrationId: number = parseInt(registrationIdStr, 10);

      this._conductor.initFocussedRegistration(registrationId);

      // apply the settings through the conductor to the tracker from the resolver
      this._conductor.applyTeamAttendances(registrationId, this._activatedRoute.snapshot.data.teamAttendance);
    }));

    this._subscriptions.push(this._tracker.focussedRegistration$.subscribe((focussedRegistration: GroupRegistrationDto) => {

      this.registration = focussedRegistration;

      if (this.tabGroup) {
        this.tabGroup.selectedIndex = 0;
      }
    }));

    this._subscriptions.push(this._tracker.focussedTeamAttendances$.subscribe((teamAttendances: TeamAttendanceDto[]) => {

      this.teamAttendanceFilterData = teamAttendances;

      if (teamAttendances.length > 0) {
        this.teamAttendanceFilter = teamAttendances[0];

        if (this.teamAttendanceFilter.attendanceActivities && this.teamAttendanceFilter.attendanceActivities.length > 0) {
          const teamId = this.teamAttendanceFilter.teamId;
          const activityId = this.teamAttendanceFilter.attendanceActivities[0].activityId;
          this.loadResultsData(teamId, activityId);
        }
      }
    }));

    this._subscriptions.push(this._tracker.focussedResults$.subscribe((danceResults: Array<DanceDetailDto>) => {

      this.isLoadingData = false;
      this.danceResults = danceResults;
    }));

  }

  ngOnDestroy() {

    this._subscriptions.map((sub) => { sub.unsubscribe(); });
    this._subscriptions = []; // For good measure
  }

  public onTeamFilterChange() {

    if (this.teamAttendanceFilter !== null && this.teamAttendanceFilter.attendanceActivities.length > 0) {
      this.loadResultsData(this.teamAttendanceFilter.teamId, this.teamAttendanceFilter.attendanceActivities[0].activityId);
    }
  }

  public onSelectedTabChange(event: MatTabChangeEvent) {

    // todo - (fragile) if a team creates 2 attendance types with the same name then this will fail
    const selectedActivity = this.teamAttendanceFilter.attendanceActivities.find(aa => aa.activityTitle === event.tab.textLabel);

    if (null !== selectedActivity && undefined !== selectedActivity) {
      this.loadResultsData(this.teamAttendanceFilter.teamId, selectedActivity.activityId);
    }

  }

  public onMarkingSheetClick(markingSheet: DanceMarkingSheetDto) {

    this.openScoreSheetZoomDialog(markingSheet.imageResourceUri);
  }

  private loadResultsData(teamId: number, activityId: number) {

    this.isLoadingData = true;
    this._conductor.initResults(teamId, activityId);
  }

  private openScoreSheetZoomDialog(markingSheetUrl: string) {
    const groupDialogRef = this.composeDialog.open(MarkingSheetZoomComponent, {
      data: { markingSheetUrl },
    });
    const dialogueSubscription = groupDialogRef.afterClosed().subscribe(result => {
      dialogueSubscription.unsubscribe();
    });
  }
}
