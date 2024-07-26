import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { ScoreSetDto, ScoreSetUpdateSubmissionDto } from 'app/models/dto';
import { CompetitionAdminConductor } from 'app/modules/competition-admin/services/competition-admin.conductor';
import { Subscription } from 'rxjs';

@Component({
  selector: 'scoreset-edit',
  templateUrl: './scoreset-edit.component.html'
})
export class ScoreSetEditComponent implements OnInit, OnDestroy {

  private _subscriptions: Subscription[] = [];
  private _namePattern = '^(?!.*\/).*$';

  public scoreSetForm: FormGroup;

  public scoreSet: ScoreSetDto;
  public isSubmitting = false;

  constructor(

    private composeDialog: MatDialog,
    private _conductor: CompetitionAdminConductor,
    private router: Router
  ) {
  }

  ngOnInit() {
      this.scoreSetForm = new FormGroup({
        name: new FormControl(this.scoreSet.name, [Validators.required, Validators.pattern(this._namePattern)])
      });
  }

  ngOnDestroy() {
  }

  onEditScoreSetSubmit() {

    if (this.scoreSetForm.valid) {

      const scoreSetUpdateSubmission: ScoreSetUpdateSubmissionDto = {
        name: this.scoreSetForm.value.name
      };

      this.isSubmitting = true;
      const subs = this._conductor.updateScoreSet(this.scoreSet.scoreSetId, scoreSetUpdateSubmission).subscribe(
        (response) => {
          subs.unsubscribe();
          this.isSubmitting = false;
          this.composeDialog.closeAll();
        },
        (error) => {
          console.error('ERROR: Updating ScoreSet Failed', error);
        }
      );

    }
  }

  onCancel() {
    this.composeDialog.closeAll();
  }
}
