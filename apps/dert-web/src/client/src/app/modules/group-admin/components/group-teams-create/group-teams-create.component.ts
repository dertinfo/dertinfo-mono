import { Component, OnDestroy, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { TeamSelectMediator } from 'app/shared/components/teams-select/services/teams-select.mediator';
import { Subscription } from 'rxjs';
import { GroupAdminConductor } from '../../services/group-admin.conductor';

@Component({
  selector: 'app-group-teams-create',
  templateUrl: './group-teams-create.template.html'
})
export class GroupTeamsCreateComponent implements OnInit, OnDestroy {

  private _subscriptions: Subscription[] = [];

  constructor(

    private teamSelectMediator: TeamSelectMediator,
    private composeDialog: MatDialog,
    private groupConductor: GroupAdminConductor
  ) {
  }

  ngOnInit() {

    this.teamSelectMediator.hideSelect();

    const addNewTeamSubs = this.teamSelectMediator.newItemChange$.subscribe( newTeamSubmission => {
      const subs = this.groupConductor.addTeam(newTeamSubmission).subscribe(
        (savedMember) => {
          subs.unsubscribe();
          this.composeDialog.closeAll();
        },
        (error) => {
          throw new Error('GroupTeamsCreateComponent - Adding Team Failed');
        }
      );
    });

    const closeModelSubs = this.teamSelectMediator.closeModal$.subscribe(() => {
      this.composeDialog.closeAll();
    });

    this._subscriptions.push(addNewTeamSubs);
  }

  ngOnDestroy() {

    this._subscriptions.map((sub) => { sub.unsubscribe(); });
    this._subscriptions = [];
  }
}
