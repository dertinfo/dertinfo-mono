import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { ScoreCategoryDto, ScoreCategoryUpdateSubmissionDto } from 'app/models/dto';
import { CompetitionAdminConductor } from 'app/modules/competition-admin/services/competition-admin.conductor';
import { Subscription } from 'rxjs';

@Component({
  selector: 'scorecategory-edit',
  templateUrl: './scorecategory-edit.component.html'
})
export class ScoreCategoryEditComponent implements OnInit, OnDestroy {

  private _subscriptions: Subscription[] = [];
  private _namePattern = '^(?!.*\/).*$';

  public scoreCategoryForm: FormGroup;

  public scoreCategory: ScoreCategoryDto;
  public isSubmitting = false;

  constructor(

    private composeDialog: MatDialog,
    private _conductor: CompetitionAdminConductor,
    private router: Router
  ) {
  }

  ngOnInit() {
      this.scoreCategoryForm = new FormGroup({
        name: new FormControl(this.scoreCategory.name, [Validators.required, Validators.pattern(this._namePattern)]),
        maxMarks: new FormControl(this.scoreCategory.maxMarks, [Validators.required, Validators.pattern(this._namePattern)]),
        tag: new FormControl(this.scoreCategory.tag, [Validators.required, Validators.pattern(this._namePattern)]),
        sortOrder: new FormControl(this.scoreCategory.sortOrder, [Validators.required, Validators.pattern(this._namePattern)])
      });
  }

  ngOnDestroy() {
  }

  onEditScoreCategorySubmit() {

    if (this.scoreCategoryForm.valid) {

      const scoreCategoryUpdateSubmission: ScoreCategoryUpdateSubmissionDto = {
        name: this.scoreCategoryForm.value.name,
        maxMarks: this.scoreCategoryForm.value.maxMarks,
        tag: this.scoreCategoryForm.value.tag,
        sortOrder: this.scoreCategoryForm.value.sortOrder,
      };

      this.isSubmitting = true;
      const subs = this._conductor.updateScoreCategory(this.scoreCategory.scoreCategoryId, scoreCategoryUpdateSubmission).subscribe(
        (response) => {
          subs.unsubscribe();
          this.isSubmitting = false;
          this.composeDialog.closeAll();
        },
        (error) => {
          console.error('ERROR: Updating ScoreCategory Failed', error);
        }
      );

    }
  }

  onCancel() {
    this.composeDialog.closeAll();
  }
}
