import { Component, OnDestroy, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';
import { ActivityTeamAttendanceDto, GroupMemberDto, TeamAttendanceDto } from '../../../../models/dto';
import { GroupMembersSelectComponent } from '../group-members-select/group-members-select.component';
import { RegistrationByGroupConductor } from '../../services/registration-by-group.conductor';
import { RegistrationByGroupTracker } from '../../services/registration-by-group.tracker';
import { GroupTeamsSelectComponent } from '../group-teams-select/group-teams-select.component';
import { TeamActivitiesSelectComponent } from '../team-activities-select/team-activities-select.component';

@Component({
  selector: 'app-group-registration-teams',
  templateUrl: './group-registration-teams.component.html',
  styleUrls: ['./group-registration-teams.component.css']
})
export class GroupRegistrationTeamsComponent implements OnInit, OnDestroy {

  private _subscriptions: Subscription[] = [];
  private routeRegistrationId: number;

  public attendingTeams$;

  public get capabilities() {
    return this._tracker.capabilities;
  }

  constructor(
    public composeDialog: MatDialog,
    private activatedRoute: ActivatedRoute,
    private _conductor: RegistrationByGroupConductor,
    private _tracker: RegistrationByGroupTracker
  ) {}

  ngOnInit() {

    this.attendingTeams$ = this._tracker.teamAttendances$; // Subscribe Direct uses | async

    const routeParamsSubscription = this.activatedRoute.parent.params.subscribe(params => {
      this._conductor.initAttendingTeams();
    });

    this._subscriptions.push(routeParamsSubscription);
  }

  ngOnDestroy() {
    this._subscriptions.map((sub) => { sub.unsubscribe(); });
    this._subscriptions = []; // For good measure
  }

  openSelectTeamDialog() {
    const dialogRef = this.composeDialog.open(GroupTeamsSelectComponent);
    const dialogueSubscription = dialogRef.afterClosed().subscribe(result => {
      dialogueSubscription.unsubscribe();
    });
  }

  openTeamSelectActivitiesDialog(teamAttendance: TeamAttendanceDto) {
    this._conductor.focusTeamAttendance(teamAttendance.id);
    const dialogRef = this.composeDialog.open(TeamActivitiesSelectComponent);
    const dialogueSubscription = dialogRef.afterClosed().subscribe(result => {
      dialogueSubscription.unsubscribe();
    });
  }

  removeTeamAttendance(teamAttendance: TeamAttendanceDto) {
    this._conductor.removeAttendanceByTeam(teamAttendance.teamId);
  }

  /**
   * [Hack] ngx datatable @15.0.2 does not correctly notice the changes and update it's rows.
   * It does see the change and the change fires as it does notice the count to update but the rows do not.
   */
  public spreadRows(teamAttendanceActivities: Array<ActivityTeamAttendanceDto>) {
    return [...teamAttendanceActivities];
  }
}
