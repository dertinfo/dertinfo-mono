import { Component, OnDestroy, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MemberSelectMediator } from 'app/shared/components/members-select/services/members-select.mediator';
import { Subscription } from 'rxjs';
import { GroupAdminConductor } from '../../services/group-admin.conductor';

@Component({
  selector: 'app-group-members-create',
  templateUrl: './group-members-create.template.html',
  providers: [MemberSelectMediator]
})
export class GroupMembersCreateComponent implements OnInit, OnDestroy {

  private _subscriptions: Subscription[] = [];

  constructor(

    private memberSelectMediator: MemberSelectMediator,
    private composeDialog: MatDialog,
    private groupConductor: GroupAdminConductor
  ) {
  }

  ngOnInit() {

    this.memberSelectMediator.hideSelect();

    const addNewMemberSubs = this.memberSelectMediator.newItemChange$.subscribe( newGroupMemberSubmission => {
      const subs = this.groupConductor.addMember(newGroupMemberSubmission).subscribe(
        (savedMember) => {
          subs.unsubscribe();
          this.composeDialog.closeAll();
        },
        (error) => {
          throw new Error('GroupMembersCreateComponent - Adding Member Failed');
        }
      );
    });

    const closeModelSubs = this.memberSelectMediator.closeModal$.subscribe(() => {
      this.composeDialog.closeAll();
    });

    this._subscriptions.push(addNewMemberSubs);
  }

  ngOnDestroy() {

    this._subscriptions.map((sub) => { sub.unsubscribe(); });
    this._subscriptions = [];
  }
}
