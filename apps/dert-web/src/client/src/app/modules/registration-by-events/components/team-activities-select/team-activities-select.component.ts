import { Component, OnDestroy, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Subscription } from 'rxjs';
import { ActivityDto, ActivityTeamAttendanceDto } from '../../../../models/dto';
import { RegistrationByEventsConductor } from '../../services/registration-by-events.conductor';
import { RegistrationByEventsTracker } from '../../services/registration-by-events.tracker';
import { TeamActivitySelectModel } from './models/team-activities-select.model';

@Component({
    selector: 'app-team-activities-select',
    templateUrl: './team-activities-select.component.html',
    styleUrls: ['./team-activities-select.component.css']
})
export class TeamActivitiesSelectComponent implements OnInit, OnDestroy {

  private _subscriptions: Subscription[] = [];
  private eventId: number;

  public allActivities: ActivityDto[] = [];
  public selectedActivityIds: number[] = [];
  public editedActivities: ActivityDto[] = [];

  public selections: TeamActivitySelectModel[] = [];

  constructor(

    private composeDialog: MatDialog,
    private _conductor: RegistrationByEventsConductor,
    private _tracker: RegistrationByEventsTracker
  ) {
  }

  ngOnInit() {

    const registrationOverviewSubscription = this._tracker.registrationOverview$.subscribe((registrationOverview) => {
      if (registrationOverview) {
        this.eventId = registrationOverview.eventId;
        this._conductor.initTeamActivities();
      }
    });

    const allActivitiesSubscription = this._tracker.activitiesForTeams$.subscribe((activities) => {
      if (activities) {
        this.allActivities = activities;
        this.selections = this.prepareSelections();
      }
    });

    const focussedTeamAttendanceSubscription = this._tracker.focussedTeamAttendance$.subscribe((teamAttendance) => {
      if (teamAttendance && teamAttendance.attendanceActivities) {
        this.selectedActivityIds = teamAttendance.attendanceActivities.map(aa => aa.activityId);
        this.selections = this.prepareSelections();
      }
    });

    this._subscriptions.push(registrationOverviewSubscription);
    this._subscriptions.push(allActivitiesSubscription);
    this._subscriptions.push(focussedTeamAttendanceSubscription);
  }

  ngOnDestroy() {
    this._subscriptions.map((sub) => { sub.unsubscribe(); });
    this._subscriptions = []; // For good measure
  }

  /**
   * Action taken when a selection event occours.
   * Manages state inside this component until save button hit.
   * @param eventTeamSelectModel the event team related to event
   */
  public onSelectActivityClick(teamActivitySelectModel: TeamActivitySelectModel) {

    const activity = this.allActivities.find(a => a.id === teamActivitySelectModel.id);
    const selectedActivity = this.selectedActivityIds.find(saId => saId === teamActivitySelectModel.id);
    const editedActivity = this.editedActivities.find(ea => ea.id === teamActivitySelectModel.id);

    if (editedActivity) {
      this.editedActivities.splice(this.editedActivities.indexOf(editedActivity), 1);
    } else {
      this.editedActivities.push(activity);
    }

    if (selectedActivity) {
      this.selectedActivityIds.splice(this.selectedActivityIds.indexOf(selectedActivity), 1);
    } else {
      this.selectedActivityIds.push(activity.id);
    }

    this.selections = this.prepareSelections();
  }

  /**
   * [Event] button click event to save the changes to the current teams selection.
   */
  public onSelectionSave() {
    this.saveSelectionChanges();
  }

  public onCancel() {
    this.composeDialog.closeAll();
  }

  /**
   * Persists the changes for the edited users in the teams selection
   */
  private saveSelectionChanges() {
    const subs = this._conductor.toggleFocussedTeamAttendanceActivities(this.editedActivities).subscribe(activityTeamAttendnaceDtos => {
      subs.unsubscribe();
      this.composeDialog.closeAll();
    });
  }

  private prepareSelections() {
    return this.allActivities.map(a => {

      const activitySelectedId: number = this.selectedActivityIds.find(saId => saId === a.id);
      const activityEdited: ActivityDto = this.editedActivities.find(ea => ea.id === a.id);

      return {
        id: a.id,
        name: a.title,
        selected: !!activitySelectedId,
        changed: !!activityEdited
      };
    });
  }
}
