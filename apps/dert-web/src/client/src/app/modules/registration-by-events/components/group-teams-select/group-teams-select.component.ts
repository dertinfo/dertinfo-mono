import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { UntypedFormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { Subscription } from 'rxjs';

import { GroupTeamDto } from 'app/models/dto';
import { TeamSelectMediator } from 'app/shared/components/teams-select/services/teams-select.mediator';
import { RegistrationByEventsConductor } from '../../services/registration-by-events.conductor';
import { RegistrationByEventsTracker } from '../../services/registration-by-events.tracker';
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
    private _conductor: RegistrationByEventsConductor,
    private _tracker: RegistrationByEventsTracker,
    private teamSelectMediator: TeamSelectMediator
  ) {
  }

  ngOnInit() {

    this.teamSelectMediator.hideSelect();

    const eventOverviewSubscription = this._tracker.registrationOverview$.subscribe((registrationOverview) => {
      if (registrationOverview) {
        this.groupId = registrationOverview.groupId;
      }
    });

    const attendingTeamsSubscription = this._tracker.attendingGroupTeams$.subscribe((eventTeams) => {
      if (eventTeams) {
        this.teamSelectMediator.applySelectedSet(eventTeams);
      }
    });

    const addNewTeamSubs = this.teamSelectMediator.newItemChange$.subscribe( newGroupTeamSubmission => {
      const subs = this._conductor.addTeam(newGroupTeamSubmission).subscribe(
        (savedTeam) => {
          subs.unsubscribe();
          this.composeDialog.closeAll();
        },
        (error) => {
          throw new Error('GroupTeamsSelectComponent - Adding New Team');
        }
      );
    });

    const selectionChangedSubs = this.teamSelectMediator.itemsChange$.subscribe( changedSelections => {
      const subs = this._conductor.toggleTeamAttendances(changedSelections).subscribe(
        (changes) => {
          subs.unsubscribe();
          this.composeDialog.closeAll();
        },
        (error) => {
          throw new Error('GroupTeamsSelectComponent - Change Team Selections Failed');
        }
      );
    });

    const closeModelSubs = this.teamSelectMediator.closeModal$.subscribe(() => {
      this.composeDialog.closeAll();
    });

    this._subscriptions.push(eventOverviewSubscription);
    this._subscriptions.push(attendingTeamsSubscription);
    this._subscriptions.push(addNewTeamSubs);
    this._subscriptions.push(selectionChangedSubs);
  }

  ngOnDestroy() {
    this._subscriptions.map((sub) => { sub.unsubscribe(); });
    this._subscriptions = []; // For good measure
  }
}
