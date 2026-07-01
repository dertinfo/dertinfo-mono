import { Component, OnDestroy, OnInit } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { AppConstants } from 'app/app.constants';
import { EventConfigurationSubmissionDto, EventDto } from 'app/models/dto';
import { EventConfigureTermsComponent } from 'app/regions/terms/components/event-configure-terms/event-configure-terms.component';
import { Subscription } from 'rxjs';
import { EventSetupConductor } from '../../services/event-setup.conductor';
import { EventSetupTracker } from '../../services/event-setup.tracker';
import { UploadImageComponent } from '../upload-image/upload-image.component';

@Component({
  selector: 'app-start',
  templateUrl: './start.component.html',
  styleUrls: ['./start.component.css']
})
export class StartComponent implements OnInit, OnDestroy {

  private _subscriptions: Subscription[] = [];

  contactDetailsFormGroup: UntypedFormGroup;
  visabilityFormGroup: UntypedFormGroup;
  eventDetailsFormGroup: UntypedFormGroup;
  eventImageFormGroup: UntypedFormGroup;
  configurationTemplateFormGroup: UntypedFormGroup;
  termsAndConditionsFormGroup: UntypedFormGroup;

  public eventName: string;
  public eventPictureUrl: string;
  public selectedVisibilityOption = 'public';
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

  public eventConfigurationTemplateOptions = [
    {
      name: 'Basic',
      value: 'Basic',
      id: 0
    },
    {
      name: 'Dancing England Rapper Tournament(Standard)',
      value: 'StandardDert',
      id: 1
    },
    {
      name: 'Dancing England Rapper Tournament(Classic)',
      value: 'ClassicDert',
      id: 2
    }
  ];

  constructor(

    private fb: UntypedFormBuilder,
    private _conductor: EventSetupConductor,
    private _tracker: EventSetupTracker,
    public composeDialog: MatDialog,
    private router: Router
  ) { }

  ngOnInit() {

    this.contactDetailsFormGroup = this.fb.group({
      contactName: ['', Validators.required],
      contactEmail: ['', Validators.required],
      contactTelephone: ['', Validators.required],
    });
    this.visabilityFormGroup = this.fb.group({
      visibility: [0, Validators.required],
      registrationOpenDate: ['', Validators.required],
      registrationCloseDate: ['', Validators.required]
    });
    this.eventDetailsFormGroup = this.fb.group({
      eventSynopsis: ['', Validators.required],
      locationTown: [''],
      locationPostcode: [''],
      eventStartDate: ['', Validators.required],
      eventEndDate: ['', Validators.required]
    });
    this.eventImageFormGroup = this.fb.group({
      eventPictureUrl: ['', Validators.required]
    });
    this.configurationTemplateFormGroup = this.fb.group({
      eventTemplate: [0, Validators.required]
    });
    this.termsAndConditionsFormGroup = this.fb.group({
      agreeToTermsAndConditions: [false, Validators.required]
    });

    this._subscriptions.push(this._tracker.eventOverview$.subscribe((eventOverview) => {
      if (eventOverview) {

        this.eventName = eventOverview.name;

        this.eventDetailsFormGroup.setValue(
          {
            locationTown: eventOverview.locationTown || '',
            locationPostcode: eventOverview.locationPostcode || '',
            eventStartDate: eventOverview.eventStartDate || '',
            eventEndDate: eventOverview.eventEndDate || '',
            eventSynopsis: eventOverview.eventSynopsis
          }
        );

        this.eventImageFormGroup.setValue(
          {
            eventPictureUrl: eventOverview.eventPictureUrl
          }
        );

        this.eventPictureUrl = eventOverview.eventPictureUrl;
      }
    }));

    this._subscriptions.push(this._tracker.userData$.subscribe((userData) => {
      if (userData) {

        let userFullName = '';
        if (userData.lastname) { userFullName = userData.lastname.trim(); }
        if (userData.firstname) { userFullName = userData.firstname.trim(); }
        if (userData.firstname && userData.lastname) { userFullName = (userData.firstname + ' ' + userData.lastname).trim(); }

        this.contactDetailsFormGroup.setValue(
          {
            contactName: userFullName,
            contactEmail: userData.email || '',
            contactTelephone: userData.phone || '',
          }
        );
      }
    }));

    this._subscriptions.push(this.visabilityFormGroup.get('visibility').valueChanges.subscribe((visibility) => {
      if (visibility === 0 || visibility === 1) { // public / restricted
        this.visabilityFormGroup.get('registrationOpenDate').setValidators([Validators.required]);
        this.visabilityFormGroup.get('registrationCloseDate').setValidators([Validators.required]);
      } else {
        this.visabilityFormGroup.get('registrationOpenDate').setValidators([]);
        this.visabilityFormGroup.get('registrationCloseDate').setValidators([]);
      }

      this.visabilityFormGroup.get('registrationOpenDate').updateValueAndValidity();
      this.visabilityFormGroup.get('registrationCloseDate').updateValueAndValidity();
      // this.visabilityFormGroup.updateValueAndValidity();
    }));
  }

