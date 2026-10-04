import { Component, OnDestroy, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute } from '@angular/router';
import { ActivityAudienceType } from 'app/models/app/Enumerations/ActivityAudienceType';
import { ActivityDto, GroupMemberDto } from 'app/models/dto';
import { Subscription } from 'rxjs';
import { EventAdminConductor } from '../../services/event-admin.conductor';
import { EventActivitiesCreateComponent } from '../event-activities-create/event-activities-create.component';

import { EventAdminTracker } from '../../services/event-admin.tracker';

@Component({
  selector: 'app-event-activities',
  templateUrl: './event-activities.component.html',
  styleUrls: ['./event-activities.component.css']
})
export class EventActivitiesComponent implements OnInit, OnDestroy {

  private _subscriptions: Subscription[] = [];

  public eventIndividualActivities$;
  public eventTeamActivities$;

  /*
  public activityCategories = [
    { name: "Attendance" },
    { name: "Accomodation" },
    { name: "Merchandise" }
  ];
  */

  public audienceTypeIndividual = ActivityAudienceType.INDIVIDUAL;
  public audienceTypeTeam = ActivityAudienceType.TEAM;

  constructor(
    public composeDialog: MatDialog,
    private _activatedRoute: ActivatedRoute,
    private _eventConductor: EventAdminConductor,
    private _eventTracker: EventAdminTracker,

    ) { }

  ngOnInit() {

    this._subscriptions.push(this._activatedRoute.parent.params.subscribe(params => {
      this._eventConductor.initActivities(params['id']);
    }));

    this.eventIndividualActivities$ = this._eventTracker.eventIndividualActivities$; // uses | async
    this.eventTeamActivities$ = this._eventTracker.eventTeamActivities$; // uses | async
  }

  ngOnDestroy() {

    this._subscriptions.map((sub) => { sub.unsubscribe(); });
    this._subscriptions = []; // For good measure
  }

  public openCreateActivityDialog() {

    const dialogRef = this.composeDialog.open(EventActivitiesCreateComponent);
    const dialogueSubscription = dialogRef.afterClosed().subscribe(result => {
      dialogueSubscription.unsubscribe();
    });
  }

  public getDetailRouteLink(activity: ActivityDto) {
    return ['./' + encodeURIComponent(activity.title) + '/' + activity.id];
  }

}
