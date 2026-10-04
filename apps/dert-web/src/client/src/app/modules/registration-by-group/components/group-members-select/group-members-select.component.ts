import { Component, OnDestroy, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { GroupMemberDto } from 'app/models/dto';
import { MemberSelectMediator } from 'app/shared/components/members-select/services/members-select.mediator';
import { Subscription } from 'rxjs';
import { MemberType } from '../../../../models/app/Enumerations/MemberType';
import { RegistrationByGroupConductor } from '../../services/registration-by-group.conductor';

import { RegistrationByGroupTracker } from '../../services/registration-by-group.tracker';

@Component({
  selector: 'app-group-members-select',
  templateUrl: './group-members-select.component.html',
  styleUrls: ['./group-members-select.component.css'],
  providers: [MemberSelectMediator]
})
export class GroupMembersSelectComponent implements OnInit, OnDestroy {

  private _subscriptions: Subscription[] = [];

  private groupId;
  private hasUploadChanges = false;

  public memberTypes = [MemberType.active, MemberType.guest];
  public allGroupMembers: GroupMemberDto[] = [];
  public selectedGroupMembers: GroupMemberDto[] = [];
  public editedGroupMembers: GroupMemberDto[] = [];

  constructor(

    private composeDialog: MatDialog,
    private _conductor: RegistrationByGroupConductor,
    private _tracker: RegistrationByGroupTracker,
    private memberSelectMediator: MemberSelectMediator
  ) {
  }

  ngOnInit() {

    this._subscriptions.push(this._tracker.registrationOverview$.subscribe((registrationOverview) => {
      if (registrationOverview) {
        this._conductor.initAllGroupMembers();
      }
    }));

    this._subscriptions.push(this._tracker.allGroupMembers$.subscribe((groupMembers) => {

      if (groupMembers) {
        this.memberSelectMediator.applyFullSet(groupMembers);
      }
    }));

    this._subscriptions.push(this._tracker.attendingGroupMembers$.subscribe((groupMembers) => {

      if (groupMembers) {
        this.memberSelectMediator.applySelectedSet(groupMembers);
      }
    }));

    this._subscriptions.push(this.memberSelectMediator.newItemChange$.subscribe(newGroupMemberSubmission => {
      const subs = this._conductor.addMember(newGroupMemberSubmission).subscribe(
        (savedMember) => {
          subs.unsubscribe();
          this.composeDialog.closeAll();
        },
        (error) => {
          console.error('ERROR: Adding Member Failed');
        }
      );
    }));

    this._subscriptions.push(this.memberSelectMediator.itemsChange$.subscribe(changedSelections => {
      const subs = this._conductor.toggleMemberAttendances(changedSelections).subscribe(
        (changes) => {
          subs.unsubscribe();
          this.composeDialog.closeAll();
        },
        (error) => {
          console.error('ERROR: Adding Member Failed');
        }
      );
    }));

    this._subscriptions.push(this.memberSelectMediator.closeModal$.subscribe(() => {
      this.composeDialog.closeAll();
    }));
  }

  ngOnDestroy() {

    this._subscriptions.map((sub) => { sub.unsubscribe(); });
    this._subscriptions = []; // For good measure
  }
}
