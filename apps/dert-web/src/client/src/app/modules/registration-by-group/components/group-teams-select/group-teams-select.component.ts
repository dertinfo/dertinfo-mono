import { Component, OnDestroy, OnInit } from '@angular/core';
import { UntypedFormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { Subscription } from 'rxjs';

import { TeamSelectMediator } from 'app/shared/components/teams-select/services/teams-select.mediator';
import { GroupTeamDto } from 'app/models/dto';
import { RegistrationByGroupConductor } from '../../services/registration-by-group.conductor';
import { RegistrationByGroupTracker } from '../../services/registration-by-group.tracker';
import { GroupTeamSelectModel } from './models/group-team-select.model';

@Component({
  selector: 'app-group-teams-select',
  templateUrl: './group-teams-select.component.html',
  styleUrls: ['./group-teams-select.component.css']
})
export class GroupTeamsSelectComponent implements OnInit, OnDestroy {

  private _subscriptions: Subscription[] = [];

  private groupId;

  public teamForm: UntypedFormGroup;

  public allGroupTeams: GroupTeamDto[] = [];
  public selectedGroupTeams: GroupTeamDto[] = [];
  public editedGroupTeams: GroupTeamDto[] = [];

  public selections: GroupTeamSelectModel[] = [];

  constructor(
    private composeDialog: MatDialog,
    private _conductor: RegistrationByGroupConductor,
    private _tracker: RegistrationByGroupTracker,
    private teamSelectMediator: TeamSelectMediator
  ) {
  }

  ngOnInit() {

    this._subscriptions.push(this._tracker.registrationOverview$.subscribe((registrationOverview) => {
      if (registrationOverview) {
        this._conductor.initAllGroupTeams();
      }
    }));

    this._subscriptions.push(this._tracker.allGroupTeams$.subscribe((groupTeams) => {
      if (groupTeams) {
        this.teamSelectMediator.applyFullSet(groupTeams);
      }
    }));

    this._subscriptions.push(this._tracker.attendingGroupTeams$.subscribe((groupTeams) => {
      if (groupTeams) {
        this.teamSelectMediator.applySelectedSet(groupTeams);
      }
    }));

    this._subscriptions.push(this.teamSelectMediator.newItemChange$.subscribe( newGroupTeamSubmission => {
      const subs = this._conductor.addTeam(newGroupTeamSubmission).subscribe(
        (savedTeam) => {
          subs.unsubscribe();
          this.composeDialog.closeAll();
        },
        (error) => {
          throw new Error('GroupTeamsSelectComponent - Adding New Team Failed');
        }
      );
    }));

    this._subscriptions.push(this.teamSelectMediator.itemsChange$.subscribe( changedSelections => {
      const subs = this._conductor.toggleTeamAttendances(changedSelections).subscribe(
        (changes) => {
          subs.unsubscribe();
          this.composeDialog.closeAll();
        },
        (error) => {
          throw new Error('GroupTeamsSelectComponent - Change Team Selections Failed');
        }
      );
    }));

    this._subscriptions.push(this.teamSelectMediator.closeModal$.subscribe(() => {
      this.composeDialog.closeAll();
    }));
  }

  ngOnDestroy() {
    this._subscriptions.map((sub) => { sub.unsubscribe(); });
    this._subscriptions = []; // For good measure
  }
}
