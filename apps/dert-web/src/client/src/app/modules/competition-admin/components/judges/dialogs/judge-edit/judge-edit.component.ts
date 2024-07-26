import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import {
  JudgeDto,
  JudgeUpdateSubmissionDto,
} from 'app/models/dto';
import { CompetitionAdminConductor } from 'app/modules/competition-admin/services/competition-admin.conductor';
import { Subscription } from 'rxjs';

@Component({
  selector: 'judge-edit',
  templateUrl: './judge-edit.component.html'
})
export class JudgeEditComponent implements OnInit, OnDestroy {

  private _subscriptions: Subscription[] = [];
  private _namePattern = '^(?!.*\/).*$';

  public form: FormGroup;

  public judge: JudgeDto;
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
      name: new FormControl(this.judge.name, [Validators.required, Validators.pattern(this._namePattern)]),
      telephone: new FormControl(this.judge.telephone, []),
      email: new FormControl(this.judge.email, [])
    });
  }

  ngOnDestroy() {
  }

  onFormSubmit() {
    if (this.form.valid) {

      // Build the basic submission
      const judgeUpdateSubmission: JudgeUpdateSubmissionDto = {
        name: this.form.value.name,
        telephone: this.form.value.telephone,
        email: this.form.value.email,
      };

      this.isSubmitting = true;

      const subs = this._conductor.updateJudge(this.judge.id, judgeUpdateSubmission).subscribe(
        (response) => {
          subs.unsubscribe();
          this.isSubmitting = false;
          this.composeDialog.closeAll();
        },
        (error) => {
          console.error('ERROR: Updating Judge Failed', error);
        }
      );

    }
  }

  onCancel() {
    this.composeDialog.closeAll();
  }
}
