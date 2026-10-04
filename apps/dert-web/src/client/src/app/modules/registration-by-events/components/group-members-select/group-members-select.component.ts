import { Component, OnDestroy, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { GroupMemberDto } from 'app/models/dto';
import { MemberSelectMediator } from 'app/shared/components/members-select/services/members-select.mediator';
import { Subscription } from 'rxjs';
import { MemberType } from '../../../../models/app/Enumerations/MemberType';
import { RegistrationByEventsConductor } from '../../services/registration-by-events.conductor';
import { RegistrationByEventsTracker } from '../../services/registration-by-events.tracker';

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
    private _conductor: RegistrationByEventsConductor,
    private _tracker: RegistrationByEventsTracker,
    private memberSelectMediator: MemberSelectMediator
  ) {
  }

  ngOnInit() {

    this.memberSelectMediator.hideSelect();

    const eventOverviewSubs = this._tracker.registrationOverview$.subscribe((registrationOverview) => {
      if (registrationOverview) {
        this.groupId = registrationOverview.groupId;
      }
    });

    const attendingMembersSubs = this._tracker.attendingGroupMembers$.subscribe((eventMembers) => {

      if (eventMembers) {
        this.memberSelectMediator.applySelectedSet(eventMembers);
      }
    });

    const addNewMemberSubs = this.memberSelectMediator.newItemChange$.subscribe(newGroupMemberSubmission => {
      const subs = this._conductor.addMember(newGroupMemberSubmission).subscribe(
        (savedMember) => {
          subs.unsubscribe();
          this.composeDialog.closeAll();
        },
        (error) => {
          console.error('ERROR: Adding Member Failed');
        }
      );
    });

    const selectionChangedSubs = this.memberSelectMediator.itemsChange$.subscribe(changedSelections => {
      const subs = this._conductor.toggleMemberAttendances(changedSelections).subscribe(
        (changes) => {
          subs.unsubscribe();
          this.composeDialog.closeAll();
        },
        (error) => {
          console.error('ERROR: Adding Member Failed');
        }
      );
    });

    const closeModelSubs = this.memberSelectMediator.closeModal$.subscribe(() => {
      this.composeDialog.closeAll();
    });

    this._subscriptions.push(eventOverviewSubs);
    this._subscriptions.push(attendingMembersSubs);
    this._subscriptions.push(addNewMemberSubs);
    this._subscriptions.push(selectionChangedSubs);
    this._subscriptions.push(closeModelSubs);
  }

  ngOnDestroy() {

    this._subscriptions.map((sub) => { sub.unsubscribe(); });
    this._subscriptions = []; // For good measure
  }
}
