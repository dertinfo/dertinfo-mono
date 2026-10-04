
import { Component, OnDestroy, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';
import {map} from 'rxjs/operators';
import { MemberType } from '../../../../models/app/Enumerations/MemberType';
import { ActivityMemberAttendanceDto, GroupMemberDto, MemberAttendanceDto } from '../../../../models/dto';
import { GroupMembersSelectComponent } from '../group-members-select/group-members-select.component';
import { RegistrationByGroupConductor } from '../../services/registration-by-group.conductor';
import { RegistrationByGroupTracker } from '../../services/registration-by-group.tracker';
import { MemberActivitiesSelectComponent } from '../member-activities-select/member-activities-select.component';

@Component({
  selector: 'app-group-registration-guests',
  templateUrl: './group-registration-guests.component.html',
  styleUrls: ['./group-registration-guests.component.css']
})
export class GroupRegistrationGuestsComponent implements OnInit, OnDestroy {

  private _subscriptions: Subscription[] = [];
  private routeRegistrationId: number;

  public attendingMembers$;

  public get capabilities() {
    return this._tracker.capabilities;
  }

  constructor(
    public composeDialog: MatDialog,
    private activatedRoute: ActivatedRoute,
    private _conductor: RegistrationByGroupConductor,
    private _tracker: RegistrationByGroupTracker
  ) { }

  ngOnInit() {

    this.attendingMembers$ = this._tracker.memberAttendances$.pipe(
      map(attendingMembers => attendingMembers
        .filter(attendingMember => attendingMember.groupMemberType === MemberType.guest)));

    this._subscriptions.push(this.activatedRoute.parent.params.subscribe(params => {
      this._conductor.initAttendingIndividuals();
    }));

  }

  ngOnDestroy() {
    this._subscriptions.map((sub) => { sub.unsubscribe(); });
    this._subscriptions = []; // For good measure
  }

  openSelectMemberDialog() {
    const dialogRef = this.composeDialog.open(GroupMembersSelectComponent);
    const dialogueSubscription = dialogRef.afterClosed().subscribe(result => {
      dialogueSubscription.unsubscribe();
    });
  }

  openMemberSelectActivitiesDialog(memberAttendance: MemberAttendanceDto) {
    this._conductor.focusMemberAttendance(memberAttendance.id);
    const dialogRef = this.composeDialog.open(MemberActivitiesSelectComponent);
    const dialogueSubscription = dialogRef.afterClosed().subscribe(result => {
      dialogueSubscription.unsubscribe();
    });
  }

  removeMemberAttendance(memberAttendance: MemberAttendanceDto) {
    this._conductor.removeAttendanceByMember(memberAttendance.groupMemberId);
  }

  /**
   * [Hack] ngx datatable @15.0.2 does not correctly notice the changes and update it's rows.
   * It does see the change and the change fires as it does notice the count to update but the rows do not.
   */
  public spreadRows(memberAttendanceActivities: Array<ActivityMemberAttendanceDto>) {
    return [...memberAttendanceActivities];
  }

}
