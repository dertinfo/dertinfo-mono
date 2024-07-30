import { Component, Inject, OnInit } from '@angular/core';
import { UntypedFormControl, UntypedFormGroup } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ScoreCard } from 'app/models/app/ScoreCard';
import { DodResultComplaintSubmissionDto } from 'app/models/dto/DodResultComplaintSubmissionDto';
import { DodSubmissionDto } from 'app/models/dto/DodSubmissionDto';
import { GroupViewConductor } from '../../services/group-view.conductor';
import { GroupViewTracker } from '../../services/group-view.tracker';

@Component({
  selector: 'app-dertofderts-reportdialog',
  templateUrl: './dertofderts-reportdialog.component.html',
  styleUrls: ['./dertofderts-reportdialog.component.scss']
})
export class DertOfDertsReportDialogComponent implements OnInit {

  private _resultId: number;
  private _groupId: number;

  public formGroup: UntypedFormGroup;
  public isReportSubmitted: boolean = false;

  constructor(
    private _conductor: GroupViewConductor,
    public dialogRef: MatDialogRef<DertOfDertsReportDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { scoreCard: ScoreCard, groupId: number }
  ) { }

  ngOnInit() {
    this.prepareForm();
    this.extractData();
  }

  public submitForm() {
    if (this.formGroup.valid) {

      const submission: DodResultComplaintSubmissionDto = {
        resultId: this._resultId,
        forScores: this.formGroup.value.reportScores,
        forComments: this.formGroup.value.reportComments,
        notes: this.formGroup.value.reportNotes
      };

      const obs$ = this._conductor.reportScoreCard(this._groupId, submission);

      const subs = obs$.subscribe(
        (dodResultComplaintDto) => this.reportScoreCardSucceeded(),
        (err) => this.reportScoreCardFailed(err),
        () => { /* Observable Completed */ }
      );
    } else {
      this.formGroup.markAllAsTouched();
    }
  }

  private extractData() {
    this._resultId = this.data.scoreCard.dodResultId;
    this._groupId = this.data.groupId;
  }

  private prepareForm() {
    this.formGroup = new UntypedFormGroup({
      reportScores: new UntypedFormControl(false, []),
      reportComments: new UntypedFormControl(false, []),
      reportNotes: new UntypedFormControl('', []),
    });
  }

  private reportScoreCardSucceeded() {
    this.isReportSubmitted = true;
  }

  private reportScoreCardFailed(error: any) {
    // todo - handle error cases.
    console.error('Submission failed', error);
  }

}
