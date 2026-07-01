import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { EventOverviewDto, EventRegistrationDto } from 'app/models/dto';
import { Observable ,  Subscription } from 'rxjs';
import { EventAdminConductor } from '../../services/event-admin.conductor';
import { EventAdminTracker } from '../../services/event-admin.tracker';

@Component({
  selector: 'app-event-overview',
  templateUrl: './event-overview.component.html',
  styleUrls: ['./event-overview.component.css']
})
export class EventOverviewComponent implements OnInit, OnDestroy {

  private _subscriptions: Subscription[] = [];

  private eventOverview$: Observable<EventOverviewDto>;

  public contactEmail: string;
  public contactName: string;
  public contactTelephone: string;
  public registrationsCount: number;

  public activeRegistrations: EventRegistrationDto[];
  public individualsCount = 0;
  public teamsCount = 0;

  constructor(
    private router: ActivatedRoute,
    private _eventConductor: EventAdminConductor,
    private _eventTracker: EventAdminTracker
  ) { }

  ngOnInit() {
    this._subscriptions.push(this._eventTracker.eventOverview$.subscribe((eventOverview) => {
      if (eventOverview) {
        this.contactEmail = eventOverview.contactEmail;
        this.contactName = eventOverview.contactName;
        this.contactTelephone = eventOverview.contactTelephone;
        this.registrationsCount = eventOverview.registrationsCount;
        this.individualsCount = eventOverview.membersAndGuestsCount;
        this.teamsCount = eventOverview.teamsCount;
      }
    }));

    this._subscriptions.push(this._eventTracker.activeRegistrations$.subscribe((activeRegistrations) => {
      this.activeRegistrations = activeRegistrations;

      if (this.activeRegistrations && this.activeRegistrations.length > 0) {
        this.individualsCount = this.activeRegistrations.map(ar => ar.memberAttendancesCount).reduce((total, num) => {
          return total + num;
        });

        this.teamsCount = this.activeRegistrations.map(ar => ar.teamAttendancesCount).reduce((total, num) => {
          return total + num;
        });
      }
    }));
  }

  ngOnDestroy() {
    this._subscriptions.map((sub) => { sub.unsubscribe(); });
    this._subscriptions = []; // For good measure
  }

}
