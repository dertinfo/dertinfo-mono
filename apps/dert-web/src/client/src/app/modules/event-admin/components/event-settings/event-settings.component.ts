
import { Component, OnDestroy, OnInit } from '@angular/core';
import { UntypedFormControl, UntypedFormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { customEmailValidator } from 'app/shared/validators/email-no-required';
import { EventDatesUpdateDto, EventOverviewDto, EventOverviewUpdateDto } from 'app/models/dto';
import { Subscription } from 'rxjs';
import { EventAdminConductor } from '../../services/event-admin.conductor';
import { EventAdminTracker } from '../../services/event-admin.tracker';

import { EventCancellationOptionsDto } from 'app/models/dto/EventCancellationOptionsDto';

@Component({
  selector: 'app-event-settings',
  templateUrl: './event-settings.component.html',
  styleUrls: ['./event-settings.component.css']
})
export class EventSettingsComponent implements OnInit, OnDestroy {

  private _subscriptions: Subscription[] = [];
  private _eventNamePattern = '^(?!.*\/).*$';
  private eventOverview: EventOverviewDto;
  private eventUpdateForm: UntypedFormGroup;
  private eventDatesUpdateForm: UntypedFormGroup;
  private eventPrivacySettingsForm: UntypedFormGroup;

  private isActiveConfirmDeleteView = false;
  private isActiveConfirmCloseView = false;

  public _isReady = false;
  public eventClosed = false;
  public eventDeleted = false;
  public eventCancelled = false;
  public eventCancelOptions: Array<string> = ['Send Cancellation Emails', 'No Communication'];

  public visibilityOptions = [
    {
      name: 'Public',
      value: 'public',
      id: 0
    },
    {
      name: 'Restricted',
      value: 'restricted',
      id: 1
    },
    {
      name: 'Private',
      value: 'private',
      id: 2
    }
  ];

  constructor(

    private activatedRoute: ActivatedRoute,
    private _eventConductor: EventAdminConductor,
    private _eventTracker: EventAdminTracker,
    private router: Router,
  ) { }
  ngOnInit() {

    this._subscriptions.push(this._eventTracker.eventOverview$.subscribe((eventOverview) => {
      if (eventOverview) {

        let doBinding = true;
        if (eventOverview.isClosing && eventOverview.isClosed) { this.eventClosed = true; doBinding = false; }
        if (eventOverview.isDeleting && eventOverview.isDeleted) { this.eventDeleted = true; doBinding = false; }
        if (eventOverview.isCancelling && eventOverview.isCancelled) { this.eventCancelled = true; doBinding = false; }

        if (doBinding) {
          this.eventOverview = eventOverview;

          this.eventUpdateForm = new UntypedFormGroup({
            eventName: new UntypedFormControl(this.eventOverview.name, [
              Validators.required,
              Validators.pattern(this._eventNamePattern)
            ]),
            contactEmail: new UntypedFormControl(this.eventOverview.contactEmail, [
              Validators.required,
              customEmailValidator()
            ]),
            contactTelephone: new UntypedFormControl(this.eventOverview.contactTelephone, [Validators.required]),
            contactName: new UntypedFormControl(this.eventOverview.contactName, []),
            eventSynopsis: new UntypedFormControl(this.eventOverview.eventSynopsis, []),
            locationTown: new UntypedFormControl(this.eventOverview.locationTown, []),
            locationPostcode: new UntypedFormControl(this.eventOverview.locationPostcode, []),
            sentEmailsBcc: new UntypedFormControl(this.eventOverview.sentEmailsBcc, [])
          });

          this.eventDatesUpdateForm = new UntypedFormGroup({
            eventStartDate: new UntypedFormControl(this.eventOverview.eventStartDate, [Validators.required]),
            eventEndDate: new UntypedFormControl(this.eventOverview.eventEndDate, [Validators.required]),
            registrationOpenDate: new UntypedFormControl(this.eventOverview.registrationOpenDate, []),
            registrationCloseDate: new UntypedFormControl(this.eventOverview.registrationCloseDate, []),
          });

          this.eventPrivacySettingsForm = new UntypedFormGroup({
            visibility: new UntypedFormControl(this.eventOverview.visibility, [Validators.required])
          });

          this._isReady = true;
        }
      }
    }));

  }

  ngOnDestroy() {
    this._subscriptions.map((sub) => { sub.unsubscribe(); });
    this._subscriptions = []; // For good measure
  }

  onEventUpdateSubmit() {

    if (this.eventUpdateForm.valid) {

      // Update the visible entities
      this.eventOverview.name = this.eventUpdateForm.value.eventName;

      const eventOverviewUpdate: EventOverviewUpdateDto = {
        eventId: this.eventOverview.id,
        eventName: this.eventUpdateForm.value.eventName,
        contactEmail: this.eventUpdateForm.value.contactEmail,
        contactTelephone: this.eventUpdateForm.value.contactTelephone,
        contactName: this.eventUpdateForm.value.contactName,
        eventSynopsis: this.eventUpdateForm.value.eventSynopsis,
        locationTown: this.eventUpdateForm.value.locationTown,
        locationPostcode: this.eventUpdateForm.value.locationPostcode,
        visibility: this.eventOverview.visibility,
        sentEmailsBcc: this.eventUpdateForm.value.sentEmailsBcc
      };

      this._eventConductor.updateEventOverview(eventOverviewUpdate);
    }
  }

  onEventPrivacySubmit() {

    if (this.eventPrivacySettingsForm.valid) {

      // Update the visible entities
      this.eventOverview.name = this.eventUpdateForm.value.eventName;

      const eventOverviewUpdate: EventOverviewUpdateDto = {
        eventId: this.eventOverview.id,
        eventName: this.eventOverview.name,
        contactEmail: this.eventOverview.contactEmail,
        contactTelephone: this.eventOverview.contactTelephone,
        contactName: this.eventOverview.contactName,
        eventSynopsis: this.eventOverview.eventSynopsis,
        locationTown: this.eventOverview.locationTown,
        locationPostcode: this.eventOverview.locationPostcode,
        visibility: this.eventPrivacySettingsForm.value.visibility,
        sentEmailsBcc: this.eventOverview.sentEmailsBcc
      };

      this._eventConductor.updateEventOverview(eventOverviewUpdate);
    }
  }

  onEventDatesUpdateSubmit() {

    if (this.eventDatesUpdateForm.valid) {

      // Update the visible entities on the page
      // - none regarding dates at this time

      const myEventStartDate = new Date(this.eventDatesUpdateForm.value.eventStartDate);
      const myEventEndDate = new Date(this.eventDatesUpdateForm.value.eventEndDate);
      const myRegistrationOpenDate = new Date(this.eventDatesUpdateForm.value.registrationOpenDate);
      const myRegistrationCloseDate = new Date(this.eventDatesUpdateForm.value.registrationCloseDate);

      const eventStartDateUTC = new Date(Date.UTC(myEventStartDate.getFullYear(), myEventStartDate.getMonth(), myEventStartDate.getDate()));
      const eventEndDateUTC = new Date(Date.UTC(myEventEndDate.getFullYear(), myEventEndDate.getMonth(), myEventEndDate.getDate()));
      const registrationOpenDateUTC = new Date(Date.UTC(
        myRegistrationOpenDate.getFullYear(),
        myRegistrationOpenDate.getMonth(),
        myRegistrationOpenDate.getDate())
      );
      const registrationCloseDateUTC = new Date(Date.UTC(
        myRegistrationCloseDate.getFullYear(),
        myRegistrationCloseDate.getMonth(),
        myRegistrationCloseDate.getDate())
      );

      const eventDatesUpdate: EventDatesUpdateDto = {
        eventId: this.eventOverview.id,
        eventStartDate: eventStartDateUTC,
        eventEndDate: eventEndDateUTC,
        registrationOpenDate: registrationOpenDateUTC,
        registrationCloseDate: registrationCloseDateUTC
      };

      this._eventConductor.updateEventDates(eventDatesUpdate);
    }
  }

  onEventDeleteClick() {
    this.isActiveConfirmDeleteView = true;
  }

  onDeleteEventConfirmed() {
    this._eventConductor.removeEvent();
  }

  onCancelEventConfirmed(cancellationOption) {

    const eventCancellationOptions: EventCancellationOptionsDto = {
      sendCommunications: cancellationOption !== 'No Communication',
      informNewRegistrations: false,
      informSubmittedRegistrations: true,
      informConfirmedRegistrations: true
    };

    this._eventConductor.cancelEvent(eventCancellationOptions);
  }

  onCloseEventConfirmed() {
    this._eventConductor.closeEvent();
  }
}
