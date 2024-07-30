import { Component, OnDestroy, OnInit } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { AppConstants } from 'app/app.constants';
import {
  GroupRegistrationDto,
  GroupRegistrationSubmissionDto
} from 'app/models/dto';
import { GroupRegisterTermsComponent } from 'app/regions/terms/components/group-register-terms/group-register-terms.component';
import { Subscription } from 'rxjs';
import { GroupRegisterConductor } from '../../services/group-register.conductor';
import { GroupRegisterTracker } from '../../services/group-register.tracker';

@Component({
  selector: 'app-start',
  templateUrl: './start.component.html',
  styleUrls: ['./start.component.css']
})
export class StartComponent implements OnInit, OnDestroy {

  private _subscriptions: Subscription[] = [];

  private eventId: number;
  public eventName: string;
  public eventSynopsis: string;
  public eventPictureUrl: string;
  public groupPictureUrl: string;

  termsAndConditionsFormGroup: UntypedFormGroup;

  constructor(
    private fb: UntypedFormBuilder,
    private _conductor: GroupRegisterConductor,
    private _tracker: GroupRegisterTracker,
    public composeDialog: MatDialog,
    private router: Router
  ) { }

  ngOnInit() {

    this.termsAndConditionsFormGroup = this.fb.group({
      agreeToTermsAndConditions: [false, Validators.required]
    });

    this._subscriptions.push(this._tracker.eventDto$.subscribe((eventDto) => {

      if (eventDto) {
        this.eventId = eventDto.id;
        this.eventName = eventDto.name;
        this.eventSynopsis = eventDto.eventSynopsis;
        this.eventPictureUrl = eventDto.eventPictureUrl;
      }
    }));

    this._subscriptions.push(this._tracker.groupDto$.subscribe((groupDto) => {
      if (groupDto) {

        this.groupPictureUrl = groupDto.groupPictureUrl;
      }
    }));
  }

  ngOnDestroy() {
    this._subscriptions.map((sub) => { sub.unsubscribe(); });
    this._subscriptions = [];
  }

  submit() {

    const registrationSubmissionDto: GroupRegistrationSubmissionDto = {
      agreeToTermsAndConditions: this.termsAndConditionsFormGroup.value.agreeToTermsAndConditions,
      eventId: this.eventId
    };

    this._conductor.submitGroupRegistration(registrationSubmissionDto).subscribe((registrationDto: GroupRegistrationDto) => {
      this._conductor.clear();
      this.router.navigate(['group-registration', registrationDto.id]);
    });
  }

  onStepperStepChange($event) {
    this._conductor.startStepperStepChanged($event);
  }

  onTermsAndConditionsClick() {
    const dialogRef = this.composeDialog.open(GroupRegisterTermsComponent, AppConstants.largeMatDialogConfig);
    dialogRef.afterClosed().subscribe(result => {

    });

  }
}
