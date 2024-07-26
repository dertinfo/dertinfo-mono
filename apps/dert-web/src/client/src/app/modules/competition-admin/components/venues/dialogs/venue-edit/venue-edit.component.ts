import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { JudgeDto, JudgeSlotJudgeUpdateSubmissionDto, VenueAllocationDto, VenueUpdateSubmissionDto } from 'app/models/dto';
import { CompetitionAdminConductor } from 'app/modules/competition-admin/services/competition-admin.conductor';
import { Subscription } from 'rxjs';

@Component({
  selector: 'venue-edit',
  templateUrl: './venue-edit.component.html'
})
export class VenueEditComponent implements OnInit, OnDestroy {

  private _subscriptions: Subscription[] = [];
  private _namePattern = '^(?!.*\/).*$';

  public form: FormGroup;

  public venue: VenueAllocationDto;
  public allJudges: Array<JudgeDto>;
  public isSubmitting = false;

  constructor(

    private composeDialog: MatDialog,
    private _conductor: CompetitionAdminConductor,
    private router: Router
  ) {
  }

  ngOnInit() {
    this.form = new FormGroup({
      name: new FormControl(this.venue.name, [Validators.required, Validators.pattern(this._namePattern)])
    });

    for (let i = 0; i < this.venue.judgeSlots.length; i++) {
      this.form.addControl(`slot_${i}`, new FormControl(this.venue.judgeSlots[i].judgeId, []));
    }
  }

  ngOnDestroy() {
  }

  onFormSubmit() {

    if (this.form.valid) {

      // Build the basic submission
      const venueUpdateSubmission: VenueUpdateSubmissionDto = {
        name: this.form.value.name,
        judgeSlotUpdates: []
      };

      // For each of the judge slots in the original identify the values
      for (let i = 0; i < this.venue.judgeSlots.length; i++) {

        const slotId = `slot_${i}`;
        const selectedJudgeId = this.form.controls[slotId].value;

        const slotUpdate: JudgeSlotJudgeUpdateSubmissionDto = {
          id: this.venue.judgeSlots[i].id,
          judgeId: selectedJudgeId
        };

        venueUpdateSubmission.judgeSlotUpdates.push(slotUpdate);
      }

      this.isSubmitting = true;

      const subs = this._conductor.updateVenue(this.venue.id, venueUpdateSubmission).subscribe(
        (response) => {
          subs.unsubscribe();
          this.isSubmitting = false;
          this.composeDialog.closeAll();
        },
        (error) => {
          throw new Error('VenueCreateComponent - Updating Venue Failed');
        }
      );

    }
  }

  public getScoreSetName(index: number) {
    return 'Select Judge For ' + this.venue.judgeSlots[index].scoreSetName;
  }

  public onJudgeSelectionChange() {

  }

  onCancel() {
    this.composeDialog.closeAll();
  }
}
