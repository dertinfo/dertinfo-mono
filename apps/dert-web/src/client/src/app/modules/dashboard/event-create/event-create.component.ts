import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { DashboardConductor } from 'app/modules/dashboard/services/dashboard.conductor';
import { EventMinimalSubmissionDto } from 'app/models/dto';
import { Subscription } from 'rxjs';

@Component({
  selector: 'event-create',
  templateUrl: './event-create.component.html'
})
export class EventCreateComponent implements OnInit, OnDestroy {

  private _subscriptions: Subscription[] = [];
  private _eventNamePattern = '^(?!.*\/).*$';

  private eventId;
  private hasUploadChanges = false;

  public eventForm: FormGroup;

  public isSubmitting = false;

  constructor(

    private composeDialog: MatDialog,
    private dashboardConductor: DashboardConductor,
    private router: Router
  ) {
  }

  ngOnInit() {
    this.eventForm = new FormGroup({
      eventName: new FormControl('', [Validators.required, Validators.pattern(this._eventNamePattern)]),
      eventSynopsis: new FormControl('', [])
    });
  }

  ngOnDestroy() {
  }

  onCancel() {
    this.composeDialog.closeAll();
  }

  onCreateEventSubmit() {

    if (this.eventForm.valid) {

      const eventMinimalSubmission: EventMinimalSubmissionDto = {
        eventName: this.eventForm.value.eventName,
        eventSynopsis: this.eventForm.value.eventSynopsis
      };

      this.isSubmitting = true;
      this.dashboardConductor.addEvent(eventMinimalSubmission).then(
        (savedEvent) => {
          this.isSubmitting = false;

          this.dashboardConductor.pinNewEvent(savedEvent);
          this.composeDialog.closeAll();
          this.router.navigate(['event-configure', savedEvent.name, savedEvent.id]);
        },
        (error) => {
          console.error('ERROR: Adding Event Failed');
        }
      );
    }
  }
}
