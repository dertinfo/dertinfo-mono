import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';
import { RegistrationByEventsConductor } from '../../services/registration-by-events.conductor';
import { RegistrationByEventsTracker } from '../../services/registration-by-events.tracker';

@Component({
  selector: 'app-event-registration-overview',
  templateUrl: './event-registration-overview.component.html',
  styleUrls: ['./event-registration-overview.component.css']
})
export class EventRegistrationOverviewComponent implements OnInit, OnDestroy {

  private _subscriptions: Subscription[] = [];
  private routeRegistrationId: number;

  public groupName: string;
  public groupPictureUrl: string;
  public eventName: string;
  public eventPictureUrl: string;
  public teamAttendanceCount: number;
  public memberAttendanceCount: number;
  public guestAttendanceCount: number;
  public totalPrice: number;
  public groupContactName: string;
  public groupContactEmail: string;
  public groupContactTelephone: string;

  constructor(
    private activatedRoute: ActivatedRoute,
    private _conductor: RegistrationByEventsConductor,
    private _tracker: RegistrationByEventsTracker
  ) {}

  ngOnInit() {

    this._subscriptions.push(this._tracker.registrationOverview$.subscribe((overview) => {
      if (overview) {
        this.groupName = overview.groupName;
        this.groupPictureUrl = overview.groupPictureUrl;
        this.eventName = overview.eventName;
        this.eventPictureUrl = overview.eventPictureUrl;
        this.teamAttendanceCount = overview.teamAttendancesCount;
        this.memberAttendanceCount = overview.memberAttendancesCount;
        this.guestAttendanceCount = overview.guestAttendancesCount;
        this.totalPrice = overview.currentTotal;
      }
    }));

    this._subscriptions.push(this._tracker.contactInfo$.subscribe((contactInfo) => {
      if (contactInfo) {
        this.groupContactName = contactInfo.contactName;
        this.groupContactEmail = contactInfo.contactEmail;
        this.groupContactTelephone = contactInfo.contactTelephone;
      }
    }));

    this._conductor.initContactInfo();
  }

  ngOnDestroy() {
    this._subscriptions.map((sub) => { sub.unsubscribe(); });
    this._subscriptions = []; // For good measure
  }

}
