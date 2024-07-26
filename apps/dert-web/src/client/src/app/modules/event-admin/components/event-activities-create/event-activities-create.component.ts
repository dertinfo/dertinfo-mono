import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { ActivitySubmissionDto } from 'app/models/dto';
import { Subscription } from 'rxjs';
import { EventAdminConductor } from '../../services/event-admin.conductor';
import { EventAdminTracker } from '../../services/event-admin.tracker';

@Component({
  selector: 'app-event-activities-create',
  templateUrl: './event-activities-create.template.html'
})
export class EventActivitiesCreateComponent implements OnInit, OnDestroy {

  private _subscriptions: Subscription[] = [];

  private hasUploadChanges = false;

  activityForm: FormGroup;
  public audienceTypes = [];

  constructor(
    private composeDialog: MatDialog,
    private eventConductor: EventAdminConductor,
    private _eventTracker: EventAdminTracker,

  ) {
  }

  ngOnInit() {

    this.audienceTypes = this.eventConductor.activityAudienceTypes;

    this.activityForm = new FormGroup({
      title: new FormControl('', [
        Validators.required
      ]),
      description: new FormControl('', []),
      price: new FormControl(0.00, [Validators.required]),
      audienceTypeId: new FormControl(1, [Validators.required]),
      isDefault: new FormControl(false),
      priceTBC: new FormControl(false),
      soldOut: new FormControl(false),
    });
  }

  ngOnDestroy() {

    this._subscriptions.map((sub) => { sub.unsubscribe(); });
    this._subscriptions = []; // For good measure
  }

  onCreateActivitySubmit() {

    if (this.activityForm.valid) {

      const eventActivitySubmission: ActivitySubmissionDto = {
        title: this.activityForm.value.title,
        description: this.activityForm.value.description,
        price: this.activityForm.value.price,
        audienceTypeId: this.activityForm.value.audienceTypeId,
        isDefault: this.activityForm.value.isDefault,
        priceTBC: this.activityForm.value.priceTBC,
        soldOut: this.activityForm.value.soldOut
      };

      const submitActivitySubscription = this.eventConductor.addActivity(eventActivitySubmission).subscribe(
        (savedActivity) => {
          submitActivitySubscription.unsubscribe();
          this.composeDialog.closeAll();
        },
        (error) => {
          console.error('ERROR: Adding Activity Failed');
        }
      );
    }
  }

  onCancel() {
    this.composeDialog.closeAll();
  }
}
