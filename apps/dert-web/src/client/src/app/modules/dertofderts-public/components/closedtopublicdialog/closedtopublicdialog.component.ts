import { Component, OnInit } from '@angular/core';
import { UntypedFormControl, UntypedFormGroup, Validators } from '@angular/forms';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { DodIdentifyJudgeSubmissionResponseDto } from 'app/models/dto/DodIdentifyJudgeSubmissionResponseDto';
import { WebsiteTermsComponent } from 'app/regions/terms/components/website-terms/website-terms.component';
import { customEmailValidator } from 'app/shared/validators/email-no-required';
import { Conductor } from '../../services/dertofderts-public.conductor';

@Component({
  selector: 'app-closedtopublicdialog',
  templateUrl: './closedtopublicdialog.component.html',
  styleUrls: ['./closedtopublicdialog.component.scss']
})
export class ClosedToPublicDialogComponent implements OnInit {

  public formGroup: UntypedFormGroup;
  public hasSubmissionError: boolean = false;
  public submissionErrorMessage: string = '';

  constructor(
    public dialogRef: MatDialogRef<ClosedToPublicDialogComponent>,
    private _conductor: Conductor,
    private _dialog: MatDialog,
    private _router: Router
  ) { }

  ngOnInit() {
    this.prepareForm();
  }

  public submitForm() {
    this.hasSubmissionError = false;

    if (!this.formGroup.valid) {
      this.formGroup.markAllAsTouched();
    } else {
      const obs$ = this._conductor.identifyJudge(this.formGroup.value);

      const subs = obs$.subscribe(
        (dodIdentifyJudgeSubmissionResponseDto) => this.submitSucceeded(dodIdentifyJudgeSubmissionResponseDto),
        (err) => this.submitFailed(err),
        () => { /* Observable Completed */ }
      );
    }
  }

  public onTermsAndConditionsClick() {
    this._dialog.open(WebsiteTermsComponent, {
      width: '850px',
      height: '80vh'
    });
  }

  public onBackToHomeClick() {

    this._router.navigate(['/dertofderts']);
    this.dialogRef.close();
  }

  private submitSucceeded(dodIdentifyJudgeSubmissionResponseDto: DodIdentifyJudgeSubmissionResponseDto) {
    this.dialogRef.close();
  }

  private submitFailed(err: any) {

    console.log(err);
    this.hasSubmissionError = true;
    this.submissionErrorMessage = err.error;
  }

  private prepareForm() {

    // Build the form group
    this.formGroup = new UntypedFormGroup({
      name: new UntypedFormControl('', [Validators.required]),
      email: new UntypedFormControl('', [Validators.required, customEmailValidator()]),
      officialJudge: new UntypedFormControl(false, []),
      officialJudgePassword: new UntypedFormControl('', []),
      agreeToTermsAndConditions: new UntypedFormControl(false, []),
    });
  }

}