  ngOnDestroy() {
    this._subscriptions.map((sub) => { sub.unsubscribe(); });
    this._subscriptions = []; // For good measure
  }

  submit() {

    const eventStartDate = new Date(this.eventDetailsFormGroup.value.eventStartDate);
    const eventStartDateUTC = new Date(Date.UTC(eventStartDate.getFullYear(), eventStartDate.getMonth(), eventStartDate.getDate()));
    const eventEndDate = new Date(this.eventDetailsFormGroup.value.eventEndDate);
    const eventEndDateUTC = new Date(Date.UTC(eventEndDate.getFullYear(), eventEndDate.getMonth(), eventEndDate.getDate()));

    let registrationOpenDateUTC = null;
    if (this.visabilityFormGroup.value.registrationOpenDate) {
      const registrationOpenDate = new Date(this.visabilityFormGroup.value.registrationOpenDate);
      registrationOpenDateUTC = new Date(
        Date.UTC(
          registrationOpenDate.getFullYear(),
          registrationOpenDate.getMonth(),
          registrationOpenDate.getDate())
        );
    }

    let registrationCloseDateUTC = null;
    if (this.visabilityFormGroup.value.registrationCloseDate) {
      const registrationCloseDate = new Date(this.visabilityFormGroup.value.registrationCloseDate);
      registrationCloseDateUTC = new Date(
        Date.UTC(registrationCloseDate.getFullYear(),
        registrationCloseDate.getMonth(),
        registrationCloseDate.getDate())
      );
    }

    const eventConfigurationSubmissionDto: EventConfigurationSubmissionDto = {
      contactName: this.contactDetailsFormGroup.value.contactName,
      contactEmail: this.contactDetailsFormGroup.value.contactEmail,
      contactTelephone: this.contactDetailsFormGroup.value.contactTelephone,
      registrationOpenDate: this.visabilityFormGroup.value.visibility !== 2 ? registrationOpenDateUTC : null,
      registrationCloseDate: this.visabilityFormGroup.value.visibility !== 2 ? registrationCloseDateUTC : null,
      eventStartDate: eventStartDateUTC,
      eventEndDate: eventEndDateUTC,
      eventSynopsis: this.eventDetailsFormGroup.value.eventSynopsis,
      locationTown: this.eventDetailsFormGroup.value.locationTown,
      locationPostcode: this.eventDetailsFormGroup.value.locationPostcode,
      visibilityType: this.visabilityFormGroup.value.visibility,
      templateType: this.configurationTemplateFormGroup.value.eventTemplate,
      agreeToTermsAndConditions: this.termsAndConditionsFormGroup.value.agreeToTermsAndConditions,
    };

    this._conductor.submitConfiguration(eventConfigurationSubmissionDto).subscribe((eventDto: EventDto) => {
      this._conductor.clear();
      // this.router.navigate(['event', escape(eventDto.name), eventDto.id]);
      this.router.navigate(['event', eventDto.name, eventDto.id]);
    });
  }

  onStepperStepChange($event) {

    this._conductor.startStepperStepChanged($event);
  }

  onTermsAndConditionsClick() {
    const dialogRef = this.composeDialog.open(EventConfigureTermsComponent, AppConstants.largeMatDialogConfig);

    dialogRef.afterClosed().subscribe(result => {

    });

  }

  openUploadImageDialog() {
    const dialogRef = this.composeDialog.open(UploadImageComponent);
    dialogRef.afterClosed().subscribe(result => {
      this._conductor.newImageUploaded();
    });
  }
}
