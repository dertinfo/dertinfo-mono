
import { Component, OnDestroy, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';
import {map} from 'rxjs/operators';
import { MemberType } from '../../../../models/app/Enumerations/MemberType';
import { ActivityMemberAttendanceDto, GroupMemberDto, MemberAttendanceDto } from '../../../../models/dto';
import { RegistrationByEventsConductor } from '../../services/registration-by-events.conductor';
import { RegistrationByEventsTracker } from '../../services/registration-by-events.tracker';
import { GroupMembersSelectComponent } from '../group-members-select/group-members-select.component';
import { MemberActivitiesSelectComponent } from '../member-activities-select/member-activities-select.component';

@Component({
  selector: 'app-event-registration-guests',
  templateUrl: './event-registration-guests.component.html',
  styleUrls: ['./event-registration-guests.component.css']
})
export class EventRegistrationGuestsComponent implements OnInit, OnDestroy {

  private _subscriptions: Subscription[] = [];
  private routeRegistrationId: number;

  public attendingMembers$;

  public get capabilities() {
    return this._conductor.capabilities;
  }

  constructor(
    public composeDialog: MatDialog,
    private activatedRoute: ActivatedRoute,
    private _conductor: RegistrationByEventsConductor,
    private _tracker: RegistrationByEventsTracker
  ) { }

  ngOnInit() {

    // Listen for route changes on the parent
    this._subscriptions.push(this.activatedRoute.parent.params.subscribe(params => {
      this._conductor.initAttendingIndividuals();
    }));

    // Bind members obs to tracker obs.
    this.attendingMembers$ = this._tracker.memberAttendances$.pipe( // uses | async
      map(attendingMembers => attendingMembers
        .filter(attendingMember => attendingMember.groupMemberType === MemberType.guest)));
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
