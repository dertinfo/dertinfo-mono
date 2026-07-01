import { Location } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { UntypedFormControl, UntypedFormGroup, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute } from '@angular/router';
import { customEmailValidator } from 'app/shared/validators/email-no-required';
import { ActivityDetailDto, ActivityDto, ActivityUpdateDto } from 'app/models/dto';
import { Observable ,  Subscription } from 'rxjs';
import { EventAdminConductor } from '../../services/event-admin.conductor';
import { EventAdminTracker } from '../../services/event-admin.tracker';
import { EventActivitiesCreateComponent } from '../event-activities-create/event-activities-create.component';

@Component({
  selector: 'app-event-activities-detail',
  templateUrl: './event-activities-detail.component.html',
  styleUrls: ['./event-activities-detail.component.css']
})
export class EventActivitiesDetailComponent implements OnInit, OnDestroy {

  private _subscriptions: Subscription[] = [];

  private audienceTypes = [];

  public _isReady = false;
  public activityDetail: ActivityDetailDto;
  public activityUpdateForm: UntypedFormGroup;

  constructor(
    private activatedRoute: ActivatedRoute,
    private _eventConductor: EventAdminConductor,
    private _eventTracker: EventAdminTracker,
    private _location: Location
  ) { }

  ngOnInit() {

    this._subscriptions.push(this.activatedRoute.params.subscribe(params => {
      this._eventConductor.initFocussedActivity(params['activityId']);
    }));

    this._subscriptions.push(this.activatedRoute.parent.params.subscribe(params => {
      this._eventConductor.initActivities(params['id']);
    }));

    this._subscriptions.push(this._eventTracker.focussedActivity$.subscribe((activityDetail) => {
      if (activityDetail) {
        this.activityDetail = activityDetail;

        this.activityUpdateForm = new UntypedFormGroup({
          title: new UntypedFormControl(this.activityDetail.title, [
            Validators.required
          ]),
          description: new UntypedFormControl(this.activityDetail.description, []),
          price: new UntypedFormControl(this.activityDetail.price, []),
          audienceTypeId: new UntypedFormControl(this.activityDetail.audienceTypeId, []),
          isDefault: new UntypedFormControl(this.activityDetail.isDefault),
          priceTBC: new UntypedFormControl(this.activityDetail.priceTBC),
          soldOut: new UntypedFormControl(this.activityDetail.soldOut)
        });

        this._isReady = true;
      }
    }));

    this.audienceTypes = this._eventConductor.activityAudienceTypes;
  }

  ngOnDestroy() {
    this._subscriptions.map((sub) => { sub.unsubscribe(); });
    this._subscriptions = []; // For good measure
  }

  onUpdateActivitySubmit() {

    if (this.activityUpdateForm.valid) {

      // Update the visible entities
      this.activityDetail.title = this.activityUpdateForm.value.name;
      this.activityDetail.description = this.activityUpdateForm.value.email;
      this.activityDetail.price = this.activityUpdateForm.value.telephone;
      this.activityDetail.audienceTypeId = this.activityUpdateForm.value.audienceTypeId;
      this.activityDetail.isDefault = this.activityUpdateForm.value.isDefault;
      this.activityDetail.priceTBC = this.activityUpdateForm.value.priceTBC;
      this.activityDetail.soldOut = this.activityUpdateForm.value.soldOut;

      const activityUpdate: ActivityUpdateDto = {
        id: this.activityDetail.id,
        title: this.activityUpdateForm.value.title,
        description: this.activityUpdateForm.value.description,
        price: this.activityUpdateForm.value.price,
        audienceTypeId: this.activityUpdateForm.value.audienceTypeId,
        isDefault: this.activityUpdateForm.value.isDefault,
        priceTBC: this.activityUpdateForm.value.priceTBC,
        soldOut: this.activityUpdateForm.value.soldOut
      };

      this._eventConductor.updateActivity(activityUpdate);
    }
  }

  onActivityRemoveClick() {
    this._eventConductor.removeActivity(this.activityDetail);
    this._location.back();
  }
}
